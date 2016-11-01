package DataTypes.Expressions.CustomExpression.ExpressionEvaluator;

import java.util.HashMap;

import DataTypes.Expressions.CustomExpression.ExpressionEvaluator.Operators.ABinaryOperatorNode;
import DataTypes.Expressions.CustomExpression.ExpressionEvaluator.Operators.AFunctionOperatorNode;
import DataTypes.Expressions.CustomExpression.ExpressionEvaluator.Operators.AUnaryOperatorNode;
import DataTypes.Expressions.CustomExpression.ExpressionEvaluator.Operators.CDivisionOperatorNode;
import DataTypes.Expressions.CustomExpression.ExpressionEvaluator.Operators.CMaxOperatorNode;
import DataTypes.Expressions.CustomExpression.ExpressionEvaluator.Operators.CMinOperatorNode;
import DataTypes.Expressions.CustomExpression.ExpressionEvaluator.Operators.CMinusOperatorNode;
import DataTypes.Expressions.CustomExpression.ExpressionEvaluator.Operators.CPlusOperatorNode;
import DataTypes.Expressions.CustomExpression.ExpressionEvaluator.Operators.CPowerOperatorNode;
import DataTypes.Expressions.CustomExpression.ExpressionEvaluator.Operators.CProductOperatorNode;
import DataTypes.Expressions.CustomExpression.ExpressionEvaluator.Operators.CRoundOperatorNode;
import DataTypes.Expressions.CustomExpression.ExpressionEvaluator.Operators.CUnaryMinusOperatorNode;

public class OperatorMetaData {
	private HashMap<String, Integer> m_mapPrecedence;
	private HashMap<String, Boolean> m_mapLeftAssociative;
	private HashMap<String, Class> m_mapUnaryOperators;
	private HashMap<String, Class> m_mapBinaryOperators;
	private HashMap<String, Class> m_mapFunctionOperators;
	
	public OperatorMetaData() {
		m_mapPrecedence = new HashMap<String, Integer>();
		m_mapLeftAssociative = new HashMap<String, Boolean>();
		m_mapUnaryOperators = new HashMap<String, Class>();
		m_mapBinaryOperators = new HashMap<String, Class>();
		m_mapFunctionOperators = new HashMap<String, Class>();
	
		InitPrecedences();
		InitAssociativities();
		InitUnaryOperators();
		InitBinaryOperators();
		InitFunctionOperators();
	}
	
	private void InitPrecedences() {
		m_mapPrecedence.put("+", 1);
		m_mapPrecedence.put("-", 1);
		m_mapPrecedence.put("*", 2);
		m_mapPrecedence.put("/", 2);
		m_mapPrecedence.put("^", 3);
		m_mapPrecedence.put("max", 0);
		m_mapPrecedence.put("min", 0);
	}
	
	private void InitAssociativities() {
		m_mapLeftAssociative.put("+", true);
		m_mapLeftAssociative.put("-", true);
		m_mapLeftAssociative.put("*", true);
		m_mapLeftAssociative.put("/", true);
		m_mapLeftAssociative.put("^", false);
	}
	
	private void InitUnaryOperators() {
		m_mapUnaryOperators.put("-", CUnaryMinusOperatorNode.class);
	}
	
	private void InitBinaryOperators() {
		m_mapBinaryOperators.put("+", CPlusOperatorNode.class);
		m_mapBinaryOperators.put("-", CMinusOperatorNode.class);
		m_mapBinaryOperators.put("*", CProductOperatorNode.class);
		m_mapBinaryOperators.put("/", CDivisionOperatorNode.class);
		m_mapBinaryOperators.put("^", CPowerOperatorNode.class);
	}
	
	private void InitFunctionOperators() {
		m_mapFunctionOperators.put("max", CMaxOperatorNode.class);
		m_mapFunctionOperators.put("min", CMinOperatorNode.class);
		m_mapFunctionOperators.put("round", CRoundOperatorNode.class);
	}
	
	public boolean IsUnaryOperator(String p_strOperator) {
		return (m_mapUnaryOperators.containsKey(p_strOperator));
	}
	
	public AUnaryOperatorNode NewUnaryOperatorNode(String p_strOperator) throws Exception {
		return ((AUnaryOperatorNode)(m_mapUnaryOperators.get(p_strOperator).newInstance()));
	}
	
	public boolean IsBinaryOperator(String p_strOperator) {
		return (m_mapBinaryOperators.containsKey(p_strOperator));
	}
	
	public ABinaryOperatorNode NewBinaryOperatorNode(String p_strOperator) throws Exception {
		return ((ABinaryOperatorNode)(m_mapBinaryOperators.get(p_strOperator).newInstance()));
	}
	
	public boolean IsFunctionOperator(String p_strOperator) {
		return (m_mapFunctionOperators.containsKey(p_strOperator));
	}
	
	public AFunctionOperatorNode NewFunctionOperatorNode(String p_strOperator) throws Exception {
		return ((AFunctionOperatorNode)(m_mapFunctionOperators.get(p_strOperator).newInstance()));
	}
	
	public boolean IsTerminal(String p_strToken) {
		return ((!IsOperator(p_strToken)) &&
				(!p_strToken.equals("(")) &&
				(!p_strToken.equals(")")));
	}
	
	public boolean IsOperator(String p_strToken) {
		return ((IsUnaryOperator(p_strToken)) ||
				(IsBinaryOperator(p_strToken)) ||
				(IsFunctionOperator(p_strToken)));
	}
	
	public boolean HasHigherPrecedence(String p_strOp1, String p_strOp2) {
		return ((m_mapPrecedence.containsKey(p_strOp1)) &&
				((m_mapPrecedence.get(p_strOp1) > m_mapPrecedence.get(p_strOp2)) ||
				 ((m_mapPrecedence.get(p_strOp1) == m_mapPrecedence.get(p_strOp2) &&
				  (m_mapLeftAssociative.get(p_strOp1))))));
	}
}
