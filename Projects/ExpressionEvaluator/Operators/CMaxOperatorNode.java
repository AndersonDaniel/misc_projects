package DataTypes.Expressions.CustomExpression.ExpressionEvaluator.Operators;

import java.util.Collections;
import java.util.List;

public class CMaxOperatorNode extends AFunctionOperatorNode {

	@Override
	double Apply(List<Double> p_lstInnerValues) {
		return (Collections.max(p_lstInnerValues));
	}
}
