using UnityEngine;
using System.Collections.Generic;

public class CSJSONDecoder {
	private CSJSONObject value;
	private CSJSONTokenizer tokenizer;
	private CSJSONToken token;

	public CSJSONDecoder(string s) {
		tokenizer = new CSJSONTokenizer(s);
		NextToken();
		value = ParseValue();
	}

	public CSJSONObject GetValue() {
		return value;
	}

	private CSJSONToken NextToken() {
		return token = tokenizer.GetNextToken();
	}

	private CSJSONObject parseArray() {
		CSJSONObject a = new CSJSONObject();
		a.f_eObjType = EJSONObjectType.eJOT_Array;
		NextToken();
		if (token.type == CSJSONTokenType.RIGHT_BRACKET) {
			return a;
		}
		while (true) {
			a.push(ParseValue());
			NextToken();
			if (token.type == CSJSONTokenType.RIGHT_BRACKET) {
				return a;
			} else if (token.type == CSJSONTokenType.COMMA) {
				NextToken();
			} else {
				Debug.LogError("Expecting ] or , but found " + token.value);
				return null;
			}
		}
	}

	private CSJSONObject parseObject() {
		CSJSONObject o = new CSJSONObject();
		o.f_eObjType = EJSONObjectType.eJOT_Object;
		string key;
		NextToken();
		if (token.type == CSJSONTokenType.RIGHT_BRACE) {
			return o;
		}
		while (true) {
			if (token.type == CSJSONTokenType.STRING) {
				key = token.stringValue;
				NextToken();
				if (token.type == CSJSONTokenType.COLON) {
					NextToken();
					o[key] = ParseValue();	
					NextToken();
					if (token.type == CSJSONTokenType.RIGHT_BRACE) {
						return o;
					} else if (token.type == CSJSONTokenType.COMMA) {
						NextToken();
					} else {
						Debug.LogError("Expecting } or , but found " + token.value);
						return null;
					}
				} else {
					Debug.LogError("Expecting : but found " + token.value);
					return null;
				}
			} else {
				Debug.LogError("Expecting string but found " + token.value);
				return null;
			}
		}
	}

	private CSJSONObject ParseValue() {
		switch (token.type) {
			case CSJSONTokenType.LEFT_BRACE:
				return parseObject();
			case CSJSONTokenType.LEFT_BRACKET:
				return parseArray();
			case CSJSONTokenType.STRING:
			case CSJSONTokenType.NUMBER:
			case CSJSONTokenType.TRUE:
			case CSJSONTokenType.FALSE:
			case CSJSONTokenType.NULL:
				return token.value;
			default:
				Debug.LogError("Unexpected " + token.value);
				return null;
		}
	}
}
