package DataTypes.Expressions.CustomExpression.ExpressionEvaluator.Operators;

import java.util.ArrayList;
import java.util.List;
import java.util.Map;

public abstract class AFunctionOperatorNode implements IASTNode {
	public ArrayList<IASTNode> ChildNodes = new ArrayList<IASTNode>();

	@Override
	public double Evaluate(Map<String, Double> p_mapRow) throws Exception {
		List<Double> lstInnerValues = new ArrayList<Double>();
		
		for (IASTNode objInnerNode : ChildNodes) {
			lstInnerValues.add(objInnerNode.Evaluate(p_mapRow));
		}
		
		return (Apply(lstInnerValues));
	}
	
	abstract double Apply(List<Double> p_lstInnerValues) throws Exception;
}