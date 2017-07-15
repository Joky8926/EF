using UnityEngine;
using System.Collections.Generic;

public delegate float CalculateDelegate(int uLevel);

public class CSLargeInt {
	public const int THOUSAND = 1000;
	private static string[] ARR_UNIT = new string[] {"", "K", "M", "G", "T", "aa", "bb", "cc", "dd", "ee", "ff", "gg",
		"hh", "ii", "jj", "kk", "ll", "mm", "nn", "oo", "pp", "qq", "rr", "ss", "tt", "uu", "vv", "ww", "xx", "yy"};
	private List<int> m_lstNum;

	public CSLargeInt() {
		m_lstNum = new List<int>();
		m_lstNum.Add(0);
	}

	public CSLargeInt(string sNum) {
		m_lstNum = new List<int>();
		string[] arrStr = sNum.Split(',');
		for (int i = arrStr.Length - 1; i >= 0; i--) {
			if (arrStr[i].Length > 3) {
				Debug.LogError("NumberBase(string sNum):" + sNum);
			}
			m_lstNum.Add(int.Parse(arrStr[i]));
		}
	}

	public CSLargeInt(int uNum, int uUnit = 0) {
		m_lstNum = new List<int>();
		m_lstNum.Add(0);
		for (int i = 0; i < uUnit; i++) {
			m_lstNum.Add(0);
		}
		while (uNum > 0) {
			m_lstNum.Add(uNum % THOUSAND);
			uNum /= THOUSAND;
		}
		NoEmpty();
	}

	public CSLargeInt(CSLargeInt copyNum) {
		m_lstNum = new List<int>(copyNum.m_lstNum.ToArray());
	}

	public static CSLargeInt operator + (CSLargeInt l, CSLargeInt r) {
		CSLargeInt nb = new CSLargeInt (l);
		List<int> lstNbNum = nb.m_lstNum;
		List<int> lstRight = r.m_lstNum;
		SAdd(lstNbNum, lstRight);
		return nb;
	}

	public static CSLargeInt operator - (CSLargeInt l, CSLargeInt r) {
		if (!r.CheckIsBigger (l)) {
			Debug.LogError("NumberBase operator - :" + l + "-" + r);
		}
		CSLargeInt nb = new CSLargeInt ();
		List<int> lstNbNum = nb.m_lstNum;
		List<int> lstLeft = l.m_lstNum;
		List<int> lstRight = r.m_lstNum;
		lstNbNum[0] = lstLeft[0];
		int uBorr = 0;
		for (int i = 1; i < lstLeft.Count; i++) {
			if (i >= lstRight.Count) {
				if (lstLeft[i] < uBorr) {
					lstNbNum.Add(lstLeft[i] + THOUSAND - uBorr);
					uBorr = 1;
				} else {
					lstNbNum.Add(lstLeft[i] - uBorr);
					uBorr = 0;
				}
			} else {
				if (lstLeft[i] < lstRight[i] + uBorr) {
					lstNbNum.Add(lstLeft[i] + THOUSAND - lstRight[i] - uBorr);
					uBorr = 1;
				} else {
					lstNbNum.Add(lstLeft[i] - lstRight[i] - uBorr);
					uBorr = 0;
				}
			}
		}
		nb.Trim ();
		return nb;
	}

	public static CSLargeInt operator * (CSLargeInt l, int uNum) {
		return l.Multiply (uNum);
	}
	
	public static CSLargeInt operator * (int uNum, CSLargeInt r) {
		return r.Multiply (uNum);
	}
	
	public static CSLargeInt operator * (CSLargeInt l, float fNum) {
		CSLargeInt nb = l.Multiply ((int)(fNum * THOUSAND));
		nb.m_lstNum.RemoveAt (0);
		nb.NoEmpty();
		return nb;
	}
	
	public static CSLargeInt operator * (float fNum, CSLargeInt r) {
		CSLargeInt nb = r.Multiply ((int)(fNum * THOUSAND));
		nb.m_lstNum.RemoveAt (0);
		nb.NoEmpty();
		return nb;
	}
	
	public static float operator / (CSLargeInt l, CSLargeInt r) {
		if (!l.CheckIsBigger(r)) {
			Debug.LogError("float operator / : l > r :" + l + "," + r);
		}
		List<int> lstLeft = l.m_lstNum;
		List<int> lstRight = r.m_lstNum;
		int uRightCount = lstRight.Count;
		if (lstLeft.Count + 1 < uRightCount) {
			return 0f;
		}
		float lNum = 0f;
		float rNum = 1f;
		if (uRightCount <= 2) {
			lNum = lstLeft[1];
			rNum = lstRight[1];
		} else {
			rNum = lstRight[uRightCount - 1] * THOUSAND + lstRight[uRightCount - 2];
			for (int i = uRightCount - 1; i >= uRightCount - 2; i--) {
				if (lstLeft.Count > i) {
					lNum = lNum * THOUSAND + lstLeft[i];
				}
			}
		}
		return lNum / rNum;
	}

	public static bool operator <= (CSLargeInt l, CSLargeInt r) {
		return l.CheckIsBigger (r);
	}

	public static bool operator >= (CSLargeInt l, CSLargeInt r) {
		return r.CheckIsBigger(l);
	}
	
	public int f_uSmallNum {
		get {
			return m_lstNum[1];
		}
	}
	
	public static CSLargeInt MultiUp(CSLargeInt baseNum, float fInc, int uAddLevel, int uGiveNum = 0, CSLargeInt targetNB = null) {
		if (fInc < 1 && uGiveNum > 0) {
			Debug.LogError("MultiUp ERROR");
		}
		CSLargeInt nb = new CSLargeInt(baseNum);
		List<int> lstNbNum = nb.m_lstNum;
		int uInc = (int)(fInc * THOUSAND);
		for (int i = 0; i < uAddLevel; i++) {
			if (targetNB != null) {
				SAdd(targetNB.m_lstNum, lstNbNum);
			}
			lstNbNum[1] += uGiveNum;
			SMultiply(lstNbNum, uInc);
			lstNbNum.RemoveAt(0);
			if (uGiveNum > 0) {
				SSubtraction(lstNbNum, uGiveNum);
			}
		}
		nb.Trim();
		return nb;
	}
	
	public static CSLargeInt MultiUpB(CSLargeInt baseNum, int uAddLevel, CSLargeInt targetNB, CalculateDelegate func) {
		CSLargeInt nb = new CSLargeInt(baseNum);
		List<int> lstNbNum = nb.m_lstNum;
		for (int i = 0; i < uAddLevel; i++) {
			int uInc = (int)(func(i + 1) * THOUSAND);
			SAdd(targetNB.m_lstNum, lstNbNum);
			SMultiply(lstNbNum, uInc);
			lstNbNum.RemoveAt(0);
		}
		nb.Trim();
		return nb;
	}
	
	private static void SSubtraction(List<int> lstNbNum, int uOtherNum) {
		int uBorr = uOtherNum;
		for (int i = 1; i < lstNbNum.Count; i++) {
			if (lstNbNum[i] >= uBorr) {
				lstNbNum[i] -= uBorr;
				break;
			}
			lstNbNum[i] = lstNbNum[i] - uBorr + THOUSAND;
			uBorr = 1;
		}
	}
	
	public bool CheckIsSmall(int uNum) {
		if (m_lstNum.Count > 2) {
			return false;
		}
		return uNum > m_lstNum[1];
	}
	
	public string ShortFormat() {
		if (m_lstNum.Count - 1 > ARR_UNIT.Length) {
			return "???";
		}
		int uIndex = m_lstNum.Count - 1;
		if (uIndex == 1) {
			return m_lstNum[1].ToString();
		}
		string str = m_lstNum [uIndex].ToString () + ".";
		string sUnit = ARR_UNIT [uIndex - 1];
		int uSecond = m_lstNum [uIndex - 1];
		if (uSecond < 100) {
			str += "0";
			if (uSecond < 10) {
				str += "0";
			}
		}
		uSecond /= 10;
		if (uSecond > 0) {
			str += uSecond;
		}
		str += sUnit;
		return str;
	}

	private CSLargeInt Multiply(int otherNum) {
		CSLargeInt nb = new CSLargeInt (this);
		SMultiply(nb.m_lstNum, otherNum);
		nb.Trim ();
		return nb;
	}
	
	private static void SMultiply(List<int> lstNbNum, int otherNum) {
		int uCarry = 0;
		for (int i = 0; i < lstNbNum.Count; i++) {
			uCarry = lstNbNum[i] * otherNum + uCarry;
			lstNbNum[i] = uCarry % THOUSAND;
			uCarry = uCarry / THOUSAND;
		}
		while (uCarry > 0) {
			lstNbNum.Add(uCarry % THOUSAND);
			uCarry = uCarry / THOUSAND;
		}
	}
	
	private static void SAdd(List<int> lstTarget, List<int> lstRight) {
		int uLeftCount = lstTarget.Count;
		int uRightCount = lstRight.Count;
		int uCount = uLeftCount;
		if (uCount < uRightCount) {
			uCount = uRightCount;
		}
		int uCarry = 0;
		int uResNum = 0;
		for (int i = 1; i < uCount; i++) {
			if (i >= uLeftCount) {
				lstTarget.Add(0);
				uResNum = lstRight[i] + uCarry;
			} else if (i >= uRightCount) {
				uResNum = lstTarget[i] + uCarry;
			} else {
				uResNum = lstTarget[i] +lstRight[i] + uCarry;
			}
			lstTarget[i] = uResNum % THOUSAND;
			uCarry = uResNum / THOUSAND;
		}
		while (uCarry > 0) {
			lstTarget.Add(uCarry % THOUSAND);
			uCarry /= THOUSAND;
		}
	}
	
	private bool CheckIsBigger(CSLargeInt otherNum) {
		List<int> lstOther = otherNum.m_lstNum;
		if (lstOther.Count > m_lstNum.Count) {
			return true;
		}
		if (lstOther.Count < m_lstNum.Count) {
			return false;
		}
		for (int i = m_lstNum.Count - 1; i > 0; i--) {
			if (lstOther[i] > m_lstNum[i]) {
				return true;
			}
			if (lstOther[i] < m_lstNum[i]) {
				return false;
			}
		}
		return true;
	}
	
	private void Trim() {
		int uCheckIndex = m_lstNum.Count;
		for (int i = uCheckIndex - 1; i > 1; i--) {
			if (m_lstNum[i] != 0) {
				break;
			}
			uCheckIndex = i;
		}
		if (uCheckIndex < m_lstNum.Count) {
			m_lstNum.RemoveRange (uCheckIndex, m_lstNum.Count - uCheckIndex);
		}
	}
	
	private void NoEmpty() {
		while (m_lstNum.Count < 2) {
			m_lstNum.Add(0);
		}
	}

	public override string ToString () {
		string sRes = "";
		for (int i = 0; i < m_lstNum.Count; i++) {
			if (i == 0) {
				sRes = NumToString(m_lstNum[i]);
			} else {
				sRes = NumToString(m_lstNum[i]) + "," + sRes;
			}
		}
		return sRes;
	}

	private static string NumToString(int uNum) {
		string str = "";
		if (uNum < 100) {
			str += "0";
			if (uNum < 10) {
				str += "0";
			}
		}
		str += uNum.ToString ();
		return str;
	}
}
