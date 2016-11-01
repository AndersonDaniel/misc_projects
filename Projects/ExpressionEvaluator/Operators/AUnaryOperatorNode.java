package DataTypes.Expressions.CustomExpression.ExpressionEvaluator.Operators;

import java.util.Map;

public abstract class AUnaryOperatorNode implements IASTNode {
	public IASTNode ChildNode;

	@Override
	public double Evaluate(Map<String, Double> p_mapRow) throws Exception {
		return (Apply(ChildNode.Evaluate(p_mapRow)));
	}
	
	abstract double Apply(double p_dValue);
}
