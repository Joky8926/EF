using UnityEngine;
using System.Collections;

public class CSConnect {
	public delegate void CallbackDelegate(string sData);
	public delegate void MsgCallbackDelegate(CSJSONObject obj);

	public static void SendMsg(string mod, string d, MsgCallbackDelegate func, CSJSONObject obj = null) {
//		string sUrl = CSUrlConst.API;
//		string sHashKey = CSGameMain.f_Common.f_sHashKey;
//		if (sHashKey == null || sHashKey == "") {
//			return;
//		}
//		sUrl += "?m=" + mod + "&d=" + d + "&r=" + CSJSON.Encode(obj) + "&hashKey=" + sHashKey + "&t=" + Random.value;
//		CallbackDelegate callback = (string sData) => {
//			func(CSJSON.Decode(sData));
//		};
//		SCRMain.Coroutine(HttpRequest(sUrl, callback));
	}

	public static void GetHashKey(CallbackDelegate func) {
//		SCRMain.Coroutine(HttpRequest(CSUrlConst.AUTHOR, func));
	}

//	private static IEnumerator HttpRequest(string url, CallbackDelegate func) {
//		WWW www = new WWW(url);
//		yield return www;
//		if (www.error != null) {
//			Debug.LogError(www.error);
//		}
//		Debug.Log(www.text);
//		func(www.text);
//	}
}
