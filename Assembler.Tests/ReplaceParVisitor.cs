using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Assembler.Tests
{
    // Expression visitor to convert simple lambda expressions to something we expect from the compiler
    internal class ReplaceParVisitor : ExpressionVisitor
    {
        private readonly ParameterExpression statePar;
        private static Dictionary<string, MethodInfo> methods = new Expression<Action>[]
        {
                () => Compiler.ExpectBool(null, false),
                () => Compiler.ExpectNumber(null, false),
                () => ((State)null).GetSymbol("a")
        }
        .Select(it => (it.Body as MethodCallExpression).Method)
        .ToDictionary(it => it.Name);

        public ReplaceParVisitor(ParameterExpression statePar)
        {
            this.statePar = statePar;
        }

        // Replace parameter "a" by "state.GetSymbol("a")"
        protected override Expression VisitParameter(ParameterExpression node)
        {
            return Expression.Call(statePar, methods[nameof(State.GetSymbol)], Expression.Constant(node.Name));
        }

        protected override Expression VisitUnary(UnaryExpression node)
        {
            if (node.NodeType == ExpressionType.Convert)
            {
                Expression oper = Visit(node.Operand);
                return
                    oper.Type == node.Type
                    ? oper
                    : Expression.Convert(oper, node.Type);
            }
            return base.VisitUnary(node);
        }

        protected override Expression VisitBinary(BinaryExpression node)
        {
            var left = Visit(node.Left);
            if (left.Type == typeof(object[]))
                left = node.Left.Type == typeof(bool)
                    ? Expression.Call(null, methods[nameof(Compiler.ExpectBool)], left, Expression.Constant(false))
                    : Expression.Call(null, methods[nameof(Compiler.ExpectNumber)], left, Expression.Constant(false));
            var right = Visit(node.Right);
            if (right.Type == typeof(object[]))
                right = node.Right.Type == typeof(bool)
                    ? Expression.Call(null, methods[nameof(Compiler.ExpectBool)], right, Expression.Constant(false))
                    : Expression.Call(null, methods[nameof(Compiler.ExpectNumber)], right, Expression.Constant(false));

            return node.Update(left, node.Conversion, right);
        }
    }
}
