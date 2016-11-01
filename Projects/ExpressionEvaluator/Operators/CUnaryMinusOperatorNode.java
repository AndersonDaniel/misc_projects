package DataTypes.Expressions.CustomExpression.ExpressionEvaluator.Operators;

public class CUnaryMinusOperatorNode extends AUnaryOperatorNode {

	@Override
	double Apply(double p_dValue) {
		return (-p_dValue);
	}

}
