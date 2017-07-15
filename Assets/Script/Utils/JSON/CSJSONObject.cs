using UnityEngine;
using System.Collections.Generic;

public enum EJSONObjectType {
    eJOT_String,
	eJOT_Int,
	eJOT_Float,
	eJOT_Bool,
    eJOT_Array,
    eJOT_Object
}

public class CSJSONObject {
	private EJSONObjectType m_eObjType;
	private Dictionary<string, CSJSONObject> m_dictObject;
	private List<CSJSONObject> m_lstObject;
	private string m_stringValue;
	private int m_intValue;
	private float m_floatValue;
	private bool m_boolValue;

    public CSJSONObject() {
        
    }

	public CSJSONObject(string value) {
		m_eObjType = EJSONObjectType.eJOT_String;
		m_stringValue = value;
	}

	public CSJSONObject(int value) {
		m_eObjType = EJSONObjectType.eJOT_Int;
		m_intValue = value;
	}

	public CSJSONObject(float value) {
		m_eObjType = EJSONObjectType.eJOT_Float;
		m_floatValue = value;
	}

	public CSJSONObject(bool value) {
		m_eObjType = EJSONObjectType.eJOT_Bool;
		m_boolValue = value;
	}

	public string GetString() {
		return m_stringValue;
	}

	public int GetInt() {
		if (m_eObjType == EJSONObjectType.eJOT_String) {
			m_intValue = int.Parse(m_stringValue);
		}
		return m_intValue;
	}

	public float GetFloat() {
		return m_floatValue;
	}

	public bool GetBool() {
		return m_boolValue;
	}

	public CSJSONObject[] GetArray() {
		return m_lstObject.ToArray();
	}

	public Dictionary<string, CSJSONObject> GetObject() {
		return m_dictObject;
	}

	public void push(CSJSONObject obj) {
		m_lstObject.Add(obj);
	}

	public override string ToString() {
		string str = "";
		switch (m_eObjType) {
			case EJSONObjectType.eJOT_Object:
				str = ObjectToString();
				break;
			case EJSONObjectType.eJOT_Array:
				str = ArrayToString();
				break;
			case EJSONObjectType.eJOT_Bool:
				str = m_boolValue.ToString();
				break;
			case EJSONObjectType.eJOT_Float:
				str = m_floatValue.ToString();
				break;
			case EJSONObjectType.eJOT_Int:
				str = m_intValue.ToString();
				break;
			case EJSONObjectType.eJOT_String:
				str = m_stringValue;
				break;
		}
		return str;
	}

	private string ObjectToString() {
		string str = "{";
		foreach (string key in m_dictObject.Keys) {
			str += key + ":" + m_dictObject[key].ToString() + ",";
		}
		str += "}";
		return str;
	}

	private string ArrayToString() {
		string str = "[";
		for (int i = 0; i < m_lstObject.Count; i++) {
			str += m_lstObject[i].ToString() + ",";
		}
		str += "]";
		return str;
	}

	public EJSONObjectType f_eObjType {
		get {
			return m_eObjType;
		}
		set {
			switch (value) {
				case EJSONObjectType.eJOT_Array:
					m_lstObject = new List<CSJSONObject>();
					break;
				case EJSONObjectType.eJOT_Object:
					m_dictObject = new Dictionary<string, CSJSONObject>();
					break;
			}
			m_eObjType = value;
		}
	}

	public CSJSONObject this[string key] {
		get {
			if (!m_dictObject.ContainsKey(key)) {
				return null;
			}
			return m_dictObject[key];
		}
		set {
			if (m_dictObject == null) {
				f_eObjType = EJSONObjectType.eJOT_Object;
			}
			m_dictObject[key] = value;
		}
	}
}
