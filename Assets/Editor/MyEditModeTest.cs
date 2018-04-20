using UnityEngine;
using UnityEditor;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using LitJson;
using UnityEditor.Sprites;
using UnityEngine.U2D;
using UnityEngine.UI;
using VisualDesignCafe.Editor.Prefabs;

public class MyEditModeTest
{
	[MenuItem("GameObject/PrintPos &#p")]
	public static void PrintPos()
	{
		var rectTrans = Selection.activeTransform as RectTransform;
		if (rectTrans != null)
		{
			Debug.Log("localPosition:" + rectTrans.localPosition);
			Debug.Log("worldPosition:" + rectTrans.position);
			Debug.Log("anchoredPosition:" + rectTrans.anchoredPosition);
			Debug.Log(rectTrans.parent.InverseTransformPoint(rectTrans.position));
		}
	}

	[Test]
	public void LoadCocoData()
	{
		var ta = Selection.activeObject as TextAsset;
		if (ta == null || !ta.name.EndsWith("_json"))
		{
			Debug.Log("Please select coco2json config!");
			return;
		}

		string cfgPath = AssetDatabase.GetAssetPath(ta);
		Debug.Log("Read " + cfgPath + " ...");
		var package = JsonMapper.ToObject<CocoPackage>(ta.text);

		Debug.Log(package.animations[0].frames[0][0].index);
	}

	[Test]
	public void GenSpriteWithCocoData()
	{
		var ta = Selection.activeObject as TextAsset;
		if (ta == null || !ta.name.EndsWith("_json"))
		{
			Debug.Log("Please select coco2json config!");
			return;
		}

		string cfgPath = AssetDatabase.GetAssetPath(ta);
		Debug.Log("Read " + cfgPath + " ...");
		var package = JsonMapper.ToObject<CocoPackage>(ta.text);

		string workDir = Path.GetDirectoryName(cfgPath);
		List<List<SpriteMetaData>> textureSpriteMetas = new List<List<SpriteMetaData>>();
		TextureImporter[] texImporters = new TextureImporter[package.texCount];
		for (int i = 0; i < package.texCount; i++)
		{
			int texId = i + 1;
			string path = Path.Combine(workDir, package.name + texId + ".png");
			var importer = (TextureImporter)AssetImporter.GetAtPath(path);
			if (importer == null)
			{
				Debug.LogError("Can not load texture:" + path);
				return;
			}
			texImporters[i] = importer;
			textureSpriteMetas.Add(new List<SpriteMetaData>());
		}
		Debug.Log("Success Load TextureImporter:" + package.texCount);

		for (int i = 0; i < package.sprites.Count; i++)
		{
			var cocoSpriteMeta = package.sprites[i];
			var sm = new SpriteMetaData
			{
				name = cocoSpriteMeta.name,
				rect = cocoSpriteMeta.rect
			};
			textureSpriteMetas[cocoSpriteMeta.texId].Add(sm);
		}

		for (int i = 0; i < package.texCount; i++)
		{
			var importer = texImporters[i];
			importer.textureType = TextureImporterType.Sprite;
			importer.spriteImportMode = SpriteImportMode.Multiple;
			importer.spritesheet = textureSpriteMetas[i].ToArray();
			EditorUtility.SetDirty(importer);
			AssetDatabase.ImportAsset(importer.assetPath, ImportAssetOptions.ForceUpdate);
			Debug.Log("Setup spriteSheet:" + importer.assetPath);
		}

		AssetDatabase.Refresh();
	}

	[Test]
	public void UnitTest()
	{
		var langDic = new SortedList<string, string>();
		langDic.Add("BTN_OPEN", "close");
		langDic.Add("BTN_CLOSE", "close");
		//langDic.Add("BTN_CLOSE", "close");
		
		Debug.Log(langDic.IndexOfKey("BTN_CLOSE"));

		//var coms = Selection.activeTransform.GetComponents<Component>();
		//foreach (var component in coms)
		//{
		//	Debug.Log(component.GetType().Name);
		//}

		//Debug.Log(rectTrans.localToWorldMatrix);
		//Debug.Log(rectTrans.worldToLocalMatrix);
		//var font1 = new E2DFontStyle();
		//font1.align = 3;
		//font1.size = 10;

		//var font2 = new E2DFontStyle();
		//font2.align = 3;
		//font2.size = 10;
		//Debug.Log(font1.Equals(font2));

		//Debug.Log(Color.white.ToBGRA());
		//Debug.Log(Color.red.ToBGRA());
		//Debug.Log(string.Format("0x{0:x}{1:x}{2:x}{3:x}", c32.a, c32.r, c32.g, c32.b));
		//var prefab = Selection.activeObject as GameObject;
		//foreach (Transform child in prefab.transform)
		//{
		//	Debug.Log(child.name);
		//}

		//string path = AssetDatabase.GetAssetPath(Selection.activeObject);
		//var objs = AssetDatabase.LoadAllAssetRepresentationsAtPath(path);
		//Debug.Log(objs.Length);

		//var atlas = Selection.activeObject as SpriteAtlas;

		//Debug.Log(atlas.spriteCount);

		//var sprite = atlas.GetSprite("p1");
		//Debug.Log(sprite.rect);
		//Debug.Log(string.Format("(xMin:{0}, yMin:{1}, xMax:{2}, yMax:{3})", sprite.rect.xMin, sprite.rect.yMin, sprite.rect.xMax, sprite.rect.yMax));

		//var tex = SpriteUtility.GetSpriteTexture(sprite, true);
		//Debug.Log(tex);
		//Debug.Log(tex.height);

		////var altasTex = Packer.GetTexturesForAtlas("DemoAtlas");
		////Debug.Log(altasTex.Length);

		//string atlasName;
		//Texture2D rawAtlasTex;
		//Packer.GetAtlasDataForSprite(sprite,out atlasName,out rawAtlasTex);
		//Debug.Log(atlasName);
		//Debug.Log(rawAtlasTex);

		//var objs = AssetDatabase.LoadAllAssetsAtPath("Assets/coco_res/characters21.png");
		//var sb = new StringBuilder();
		//foreach (var o in objs)
		//{
		//    sb.AppendLine(o.name + ":" + o.GetType().Name);
		//}

		//sb.Insert(0, "assetCount:" + objs.Length + "\n");
		//Debug.Log(sb);
	}

	[Test]
	public void ResetSpriteSheet()
	{
		var importer = (TextureImporter)AssetImporter.GetAtPath("Assets/scene_bg11.png");
		var data = new SpriteMetaData[1];
		var meta = new SpriteMetaData();
		meta.name = "scene_1";
		meta.rect = new Rect(1722, 2048 - 782, 168, 224);
		data[0] = meta;
		importer.spritesheet = data;

		EditorUtility.SetDirty((UnityEngine.Object)importer);
		//EditorUtility.SetDirty((UnityEngine.Object) texture);
		AssetDatabase.WriteImportSettingsIfDirty(importer.assetPath);

		EditorApplication.delayCall += (EditorApplication.CallbackFunction)(() =>
	   {
		   AssetDatabase.Refresh();
	   });

		//AssetDatabase.Refresh();
	}

	[Test]
	public void ConvertCocoTex()
	{
		var rawTex = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/scene_bg11.png");
		int w = 168;
		int h = 224;
		var pixels = rawTex.GetPixels(1722, 558, w, h);
		var newTex = new Texture2D(w, h);
		newTex.SetPixels(pixels);
		File.WriteAllBytes("Assets/newTex.png", newTex.EncodeToPNG());

		Debug.Log(rawTex);
		AssetDatabase.Refresh();
	}

	// A UnityTest behaves like a coroutine in PlayMode
	// and allows you to yield null to skip a frame in EditMode
	[Test]
	public void ConvertCocoRes()
	{
		var ta = Selection.activeObject as TextAsset;
		if (ta == null) return;

		Debug.Log(ta.name);
		var table = JsonMapper.ToObject<List<CocoSpriteMeta>>(ta.text);

		var importer = (TextureImporter)AssetImporter.GetAtPath("Assets/coco_res/" + ta.name + ".png");
		importer.textureType = TextureImporterType.Sprite;
		importer.spriteImportMode = SpriteImportMode.Multiple;
		var rawTex = AssetDatabase.LoadAssetAtPath<Texture2D>(importer.assetPath);
		var texHeight = rawTex.height;

		var spriteMetas = new SpriteMetaData[table.Count];
		for (var i = 0; i < table.Count; i++)
		{
			var meta = table[i];
			var spriteMeta = new SpriteMetaData();
			spriteMeta.name = meta.name;
			spriteMeta.rect = new Rect(meta.x, texHeight + meta.y, meta.w, meta.h);
			spriteMetas[i] = spriteMeta;
		}

		importer.spritesheet = spriteMetas;
		Debug.Log("All Sprite Count:" + spriteMetas.Length);

		EditorUtility.SetDirty(importer);
		EditorUtility.SetDirty(rawTex);
		AssetDatabase.WriteImportSettingsIfDirty(importer.assetPath);

		//string dirPath = Path.GetDirectoryName(importer.assetPath) + "/" + ta.name + "_imgs";
		//Directory.CreateDirectory(dirPath);

		//foreach (var spriteMeta in spriteMetas)
		//{
		//	int w = (int)spriteMeta.rect.width;
		//	int h = (int)spriteMeta.rect.height;
		//	var colors = rawTex.GetPixels((int)spriteMeta.rect.x, (int)spriteMeta.rect.y, w, h);
		//	var spriteTex = new Texture2D(w, h);
		//	spriteTex.SetPixels(colors);
		//	File.WriteAllBytes(dirPath + "/" + spriteMeta.name + ".png", spriteTex.EncodeToPNG());
		//}

		AssetDatabase.Refresh();
	}

	[Test]
	public void CsvTest()
	{
		E2DLocalization.LoadCsv();
		E2DLocalization.SaveCsv();
	}

	[Test]
	public void AnimationClipTest()
	{
		GameObject go = GameObject.Find("DemoPanel");
		var clip = Selection.activeObject as AnimationClip;
		Debug.Log(clip.GetClipFrameCount());
		var allBindings = AnimationUtility.GetCurveBindings(clip);
		foreach (var binding in allBindings)
		{
			Debug.Log(AnimationUtility.GetAnimatedObject(go, binding).name);
		}
	}
}

public class CocoSpriteMeta
{
	public string name;
	public int x;
	public int y;
	public int w;
	public int h;
}