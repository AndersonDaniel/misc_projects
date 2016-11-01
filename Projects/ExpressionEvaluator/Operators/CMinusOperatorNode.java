package DataTypes.Expressions.CustomExpression.ExpressionEvaluator.Operators;

public class CMinusOperatorNode extends ABinaryOperatorNode {

	@Override
	double Apply(double p_dLeft, double p_dRight) {
		return (p_dLeft - p_dRight);
	}

}
