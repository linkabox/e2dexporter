using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;
using UnityEngine.U2D;
using Debug = UnityEngine.Debug;

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
		if (GUILayout.Button("Build All", GUILayout.Height(50)))
		{
			E2DLayoutExporter.Export(package, textures, uiPrefabFolder, exportFolder, false);
		}

		if (GUILayout.Button("Build Config", GUILayout.Height(50)))
		{
			E2DLayoutExporter.Export(package, textures, uiPrefabFolder, exportFolder, true);
		}

		_target.ApplyModifiedProperties();
	}
}

public class E2DLayoutExporter : EditorWindow
{
	public const string cachedPath = "Assets/Editor/E2D/E2DExporter.asset";
	public static E2DLayoutExporter instance;
	private static List<GameObject> _instGoList;

	private E2DExporterData data;

	[MenuItem("Window/E2DLayoutExporter &#u")]
	static void Init()
	{
		if (instance != null)
		{
			instance.Close();
			return;
		}

		var data = AssetDatabase.LoadAssetAtPath<E2DExporterData>(cachedPath);
		if (data == null)
		{
			data = ScriptableObject.CreateInstance<E2DExporterData>();
			AssetDatabase.CreateAsset(data, cachedPath);
		}

		var window = EditorWindow.GetWindow<E2DLayoutExporter>();
		window.data = data;
		window.Show();
		instance = window;
	}

	void OnEnable()
	{

	}

	void OnDisable()
	{

	}

	void OnGUI()
	{
		data.OnGUI();
	}

	public static void Export(string package, Texture2D[] uiAtlas, DefaultAsset prefabFolder, string exportDir, bool onlyConfig)
	{
		if (uiAtlas == null)
		{
			Debug.LogError("UI图集不能为空");
			return;
		}

		if (prefabFolder == null)
		{
			Debug.LogError("Prefab目录不能为空");
			return;
		}

		if (uiAtlas.Length <= 0)
		{
			Debug.LogError("Textures设置错误");
			return;
		}

		var logger = new StringBuilder();
		var e2dPackage = new E2DPackage(package, uiAtlas);
		E2DPackage.active = e2dPackage;

		//解析UIPrefab
		string prefabDir = AssetDatabase.GetAssetPath(prefabFolder);
		var prefabGUIDs = AssetDatabase.FindAssets("t:prefab", new[] { prefabDir });
		logger.AppendLine("Analyze UIPrefabs:" + prefabGUIDs.Length);

		_instGoList = new List<GameObject>();
		//先将所有ui_prefab注册好，再对prefab进行解析，因为存在嵌套引用的情况
		foreach (var uid in prefabGUIDs)
		{
			string prefabPath = AssetDatabase.GUIDToAssetPath(uid);
			var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
			GameObject instGo = Instantiate(prefab, E2DLocalization.E2DUIRoot.transform);
			Transform instTrans = instGo.transform;
			if (instTrans.localScale != Vector3.one)
			{
				Debug.LogErrorFormat("根节点Scale{0}不符合要求: {1}", instTrans.localScale, prefab.name);
				prefab.transform.localScale = Vector3.one;
				instTrans.localScale = Vector3.one;
			}

			if (instTrans.localPosition != Vector3.zero)
			{
				Debug.LogErrorFormat("根节点Pos{0}不符合要求: {1}", instTrans.localPosition, prefab.name);
				prefab.transform.localPosition = Vector3.zero;
				instTrans.localPosition = Vector3.zero;
			}

			instGo.name = prefab.name;
			e2dPackage.AddWidget(instTrans);
			_instGoList.Add(instGo);
			logger.AppendLine("register:" + prefabPath);
		}

		logger.AppendLine();
		logger.AppendLine("=======================================");
		foreach (var widget in e2dPackage.widgets)
		{
			widget.Convert();
			logger.AppendLine("convert:" + widget.name);
		}

		logger.AppendLine();
		logger.AppendLine("=======================================");

		//生成coco配置
		string cocoPath = Path.Combine(exportDir, e2dPackage.name + ".coco.bytes");
		File.WriteAllText(cocoPath, e2dPackage.Export());
		logger.AppendLine("Export e2dPackage:" + cocoPath);
		logger.AppendLine("sprite:" + e2dPackage.sprites.Count);
		logger.AppendLine("uiLayout:" + e2dPackage.widgets.Count);

		//拷贝lang本地化配置
		string langDest = Path.Combine(exportDir, "lang.csv");
		FileUtil.ReplaceFile(E2DLocalization.LangPath, langDest);
		logger.AppendLine("Export Lang csv:" + langDest);

		if (!onlyConfig)
		{
			//导出图集打包后的贴图
			for (var i = 0; i < e2dPackage.textures.Count; i++)
			{
				string source = AssetDatabase.GetAssetPath(e2dPackage.textures[i]);
				string dest = Path.Combine(exportDir, e2dPackage.name + (i + 1) + ".png");
				FileUtil.ReplaceFile(source, dest);
				logger.AppendLine("Export Texture:" + dest);
				//Process.Start(Path.GetFullPath("Assets/Editor/E2D/alpha.bat"), dest);
			}
		}

		//清理实例化的Prefab
		foreach (var go in _instGoList)
		{
			DestroyImmediate(go);
		}

		Debug.Log(logger);
		EditorUtility.DisplayDialog("Finish", "生成成功!", "OK");
		AssetDatabase.Refresh();
	}
}
