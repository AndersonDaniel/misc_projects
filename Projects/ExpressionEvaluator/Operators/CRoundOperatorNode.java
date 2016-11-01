package DataTypes.Expressions.CustomExpression.ExpressionEvaluator.Operators;

import java.math.BigDecimal;
import java.math.RoundingMode;
import java.util.List;

public class CRoundOperatorNode extends AFunctionOperatorNode {

	@Override
	double Apply(List<Double> p_lstInnerValues) throws Exception {
		if (p_lstInnerValues.size() != 2) {
			throw new Exception("Invalid number of parameters to round: " + p_lstInnerValues.size());
		}
		
		return (new BigDecimal(p_lstInnerValues.get(0)).setScale(p_lstInnerValues.get(1).intValue(), RoundingMode.HALF_UP).doubleValue());
	}
	
}
