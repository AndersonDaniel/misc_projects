package DataTypes.Expressions.CustomExpression.ExpressionEvaluator.Operators;

import java.util.Map;

public abstract class ABinaryOperatorNode implements IASTNode {
	public IASTNode LeftNode;
	public IASTNode RightNode;
	
	@Override
	public double Evaluate(Map<String, Double> p_mapRow) throws Exception {
		return (Apply(LeftNode.Evaluate(p_mapRow), RightNode.Evaluate(p_mapRow)));
	}
	
	abstract double Apply(double p_dLeft, double p_dRight) throws Exception;
}
