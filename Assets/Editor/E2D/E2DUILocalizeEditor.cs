using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

[CanEditMultipleObjects]
[CustomEditor(typeof(E2DUILocalize), true)]
public class E2DUILocalizeEditor : Editor
{
	private int _selectedIndex;

	public string curKey
	{
		get
		{
			if (mKeys != null && _selectedIndex < mKeys.Length)
			{
				return mKeys[_selectedIndex];
			}

			return null;
		}
	}

	private string[] mKeys;
	private E2DUILocalize _target;

	private bool _toggle = true;
	private string _newKey;

	void OnEnable()
	{
		_target = target as E2DUILocalize;
		Rebuild();
	}

	public void Rebuild()
	{
		string oldKey = _target.key;
		_selectedIndex = 0;
		var dict = E2DLocalization.LangMap;
		if (dict.Count > 0)
		{
			mKeys = dict.Keys.ToArray();
			for (int i = 0; i < mKeys.Length; i++)
			{
				if (mKeys[i] == oldKey)
				{
					_selectedIndex = i;
					break;
				}
			}
		}
	}

	public override void OnInspectorGUI()
	{
		GUILayout.Space(6f);

		if (mKeys != null && mKeys.Length > 0)
		{
			_selectedIndex = EditorGUILayout.Popup("Key:", _selectedIndex, mKeys);
			_target.key = this.curKey;
			string newVal = EditorGUILayout.TextField("Value:", _target.val);
			if (newVal != _target.val)
			{
				E2DLocalization.Set(_target.key, newVal);
			}
		}
		else
		{
			EditorGUILayout.LabelField("当前没有任何Key");
		}

		EditorGUILayout.Space();

		if (GUILayout.Button("Language", "MiniToolbarButton")) _toggle = !_toggle;
		if (_toggle)
		{
			GUILayout.BeginVertical("ProgressBarBack");
			GUILayout.BeginHorizontal();
			_newKey = EditorGUILayout.TextField("New Key:", _newKey);
			if (GUILayout.Button("Add Key"))
			{
				E2DLocalization.AddKey(_newKey);
				Rebuild();
				Repaint();
				return;
			}
			GUILayout.EndHorizontal();

			EditorGUILayout.Space();
			E2DLocalization.LangIndex = EditorGUILayout.Popup("Language:", E2DLocalization.LangIndex, E2DLocalization.Langs);

			EditorGUILayout.Space();
			if (GUILayout.Button("SaveCsv"))
			{
				E2DLocalization.SaveCsv();
			}
			if (GUILayout.Button("ReloadCsv"))
			{
				E2DLocalization.LoadCsv();
				Rebuild();
				Repaint();
				return;
			}
			GUILayout.EndVertical();
		}
	}
}
