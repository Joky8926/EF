using UnityEngine;
using System.Collections.Generic;

public class CSJSONEncoder {
	private string jsonString;

	public CSJSONEncoder(CSJSONObject value) {
		jsonString = ConvertToString(value);
	}

	public string GetString() {
		return jsonString;
	}

	private string ConvertToString(CSJSONObject value) {
		if (value == null) {
			return "null";
		}
		switch (value.f_eObjType) {
			case EJSONObjectType.eJOT_String:
				return EscapeString(value.GetString());
			case EJSONObjectType.eJOT_Int:
				return value.GetInt().ToString();
			case EJSONObjectType.eJOT_Float:
				return value.GetFloat().ToString();
			case EJSONObjectType.eJOT_Bool:
				return value.GetBool() ? "true" : "false";
			case EJSONObjectType.eJOT_Array:
				return ArrayToString(value.GetArray());
			case EJSONObjectType.eJOT_Object:
				return ObjectToString(value.GetObject());
		}
		return "null";
	}

	private string EscapeString(string str) {
		string s = "";
		char ch;
		int len = str.Length;
		for (int i = 0; i < len; i++) {
			ch = str[i];
			switch (ch) {
				case '"':
					s += "\\\"";
					break;
				case '\\':
					s += "\\\\";
					break;
				case '\b':
					s += "\\b";
					break;
				case '\f':
					s += "\\f";
					break;
				case '\n':
					s += "\\n";
					break;
				case '\r':
					s += "\\r";
					break;
				case '\t':
					s += "\\t";
					break;
				default:
					if (ch < ' ')
					{
						string hexCode = ((int)ch).ToString("x");
						string zeroPad = hexCode.Length == 2 ? "00" : "000";
						s += "\\u" + zeroPad + hexCode;
					} else {
						s += ch;
					}
					break;
			}
		}
		return "\"" + s + "\"";
	}

	private string ArrayToString(CSJSONObject[] a) {
		string s = "";
		for (int i = 0; i < a.Length; i++) {
			if (s.Length > 0) {
				s += ",";
			}
			s += ConvertToString(a[i]);
		}
		return "[" + s + "]";
	}

	private string ObjectToString(Dictionary<string, CSJSONObject> o) {
		string s = "";
		CSJSONObject value;
		foreach (string key in o.Keys) {
			value = o[key];
			if (s.Length > 0) {
				s += ",";
			}
			s += EscapeString(key) + ":" + ConvertToString(value);
		}
		return "{" + s + "}";
	}
}
