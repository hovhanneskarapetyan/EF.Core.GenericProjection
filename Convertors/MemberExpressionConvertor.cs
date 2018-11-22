using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;


namespace EF.Core.GenericProjection.Convertors
{
    public class MemberExpressionConvertor
    {
        public static MemberExpression Map<T>(Expression<Func<T, object>> exp)
        {
            var body = exp.Body;
            MemberExpression memberExp;
            if (body is UnaryExpression unaryExpression)
            {
                memberExp = unaryExpression.Operand as MemberExpression;
            }
            else
                memberExp = body as MemberExpression;


            return memberExp;
        }

        public static ICollection<MemberExpression> Map<T>(ICollection<Expression<Func<T, object>>> exps)
        {
            return exps.Select(Map).ToList();
        }
    }
}