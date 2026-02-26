using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace OrderPaymentSystem.Shared.Specifications.Helpers
{

    public class ParameterRebinder : ExpressionVisitor
    {
        private readonly ParameterExpression _parameter;

        public ParameterRebinder(ParameterExpression parameter)
        {
            _parameter = parameter;
        }

        protected override Expression VisitParameter(ParameterExpression node)
        {
            return base.VisitParameter(node.Type == _parameter.Type && node.Name == _parameter.Name ? _parameter : node);
        }
    }

}
