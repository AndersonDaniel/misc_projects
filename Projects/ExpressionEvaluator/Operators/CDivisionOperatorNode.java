package DataTypes.Expressions.CustomExpression.ExpressionEvaluator.Operators;

public class CDivisionOperatorNode extends ABinaryOperatorNode {

	@Override
	double Apply(double p_dLeft, double p_dRight) throws Exception {
		if (p_dLeft == 0) {
			throw new Exception("Division by zero!");
		}
		
		return (p_dLeft / p_dRight);
	}

}
