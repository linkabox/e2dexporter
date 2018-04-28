using System.Collections.Generic;
using LitJson;
using UnityEditor;
using UnityEngine;

public class CocoAnimMeta
{
	public int id;
	public string name;
	public List<int> components;

	//ejoy2d每帧可能包含多个子Sprite对象的变换
	public List<List<CocoFrameTrans>> frames;
	public int max_render_count;
}

public class CocoFrameTrans
{
	public int index;
	public float[] mat; //matrix3x2
}

public static class CocoResMgr
{
	private static CocoPackage _cocoPackage;

	public static List<CocoAnimMeta> AnimationList
	{
		get
		{
			if (_cocoPackage == null)
			{
				LoadRes();
			}

			return _cocoPackage.animations;
		}
	}

	private static Dictionary<int, Sprite> _allSprites;
	public static Dictionary<int, Sprite> SpriteDic
	{
		get
		{
			if (_allSprites == null)
			{
				LoadRes();
			}

			return _allSprites;
		}
	}

	private static Dictionary<int, CocoSpritaMeta> _spriteMetas;
	public static Dictionary<int, CocoSpritaMeta> SpriteMetas
	{
		get
		{
			if (_spriteMetas == null)
			{
				LoadRes();
			}

			return _spriteMetas;
		}
	}


	public static void LoadRes()
	{
		if (_cocoPackage != null) return;

		string resDir = "Assets/coco_res/";
		//加载动画配置
		var ta = AssetDatabase.LoadAssetAtPath<TextAsset>(resDir + "characters2_pak.json");
		_cocoPackage = JsonMapper.ToObject<CocoPackage>(ta.text);

		_spriteMetas = new Dictionary<int, CocoSpritaMeta>();
		foreach (var spritaMeta in _cocoPackage.sprites)
		{
			_spriteMetas.Add(spritaMeta.id, spritaMeta);
		}

		//加载所有Sprite
		_allSprites = new Dictionary<int, Sprite>();
		string[] files = { "characters21.png", "characters22.png", "characters23.png", "characters24.png" };
		foreach (var file in files)
		{
			var objs = AssetDatabase.LoadAllAssetRepresentationsAtPath(resDir + file);
			foreach (var o in objs)
			{
				var sprite = o as Sprite;
				if (sprite != null)
				{
					int id = int.Parse(sprite.name.Replace("pic_", ""));
					_allSprites.Add(id, sprite);
				}
			}
		}
	}

	public static CocoAnimMeta GetAnimMeta(int index)
	{
		return AnimationList[index];
	}

	public static CocoAnimMeta GetAnimMetaByName(string name)
	{
		var animList = AnimationList;
		foreach (var animMeta in animList)
		{
			if (animMeta.name == name)
			{
				return animMeta;
			}
		}

		return null;
	}

	public static Sprite GetSprite(int id)
	{
		Sprite sprite;
		SpriteDic.TryGetValue(id, out sprite);
		return sprite;
	}

	public static CocoSpritaMeta GetSpriteMeta(int id)
	{
		CocoSpritaMeta meta;
		SpriteMetas.TryGetValue(id, out meta);
		return meta;
	}
}
