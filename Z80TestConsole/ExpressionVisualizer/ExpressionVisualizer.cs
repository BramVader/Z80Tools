using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;

namespace Z80TestConsole
{
    public static class ExpressionVisualizer
    {
        public enum FieldType
        {
            Default = 1,
            Type = 2,
            String = 3,
            Number = 4,
            Keyword = 5,
            Comment = 6,
            Identifier = 7,
            Quote = 8
        }

        public abstract class BaseFormatter : IFormatProvider
        {
            public abstract object GetFormat(Type formatType);

            public abstract string Finalize(string text);
        }

        public class HtmlFormatter : BaseFormatter, ICustomFormatter
        {
            public override object GetFormat(Type formatType)
            {
                if (formatType == typeof(ICustomFormatter))
                    return this;
                return null;
            }

            public string Format(string format, object arg, IFormatProvider formatProvider)
            {
                string value = DefaultFormat(format, arg);
                if (!String.IsNullOrEmpty(format))
                {
                    FieldType fieldType;
                    if (Enum.TryParse(format, out fieldType))
                    {
                        if (fieldType == FieldType.Quote)
                        {
                            return "<div class='Quote'>" + value + "</div>";    // Not HtmlEncoded!
                        }
                        else
                        {
                            string encodedValue = WebUtility.HtmlEncode(value);
                            switch (fieldType)
                            {
                                case FieldType.Comment:
                                    return "<span class='Comment'>" + encodedValue + "</span>";
                                case FieldType.Keyword:
                                    return "<span class='Keyword'>" + encodedValue + "</span>";
                                case FieldType.Number:
                                    return "<span class='Number'>" + encodedValue + "</span>";
                                case FieldType.String:
                                    return "<span class='String'>" + encodedValue + "</span>";
                                case FieldType.Type:
                                    return "<span class='Type'>" + encodedValue + "</span>";
                                case FieldType.Identifier:
                                    return "<span class='Identifier'>" + encodedValue + "</span>";
                                default:
                                    throw new Exception("Unknown FieldType: " + fieldType);
                            }
                        }
                    }
                }
                return value;
            }

            private string DefaultFormat(string format, object arg)
            {
                if (arg is IFormattable)
                    return ((IFormattable)arg).ToString(format, CultureInfo.CurrentCulture);
                else if (arg != null)
                    return arg.ToString();
                else
                    return String.Empty;
            }

            public override string Finalize(string text)
            {
                return text
                    .Replace("\r\n", "<br />")
                    .Replace("\t", "&nbsp;&nbsp;&nbsp;&nbsp;");
            }
        }


        private class Visitor
        {
            private class VisitContext
            {
                public bool Hexmode;
                public int Indent;
                public Dictionary<ParameterExpression, string> Names;
                public BaseFormatter Formatter;
                public int Level;

                private char varCounter;

                public string GetVarName(ParameterExpression par)
                {
                    if (!String.IsNullOrEmpty(par.Name))
                        return par.Name;
                    string name;
                    if (!Names.TryGetValue(par, out name))
                    {
                        name = new string(varCounter++, 1);
                        Names.Add(par, name);
                    }
                    return name;
                }

                public string GetIndent(int? indentDelta = null)
                {
                    if (indentDelta.HasValue)
                    {
                        if (indentDelta.Value < 0)
                        {
                            Indent += indentDelta.Value;        // prefix 
                            return new String('\t', Indent);
                        }
                        else
                        {
                            Indent += indentDelta.Value;        // postfix
                            return new String('\t', Indent - indentDelta.Value);
                        }
                    }
                    else
                    {
                        return new String('\t', Indent);
                    }
                }

                public string Newline(int? indentDelta = null)
                {
                    return "\r\n" + GetIndent(indentDelta);
                }

                public VisitContext()
                {
                    Names = new Dictionary<ParameterExpression, string>();
                    Indent = 0;
                    varCounter = 'a';
                    Formatter = new HtmlFormatter();
                    Level = 0;
                }
            }

            private static Dictionary<Type, string> IntegralNames = new Dictionary<Type, string>
            {
                { typeof(bool),   "bool" },
                { typeof(byte),   "byte" },
                { typeof(sbyte),  "sbyte" },
                { typeof(char),   "char" },
                { typeof(decimal),"decimal" },
                { typeof(double), "double" },
                { typeof(float),  "float" },
                { typeof(int),    "int" },
                { typeof(uint),   "uint" },
                { typeof(long),   "long" },
                { typeof(ulong),  "ulong" },
                { typeof(object), "object" },
                { typeof(short),  "short" },
                { typeof(ushort), "ushort" },
                { typeof(string), "string" },
            };

            private static HashSet<Type> NumericTypes = new HashSet<Type>
            {
                typeof(byte),
                typeof(sbyte),
                typeof(char),
                typeof(decimal),
                typeof(double),
                typeof(float),
                typeof(int),
                typeof(uint),
                typeof(long),
                typeof(ulong),
                typeof(object),
                typeof(short),
                typeof(ushort)
            };


            // Precedence goes from lowest to highest
            // If going from higher to lower precedence, parentheses are needed.
            private enum Precedence
            {
                SameLevel = -1,

                Sequence = 1,           // , Sequential evaluation
                Assignment = 2,         // =  *=  /=  %=  +=  -=  <<=  >>=  &=  ^=  |= =>
                Conditional = 3,        // ?:
                NullCoalescing = 4,     // ??
                ConditionalOr = 5,      // ||
                ConditionalAnd = 6,     // &&
                LogicalOr = 7,          // |
                LogicalXor = 8,         // ^
                LogicalAnd = 9,         // &
                Equality = 10,          // ==  !=
                Relational = 11,        // <  >  <=  >=  is  as
                Shift = 12,             // <<  >>
                Additive = 13,          // +  -
                Multiplicative = 14,    // *  /  %
                Unary = 15,             // +  -  !  ~  ++x  --x  (T)x(cast) *(dereference) &(reference)
                Exponentiation = 16,    // Power-operator (^, not available in C#)
                Primary = 17,           // x.y  f(x)  a[x]  x++  x--  new typeof  checked  unchecked

                Highest = 20,
            }

            private VisitContext context;

            public Visitor()
            {
            }

            Regex identifierRegEx = new Regex(@"^(?:[A-Za-z_][A-Za-z_0-9]+(?:\.|\+|\[\])?)+$");
            Regex genericRegEx = new Regex(@"[^`]+");

            private bool IsIdentifier(string name)
            {
                return identifierRegEx.IsMatch(name);
            }

            private bool IsNumericType(Type type)
            {
                return NumericTypes.Contains(type) ||
                       NumericTypes.Contains(Nullable.GetUnderlyingType(type));
            }

            public string GetKeyword(string keyword)
            {
                return String.Format(context.Formatter, "{0:Keyword}", keyword);
            }

            private string GetTypeName(Type type, bool useIntegralName = true)
            {
                if (type.IsGenericType)
                {
                    return String.Format(context.Formatter, "{0:Type}", genericRegEx.Match(type.Name).Value) + "&lt;" + String.Join(", ", type.GetGenericArguments().Select(tp => GetTypeName(tp, useIntegralName))) + "&gt;";
                }

                if (type.IsArray)
                {
                    return GetTypeName(type.GetElementType()) +
                        String.Format(context.Formatter, "{0:Symbol}{1:Number}{2:Symbol}", "[", new String(',', type.GetArrayRank() - 1), "]");
                }
                if (useIntegralName)
                {
                    string integralName;
                    if (IntegralNames.TryGetValue(type, out integralName))
                        return String.Format(context.Formatter, "{0:Keyword}", integralName);
                }
                return IsIdentifier(type.Name) ? String.Format(context.Formatter, "{0:Type}", type.Name) : "";
            }

            public string Convert(Expression expr, BaseFormatter formatter = null, bool hexMode = false)
            {
                context = new VisitContext() { Hexmode = hexMode, Formatter = formatter ?? new HtmlFormatter() };
                string result = Visit(expr, Precedence.Highest);
                return context.Formatter.Finalize(result);
            }

            private string VisitMemberBinding(MemberBinding binding, Precedence precedence)
            {
                switch (binding.BindingType)
                {
                    // Summary:
                    //     A binding that represents initializing a member with the value of an expression.
                    case MemberBindingType.Assignment:
                        var asgBinding = binding as MemberAssignment;
                        return String.Format(context.Formatter, "{0:Identifier} = {1}",
                            asgBinding.Member.Name,
                            Visit(asgBinding.Expression, Precedence.Assignment)
                        );

                    //
                    // Summary:
                    //     A binding that represents recursively initializing members of a member.
                    case MemberBindingType.MemberBinding:
                        var membBinding = binding as MemberMemberBinding;
                        return String.Format(context.Formatter, "{0:Identifier} = {{ {1} }}",
                            membBinding.Member.Name,
                            String.Join(", ", membBinding.Bindings.Select(mb => VisitMemberBinding(mb, precedence))));

                    //
                    // Summary:
                    //     A binding that represents initializing a member of type System.Collections.IList
                    //     or System.Collections.Generic.ICollection<T> from a list of elements.
                    case MemberBindingType.ListBinding:
                        var listBinding = binding as MemberListBinding;
                        return String.Format(context.Formatter, "{0:Identifier} = {{ {1} }}",
                            listBinding.Member.Name,
                            String.Join(", ", listBinding.Initializers.Select(el => "{ " + String.Join(", ", el.Arguments.Select(arg => Visit(arg, Precedence.Sequence))) + " }"))
                        );

                    default:
                        return "<unknown binding>";
                }
            }

            private string Visit(Expression expr, Precedence parentPrecedence, Precedence precedence = Precedence.SameLevel, bool hasInitializer = false)
            {
                if (precedence == Precedence.SameLevel)
                    precedence = parentPrecedence;

                context.Level++;

                string result = null;

                var ue = expr as UnaryExpression;
                var be = expr as BinaryExpression;
                var mc = expr as MethodCallExpression;
                var ce = expr as ConditionalExpression;
                var me = expr as MemberExpression;
                if (expr == null) return String.Empty;
                StringBuilder sb;

                switch (expr.NodeType)
                {
                    // Summary:
                    //     An addition operation, such as a + b, without overflow checking, for numeric
                    //     operands.
                    case ExpressionType.Add:
                    case ExpressionType.AddChecked:
                        precedence = Precedence.Additive;
                        result = String.Format(context.Formatter, "{0} + {1}", Visit(be.Left, precedence), Visit(be.Right, precedence));
                        break;
                    //
                    // Summary:
                    //     A bitwise or logical AND operation, such as (a & b) in C# and (a And b) in
                    //     Visual Basic.
                    case ExpressionType.And:
                        precedence = Precedence.LogicalAnd;
                        result = String.Format(context.Formatter, "{0} & {1}", Visit(be.Left, precedence), Visit(be.Right, precedence));
                        break;
                    //
                    // Summary:
                    //     A conditional AND operation that evaluates the second operand only if the
                    //     first operand evaluates to true. It corresponds to (a && b) in C# and (a
                    //     AndAlso b) in Visual Basic.
                    case ExpressionType.AndAlso:
                        precedence = Precedence.ConditionalAnd;
                        result = String.Format(context.Formatter, "{0} && {1}", Visit(be.Left, precedence), Visit(be.Right, precedence));
                        break;
                    //
                    // Summary:
                    //     An operation that obtains the length of a one-dimensional array, such as
                    //     array.Length.
                    case ExpressionType.ArrayLength:
                        result = String.Format(context.Formatter, "{0}.Length", Visit(ue.Operand, precedence, Precedence.Primary));
                        break;
                    //
                    // Summary:
                    //     An indexing operation in a one-dimensional array, such as array[index] in
                    //     C# or array(index) in Visual Basic.
                    case ExpressionType.ArrayIndex:
                        result = String.Format(context.Formatter, "{0}[{1}]", Visit(be.Left, precedence, Precedence.Primary), Visit(be.Right, precedence, Precedence.Primary));
                        break;
                    //
                    // Summary:
                    //     A method call, such as in the obj.sampleMethod() expression.
                    case ExpressionType.Call:
                        {
                            bool isExtensionMethod = mc.Method.GetCustomAttributes(true).Any(at => at is System.Runtime.CompilerServices.ExtensionAttribute);
                            string[] args = mc.Arguments.Skip(isExtensionMethod ? 1 : 0).Select(arg => Visit(arg, precedence, Precedence.Sequence)).ToArray();
                            if (isExtensionMethod)     // Extension method
                                result = String.Format(context.Formatter, "{2}.{0:Identifier}({1})", mc.Method.Name, String.Join(", ", args),
                                    Visit(mc.Arguments.First(), precedence));
                            else if (mc.Object == null)  // Static method
                                result = String.Format(context.Formatter, "{2}.{0:Identifier}({1})", mc.Method.Name, String.Join(", ", args),
                                    GetTypeName(mc.Method.DeclaringType, false));
                            else // Instance method
                                result = String.Format(context.Formatter, "{2}.{0:Identifier}({1})", mc.Method.Name, String.Join(", ", args),
                                    Visit(mc.Object, precedence));
                        }
                        break;
                    //
                    // Summary:
                    //     A node that represents a null coalescing operation, such as (a ?? b) in C#
                    //     or If(a, b) in Visual Basic.
                    case ExpressionType.Coalesce:
                        precedence = Precedence.NullCoalescing;
                        result = String.Format(context.Formatter, "{0} ?? {1}", Visit(be.Left, precedence), Visit(be.Right, precedence));
                        break;
                    //
                    // Summary:
                    //     A conditional operation, such as a > b ? a : b in C# or If(a > b, a, b) in
                    //     Visual Basic.
                    case ExpressionType.Conditional:
                        if (ce.Type == typeof(void))
                        {
                            bool needAccos =
                                ce.IfTrue.NodeType == ExpressionType.Block ||
                                ce.IfFalse.NodeType == ExpressionType.Block;
                            bool elseIf =
                                ce.IfFalse.NodeType == ExpressionType.Conditional && (ce.IfFalse as ConditionalExpression).Type == typeof(void);

                            sb = new StringBuilder(GetKeyword("if"));
                            sb.Append(" (").Append(Visit(ce.Test, precedence)).Append(')');
                            if (needAccos) sb.Append(context.Newline()).Append('{');
                            context.Indent++;
                            sb.Append(context.Newline()).Append(Visit(ce.IfTrue, precedence)).Append(';');
                            context.Indent--;
                            if (needAccos) sb.Append(context.Newline()).Append('}');
                            if (ce.IfFalse.NodeType != ExpressionType.Default)
                            {
                                if (elseIf)
                                {
                                    sb.Append(context.Newline()).Append(GetKeyword("else")).Append(' ');
                                    if (needAccos) sb.Append(context.Newline()).Append('{');
                                    sb.Append(Visit(ce.IfFalse, precedence));
                                }
                                else
                                {
                                    sb.Append(context.Newline()).Append(GetKeyword("else"));
                                    if (needAccos) sb.Append(context.Newline()).Append('{');
                                    context.Indent++;
                                    sb.Append(context.Newline()).Append(Visit(ce.IfFalse, precedence)).Append(';');
                                    context.Indent--;
                                    if (needAccos) sb.Append(context.Newline()).Append('}');
                                }
                            }
                            result = sb.ToString();
                        }
                        else        // ? : conditional operator
                        {
                            sb = new StringBuilder();
                            int oldIndent = context.Indent;
                            context.Indent++;
                            sb.Append(Visit(ce.Test, precedence)).Append(" ?");
                            sb.Append(context.Newline()).Append(Visit(ce.IfTrue, precedence)).Append(" :");
                            context.Indent = oldIndent;
                            if (ce.IfFalse.NodeType != ExpressionType.Conditional) context.Indent++;
                            sb.Append(context.Newline()).Append(Visit(ce.IfFalse, precedence));
                            context.Indent = oldIndent;
                            result = sb.ToString();
                        }
                        break;
                    //
                    // Summary:
                    //     A constant value.
                    case ExpressionType.Constant:
                        var cc = expr as ConstantExpression;
                        string ccToString = cc.ToString();
                        if (IsIdentifier(ccToString))
                            result = expr.Type.IsEnum ?
                                String.Format(context.Formatter, "{0:Type}.{1:Identifier}", expr.Type.Name, ccToString) :
                                String.Format(context.Formatter, "{0:Identifier}", ccToString);
                        else
                        if (context.Hexmode && cc.Type == typeof(byte))
                            result = String.Format(context.Formatter, "{0:Number}", String.Format("0x{0:X2}", (byte)cc.Value));
                        else
                        if (context.Hexmode && cc.Type == typeof(ushort))
                            result = String.Format(context.Formatter, "{0:Number}", String.Format("0x{0:X4}", (ushort)cc.Value));
                        else
                        if (context.Hexmode && cc.Type == typeof(int))
                            result = String.Format(context.Formatter, "{0:Number}", String.Format("0x{0:X}", (int)cc.Value));
                        else
                            result =
                                expr.Type == typeof(string) || expr.Type == typeof(char) ?
                                String.Format(context.Formatter, "{0:String}", ccToString) :
                                    IsNumericType(expr.Type) ?
                                        String.Format(context.Formatter, "{0:Number}", ccToString) :
                                        cc.Value.ToString();
                        break;
                    //
                    // Summary:
                    //     A cast or conversion operation, such as (SampleType)obj in C#or CType(obj,
                    //     SampleType) in Visual Basic.
                    case ExpressionType.Convert:
                    case ExpressionType.ConvertChecked:
                        if (ue.Operand.Type.IsEnum)
                        {
                            result = Visit(ue.Operand, precedence);
                        }
                        else
                        {
                            string typeName = GetTypeName(ue.Type);
                            if (String.IsNullOrEmpty(typeName) || ue.Type == typeof(object) || Nullable.GetUnderlyingType(ue.Type) == ue.Operand.Type)
                            {
                                result = Visit(ue.Operand, precedence);
                            }
                            else
                            {
                                precedence = Precedence.Unary;
                                result = String.Format(context.Formatter, "({0}){1}", typeName, Visit(ue.Operand, precedence));
                            }
                        }
                        break;

                    case ExpressionType.Divide:
                        precedence = Precedence.Multiplicative;
                        result = String.Format(context.Formatter, "{0} / {1}", Visit(be.Left, precedence), Visit(be.Right, precedence));
                        break;
                    //
                    // Summary:
                    //     A node that represents an equality comparison, such as (a == b) in C# or
                    //     (a:b) in Visual Basic.
                    case ExpressionType.Equal:
                        precedence = Precedence.Equality;
                        result = String.Format(context.Formatter, "{0} == {1}", Visit(be.Left, precedence), Visit(be.Right, precedence));
                        break;
                    //
                    // Summary:
                    //     A bitwise or logical XOR operation, such as (a ^ b) in C# or (a Xor b) in
                    //     Visual Basic.
                    case ExpressionType.ExclusiveOr:
                        precedence = Precedence.LogicalXor;
                        result = String.Format(context.Formatter, "{0} ^ {1}", Visit(be.Left, precedence), Visit(be.Right, precedence));
                        break;
                    //
                    // Summary:
                    //     A "greater than" comparison, such as (a > b).
                    case ExpressionType.GreaterThan:
                        precedence = Precedence.Relational;
                        result = String.Format(context.Formatter, "{0} > {1}", Visit(be.Left, precedence), Visit(be.Right, precedence));
                        break;
                    //
                    // Summary:
                    //     A "greater than or equal to" comparison, such as (a >= b).
                    case ExpressionType.GreaterThanOrEqual:
                        precedence = Precedence.Relational;
                        result = String.Format(context.Formatter, "{0} >= {1}", Visit(be.Left, precedence), Visit(be.Right, precedence));
                        break;
                    //
                    // Summary:
                    //     An operation that invokes a delegate or lambda expression, such as sampleDelegate.Invoke().
                    case ExpressionType.Invoke:
                        var inve = expr as InvocationExpression;
                        result = String.Format(context.Formatter, "{0}({1})", Visit(inve.Expression, precedence), String.Join(", ", inve.Arguments.Select(arg => Visit(arg, precedence, Precedence.Sequence))));
                        break;
                    //
                    // Summary:
                    //     A lambda expression, such as a => a + a in C# or Function(a) a + a in Visual
                    //     Basic.
                    case ExpressionType.Lambda:
                        var lmbx = expr as LambdaExpression;
                        var body = Visit(lmbx.Body, precedence, Precedence.Primary);
                        var parameters = String.Format(context.Formatter,
                                    lmbx.Parameters.Count == 1 ? "{0}" : "({0})",
                                    String.Join(", ", lmbx.Parameters.Select(par => Visit(par, precedence, Precedence.Sequence)))
                                );
                        if (body.Contains('\n'))
                        {
                            sb = new StringBuilder();
                            sb.Append(parameters).Append(" =>");
                            context.Indent++;
                            sb.Append(context.Newline());
                            sb.Append(Visit(lmbx.Body, precedence, Precedence.Primary));
                            context.Indent++;
                            result = sb.ToString();
                        }
                        else
                        {
                            result = String.Format(context.Formatter, "{0} => {1}",
                                parameters,
                                body
                            );
                        }
                        break;
                    //
                    // Summary:
                    //     A bitwise left-shift operation, such as (a << b).
                    case ExpressionType.LeftShift:
                        precedence = Precedence.Shift;
                        result = String.Format(context.Formatter, "{0} << {1}", Visit(be.Left, precedence), Visit(be.Right, precedence));
                        break;
                    //
                    // Summary:
                    //     A "less than" comparison, such as (a < b).
                    case ExpressionType.LessThan:
                        precedence = Precedence.Relational;
                        result = String.Format(context.Formatter, "{0} < {1}", Visit(be.Left, precedence), Visit(be.Right, precedence));
                        break;
                    //
                    // Summary:
                    //     A "less than or equal to" comparison, such as (a <= b).
                    case ExpressionType.LessThanOrEqual:
                        precedence = Precedence.Relational;
                        result = String.Format(context.Formatter, "{0} <= {1}", Visit(be.Left, precedence), Visit(be.Right, precedence));
                        break;
                    //
                    // Summary:
                    //     An operation that creates a new System.Collections.IEnumerable object and
                    //     initializes it from a list of elements, such as new List<SampleType>(){ a,
                    //     b, c } in C# or Dim sampleList:{ a, b, c } in Visual Basic.
                    case ExpressionType.ListInit:
                        var lexp = expr as ListInitExpression;
                        result = Visit(lexp.NewExpression, precedence, Precedence.SameLevel, lexp.Initializers.Any());
                        if (lexp.Initializers.Any())
                        {
                            result += " { " + String.Join(", ", lexp.Initializers.Select(ei =>
                                (ei.Arguments.Count() > 1 ? "{ " : "") +
                                String.Join(", ", ei.Arguments.Select(arg => Visit(arg, Precedence.Sequence))) +
                                (ei.Arguments.Count() > 1 ? "} " : "")
                            )) + " }";
                        }
                        break;
                    //
                    // Summary:
                    //     An operation that reads from a field or property, such as obj.SampleProperty.
                    case ExpressionType.MemberAccess:
                        string obj = Visit(me.Expression, precedence);
                        result = obj == String.Empty ?
                            me.Member.Name :
                            String.Format(context.Formatter, "{0}.{1:Identifier}", obj, me.Member.Name);
                        break;

                    //
                    // Summary:
                    //     An arithmetic remainder operation, such as (a % b) in C# or (a Mod b) in
                    //     Visual Basic.
                    case ExpressionType.Modulo:
                        precedence = Precedence.Multiplicative;
                        result = String.Format(context.Formatter, "{0} % {1}", Visit(be.Left, precedence), Visit(be.Right, precedence));
                        break;
                    //
                    // Summary:
                    //     A multiplication operation, such as (a * b), without overflow checking, for
                    //     numeric operands.
                    case ExpressionType.MultiplyChecked:
                    case ExpressionType.Multiply:
                        precedence = Precedence.Multiplicative;
                        result = String.Format(context.Formatter, "{0} * {1}", Visit(be.Left, precedence), Visit(be.Right, precedence));
                        break;
                    //
                    // Summary:
                    //     An arithmetic negation operation, such as (-a). The object a should not be
                    //     modified in place.
                    case ExpressionType.Negate:
                    case ExpressionType.NegateChecked:
                        precedence = Precedence.Unary;
                        result = String.Format(context.Formatter, "-{0}", Visit(ue.Operand, precedence));
                        break;
                    //
                    // Summary:
                    //     A unary plus operation, such as (+a). The result of a predefined unary plus
                    //     operation is the value of the operand, but user-defined implementations might
                    //     have unusual results.
                    case ExpressionType.UnaryPlus:
                        precedence = Precedence.Unary;
                        result = String.Format(context.Formatter, "+{0}", Visit(ue.Operand, precedence));
                        break;

                    //
                    // Summary:
                    //     An operation that creates a new object and initializes one or more of its
                    //     members, such as new Point { X: Y:2 } in C# or New Point With {.X =
                    //     1, .Y:2} in Visual Basic.
                    case ExpressionType.MemberInit:
                        {
                            var mbi = expr as MemberInitExpression;
                            bool hasBindings = mbi.Bindings.Any();
                            sb = new StringBuilder();
                            sb.Append(Visit(mbi.NewExpression, precedence, Precedence.Highest, hasBindings));
                            if (hasBindings)
                            {
                                sb.Append(context.Newline()).Append('{');
                                context.Indent++;
                                sb.Append(context.Newline());
                                sb.Append(
                                    String.Join("," + context.Newline(), mbi.Bindings.Select(bi => VisitMemberBinding(bi, precedence)))
                                );
                                context.Indent--;
                                sb.Append(context.Newline());
                                sb.Append('}');
                            }
                            result = sb.ToString();
                        }
                        break;

                    //
                    // Summary:
                    //     An operation that calls a constructor to create a new object, such as new
                    //     SampleType().
                    case ExpressionType.New:
                        {
                            var newx = expr as NewExpression;
                            bool hasArguments = newx.Arguments != null && newx.Arguments.Any();
                            sb = new StringBuilder(GetKeyword("new"));
                            sb.Append(' ');
                            sb.Append(GetTypeName(newx.Type));
                            if (hasArguments || !hasInitializer)
                            {
                                sb.Append('(');
                                sb.Append(String.Join(", ", newx.Arguments.Select(arg => Visit(arg, Precedence.Sequence))));
                                sb.Append(')');
                            }
                            result = sb.ToString();
                        }
                        break;
                    //
                    // Summary:
                    //     An operation that creates a new one-dimensional array and initializes it
                    //     from a list of elements, such as new SampleType[]{a, b, c} in C# or New SampleType(){a,
                    //     b, c} in Visual Basic.
                    case ExpressionType.NewArrayInit:
                        var arrx = expr as NewArrayExpression;
                        if (arrx.Expressions == null || !arrx.Expressions.Any())
                            result = String.Format(context.Formatter, "{0:Keyword} {1}[{2:Number}]",
                                "new",
                                GetTypeName(arrx.Type.GetElementType()),
                                "0"
                            );
                        else
                            result = String.Format(context.Formatter, "{0:Keyword} {1} {2}",
                                "new",
                                GetTypeName(arrx.Type),
                                String.Format("{{ {0} }}", String.Join(", ", arrx.Expressions.Select(it => Visit(it, precedence, Precedence.Sequence))))
                            );
                        break;

                    //
                    // Summary:
                    //     An operation that creates a new array, in which the bounds for each dimension
                    //     are specified, such as new SampleType[dim1, dim2] in C# or New SampleType(dim1,
                    //     dim2) in Visual Basic.
                    case ExpressionType.NewArrayBounds:
                        var arrbx = expr as NewArrayExpression;
                        result = String.Format(context.Formatter, "{0:Keyword} {1}[{2}]",
                            "new",
                            GetTypeName(arrbx.Type.GetElementType()),
                            String.Join(", ", arrbx.Expressions.Select(it => Visit(it, precedence, Precedence.Sequence)))
                        );
                        break;
                    //
                    // Summary:
                    //     A bitwise complement or logical negation operation. In C#, it is equivalent
                    //     to (~a) for integral types and to (!a) for Boolean values. In Visual Basic,
                    //     it is equivalent to (Not a). The object a should not be modified in place.
                    case ExpressionType.Not:
                        if (ue.Operand.Type == typeof(bool))
                            result = String.Format(context.Formatter, "!{0}", Visit(ue.Operand, precedence, Precedence.Unary));
                        result = String.Format(context.Formatter, "~{0}", Visit(ue.Operand, precedence, Precedence.Unary));
                        break;
                    //
                    // Summary:
                    //     An inequality comparison, such as (a != b) in C# or (a <> b) in Visual Basic.
                    case ExpressionType.NotEqual:
                        precedence = Precedence.Equality;
                        result = String.Format(context.Formatter, "{0} != {1}", Visit(be.Left, precedence), Visit(be.Right, precedence));
                        break;
                    //
                    // Summary:
                    //     A bitwise or logical OR operation, such as (a | b) in C# or (a Or b) in Visual
                    //     Basic.
                    case ExpressionType.Or:
                        precedence = Precedence.LogicalOr;
                        result = String.Format(context.Formatter, "{0} | {1}", Visit(be.Left, precedence), Visit(be.Right, precedence));
                        break;
                    //
                    // Summary:
                    //     A short-circuiting conditional OR operation, such as (a || b) in C# or (a
                    //     OrElse b) in Visual Basic.
                    case ExpressionType.OrElse:
                        precedence = Precedence.ConditionalOr;
                        result = String.Format(context.Formatter, "({0} || {1})", Visit(be.Left, precedence), Visit(be.Right, precedence));
                        break;
                    //
                    // Summary:
                    //     A reference to a parameter or variable that is defined in the context of
                    //     the expression. For more information, see System.Linq.Expressions.ParameterExpression.
                    case ExpressionType.Parameter:
                        var parx = expr as ParameterExpression;
                        result = String.Format(context.Formatter, "{0:Identifier}", context.GetVarName(parx));
                        break;
                    //
                    // Summary:
                    //     A mathematical operation that raises a number to a power, such as (a ^ b)
                    //     in Visual Basic.
                    case ExpressionType.Power:
                        precedence = Precedence.Exponentiation;
                        result = String.Format(context.Formatter, "{0}^{1})", Visit(be.Left, precedence), Visit(be.Right, precedence));
                        break;
                    //
                    // Summary:
                    //     An expression that has a constant value of type System.Linq.Expressions.Expression.
                    //     A System.Linq.Expressions.ExpressionType.Quote node can contain references
                    //     to parameters that are defined in the context of the expression it represents.
                    case ExpressionType.Quote:
                        result = String.Format(context.Formatter, "{0:Quote}", Visit(ue.Operand, precedence, Precedence.Primary));
                        break;

                    //
                    // Summary:
                    //     A bitwise right-shift operation, such as (a >> b).
                    case ExpressionType.RightShift:
                        precedence = Precedence.Shift;
                        result = String.Format(context.Formatter, "{0} >> {1}", Visit(be.Left, precedence), Visit(be.Right, precedence));
                        break;
                    //
                    // Summary:
                    //     A subtraction operation, such as (a - b), without overflow checking, for
                    //     numeric operands.
                    case ExpressionType.Subtract:
                    case ExpressionType.SubtractChecked:
                        precedence = Precedence.Additive;
                        result = String.Format(context.Formatter, "{0} - {1}", Visit(be.Left, precedence), Visit(be.Right, precedence));
                        break;
                    //
                    // Summary:
                    //     An explicit reference or boxing conversion in which null is supplied if the
                    //     conversion fails, such as (obj as SampleType) in C# or TryCast(obj, SampleType)
                    //     in Visual Basic.
                    case ExpressionType.TypeAs:
                        precedence = Precedence.Unary;
                        result = String.Format(context.Formatter, "{0} as {1}", Visit(ue.Operand, precedence), ue.Type.ToString());
                        break;
                    //
                    // Summary:
                    //     A type test, such as obj is SampleType in C# or TypeOf obj is SampleType
                    //     in Visual Basic.
                    case ExpressionType.TypeIs:
                        var tbex = expr as TypeBinaryExpression;
                        precedence = Precedence.Unary;
                        result = String.Format(context.Formatter, "{0} is {1}", Visit(tbex.Expression, precedence), tbex.TypeOperand.ToString());
                        break;
                    //
                    // Summary:
                    //     An assignment operation, such as (a = b).
                    case ExpressionType.Assign:
                        precedence = Precedence.Assignment;
                        result = String.Format(context.Formatter, "{0} = {1}", Visit(be.Left, precedence), Visit(be.Right, precedence));
                        break;
                    //
                    // Summary:
                    //     A block of expressions.
                    case ExpressionType.Block:
                        var blkx = expr as BlockExpression;
                        sb = new StringBuilder();
                        sb.Append(context.Newline()).Append('{');
                        context.Indent++;

                        for (int n = 0; n < blkx.Variables.Count; n++)
                        {
                            var variable = blkx.Variables[n];
                            sb.Append(context.Newline()).AppendFormat("{0} {1};", variable.Type.ToString(), context.GetVarName(variable));
                        }
                        if (blkx.Variables.Any())
                            sb.Append(context.Newline());
                        foreach (var line in blkx.Expressions)
                        {
                            sb.Append(context.Newline()).AppendFormat("{0};", Visit(line, precedence, Precedence.Primary));
                        }
                        sb.Append(context.Newline(-1)).Append('}').Append(context.Newline());
                        result = sb.ToString();
                        break;
                    //
                    // Summary:
                    //     Debugging information.
                    case ExpressionType.DebugInfo:
                        result = "TODO6";
                        break;
                    //
                    // Summary:
                    //     A unary decrement operation, such as (a - 1) in C# and Visual Basic. The
                    //     object a should not be modified in place.
                    case ExpressionType.Decrement:
                        result = String.Format(context.Formatter, "{0} - {1}", Visit(ue.Operand, precedence, Precedence.Additive), System.Convert.ChangeType(1, ue.Type));
                        break;
                    //
                    // Summary:
                    //     A dynamic operation.
                    case ExpressionType.Dynamic:
                        result = "TODO7";
                        break;
                    //
                    // Summary:
                    //     A default value.
                    case ExpressionType.Default:
                        var defx = expr as DefaultExpression;
                        result = String.Format(context.Formatter, "{0:Keyword}({1:Type})", "default", defx.Type.ToString());
                        break;
                    //
                    // Summary:
                    //     An extension expression.
                    case ExpressionType.Extension:
                        result = "TODO8";
                        break;
                    //
                    // Summary:
                    //     A "go to" expression, such as goto Label in C# or GoTo Label in Visual Basic.
                    case ExpressionType.Goto:
                        var gotx = expr as GotoExpression;
                        switch (gotx.Kind)
                        {
                            case GotoExpressionKind.Break:
                                result = "break;";
                                break;
                            case GotoExpressionKind.Continue:
                                result = "continue;";
                                break;
                            case GotoExpressionKind.Goto:
                                result = String.Format(context.Formatter, "goto {0};", gotx.Target.Name);
                                break;
                            default: // GotoExpressionKind.result = :
                                if (gotx.Value == null)
                                    result = "result = ;";
                                else
                                    result = String.Format(context.Formatter, "result =  {0};", Visit(gotx.Value, precedence, Precedence.Highest));
                                break;
                        }
                        break;

                    //
                    // Summary:
                    //     A unary increment operation, such as (a + 1) in C# and Visual Basic. The
                    //     object a should not be modified in place.
                    case ExpressionType.Increment:
                        result = String.Format(context.Formatter, "{0} + {1}", Visit(ue.Operand, precedence, Precedence.Additive), System.Convert.ChangeType(1, ue.Type));
                        break;
                    //
                    // Summary:
                    //     An index operation or an operation that accesses a property that takes arguments.
                    case ExpressionType.Index:
                        var idxx = expr as IndexExpression;
                        var field = idxx.Object as MemberExpression;
                        result = String.Format(context.Formatter, "{0}[{1}]", Visit(field, precedence, Precedence.Primary),
                            String.Join(", ", idxx.Arguments.Select(arg => Visit(arg, precedence, Precedence.Sequence))));
                        break;
                    //
                    // Summary:
                    //     A label.
                    case ExpressionType.Label:
                        var labx = expr as LabelExpression;
                        result = String.Format(context.Formatter, "{0}:", labx.Target.Name);
                        break;
                    //
                    // Summary:
                    //     A list of run-time variables. For more information, see System.Linq.Expressions.RuntimeVariablesExpression.
                    case ExpressionType.RuntimeVariables:
                        result = "TODO9";
                        break;
                    //
                    // Summary:
                    //     A loop, such as for or while.
                    case ExpressionType.Loop:
                        result = "TODO10";
                        break;

                    //
                    // Summary:
                    //     A switch operation, such as switch in C# or Select case ExpressionType.in Visual Basic.
                    case ExpressionType.Switch:
                        result = "TODO11";
                        break;
                    //
                    // Summary:
                    //     An operation that throws an exception, such as throw new Exception().
                    case ExpressionType.Throw:
                        result = "TODO12";
                        break;
                    //
                    // Summary:
                    //     A try-catch expression.
                    case ExpressionType.Try:
                        result = "TODO13";
                        break;
                    //
                    // Summary:
                    //     An unbox value type operation, such as unbox and unbox.any instructions in
                    //     MSIL.
                    case ExpressionType.Unbox:
                        result = "TODO14";
                        break;
                    //
                    // Summary:
                    //     An addition compound assignment operation, such as (a += b), without overflow
                    //     checking, for numeric operands.
                    case ExpressionType.AddAssign:
                        precedence = Precedence.Assignment;
                        result = String.Format(context.Formatter, "{0} += {1}", Visit(be.Left, precedence), Visit(be.Right, precedence));
                        break;
                    //
                    // Summary:
                    //     A bitwise or logical AND compound assignment operation, such as (a &= b)
                    //     in C#.
                    case ExpressionType.AndAssign:
                        precedence = Precedence.Assignment;
                        result = String.Format(context.Formatter, "{0} &= {1}", Visit(be.Left, precedence), Visit(be.Right, precedence));
                        break;
                    //
                    // Summary:
                    //     An division compound assignment operation, such as (a /= b), for numeric
                    //     operands.
                    case ExpressionType.DivideAssign:
                        precedence = Precedence.Assignment;
                        result = String.Format(context.Formatter, "{0} /= {1}", Visit(be.Left, precedence), Visit(be.Right, precedence));
                        break;
                    //
                    // Summary:
                    //     A bitwise or logical XOR compound assignment operation, such as (a ^= b)
                    //     in C#.
                    case ExpressionType.ExclusiveOrAssign:
                        precedence = Precedence.Assignment;
                        result = String.Format(context.Formatter, "{0} ^= {1}", Visit(be.Left, precedence), Visit(be.Right, precedence));
                        break;
                    //
                    // Summary:
                    //     A bitwise left-shift compound assignment, such as (a <<= b).
                    case ExpressionType.LeftShiftAssign:
                        precedence = Precedence.Assignment;
                        result = String.Format(context.Formatter, "{0} <<= {1}", Visit(be.Left, precedence), Visit(be.Right, precedence));
                        break;
                    //
                    // Summary:
                    //     An arithmetic remainder compound assignment operation, such as (a %= b) in
                    //     C#.
                    case ExpressionType.ModuloAssign:
                        precedence = Precedence.Assignment;
                        result = String.Format(context.Formatter, "{0} %= {1}", Visit(be.Left, precedence), Visit(be.Right, precedence));
                        break;
                    //
                    // Summary:
                    //     A multiplication compound assignment operation, such as (a *= b), without
                    //     overflow checking, for numeric operands.
                    case ExpressionType.MultiplyAssign:
                        precedence = Precedence.Assignment;
                        result = String.Format(context.Formatter, "{0} *= {1}", Visit(be.Left, precedence), Visit(be.Right, precedence));
                        break;
                    //
                    // Summary:
                    //     A bitwise or logical OR compound assignment, such as (a |= b) in C#.
                    case ExpressionType.OrAssign:
                        precedence = Precedence.Assignment;
                        result = String.Format(context.Formatter, "{0} |= {1}", Visit(be.Left, precedence), Visit(be.Right, precedence));
                        break;
                    //
                    // Summary:
                    //     A compound assignment operation that raises a number to a power, such as
                    //     (a ^= b) in Visual Basic.
                    case ExpressionType.PowerAssign:
                        precedence = Precedence.Assignment;
                        result = "TODO15";
                        break;
                    //
                    // Summary:
                    //     A bitwise right-shift compound assignment operation, such as (a >>= b).
                    case ExpressionType.RightShiftAssign:
                        precedence = Precedence.Assignment;
                        result = String.Format(context.Formatter, "{0} >>= {1}", Visit(be.Left, precedence), Visit(be.Right, precedence));
                        break;
                    //
                    // Summary:
                    //     A subtraction compound assignment operation, such as (a -= b), without overflow
                    //     checking, for numeric operands.
                    case ExpressionType.SubtractAssign:
                        precedence = Precedence.Assignment;
                        result = String.Format(context.Formatter, "{0} -= {1}", Visit(be.Left, precedence), Visit(be.Right, precedence));
                        break;
                    //
                    // Summary:
                    //     An addition compound assignment operation, such as (a += b), with overflow
                    //     checking, for numeric operands.
                    case ExpressionType.AddAssignChecked:
                        precedence = Precedence.Assignment;
                        result = String.Format(context.Formatter, "{0} += {1}", Visit(be.Left, precedence), Visit(be.Right, precedence));
                        break;
                    //
                    // Summary:
                    //     A multiplication compound assignment operation, such as (a *= b), that has
                    //     overflow checking, for numeric operands.
                    case ExpressionType.MultiplyAssignChecked:
                        precedence = Precedence.Assignment;
                        result = String.Format(context.Formatter, "{0} *= {1}", Visit(be.Left, precedence), Visit(be.Right, precedence));
                        break;
                    //
                    // Summary:
                    //     A subtraction compound assignment operation, such as (a -= b), that has overflow
                    //     checking, for numeric operands.
                    case ExpressionType.SubtractAssignChecked:
                        precedence = Precedence.Assignment;
                        result = String.Format(context.Formatter, "{0} -= {1}", Visit(be.Left, precedence, Precedence.Assignment), Visit(be.Right, precedence, Precedence.Assignment));
                        break;
                    //
                    // Summary:
                    //     A unary prefix increment, such as (++a). The object a should be modified
                    //     in place.
                    case ExpressionType.PreIncrementAssign:
                        precedence = Precedence.Unary;
                        result = String.Format(context.Formatter, "++{0}", Visit(ue.Operand, precedence));
                        break;
                    //
                    // Summary:
                    //     A unary prefix decrement, such as (--a). The object a should be modified
                    //     in place.
                    case ExpressionType.PreDecrementAssign:
                        result = String.Format(context.Formatter, "--{0}", Visit(ue.Operand, precedence));
                        break;
                    //
                    // Summary:
                    //     A unary postfix increment, such as (a++). The object a should be modified
                    //     in place.
                    case ExpressionType.PostIncrementAssign:
                        precedence = Precedence.Primary;
                        result = String.Format(context.Formatter, "{0}++", Visit(ue.Operand, precedence));
                        break;
                    //
                    // Summary:
                    //     A unary postfix decrement, such as (a--). The object a should be modified
                    //     in place.
                    case ExpressionType.PostDecrementAssign:
                        precedence = Precedence.Primary;
                        result = String.Format(context.Formatter, "{0}--", Visit(ue.Operand, precedence));
                        break;
                    //
                    // Summary:
                    //     An exact type test.
                    case ExpressionType.TypeEqual:
                        result = "TODO16";
                        break;
                    //
                    // Summary:
                    //     A ones complement operation, such as (~a) in C#.
                    case ExpressionType.OnesComplement:
                        result = String.Format(context.Formatter, "~{0}", Visit(ue.Operand, precedence, Precedence.Unary));
                        break;
                    //
                    // Summary:
                    //     A true condition value.
                    case ExpressionType.IsTrue:
                        result = "TODO18";
                        break;
                    //
                    // Summary:
                    //     A false condition value.
                    case ExpressionType.IsFalse:
                        result = "TODO19";
                        break;
                }

                //result = String.Format(context.Formatter, "{0:Comment}", (int)parentPrecedence + "|" + (int)precedence) + result;

                if (parentPrecedence > precedence && parentPrecedence != Precedence.Highest)
                {
                    result = "(" + result + ")";
                }
                //result = "↑" + result + "↓";

                context.Level--;
                return result;

            }
        }

        public static string Evaluate(this Expression expr, BaseFormatter formatter = null, bool hexMode = false)
        {
            return new Visitor().Convert(expr, formatter, hexMode);
        }
    }
}
