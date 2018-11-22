﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace EF.Core.GenericProjection.Extentions
{
    public static class QuerableExtentions
    {
        public static IQueryable<TOut> Select<TIn,TOut>(this IQueryable<TIn> query, ICollection<Expression<Func<TIn, object>>> exps)
        {
            return query.Select(ExpressionGenerator.BuildLambda<TIn, TOut>(exps));
        }
    }
}