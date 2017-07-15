using UnityEngine;
using System.Collections;

public class CSJSON {

    public static string Encode(CSJSONObject o) {
		CSJSONEncoder encoder = new CSJSONEncoder(o);
        return encoder.GetString();
    }

	public static CSJSONObject Decode(string s) {
		CSJSONDecoder decoder = new CSJSONDecoder(s);
		return decoder.GetValue();
	}
}
