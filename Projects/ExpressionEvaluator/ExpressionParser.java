package DataTypes.Expressions.CustomExpression.ExpressionEvaluator;

import java.util.Stack;

import DataTypes.Expressions.CustomExpression.ExpressionEvaluator.Operators.ABinaryOperatorNode;
import DataTypes.Expressions.CustomExpression.ExpressionEvaluator.Operators.AFunctionOperatorNode;
import DataTypes.Expressions.CustomExpression.ExpressionEvaluator.Operators.AUnaryOperatorNode;
import DataTypes.Expressions.CustomExpression.ExpressionEvaluator.Operators.CConstNode;
import DataTypes.Expressions.CustomExpression.ExpressionEvaluator.Operators.CVariableNode;
import DataTypes.Expressions.CustomExpression.ExpressionEvaluator.Operators.IASTNode;

public class ExpressionParser {
	
	private StringTokenizer m_objTokenizer;
	private OperatorMetaData m_objOperatorMetaData;
	
	private String CONST_SENTINEL = "";
	
	public IASTNode EParse(String p_strExpression) throws Exception {
		m_objTokenizer = new StringTokenizer(p_strExpression);
		m_objOperatorMetaData = new OperatorMetaData();
		
		Stack<String> stkOperators = new Stack<String>();
		Stack<IASTNode> stkOperands = new Stack<IASTNode>();
		
		// Push sentinel
		stkOperators.push(CONST_SENTINEL);
		
		E(stkOperators, stkOperands);
		
		m_objTokenizer.expectToken(m_objTokenizer.CONST_STRING_END);
		
		if (!stkOperands.isEmpty()) {
			return (stkOperands.pop());
		}
		
		throw new Exception("Error parsing expression!");
	}
	
	private void E(Stack<String> p_stkOperators, Stack<IASTNode> p_stkOperands) throws Exception {
		P(p_stkOperators, p_stkOperands);
		String strNextToken = m_objTokenizer.getNextToken();
		while (m_objOperatorMetaData.IsBinaryOperator(strNextToken)) {
			PushOperator(strNextToken, p_stkOperators, p_stkOperands);
			m_objTokenizer.consumeToken();
			P(p_stkOperators, p_stkOperands);
			
			strNextToken = m_objTokenizer.getNextToken();
		}
		
		while (p_stkOperators.peek() != CONST_SENTINEL) {
			PopOperator(p_stkOperators, p_stkOperands);
		}
	}
	
	private void P(Stack<String> p_stkOperators, Stack<IASTNode> p_stkOperands) throws Exception {
		String strNextToken = m_objTokenizer.getNextToken();
		if (m_objOperatorMetaData.IsTerminal(strNextToken)) {
			p_stkOperands.push(MakeLeaf(strNextToken));
			m_objTokenizer.consumeToken();
		} else if (strNextToken.equals("(")) {
			m_objTokenizer.consumeToken();
			p_stkOperators.push(CONST_SENTINEL);
			E(p_stkOperators, p_stkOperands);
			m_objTokenizer.expectToken(")");
			p_stkOperators.pop();
		} else if (m_objOperatorMetaData.IsUnaryOperator(strNextToken)) {
			PushOperator(strNextToken, p_stkOperators, p_stkOperands);
			m_objTokenizer.consumeToken();
			P(p_stkOperators, p_stkOperands);
		} else if (m_objOperatorMetaData.IsFunctionOperator(strNextToken)) {
			F(p_stkOperators, p_stkOperands);
		} else {
			throw new Exception("Parse error - unexpected token: '" + strNextToken + "'");
		}
	}
	
	private void F(Stack<String> p_stkOperators, Stack<IASTNode> p_stkOperands) throws Exception {
		p_stkOperands.push(m_objOperatorMetaData.NewFunctionOperatorNode(m_objTokenizer.getNextToken()));
		m_objTokenizer.consumeToken();
		m_objTokenizer.expectToken("(");
		I(p_stkOperators, p_stkOperands);
		m_objTokenizer.expectToken(")");
	}
	
	private void I(Stack<String> p_stkOperators, Stack<IASTNode> p_stkOperands) throws Exception {
		AFunctionOperatorNode objFunctionNode = (AFunctionOperatorNode)(p_stkOperands.pop());
		E(p_stkOperators, p_stkOperands);
		objFunctionNode.ChildNodes.add(p_stkOperands.pop());
		p_stkOperands.push(objFunctionNode);
		if (m_objTokenizer.getNextToken().equals(",")) {
			m_objTokenizer.consumeToken();
			I(p_stkOperators, p_stkOperands);
		}
	}
	
	private void PopOperator(Stack<String> p_stkOperators,
							 Stack<IASTNode> p_stkOperands) throws Exception {
		if (m_objOperatorMetaData.IsBinaryOperator(p_stkOperators.peek())) {
			
			ABinaryOperatorNode objBinaryOperator =
					m_objOperatorMetaData.NewBinaryOperatorNode(p_stkOperators.pop());
			objBinaryOperator.RightNode = p_stkOperands.pop();
			objBinaryOperator.LeftNode = p_stkOperands.pop();
			p_stkOperands.push(objBinaryOperator);
		} else if (m_objOperatorMetaData.IsUnaryOperator(p_stkOperators.peek())) {
			AUnaryOperatorNode objUnaryOperator =
					m_objOperatorMetaData.NewUnaryOperatorNode(p_stkOperators.pop());
			objUnaryOperator.ChildNode = p_stkOperands.pop();
			p_stkOperands.push(objUnaryOperator);
		} else if (m_objOperatorMetaData.IsFunctionOperator(p_stkOperators.peek())) {
			// TODO: implement
		}
	}
	
	private void PushOperator(String p_strOperator,
							  Stack<String> p_stkOperators,
							  Stack<IASTNode> p_stkOperands) throws Exception {
		while (m_objOperatorMetaData.HasHigherPrecedence(p_stkOperators.peek(), p_strOperator)) {
			PopOperator(p_stkOperators, p_stkOperands);
		}
		
		p_stkOperators.push(p_strOperator);
	}
	
	private IASTNode MakeLeaf(String p_strToken) throws Exception {
		try {
			double dConst = Double.parseDouble(p_strToken);
			return (new CConstNode(dConst));
		} catch (Exception ex) {
			if (m_objOperatorMetaData.IsUnaryOperator(p_strToken)) {
				return (m_objOperatorMetaData.NewUnaryOperatorNode(p_strToken));
			} else if (m_objOperatorMetaData.IsBinaryOperator(p_strToken)) {
				return (m_objOperatorMetaData.NewBinaryOperatorNode(p_strToken));
			} else if (m_objOperatorMetaData.IsFunctionOperator(p_strToken)) {
				return (m_objOperatorMetaData.NewFunctionOperatorNode(p_strToken));
			}
			
			return (new CVariableNode(p_strToken));
		}
	}
}
