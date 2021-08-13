using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Assembler
{
    public static class ExpressionEvaluator
    {
        private class Visitor
        {
            public Visitor()
            {
            }

            private class VisitContext
            {
                public string LinePrefix { get; set; }
                public bool Hexmode { get; set; }
                public int Indent { get; set; }
                public Dictionary<ParameterExpression, string> Names { get; set; }

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

                public VisitContext()
                {
                    Names = new Dictionary<ParameterExpression, string>();
                    Indent = 0;
                    varCounter = 'a';
                }
            }

            public string Visit(Expression expr, bool hexMode = false, string linePrefix = "")
            {
                return Visit(new VisitContext() { Hexmode = hexMode, LinePrefix = linePrefix }, expr);
            }

            private string Visit(VisitContext context, Expression expr)
            {
                var ue = expr as UnaryExpression;
                var be = expr as BinaryExpression;
                var mc = expr as MethodCallExpression;
                var ce = expr as ConditionalExpression;
                var me = expr as MemberExpression;
                if (expr == null) return String.Empty;
                switch (expr.NodeType)
                {
                    // Summary:
                    //     An addition operation, such as a + b, without overflow checking, for numeric
                    //     operands.
                    case ExpressionType.Add:
                    case ExpressionType.AddChecked:
                        return String.Format("({0} + {1})", Visit(context, be.Left), Visit(context, be.Right));
                    //
                    // Summary:
                    //     A bitwise or logical AND operation, such as (a & b) in C# and (a And b) in
                    //     Visual Basic.
                    case ExpressionType.And:
                        return String.Format("({0} && {1})", Visit(context, be.Left), Visit(context, be.Right));
                    //
                    // Summary:
                    //     A conditional AND operation that evaluates the second operand only if the
                    //     first operand evaluates to true. It corresponds to (a && b) in C# and (a
                    //     AndAlso b) in Visual Basic.
                    case ExpressionType.AndAlso:
                        return String.Format("({0} & {1})", Visit(context, be.Left), Visit(context, be.Right));
                    //
                    // Summary:
                    //     An operation that obtains the length of a one-dimensional array, such as
                    //     array.Length.
                    case ExpressionType.ArrayLength:
                        return String.Format("{0}.Length", Visit(context, ue.Operand));
                    //
                    // Summary:
                    //     An indexing operation in a one-dimensional array, such as array[index] in
                    //     C# or array(index) in Visual Basic.
                    case ExpressionType.ArrayIndex:
                        return String.Format("{0}[{1}]", Visit(context, be.Left), Visit(context, be.Right));
                    //
                    // Summary:
                    //     A method call, such as in the obj.sampleMethod() expression.
                    case ExpressionType.Call:
                        return String.Format("{0}({1})", mc.Method.Name, String.Join(", ", mc.Arguments.Select(arg => Visit(context, arg))));
                    //
                    // Summary:
                    //     A node that represents a null coalescing operation, such as (a ?? b) in C#
                    //     or If(a, b) in Visual Basic.
                    case ExpressionType.Coalesce:
                        return String.Format("({0} ?? {1})", Visit(context, be.Left), Visit(context, be.Right));
                    //
                    // Summary:
                    //     A conditional operation, such as a > b ? a : b in C# or If(a > b, a, b) in
                    //     Visual Basic.
                    case ExpressionType.Conditional:
                        return String.Format("{0} ? {1} : {2}", Visit(context, ce.Test), Visit(context, ce.IfTrue), Visit(context, ce.IfFalse));
                    //
                    // Summary:
                    //     A constant value.
                    case ExpressionType.Constant:
                        var cc = expr as ConstantExpression;
                        if (context.Hexmode && cc.Type == typeof(byte))
                            return String.Format("0x{0:X}", (byte)cc.Value);
                        if (context.Hexmode && cc.Type == typeof(ushort))
                            return String.Format("0x{0:X}", (ushort)cc.Value);
                        if (context.Hexmode && cc.Type == typeof(int))
                            return String.Format("0x{0:X}", (int)cc.Value);
                        return cc.Value.ToString();
                    //
                    // Summary:
                    //     A cast or conversion operation, such as (SampleType)obj in C#or CType(obj,
                    //     SampleType) in Visual Basic. For a numeric conversion, if the converted value
                    //     is too large for the destination type, no exception is thrown.
                    case ExpressionType.Convert:
                    case ExpressionType.ConvertChecked:
                        return String.Format("(({0}){1})", ue.Type.ToString(), Visit(context, ue.Operand));

                    case ExpressionType.Divide:
                        return String.Format("({0} / {1})", Visit(context, be.Left), Visit(context, be.Right));
                    //
                    // Summary:
                    //     A node that represents an equality comparison, such as (a == b) in C# or
                    //     (a:b) in Visual Basic.
                    case ExpressionType.Equal:
                        return String.Format("({0} == {1})", Visit(context, be.Left), Visit(context, be.Right));
                    //
                    // Summary:
                    //     A bitwise or logical XOR operation, such as (a ^ b) in C# or (a Xor b) in
                    //     Visual Basic.
                    case ExpressionType.ExclusiveOr:
                        return String.Format("({0} ^ {1})", Visit(context, be.Left), Visit(context, be.Right));
                    //
                    // Summary:
                    //     A "greater than" comparison, such as (a > b).
                    case ExpressionType.GreaterThan:
                        return String.Format("({0} > {1})", Visit(context, be.Left), Visit(context, be.Right));
                    //
                    // Summary:
                    //     A "greater than or equal to" comparison, such as (a >= b).
                    case ExpressionType.GreaterThanOrEqual:
                        return String.Format("({0} >= {1})", Visit(context, be.Left), Visit(context, be.Right));
                    //
                    // Summary:
                    //     An operation that invokes a delegate or lambda expression, such as sampleDelegate.Invoke().
                    case ExpressionType.Invoke:
                        var inve = expr as InvocationExpression;
                        return String.Format("{0}({1})", Visit(context, inve.Expression), String.Join(", ", inve.Arguments.Select(arg => Visit(context, arg))));
                    //
                    // Summary:
                    //     A lambda expression, such as a => a + a in C# or Function(a) a + a in Visual
                    //     Basic.
                    case ExpressionType.Lambda:
                        var lmbe = expr as LambdaExpression;
                        return String.Format("({0}) => {1})", String.Join(", ", lmbe.Parameters.Select(par => Visit(context, par))), Visit(context, lmbe.Body));
                    //
                    // Summary:
                    //     A bitwise left-shift operation, such as (a << b).
                    case ExpressionType.LeftShift:
                        return String.Format("({0} << {1})", Visit(context, be.Left), Visit(context, be.Right));
                    //
                    // Summary:
                    //     A "less than" comparison, such as (a < b).
                    case ExpressionType.LessThan:
                        return String.Format("({0} < {1})", Visit(context, be.Left), Visit(context, be.Right));
                    //
                    // Summary:
                    //     A "less than or equal to" comparison, such as (a <= b).
                    case ExpressionType.LessThanOrEqual:
                        return String.Format("({0} <= {1})", Visit(context, be.Left), Visit(context, be.Right));
                    //
                    // Summary:
                    //     An operation that creates a new System.Collections.IEnumerable object and
                    //     initializes it from a list of elements, such as new List<SampleType>(){ a,
                    //     b, c } in C# or Dim sampleList:{ a, b, c } in Visual Basic.
                    case ExpressionType.ListInit:
                        return "TODO";
                    //
                    // Summary:
                    //     An operation that reads from a field or property, such as obj.SampleProperty.
                    case ExpressionType.MemberAccess:
                        return String.Format("{0}.{1}", Visit(context, me.Expression), me.Member.Name);
                    //
                    // Summary:
                    //     An operation that creates a new object and initializes one or more of its
                    //     members, such as new Point { X: Y:2 } in C# or New Point With {.X =
                    //     1, .Y:2} in Visual Basic.
                    case ExpressionType.MemberInit:
                        return "TODO";
                    //
                    // Summary:
                    //     An arithmetic remainder operation, such as (a % b) in C# or (a Mod b) in
                    //     Visual Basic.
                    case ExpressionType.Modulo:
                        return String.Format("({0} % {1})", Visit(context, be.Left), Visit(context, be.Right));
                    //
                    // Summary:
                    //     A multiplication operation, such as (a * b), without overflow checking, for
                    //     numeric operands.
                    case ExpressionType.MultiplyChecked:
                    case ExpressionType.Multiply:
                        return String.Format("({0} * {1})", Visit(context, be.Left), Visit(context, be.Right));
                    //
                    // Summary:
                    //     An arithmetic negation operation, such as (-a). The object a should not be
                    //     modified in place.
                    case ExpressionType.Negate:
                    case ExpressionType.NegateChecked:
                        return String.Format("-{0}", Visit(context, ue.Operand));
                    //
                    // Summary:
                    //     A unary plus operation, such as (+a). The result of a predefined unary plus
                    //     operation is the value of the operand, but user-defined implementations might
                    //     have unusual results.
                    case ExpressionType.UnaryPlus:
                        return String.Format("+{0}", Visit(context, ue.Operand));
                    //
                    // Summary:
                    //     An operation that calls a constructor to create a new object, such as new
                    //     SampleType().
                    case ExpressionType.New:
                        var newx = expr as NewExpression;
                        return "TODO";
                    //
                    // Summary:
                    //     An operation that creates a new one-dimensional array and initializes it
                    //     from a list of elements, such as new SampleType[]{a, b, c} in C# or New SampleType(){a,
                    //     b, c} in Visual Basic.
                    case ExpressionType.NewArrayInit:
                        return "TODO";
                    //
                    // Summary:
                    //     An operation that creates a new array, in which the bounds for each dimension
                    //     are specified, such as new SampleType[dim1, dim2] in C# or New SampleType(dim1,
                    //     dim2) in Visual Basic.
                    case ExpressionType.NewArrayBounds:
                        return "TODO";
                    //
                    // Summary:
                    //     A bitwise complement or logical negation operation. In C#, it is equivalent
                    //     to (~a) for integral types and to (!a) for Boolean values. In Visual Basic,
                    //     it is equivalent to (Not a). The object a should not be modified in place.
                    case ExpressionType.Not:
                        if (ue.Operand.Type == typeof(bool))
                            return String.Format("!{0}", Visit(context, ue.Operand));
                        return String.Format("~{0}", Visit(context, ue.Operand));
                    //
                    // Summary:
                    //     An inequality comparison, such as (a != b) in C# or (a <> b) in Visual Basic.
                    case ExpressionType.NotEqual:
                        return String.Format("({0} != {1})", Visit(context, be.Left), Visit(context, be.Right));
                    //
                    // Summary:
                    //     A bitwise or logical OR operation, such as (a | b) in C# or (a Or b) in Visual
                    //     Basic.
                    case ExpressionType.Or:
                        return String.Format("({0} | {1})", Visit(context, be.Left), Visit(context, be.Right));
                    //
                    // Summary:
                    //     A short-circuiting conditional OR operation, such as (a || b) in C# or (a
                    //     OrElse b) in Visual Basic.
                    case ExpressionType.OrElse:
                        return String.Format("({0} || {1})", Visit(context, be.Left), Visit(context, be.Right));
                    //
                    // Summary:
                    //     A reference to a parameter or variable that is defined in the context of
                    //     the expression. For more information, see System.Linq.Expressions.ParameterExpression.
                    case ExpressionType.Parameter:
                        var parx = expr as ParameterExpression;
                        return String.Format("{0}", context.GetVarName(parx));
                    //
                    // Summary:
                    //     A mathematical operation that raises a number to a power, such as (a ^ b)
                    //     in Visual Basic.
                    case ExpressionType.Power:
                        return "TODO";
                    //
                    // Summary:
                    //     An expression that has a constant value of type System.Linq.Expressions.Expression.
                    //     A System.Linq.Expressions.ExpressionType.Quote node can contain references
                    //     to parameters that are defined in the context of the expression it represents.
                    case ExpressionType.Quote:
                        return Visit(context, ue.Operand);
                    //
                    // Summary:
                    //     A bitwise right-shift operation, such as (a >> b).
                    case ExpressionType.RightShift:
                        return String.Format("({0} >> {1})", Visit(context, be.Left), Visit(context, be.Right));
                    //
                    // Summary:
                    //     A subtraction operation, such as (a - b), without overflow checking, for
                    //     numeric operands.
                    case ExpressionType.Subtract:
                    case ExpressionType.SubtractChecked:
                        return String.Format("({0} - {1})", Visit(context, be.Left), Visit(context, be.Right));
                    //
                    // Summary:
                    //     An explicit reference or boxing conversion in which null is supplied if the
                    //     conversion fails, such as (obj as SampleType) in C# or TryCast(obj, SampleType)
                    //     in Visual Basic.
                    case ExpressionType.TypeAs:
                        return String.Format("{0} as {1})", Visit(context, ue.Operand), ue.Type.ToString());
                    //
                    // Summary:
                    //     A type test, such as obj is SampleType in C# or TypeOf obj is SampleType
                    //     in Visual Basic.
                    case ExpressionType.TypeIs:
                        var tbex = expr as TypeBinaryExpression;
                        return String.Format("{0} is {1})", Visit(context, tbex.Expression), tbex.TypeOperand.ToString());
                    //
                    // Summary:
                    //     An assignment operation, such as (a = b).
                    case ExpressionType.Assign:
                        return String.Format("{0} = {1}", Visit(context, be.Left), Visit(context, be.Right));
                    //
                    // Summary:
                    //     A block of expressions.
                    case ExpressionType.Block:
                        var blkx = expr as BlockExpression;
                        var sb = new StringBuilder();
                        Func<int, StringBuilder> addIndent = (m) =>
                        {
                            for (int n = 0; n < m; n++)
                                sb.Append("    ");
                            return sb;
                        };
                        addIndent(context.Indent++).AppendLine("{").Append(context.LinePrefix);
                        for (int n = 0; n < blkx.Variables.Count; n++)
                        {
                            var variable = blkx.Variables[n];
                            addIndent(context.Indent).AppendFormat("{0} {1};", variable.Type.ToString(), context.GetVarName(variable));
                            sb.AppendLine().Append(context.LinePrefix);
                        }
                        sb.AppendLine().Append(context.LinePrefix);
                        foreach (var line in blkx.Expressions)
                        {
                            addIndent(context.Indent).AppendFormat("{0};", Visit(context, line));
                            sb.AppendLine().Append(context.LinePrefix);
                        }
                        addIndent(--context.Indent).AppendLine("}").Append(context.LinePrefix);
                        return sb.ToString();
                    //
                    // Summary:
                    //     Debugging information.
                    case ExpressionType.DebugInfo:
                        return "TODO";
                    //
                    // Summary:
                    //     A unary decrement operation, such as (a - 1) in C# and Visual Basic. The
                    //     object a should not be modified in place.
                    case ExpressionType.Decrement:
                        return String.Format("({0} - {1})", Visit(context, ue.Operand), Convert.ChangeType(1, ue.Type));
                    //
                    // Summary:
                    //     A dynamic operation.
                    case ExpressionType.Dynamic:
                        return "TODO";
                    //
                    // Summary:
                    //     A default value.
                    case ExpressionType.Default:
                        var defx = expr as DefaultExpression;
                        return String.Format("default({0})", defx.Type.ToString());
                    //
                    // Summary:
                    //     An extension expression.
                    case ExpressionType.Extension:
                        return "TODO";
                    //
                    // Summary:
                    //     A "go to" expression, such as goto Label in C# or GoTo Label in Visual Basic.
                    case ExpressionType.Goto:
                        var gotx = expr as GotoExpression;
                        switch (gotx.Kind)
                        {
                            case GotoExpressionKind.Break:
                                return "break;";
                            case GotoExpressionKind.Continue:
                                return "continue;";
                            case GotoExpressionKind.Goto:
                                return String.Format("goto {0};", gotx.Target.Name);
                            default: // GotoExpressionKind.Return:
                                if (gotx.Value == null)
                                    return "return;";
                                else
                                    return String.Format("return {0};", Visit(context, gotx.Value));
                        }
                    //
                    // Summary:
                    //     A unary increment operation, such as (a + 1) in C# and Visual Basic. The
                    //     object a should not be modified in place.
                    case ExpressionType.Increment:
                        return String.Format("({0} + {1})", Visit(context, ue.Operand), Convert.ChangeType(1, ue.Type));
                    //
                    // Summary:
                    //     An index operation or an operation that accesses a property that takes arguments.
                    case ExpressionType.Index:
                        var idxx = expr as IndexExpression;
                        var field = idxx.Object as MemberExpression;
                        return String.Format("{0}[{1}]", Visit(context, field),
                            String.Join(", ", idxx.Arguments.Select(arg => Visit(context, arg))));
                    //
                    // Summary:
                    //     A label.
                    case ExpressionType.Label:
                        var labx = expr as LabelExpression;
                        return string.Format("{0}:", labx.Target.Name);
                    //
                    // Summary:
                    //     A list of run-time variables. For more information, see System.Linq.Expressions.RuntimeVariablesExpression.
                    case ExpressionType.RuntimeVariables:
                        return "TODO";
                    //
                    // Summary:
                    //     A loop, such as for or while.
                    case ExpressionType.Loop:
                        return "TODO";

                    //
                    // Summary:
                    //     A switch operation, such as switch in C# or Select case ExpressionType.in Visual Basic.
                    case ExpressionType.Switch:
                        return "TODO";
                    //
                    // Summary:
                    //     An operation that throws an exception, such as throw new Exception().
                    case ExpressionType.Throw:
                        return "TODO";
                    //
                    // Summary:
                    //     A try-catch expression.
                    case ExpressionType.Try:
                        return "TODO";
                    //
                    // Summary:
                    //     An unbox value type operation, such as unbox and unbox.any instructions in
                    //     MSIL.
                    case ExpressionType.Unbox:
                        return "TODO";
                    //
                    // Summary:
                    //     An addition compound assignment operation, such as (a += b), without overflow
                    //     checking, for numeric operands.
                    case ExpressionType.AddAssign:
                        return String.Format("{0} += {1}", Visit(be.Left), Visit(be.Right));
                    //
                    // Summary:
                    //     A bitwise or logical AND compound assignment operation, such as (a &= b)
                    //     in C#.
                    case ExpressionType.AndAssign:
                        return String.Format("{0} &= {1}", Visit(be.Left), Visit(be.Right));
                    //
                    // Summary:
                    //     An division compound assignment operation, such as (a /= b), for numeric
                    //     operands.
                    case ExpressionType.DivideAssign:
                        return String.Format("{0} /= {1}", Visit(be.Left), Visit(be.Right));
                    //
                    // Summary:
                    //     A bitwise or logical XOR compound assignment operation, such as (a ^= b)
                    //     in C#.
                    case ExpressionType.ExclusiveOrAssign:
                        return String.Format("{0} ^= {1}", Visit(be.Left), Visit(be.Right));
                    //
                    // Summary:
                    //     A bitwise left-shift compound assignment, such as (a <<= b).
                    case ExpressionType.LeftShiftAssign:
                        return String.Format("{0} <<= {1}", Visit(be.Left), Visit(be.Right));
                    //
                    // Summary:
                    //     An arithmetic remainder compound assignment operation, such as (a %= b) in
                    //     C#.
                    case ExpressionType.ModuloAssign:
                        return String.Format("{0} %= {1}", Visit(be.Left), Visit(be.Right));
                    //
                    // Summary:
                    //     A multiplication compound assignment operation, such as (a *= b), without
                    //     overflow checking, for numeric operands.
                    case ExpressionType.MultiplyAssign:
                        return String.Format("{0} *= {1}", Visit(be.Left), Visit(be.Right));
                    //
                    // Summary:
                    //     A bitwise or logical OR compound assignment, such as (a |= b) in C#.
                    case ExpressionType.OrAssign:
                        return String.Format("{0} |= {1}", Visit(be.Left), Visit(be.Right));
                    //
                    // Summary:
                    //     A compound assignment operation that raises a number to a power, such as
                    //     (a ^= b) in Visual Basic.
                    case ExpressionType.PowerAssign:
                        return "TODO";
                    //
                    // Summary:
                    //     A bitwise right-shift compound assignment operation, such as (a >>= b).
                    case ExpressionType.RightShiftAssign:
                        return String.Format("{0} >>= {1}", Visit(be.Left), Visit(be.Right));
                    //
                    // Summary:
                    //     A subtraction compound assignment operation, such as (a -= b), without overflow
                    //     checking, for numeric operands.
                    case ExpressionType.SubtractAssign:
                        return String.Format("{0} -= {1}", Visit(be.Left), Visit(be.Right));
                    //
                    // Summary:
                    //     An addition compound assignment operation, such as (a += b), with overflow
                    //     checking, for numeric operands.
                    case ExpressionType.AddAssignChecked:
                        return String.Format("{0} += {1}", Visit(be.Left), Visit(be.Right));
                    //
                    // Summary:
                    //     A multiplication compound assignment operation, such as (a *= b), that has
                    //     overflow checking, for numeric operands.
                    case ExpressionType.MultiplyAssignChecked:
                        return String.Format("{0} *= {1}", Visit(be.Left), Visit(be.Right));
                    //
                    // Summary:
                    //     A subtraction compound assignment operation, such as (a -= b), that has overflow
                    //     checking, for numeric operands.
                    case ExpressionType.SubtractAssignChecked:
                        return String.Format("{0} -= {1}", Visit(be.Left), Visit(be.Right));
                    //
                    // Summary:
                    //     A unary prefix increment, such as (++a). The object a should be modified
                    //     in place.
                    case ExpressionType.PreIncrementAssign:
                        return String.Format("++{0}", Visit(context, ue.Operand));
                    //
                    // Summary:
                    //     A unary prefix decrement, such as (--a). The object a should be modified
                    //     in place.
                    case ExpressionType.PreDecrementAssign:
                        return String.Format("--{0}", Visit(context, ue.Operand));
                    //
                    // Summary:
                    //     A unary postfix increment, such as (a++). The object a should be modified
                    //     in place.
                    case ExpressionType.PostIncrementAssign:
                        return String.Format("{0}++", Visit(context, ue.Operand));
                    //
                    // Summary:
                    //     A unary postfix decrement, such as (a--). The object a should be modified
                    //     in place.
                    case ExpressionType.PostDecrementAssign:
                        return String.Format("{0}--", Visit(context, ue.Operand));
                    //
                    // Summary:
                    //     An exact type test.
                    case ExpressionType.TypeEqual:
                        return "TODO";
                    //
                    // Summary:
                    //     A ones complement operation, such as (~a) in C#.
                    case ExpressionType.OnesComplement:
                        return "TODO";
                    //
                    // Summary:
                    //     A true condition value.
                    case ExpressionType.IsTrue:
                        return "TODO";
                    //
                    // Summary:
                    //     A false condition value.
                    case ExpressionType.IsFalse:
                        return "TODO";
                }
                return "Unknown";
            }
        }

        public static string Evaluate(this Expression expr, bool hexMode = false, string linePrefix = "")
        {
            return new Visitor().Visit(expr, hexMode, linePrefix);
        }
    }
}
