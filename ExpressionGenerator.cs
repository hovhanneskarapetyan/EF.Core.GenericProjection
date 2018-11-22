using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using EF.Core.GenericProjection.Convertors;
using EF.Core.GenericProjection.Models;

namespace EF.Core.GenericProjection
{
    public static class ExpressionGenerator
    {
        public static Expression<Func<TIn, TResult>> BuildLambda<TIn, TResult>(ICollection<Expression<Func<TIn, object>>> properties)
        {
            return BuildLambda<TIn, TResult>(PropertyMapInfoNode.Merge<TResult>(PropertyMapInfoConvertor.Map<TIn>(properties.ToList()).ToList()));
        }

        public static Expression<Func<TIn, TResult>> BuildLambda<TIn, TResult>(PropertyMapInfoNode head)
        {

            ParameterExpression param = Expression.Parameter(typeof(TIn), "item");
            var ctor = Expression.New(typeof(TResult));

            var memberInit = Expression.MemberInit(ctor, GetMemberAssignments(head, param, ""));
            return
                Expression.Lambda<Func<TIn, TResult>>(memberInit, param);

        }

        private static Expression GetPropertyPath(ParameterExpression param, string propertyName)
        {
            Expression body = param;
            foreach (var member in propertyName.Split('.'))
            {
                body = Expression.PropertyOrField(body, member);
            }

            return body;
        }

        private static IEnumerable<MemberAssignment> GetMemberAssignments(PropertyMapInfoNode item, ParameterExpression param,
            string path)
        {
            var memberAssignment = new List<MemberAssignment>();
            if (item.IsComplexType)
            {
                foreach (var child in item.Children)
                {
                    var propertyPath = path == "" ? child.PropertyName : $"{path}.{child.PropertyName}";

                    if (child.IsComplexType)
                    {
                        var newCtor = Expression.New(child.PropertyType);

                        var newAssignments = GetMemberAssignments(child, param, propertyPath);

                        var memberInit = Expression.MemberInit(newCtor, newAssignments);

                        var displayValueProperty = item.PropertyType.GetProperty(child.AssignPropertyName);

                        var displayValueAssignment = Expression.Bind(
                            displayValueProperty, memberInit);

                        memberAssignment.Add(displayValueAssignment);

                    }
                    else
                    {
                        var displayValueProperty = item.PropertyType.GetProperty(child.AssignPropertyName);

                        var displayValueAssignment =
                            Expression.Bind(displayValueProperty, GetPropertyPath(param, propertyPath));
                        memberAssignment.Add(displayValueAssignment);
                    }
                }
            }

            return memberAssignment;

        }
    }
}
