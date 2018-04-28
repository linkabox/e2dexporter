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
	private static List<CocoAnimMeta> _allAnimations;

	public static List<CocoAnimMeta> AnimationList
	{
		get
		{
			if (_allAnimations == null)
			{
				LoadRes();
			}

			return _allAnimations;
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


	public static void LoadRes()
	{
		if (_allAnimations != null) return;

		string resDir = "Assets/coco_res/";
		//加载动画配置
		var ta = AssetDatabase.LoadAssetAtPath<TextAsset>(resDir + "characters2_anim.json");
		_allAnimations = JsonMapper.ToObject<List<CocoAnimMeta>>(ta.text);

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
}
