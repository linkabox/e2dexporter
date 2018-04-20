using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(E2DAnimator), true)]
public class E2DAnimatorEditor : Editor
{
	private E2DAnimator _target;
	private SerializedProperty _clipsProp;
	//private string[] _clipNames;
	//private int _selectedIndex;

	void OnEnable()
	{
		_target = target as E2DAnimator;
		_clipsProp = serializedObject.FindProperty("exportClips");
		Rebuild();
	}

	void Rebuild()
	{
		//var clipNames = new List<string>();
		//foreach (var clip in _target.exportClips)
		//{
		//	if (clip != null)
		//	{
		//		clipNames.Add(clip.name);
		//	}
		//}

		//_clipNames = clipNames.ToArray();
		//if (_selectedIndex >= _clipNames.Length)
		//{
		//	_selectedIndex = 0;
		//}
	}

	public override void OnInspectorGUI()
	{
		serializedObject.Update();
		EditorGUILayout.Space();

		//int newIndex = EditorGUILayout.Popup("CurClip:", _selectedIndex, _clipNames);
		//if (newIndex != _selectedIndex)
		//{
		//	_selectedIndex = newIndex;
		//	_target.SetCurClip(_clipNames[_selectedIndex]);
		//}

		EditorGUI.BeginChangeCheck();
		EditorGUILayout.PropertyField(_clipsProp, new GUIContent("exportClips"), true);
		if (EditorGUI.EndChangeCheck())
		{
			serializedObject.ApplyModifiedProperties();
			Rebuild();
		}

		//EditorGUILayout.Space();
		//EditorGUILayout.BeginHorizontal();
		//_target.slider = EditorGUILayout.Slider("Progress:", _target.slider, 0, 1);
		//if (GUILayout.Button("Reset"))
		//{
		//	_target.slider = 0;
		//}
		//EditorGUILayout.EndHorizontal();
	}
}
