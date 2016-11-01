package DataTypes.Expressions.CustomExpression.ExpressionEvaluator.Operators;

public class CPlusOperatorNode extends ABinaryOperatorNode {

	@Override
	double Apply(double p_dLeft, double p_dRight) {
		return (p_dLeft + p_dRight);
	}
}
