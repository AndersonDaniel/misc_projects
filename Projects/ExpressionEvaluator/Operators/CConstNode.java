package DataTypes.Expressions.CustomExpression.ExpressionEvaluator.Operators;

import java.util.Map;

public class CConstNode implements IASTNode {

	private double m_dblValue;
	
	public CConstNode(double p_dblValue) {
		m_dblValue = p_dblValue;
	}
	
	@Override
	public double Evaluate(Map<String, Double> p_mapRow) {
		return (m_dblValue);
	}

}
