using UnityEditor;
using UnityEngine;

public class E2DExporterData : ScriptableObject
{
	public string package = "";
	public string exportFolder = "";
	public Texture2D[] textures;
	public DefaultAsset uiPrefabFolder;

	private SerializedProperty _texProp;
	private SerializedObject _target;

	public void OnEnable()
	{
		_target = new SerializedObject(this);
		_texProp = _target.FindProperty("textures");
	}

	public void OnGUI()
	{
		package = EditorGUILayout.TextField("包名:", package);
		EditorGUILayout.PropertyField(_texProp, true);
		uiPrefabFolder = (DefaultAsset)EditorGUILayout.ObjectField("Prefab目录:", uiPrefabFolder, typeof(DefaultAsset), false);
		exportFolder = EditorGUILayout.TextField("导出目录:", exportFolder);

		EditorGUILayout.BeginHorizontal();
		GUILayout.FlexibleSpace();
		if (GUILayout.Button("Select"))
		{
			exportFolder = EditorUtility.OpenFolderPanel("导出目录", "", "");
		}
		EditorGUILayout.EndHorizontal();

		EditorGUILayout.Space();
		GUI.color = Color.red;
		if (GUILayout.Button("Force Build All", GUILayout.Height(50)))
		{
			E2DLayoutExporter.Export(package, textures, uiPrefabFolder, exportFolder, E2DLayoutExporter.BUILD_FORCE_ALL);
		}
		GUI.color = Color.white;

		if (GUILayout.Button("Build Default", GUILayout.Height(50)))
		{
			E2DLayoutExporter.Export(package, textures, uiPrefabFolder, exportFolder, E2DLayoutExporter.BUILD_DEFAULT);
		}

		if (GUILayout.Button("Build Config", GUILayout.Height(50)))
		{
			E2DLayoutExporter.Export(package, textures, uiPrefabFolder, exportFolder, E2DLayoutExporter.BUILD_CONFIG);
		}

		_target.ApplyModifiedProperties();
	}
}