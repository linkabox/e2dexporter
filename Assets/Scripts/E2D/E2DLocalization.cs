using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

public static class E2DLocalization
{
	public const string LangPath = "Assets/UI/lang.csv";

	private static string[] _langs;

	public static string[] Langs
	{
		get
		{
			if (_langs == null)
			{
				LoadCsv();
			}
			return _langs;
		}
	}

	private static int _langIndex;
	public static int LangIndex
	{
		get { return _langIndex; }
		set { SelectLanguage(value); }
	}


	public static string language
	{
		get { return _langs[_langIndex]; }
		set { SelectLanguage(value); }
	}

	private static SortedList<string, string[]> _langMap;  //Key,Lang[]
	public static SortedList<string, string[]> LangMap
	{
		get
		{
			if (_langMap == null)
			{
				LoadCsv();
			}

			return _langMap;
		}
	}

	private static GameObject _uiRoot;
	public static GameObject E2DUIRoot
	{
		get
		{
			if (_uiRoot == null)
			{
				var canvas = Object.FindObjectOfType<Canvas>();
				if (canvas != null)
				{
					_uiRoot = canvas.gameObject;
				}
				else
				{
					Debug.LogError("场景不存在UIRoot");
				}
			}

			return _uiRoot;
		}
	}

	public static bool TryGet(string key, out string val)
	{
		if (!string.IsNullOrEmpty(key))
		{
			string[] langs;
			if (LangMap.TryGetValue(key, out langs))
			{
				if (_langIndex < langs.Length)
				{
					val = langs[_langIndex];
					return true;
				}
			}
		}

		val = "";
		return false;
	}

	public static string Get(string key)
	{
		if (!string.IsNullOrEmpty(key))
		{
			string[] langs;
			if (LangMap.TryGetValue(key, out langs))
			{
				if (_langIndex < langs.Length)
				{
					return langs[_langIndex];
				}
			}
		}

		return "";
	}

	public static int GetKeyIndex(string key)
	{
		if (!string.IsNullOrEmpty(key))
		{
			return LangMap.IndexOfKey(key);
		}

		return -1;
	}

	public static bool Set(string key, string val)
	{
		if (!string.IsNullOrEmpty(key))
		{
			string[] langs;
			if (LangMap.TryGetValue(key, out langs))
			{
				if (_langIndex < langs.Length && langs[_langIndex] != val)
				{
					langs[_langIndex] = val;
					SaveCsv();
					return true;
				}
			}
		}

		return false;
	}

	public static bool AddKey(string key)
	{
		if (!string.IsNullOrEmpty(key))
		{
			key = key.ToUpper();
			string[] newLangs;
			if (!LangMap.TryGetValue(key, out newLangs))
			{
				newLangs = new string[Langs.Length];
				LangMap.Add(key, newLangs);
				SaveCsv();
				Debug.Log("AddKey:" + key);
				return true;
			}
		}

		return false;
	}

	public static bool DeleteKey(string key)
	{
		if (LangMap.Remove(key))
		{
			SaveCsv();
			Debug.Log("DeleteKey:" + key);
			return true;
		}

		return false;
	}

	public static int GetLanguageIndex(string lang)
	{
		for (int index = 0; index < _langs.Length; ++index)
		{
			if (string.Equals(lang, _langs[index], StringComparison.Ordinal))
				return index;
		}
		return -1;
	}

	public static bool SelectLanguage(string language)
	{
		int newIndex = GetLanguageIndex(language);
		return SelectLanguage(newIndex);
	}

	public static bool SelectLanguage(int newIndex)
	{
		if (newIndex != -1 && newIndex < Langs.Length)
		{
			if (newIndex != _langIndex)
			{
				_langIndex = newIndex;
				E2DUIRoot.BroadcastMessage("OnLocalize", SendMessageOptions.DontRequireReceiver);
				PlayerPrefs.SetString("E2DLocalization", language);
				return true;
			}
		}

		return false;
	}

	public static void LoadCsv()
	{
		_langIndex = 0;
		_langMap = new SortedList<string, string[]>();
		var lines = File.ReadAllLines(E2DLocalization.LangPath);
		for (var i = 0; i < lines.Length; i++)
		{
			var line = lines[i];
			var cols = line.Split(',');
			if (i == 0)
			{
				_langs = new string[cols.Length - 1];
				for (int j = 1; j < cols.Length; j++)
				{
					_langs[j - 1] = cols[j];
				}
			}
			else
			{
				string key = cols[0];
				var langs = new string[_langs.Length];
				_langMap.Add(key, langs);
				for (int j = 1; j < cols.Length; j++)
				{
					langs[j - 1] = cols[j];
				}
			}
		}

		E2DUIRoot.BroadcastMessage("OnLocalize", SendMessageOptions.DontRequireReceiver);
		Debug.Log("Load Localization csv success!");
	}

	public static void SaveCsv()
	{
		if (_langMap == null) return;

		using (var file = new StreamWriter(LangPath))
		{
			file.WriteLine("key," + string.Join(",", _langs));
			for (int i = 0; i < _langMap.Keys.Count; i++)
			{
				string key = _langMap.Keys[i];
				if (!string.IsNullOrEmpty(key))
				{
					file.WriteLine(key + "," + string.Join(",", _langMap.Values[i]));
				}
				else
				{
					Debug.LogError("Key为空字符串，请检查!");
				}
			}
		}

		E2DUIRoot.BroadcastMessage("OnLocalize", SendMessageOptions.DontRequireReceiver);
		Debug.Log("Save Localization csv success!");
	}
}
