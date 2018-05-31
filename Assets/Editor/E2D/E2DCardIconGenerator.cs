using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using VisualDesignCafe.Editor.Prefabs;

public class E2DCardIconGenerator : EditorWindow
{
	public const string savePath = "Assets/UI/Prefabs/card_icon";
	public static E2DCardIconGenerator instance;

	[MenuItem("E2D/E2DCardIconGenerator")]
	static void Init()
	{
		if (instance != null)
		{
			instance.Close();
			return;
		}

		var window = EditorWindow.GetWindow<E2DCardIconGenerator>();
		window.Show();
		instance = window;
	}

	public Sprite[] icons;

	private GameObject[] templatePrefabs;
	private SerializedProperty _iconProp;
	private SerializedObject _target;

	void OnEnable()
	{
		_target = new SerializedObject(this);

		templatePrefabs = new GameObject[4];
		for (int i = 0; i < 4; i++)
		{
			var template = AssetDatabase.LoadAssetAtPath<GameObject>(savePath + "/mage" + (i + 1) + ".prefab");
			templatePrefabs[i] = template;
		}
		_iconProp = _target.FindProperty("icons");
	}

	void OnGUI()
	{
		EditorGUILayout.PropertyField(_iconProp, true);

		EditorGUILayout.PrefixLabel("Template:");
		for (int i = 0; i < templatePrefabs.Length; i++)
		{
			var template = templatePrefabs[i];
			EditorGUILayout.ObjectField(template, typeof(GameObject), false);
		}

		EditorGUILayout.Space();
		if (GUILayout.Button("Gen", GUILayout.Height(50)))
		{
			foreach (var icon in icons)
			{
				GenIconPrefab(icon, templatePrefabs);
			}
		}

		if (GUILayout.Button("Refresh All", GUILayout.Height(50)))
		{
			RefreshAllIconPrefab(templatePrefabs);
			EditorUtility.DisplayDialog("提示", "更新完成", "OK");
		}

		_target.ApplyModifiedProperties();
	}

	public static void GenIconPrefab(Sprite icon, GameObject[] templatePrefabs)
	{
		if (icon == null)
		{
			Debug.LogError("icon is null");
			return;
		}

		if (icon.name.Contains("mage")) return;


		string typeName = icon.name.Replace("_icon", "");
		for (var i = 0; i < templatePrefabs.Length; i++)
		{
			var templatePrefab = templatePrefabs[i];
			var go = GameObject.Instantiate(templatePrefab, E2DLocalization.E2DUIRoot.transform);
			go.name = typeName + (i + 1);
			for (int j = 0; j < i + 1; j++)
			{
				string itemName = "bg_" + (j + 1);
				var item_icon = go.transform.Find(itemName + "/icon").GetComponent<Image>();
				item_icon.sprite = icon;
				EditorExt.MakePixelPerfect(item_icon.rectTransform);
			}

			var prefabCom = go.GetComponent<Prefab>();
			if (prefabCom != null)
				DestroyImmediate(prefabCom);

			var guid = go.GetComponent<VisualDesignCafe.Editor.Prefabs.Guid>();
			if (guid != null)
				DestroyImmediate(guid);

			string path = savePath + "/" + go.name + ".prefab";
			var sourcePrefab = AssetDatabase.LoadAssetAtPath(path, typeof(GameObject));
			if (sourcePrefab == null)
			{
				sourcePrefab = PrefabUtility.CreateEmptyPrefab(path);
			}
			PrefabUtility.ReplacePrefab(go, sourcePrefab);
			DestroyImmediate(go);
		}
	}

	public static void RefreshAllIconPrefab(GameObject[] templatePrefabs)
	{
		var prefabGUIDs = AssetDatabase.FindAssets("t:prefab", new[] { savePath });
		foreach (var uid in prefabGUIDs)
		{
			string prefabPath = AssetDatabase.GUIDToAssetPath(uid);
			var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);

			if (prefab.name.StartsWith("mage"))
				continue;

			int count = int.Parse(prefab.name.Substring(prefab.name.Length - 1));
			var template = templatePrefabs[count - 1];
			for (int j = 0; j < count; j++)
			{
				string bgName = "bg_" + (j + 1);
				string iconName = bgName + "/icon";

				try
				{
					var template_bg = template.transform.Find(bgName).GetComponent<Image>();
					var item_bg = prefab.transform.Find(bgName).GetComponent<Image>();

					item_bg.rectTransform.anchoredPosition = template_bg.rectTransform.anchoredPosition;
					item_bg.rectTransform.localScale = template_bg.rectTransform.localScale;

					var template_icon = template.transform.Find(iconName).GetComponent<Image>();
					var item_icon = prefab.transform.Find(iconName).GetComponent<Image>();
					item_icon.rectTransform.anchoredPosition = template_icon.rectTransform.anchoredPosition;
					item_icon.rectTransform.localScale = template_icon.rectTransform.localScale;
				}
				catch (Exception e)
				{
					Debug.LogError(e.Message + "|" + bgName + "|" + iconName);
					throw;
				}
			}

			EditorUtility.SetDirty(prefab);
		}

		AssetDatabase.SaveAssets();
	}
}
