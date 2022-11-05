using System;
using System.Linq;
using System.Linq.Expressions;

namespace CodeNotion.Template.Business.Extensions;

public static class ExpressionExtensions
{
    public static Expression<Func<T, bool>> And<T>(this Expression<Func<T, bool>> expr1, Expression<Func<T, bool>> expr2)
    {
        var parameter = Expression.Parameter(typeof(T));

        var leftVisitor = new ReplaceExpressionVisitor(expr1.Parameters[0], parameter);
        var left = leftVisitor.Visit(expr1.Body);

        var rightVisitor = new ReplaceExpressionVisitor(expr2.Parameters[0], parameter);
        var right = rightVisitor.Visit(expr2.Body);

        if (left == null || right == null)
        {
            throw new ArgumentException("Expressions cannot be null");
        }

        return Expression.Lambda<Func<T, bool>>(Expression.AndAlso(left, right), parameter);
    }

    public static Expression<Func<T, bool>> Or<T>(this Expression<Func<T, bool>>[] expressions)
    {
        if (expressions.Length == 0)
        {
            throw new ArgumentException("Expressions cannot be empty");
        }

        var result = expressions[0];
        var parameter = Expression.Parameter(typeof(T));

        foreach (var expression in expressions.Skip(1))
        {
            var leftVisitor = new ReplaceExpressionVisitor(result.Parameters[0], parameter);
            var left = leftVisitor.Visit(result.Body);

            var rightVisitor = new ReplaceExpressionVisitor(expression.Parameters[0], parameter);
            var right = rightVisitor.Visit(expression.Body);

            if (left == null || right == null)
            {
                throw new ArgumentException("Expressions cannot be null");
            }

            result = Expression.Lambda<Func<T, bool>>(Expression.OrElse(left, right), parameter);
        }

        return result;
    }

    public static Expression<Func<T, bool>> Not<T>(this Expression<Func<T, bool>> expr)
    {
        var parameter = Expression.Parameter(typeof(T));

        var leftVisitor = new ReplaceExpressionVisitor(expr.Parameters[0], parameter);
        var left = leftVisitor.Visit(expr.Body);
        
        if (left == null)
        {
            throw new ArgumentException("Expression cannot be null");
        }

        return Expression.Lambda<Func<T, bool>>(Expression.Not(left), parameter);
    }
}

public class ReplaceExpressionVisitor : ExpressionVisitor
{
    private readonly Expression _oldValue;
    private readonly Expression _newValue;

    public ReplaceExpressionVisitor(Expression oldValue, Expression newValue)
    {
        _oldValue = oldValue;
        _newValue = newValue;
    }

    public override Expression? Visit(Expression? node)
    {
        return node == _oldValue ? _newValue : base.Visit(node);
    }
}