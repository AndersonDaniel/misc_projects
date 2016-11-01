package DataTypes.Expressions.CustomExpression.ExpressionEvaluator.Operators;

import java.util.Map;

public class CVariableNode implements IASTNode {

	private String m_strVariableName;
	
	public CVariableNode(String p_strVariableName) {
		m_strVariableName = p_strVariableName.toUpperCase();
	}
	
	@Override
	public double Evaluate(Map<String, Double> p_mapRow) {
		if (p_mapRow.containsKey(m_strVariableName)) {
			return (p_mapRow.get(m_strVariableName));
		}
		
		return (0);
	}
	
}