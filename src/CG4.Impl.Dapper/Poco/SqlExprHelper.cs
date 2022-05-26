﻿using System.Linq.Expressions;
using CG4.Impl.Dapper.Poco.Expressions;

namespace CG4.Impl.Dapper.Poco
{
    public static class SqlExprHelper
    {
        public static Expr GenerateWhere<TEntity>(Expression<Func<TEntity, bool>> expression)
            where TEntity : class
        {
            var alias = expression.Parameters.First().Name;

            return GenerateWhere(expression, alias);
        }

        public static ExprBoolean GenerateWhere<TEntity>(Expression<Func<TEntity, bool>> expression, string alias)
            where TEntity : class
        {
            var builder = new ExprBuilder(PocoHub.GetMap<TEntity>(), alias);

            return (ExprBoolean)builder.ParseExpr(expression.Body);
        }

        public static ExprColumn GenerateColumn<TEntity, TKey>(Expression<Func<TEntity, TKey>> keySelector)
            where TEntity : class
        {
            var alias = keySelector.Parameters.First().Name;

            return GenerateColumn(keySelector, alias);
        }

        public static ExprColumn GenerateColumn<TEntity, TKey>(Expression<Func<TEntity, TKey>> keySelector, string alias)
            where TEntity : class
        {
            var builder = new ExprBuilder(PocoHub.GetMap<TEntity>(), alias);
            var expr = builder.ParseExpr(keySelector.Body);

            if (expr is ExprColumn exprCol)
            {
                return exprCol;
            }

            throw new InvalidOperationException("Должно быть выбрано поле сущности");
        }

        public static void SetAlias(ExprBoolean expr, string alias)
        {
            switch (expr)
            {
                case ExprBoolAnd exprAnd:
                    SetAlias(exprAnd.Left, alias);
                    SetAlias(exprAnd.Right, alias);
                    return;
                case ExprBoolOr exprOr:
                    SetAlias(exprOr.Left, alias);
                    SetAlias(exprOr.Right, alias);
                    return;
                case ExprBoolEqPredicate eqPred:
                    eqPred.Column.Alias = alias;
                    return;
                case ExprBoolNotEqPredicate neqPred:
                    neqPred.Column.Alias = alias;
                    return;
                default:
                    throw new ArgumentException($"Type {expr.GetType().Name} not supported");
            }
        }
    }
}