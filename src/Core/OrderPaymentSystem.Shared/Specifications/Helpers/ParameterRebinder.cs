using System.Linq.Expressions;

namespace OrderPaymentSystem.Shared.Specifications.Helpers;

/// <summary>
/// Хелпер для переопределения параметров в дереве выражения
/// </summary>
public class ParameterRebinder : ExpressionVisitor
{
    private readonly ParameterExpression _parameter;

    /// <summary>
    /// Конструктор
    /// </summary>
    /// <param name="parameter">Параметр выражения</param>
    public ParameterRebinder(ParameterExpression parameter)
    {
        _parameter = parameter;
    }

    /// <summary>
    /// Переопределяет параметры в дереве выражений.
    /// Этот метод заменяет в узле параметра (ParameterExpression) все вхождения исходного параметра
    /// </summary>
    /// <param name="node">Текущий узел <see cref="ParameterExpression"/> для посещения</param>
    /// <returns>Модифицированный или исходный узел выражения</returns>
    protected override Expression VisitParameter(ParameterExpression node)
    {
        return base.VisitParameter(node.Type == _parameter.Type && node.Name == _parameter.Name ? _parameter : node);
    }
}
