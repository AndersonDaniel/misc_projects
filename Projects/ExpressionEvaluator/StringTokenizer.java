package DataTypes.Expressions.CustomExpression.ExpressionEvaluator;

public class StringTokenizer {
	private String m_strExpression;
	public String CONST_STRING_END = "$";
	
	public StringTokenizer(String p_strExpression) {
		m_strExpression = p_strExpression.replace(" ", "").toLowerCase() + CONST_STRING_END;
	}
	
	public String getNextToken() {
		int nCurrPos = 0;
		char cCurr = m_strExpression.charAt(nCurrPos++);

		if (cCurr == '(' || cCurr == ')') {
			return (new String(new char[] { cCurr }));
		}
		
		String strCurr = "";
		
		boolean bIsText, bIsDigit, bTempIsText, bTempIsDigit;
		
		bTempIsText = bIsText = (Character.isAlphabetic((int)cCurr) || cCurr == '_');
		bTempIsDigit = bIsDigit = (Character.isDigit((int)cCurr) || cCurr == '.');
		boolean bContinue = true;
		
		while (bContinue) {
			strCurr += cCurr;
			if (nCurrPos >= m_strExpression.length()) {
				bContinue = false;
			} else {
				cCurr = m_strExpression.charAt(nCurrPos++);
				bTempIsText = (Character.isAlphabetic((int)cCurr) || cCurr == '_');
				bTempIsDigit = (Character.isDigit((int)cCurr) || cCurr == '.');
				bContinue = ((bTempIsText == bIsText) && 
							 (bTempIsDigit == bIsDigit) &&
							 (bTempIsText || bTempIsDigit));
			}
		}
		
		return (strCurr);
	}
	
	public void consumeToken() {
		String strNextToken = getNextToken();
		if (strNextToken.charAt(0) == '+' || 
			strNextToken.charAt(0) == '*' || 
			strNextToken.charAt(0) == '(' ||
			strNextToken.charAt(0) == ')' ||
			strNextToken.charAt(0) == '^') {
			strNextToken = "\\" + strNextToken;
		}
		
		m_strExpression = m_strExpression.replaceFirst(strNextToken, "");
	}
	
	public void expectToken(String p_strToken) throws Exception {
		if (!getNextToken().equals(p_strToken)) {
			throw new Exception("Error! Exptected token '" + p_strToken + "', got instead '" + getNextToken() + "'");
		} else {
			consumeToken();
		}
	}
}
