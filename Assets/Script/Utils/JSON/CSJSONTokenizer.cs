using UnityEngine;
using System;
using System.Collections;
using System.Globalization;

public class CSJSONTokenizer {
	private CSJSONObject obj;
	private string jsonString;
	private int loc;
	private char ch;

	public CSJSONTokenizer(string s) {
		jsonString = s;
		
		loc = 0;
		NextChar();
	}

	public CSJSONToken GetNextToken() {
		CSJSONToken token = new CSJSONToken();
		SkipIgnored();
		switch (ch) {
			case '{':
				token.type = CSJSONTokenType.LEFT_BRACE;
				NextChar();
				break;
			case '}':
				token.type = CSJSONTokenType.RIGHT_BRACE;
				NextChar();
				break;
			case '[':
				token.type = CSJSONTokenType.LEFT_BRACKET;
				NextChar();
				break;
			case ']':
				token.type = CSJSONTokenType.RIGHT_BRACKET;
				NextChar();
				break;
			case ',':
				token.type = CSJSONTokenType.COMMA;
				NextChar();
				break;
			case ':':
				token.type = CSJSONTokenType.COLON;
				NextChar();
				break;
			case 't':
				string possibleTrue = "t" + NextChar() + NextChar() + NextChar();
				if (possibleTrue == "true")
				{
					token.type = CSJSONTokenType.TRUE;
					token.value = new CSJSONObject(true);
					NextChar();
				} else {
					Debug.LogError("Expecting 'true' but found " + possibleTrue);
				}
				break;
			case 'f':
				string possibleFalse = "f" + NextChar() + NextChar() + NextChar() + NextChar();
				if (possibleFalse == "false")
				{
					token.type = CSJSONTokenType.FALSE;
					token.value = new CSJSONObject(false);
					NextChar();
				} else {
					Debug.LogError("Expecting 'false' but found " + possibleFalse);
				}
				break;
			case 'n':
				string possibleNull = "n" + NextChar() + NextChar() + NextChar();
				if (possibleNull == "null")
				{
					token.type = CSJSONTokenType.NULL;
					token.value = null;
					NextChar();
				} else {
					Debug.LogError("Expecting 'null' but found " + possibleNull);
				}
				break;
			case '"':
				token = readString();
				break;
			default:
				if (IsDigit(ch) || ch == '-') {
					token = readNumber();
				} else if (f_bEnd) {
					return null;
				} else {
					Debug.LogError("Unexpected " + ch + " encountered");
				}
				break;
		}
		return token;
	}

	private CSJSONToken readString() {
		CSJSONToken token = new CSJSONToken();
		token.type = CSJSONTokenType.STRING;
		string str = "";
		NextChar();
		while (ch != '"' && !f_bEnd) {
			if (ch == '\\') {
				NextChar();
				switch (ch) {
					case '"':
						str += '"';
						break;
					case '/':
						str += "/";
						break;
					case '\\':
						str += '\\';
						break;
					case 'b':
						str += '\b';
						break;
					case 'f':
						str += '\f';
						break;
					case 'n':
						str += '\n';
						break;
					case 'r':
						str += '\r';
						break;
					case 't':
						str += '\t';
						break;
					case 'u':
						string hexValue = "";
						for (int i = 0; i < 4; i++) {
							if (!IsHexDigit(NextChar())) {
								Debug.LogError(" Excepted a hex digit, but found: " + ch);
							}
							hexValue += ch;
						}
						str += (char)int.Parse(hexValue, NumberStyles.HexNumber);
						break;
					default:
						str += '\\' + ch;
						break;
				}
			} else {
				str += ch;
			}
			NextChar();
		}
		if (f_bEnd) {
			Debug.LogError("Unterminated string literal");
		}
		NextChar();
		token.stringValue = str;
		return token;
	}

	private CSJSONToken readNumber() {
		CSJSONToken token = new CSJSONToken();
		token.type = CSJSONTokenType.NUMBER;
		string input = "";
		bool bInt = true;
		if (ch == '-') {
			input += '-';
			NextChar();
		}
		if (!IsDigit(ch)) {
			Debug.LogError("Expecting a digit");
		}
		if (ch == '0') {
			input += ch;
			NextChar();
			if (IsDigit(ch)) {
				Debug.LogError("A digit cannot immediately follow 0");
			}
		} else {
			while (IsDigit(ch)) {
				input += ch;
				NextChar();
			}
		}
		if (ch == '.') {
			bInt = false;
			input += '.';
			NextChar();
			if (!IsDigit(ch)) {
				Debug.LogError("Expecting a digit");
			}
			while (IsDigit(ch)) {
				input += ch;
				NextChar();
			}
		}
		if (ch == 'e' || ch == 'E') {
			bInt = false;
			input += "e";
			NextChar();
			if (ch == '+' || ch == '-') {
				input += ch;
				NextChar();
			}
			if (!IsDigit(ch)) {
				Debug.LogError("Scientific notation number needs exponent value");
			}
			while (IsDigit(ch)) {
				input += ch;
				NextChar();
			}
		}
		if (bInt) {
			token.value = new CSJSONObject(int.Parse(input));
		} else {
			token.value = new CSJSONObject(float.Parse(input));
		}
        return token;
	}

	private char NextChar() {
		if (f_bEnd) {
			return ch = ' ';
		}
		return ch = jsonString[loc++];
	}

	private void SkipIgnored() {
		SkipWhite();
		SkipComments();
		SkipWhite();
	}

	private void SkipComments() {
		if (ch == '/') {
			NextChar();
			switch (ch) {
				case '/':
					do
					{
						NextChar();
					} while (ch != '\n' && !f_bEnd);
					NextChar();
					break;
				case '*':
					NextChar();
					while (true) {
						if (ch == '*')
						{
							NextChar();
							if (ch == '/')
							{
								NextChar();
								break;
							}
						} else {
							NextChar();
						}
						if (f_bEnd) {
							Debug.LogError("Multi-line comment not closed");
						}
					}
					break;
				default:
					Debug.LogError("Unexpected " + ch + " encountered (expecting '/' or '*' )");
					break;
			}
		}
	}

	private void SkipWhite() {
		while (IsWhiteSpace(ch)) {
			NextChar();
		}
	}

	private bool IsWhiteSpace(char ch) {
		return (ch == ' ' || ch == '\t' || ch == '\n');
	}

	private bool IsDigit(char ch) {
		return (ch >= '0' && ch <= '9');
	}

	private bool IsHexDigit(char ch) {
		return (IsDigit(ch) || (ch >= 'A' && ch <= 'F') || (ch >= 'a' && ch <= 'f'));
	}

	private bool f_bEnd {
		get {
			return loc >= jsonString.Length;
		}
	}
}
