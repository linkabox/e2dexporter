using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;
using UnityEngine.U2D;
using Debug = UnityEngine.Debug;

public class E2DAssetImporter : AssetPostprocessor
{
	private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
	{
		foreach (string path in importedAssets)
		{
			if (Path.GetExtension(path).Equals(".tpsheet"))
			{
				Debug.Log("Set <tpsheet> update flag:" + path);
				EditorPrefs.SetBool("E2D_" + Path.GetFileNameWithoutExtension(path), true);
			}
			else if (path.Contains("UI/RawImages"))
			{
				Debug.Log("Set <rawImage> update flag:" + path);
				EditorPrefs.SetBool("E2D_" + Path.GetFileNameWithoutExtension(path), true);
			}
		}
	}
}

public class E2DLayoutExporter : EditorWindow
{
	public const string cachedPath = "Assets/Editor/E2D/E2DExporter.asset";
	public static E2DLayoutExporter instance;
	private static List<GameObject> _instGoList;

	private E2DExporterData data;

	[MenuItem("E2D/E2DLayoutExporter &#u")]
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
		if (data != null)
			data.OnGUI();
	}

	public const int BUILD_DEFAULT = 0;         //默认模式，图集和rawimage未变更不重新导出
	public const int BUILD_CONFIG = 1;          //仅重新生成coco配置，不对贴图进行处理
	public const int BUILD_FORCE_ALL = 2;       //强制Build所有资源
	public static void Export(string package, Texture2D[] uiAtlas, DefaultAsset prefabFolder, string exportDir, int mode)
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
			//Prefab根节点隐藏的忽略导出
			if (prefab.activeSelf)
			{
				GameObject instGo = Instantiate(prefab, E2DLocalization.E2DUIRoot.transform);
				var instTrans = instGo.transform as RectTransform;
				if (instTrans.localScale != Vector3.one)
				{
					Debug.LogErrorFormat("根节点Scale{0}不符合要求: {1}", instTrans.localScale, prefab.name);
					prefab.transform.localScale = Vector3.one;
					instTrans.localScale = Vector3.one;
				}

				if (instTrans.anchoredPosition != Vector2.zero)
				{
					Debug.LogErrorFormat("根节点Pos{0}不符合要求: {1}", instTrans.anchoredPosition, prefab.name);
					instTrans.anchoredPosition = Vector3.zero;
				}

				instGo.name = prefab.name;
				e2dPackage.AddWidget(instTrans);
				_instGoList.Add(instGo);
				logger.AppendLine("register:" + prefabPath);
			}
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

		prefabGUIDs = AssetDatabase.FindAssets("t:prefab", new[] { "Assets/UI/BitmapFonts" });
		logger.AppendLine("Analyze BitmapFonts:" + prefabGUIDs.Length);

		foreach (var uid in prefabGUIDs)
		{
			string prefabPath = AssetDatabase.GUIDToAssetPath(uid);
			var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
			//Prefab根节点隐藏的忽略导出
			if (prefab.activeSelf)
			{
				var e2dBitmapFont = prefab.GetComponent<E2DBitmapFont>();
				if (e2dBitmapFont != null)
				{
					e2dPackage.RegisterCom(e2dBitmapFont);
					logger.AppendLine("register:" + prefabPath);
				}
			}
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

		bool forceRefresh = mode == BUILD_FORCE_ALL;
		if (mode != BUILD_CONFIG)
		{
			//导出图集打包后的贴图
			for (var i = 0; i < e2dPackage.textures.Count; i++)
			{
				string source = AssetDatabase.GetAssetPath(e2dPackage.textures[i]);
				string atlasName = Path.GetFileNameWithoutExtension(source);
				string key = "E2D_" + atlasName;
				if (forceRefresh || EditorPrefs.GetBool(key, true))
				{
					EditorPrefs.SetBool(key, false);
					string dest = Path.Combine(exportDir, e2dPackage.name + (i + 1) + ".png");
					FileUtil.ReplaceFile(source, dest);
					logger.AppendLine("Export AtlasTexture:" + dest);
					Process.Start("premultiply_alpha.bat", dest);
				}
			}

			Directory.CreateDirectory(exportDir + "/" + E2DPackage.RawImageDir);
			foreach (var texture in e2dPackage.rawImageSet)
			{
				string source = AssetDatabase.GetAssetPath(texture);
				string dest = Path.Combine(exportDir + "/" + E2DPackage.RawImageDir, texture.name + ".png");
				string key = "E2D_" + texture.name;
				if (forceRefresh || EditorPrefs.GetBool(key, true))
				{
					EditorPrefs.SetBool(key, false);
					FileUtil.ReplaceFile(source, dest);
					logger.AppendLine("Export RawTexture:" + dest);
					Process.Start("premultiply_alpha.bat", dest);
				}
			}
		}

		//清理实例化的Prefab
		foreach (var go in _instGoList)
		{
			DestroyImmediate(go);
		}

		//将coco导出为bin格式
		string workingDir = Path.GetDirectoryName(exportDir);
		Process p = new Process();
		p.StartInfo.WorkingDirectory = workingDir;
		p.StartInfo.FileName = Path.Combine(workingDir, @"platform\msvc\output\Debug\ej2dx.exe");
		p.StartInfo.Arguments = string.Format("-d {0}\\ -r coco_packer -u {1}", workingDir, e2dPackage.name);
		p.StartInfo.UseShellExecute = false;
		p.Start();
		p.WaitForExit();

		Debug.Log(logger);
		//EditorUtility.DisplayDialog("Finish", "生成成功!", "OK");
		AssetDatabase.Refresh();
	}
}
