package DataTypes.Expressions.CustomExpression.ExpressionEvaluator.Operators;

import java.util.Collections;
import java.util.List;

public class CMinOperatorNode extends AFunctionOperatorNode {

	@Override
	double Apply(List<Double> p_lstInnerValues) {
		return (Collections.min(p_lstInnerValues));
	}

}
