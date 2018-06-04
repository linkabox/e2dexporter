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
	private E2DUILocalize _target;
	void OnEnable()
	{
		_target = target as E2DUILocalize;
	}

	public override void OnInspectorGUI()
	{
		EditorGUILayout.BeginHorizontal();
		if (GUILayout.Button("Reload Csv"))
		{
			E2DLocalization.LoadCsv();
		}

		E2DLocalization.LangIndex = EditorGUILayout.Popup(E2DLocalization.LangIndex, E2DLocalization.Langs);
		GUILayout.FlexibleSpace();
		EditorGUILayout.EndHorizontal();

		serializedObject.Update();

		GUILayout.Space(6f);
		EditorGUIUtility.labelWidth = 80f;

		GUILayout.BeginHorizontal();
		_target.key = EditorGUILayout.TextField("Key", _target.key);

		string myKey = _target.key;
		bool isPresent = E2DLocalization.ContainsKey(myKey);
		GUI.color = isPresent ? Color.green : Color.red;
		GUILayout.BeginVertical(GUILayout.Width(22f));
		GUILayout.Space(2f);
		GUILayout.Label(isPresent ? "\u2714" : "\u2718", "TL SelectionButtonNew", GUILayout.Height(20f));
		GUILayout.EndVertical();
		GUI.color = Color.white;
		GUILayout.EndHorizontal();

		if (isPresent)
		{
			GUILayout.BeginVertical("ProgressBarBack");
			string[] langs = E2DLocalization.Langs;
			string[] fields = E2DLocalization.GetLangsByKey(myKey);

			for (int i = 0; i < fields.Length; i++)
			{
				GUI.color = i == E2DLocalization.LangIndex ? Color.red : Color.white;
				GUILayout.Label(langs[i] + "\t" + fields[i]);
				GUI.color = Color.white;
			}
			GUILayout.EndVertical();
		}

		serializedObject.ApplyModifiedProperties();
	}

}
