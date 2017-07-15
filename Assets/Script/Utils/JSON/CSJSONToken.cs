using UnityEngine;
using System.Collections;

public class CSJSONToken {
	private int _type;
	private CSJSONObject _value;
	private string _stringValue;

	public CSJSONToken() {
		
	}

	public int type {
		get {
			return _type;
		}
		set {
			_type = value;
		}
	}

	public CSJSONObject value {
		get {
			return _value;
		}
		set {
			_value = value;
		}
	}

	public string stringValue {
		get {
			return _stringValue;
		}
		set {
			_value = new CSJSONObject(value);
			_stringValue = value;
		}
	}
}
