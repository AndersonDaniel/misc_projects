package DataTypes.Expressions.CustomExpression.ExpressionEvaluator.Operators;

public class CPowerOperatorNode extends ABinaryOperatorNode {

	@Override
	double Apply(double p_dLeft, double p_dRight) throws Exception {
		return (Math.pow(p_dLeft, p_dRight));
	}
}
