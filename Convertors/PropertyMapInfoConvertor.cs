using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using EF.Core.GenericProjection.Models;

namespace EF.Core.GenericProjection.Convertors
{
    public static class PropertyMapInfoConvertor
    {
        private static PropertyMapInfoNode Map(MemberExpression exp)
        {
            PropertyMapInfoNode root = null;
            while (exp != null)
            {
                var child = new PropertyMapInfoNode()
                {
                    Path = exp.ToString(),
                    PropertyType = (exp.Member as PropertyInfo).PropertyType,
                    PropertyName = exp.Member.Name,
                    AssignPropertyName = exp.Member.Name,
                    Children = new List<PropertyMapInfoNode>(),
                };
                if (root == null)
                    root = child;
                else
                {
                    child.Children.Add(root);
                    root = child;
                }

                exp = exp.Expression as MemberExpression;
            }

            return root;


        }
        

        public static PropertyMapInfoNode Map<T>(Expression<Func<T, object>> exp)
        {
            return Map(MemberExpressionConvertor.Map(exp));
        }

        public static ICollection<PropertyMapInfoNode> Map<T>(ICollection<Expression<Func<T, object>>> exps)
        {
            return exps.Select(Map).ToList();
        }

    }
}