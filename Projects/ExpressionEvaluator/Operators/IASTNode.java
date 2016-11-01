package DataTypes.Expressions.CustomExpression.ExpressionEvaluator.Operators;

import java.util.Map;

public interface IASTNode {
	double Evaluate(Map<String, Double> p_mapRow) throws Exception;
}
