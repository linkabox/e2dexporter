using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;
using Debug = UnityEngine.Debug;
using Object = UnityEngine.Object;

public static class E2DLocalization
{
	[MenuItem("E2D/Localization/Open lang csv")]
	public static void OpenConfig()
	{
		Process.Start(Path.GetFullPath(LangPath));
	}

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

	private static List<string> _langKeys;                  //保持与csv一致的索引

	private static List<string> LangKeys
	{
		get
		{
			if (_langKeys == null)
			{
				LoadCsv();
			}

			return _langKeys;
		}
	}
	private static Dictionary<string, string[]> _langMap;  //Key,Lang[]
	private static Dictionary<string, string[]> LangMap
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

	public static bool ContainsKey(string key)
	{
		return LangMap.ContainsKey(key);
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

	public static string[] GetLangsByKey(string key)
	{
		if (string.IsNullOrEmpty(key)) return null;

		string[] langs;
		LangMap.TryGetValue(key, out langs);
		return langs;
	}

	public static int GetKeyIndex(string key)
	{
		if (!string.IsNullOrEmpty(key))
		{
			return LangKeys.IndexOf(key);
		}

		return -1;
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

	private static bool LoadCSV(byte[] bytes)
	{
		if (bytes == null) return false;
		ByteReader reader = new ByteReader(bytes);

		// The first line should contain "KEY", followed by languages.
		BetterList<string> header = reader.ReadCSV();

		// There must be at least two columns in a valid CSV file
		if (header.size < 2) return false;
		header.RemoveAt(0);

		_langIndex = 0;
		_langs = new string[header.size];
		for (int i = 0; i < header.size; i++)
		{
			_langs[i] = header[i];
		}

		_langMap = new Dictionary<string, string[]>();
		_langKeys = new List<string>();
		for (; ; )
		{
			BetterList<string> temp = reader.ReadCSV();
			if (temp == null || temp.size == 0) break;

			string key = temp[0];
			if (string.IsNullOrEmpty(key)) continue;

			var fields = new string[_langs.Length];
			for (int i = 1; i < temp.size; i++)
			{
				fields[i - 1] = temp[i];
			}

			_langMap.Add(key, fields);
			_langKeys.Add(key);
		}

		return true;
	}

	[MenuItem("E2D/Localization/Reload csv")]
	public static void LoadCsv()
	{
		var bytes = File.ReadAllBytes(E2DLocalization.LangPath);
		LoadCSV(bytes);

		E2DUIRoot.BroadcastMessage("OnLocalize", SendMessageOptions.DontRequireReceiver);
		Debug.Log("Load Localization csv success!");
	}
}
