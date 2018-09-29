using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
#endif


public class E2DPackage
{
	public const string RawImageDir = "raw_images";
	public static E2DPackage active;

	//只有在注册列表中的组件才会被导出到配置
	public List<IE2DExport> registedComs = new List<IE2DExport>();

	//屏幕坐标缩放比例
	public const int SCREEN_SCALE = 16;
	public string name;

	public List<Texture2D> textures;
	public List<E2DSprite> sprites;
	public List<E2DWidget> widgets;
	public List<E2DFontStyle> fontStyles;

	public Dictionary<string, E2DWidget> uiRefMap;
	public Dictionary<Sprite, E2DSprite> spriteRefMap;

	public Sprite defaultSprite;
	public HashSet<Texture> rawImageSet;

	public E2DPackage(string package, string defaultSpriteName, Texture2D[] textures)
	{
		this.name = package;

		this.textures = new List<Texture2D>();
		for (int i = 0; i < textures.Length; i++)
		{
			if (textures[i] != null)
			{
				this.textures.Add(textures[i]);
			}
		}

		//读取相关图集的所有Sprite
		List<Sprite> allSprites = new List<Sprite>();
#if UNITY_EDITOR
		foreach (var texture in this.textures)
		{
			var objs = AssetDatabase.LoadAllAssetRepresentationsAtPath(AssetDatabase.GetAssetPath(texture));
			foreach (var o in objs)
			{
				var sprite = o as Sprite;
				if (sprite != null)
				{
					allSprites.Add(sprite);
					if (sprite.name == defaultSpriteName)
					{
						this.defaultSprite = sprite;
					}
				}
			}
		}
#endif

		this.sprites = new List<E2DSprite>(allSprites.Count);
		this.spriteRefMap = new Dictionary<Sprite, E2DSprite>();
		for (int i = 0; i < allSprites.Count; i++)
		{
			var sprite = allSprites[i];
			var e2dSprite = new E2DSprite(sprite, this.textures, E2DExportSpriteData.Instance.Contain(sprite));
			this.sprites.Add(e2dSprite);
			RegisterCom(e2dSprite);
			spriteRefMap.Add(sprite, e2dSprite);
		}

		this.widgets = new List<E2DWidget>();
		this.uiRefMap = new Dictionary<string, E2DWidget>();

		this.fontStyles = new List<E2DFontStyle>();
		this.rawImageSet = new HashSet<Texture>();
	}

	public void AddRawTexture(Texture texture)
	{
		this.rawImageSet.Add(texture);
	}

	public E2DWidget AddWidget(Transform prefab)
	{
		var widget = new E2DWidget(prefab);
		RegisterCom(widget);
		this.widgets.Add(widget);
		this.uiRefMap.Add(prefab.name, widget);
		return widget;
	}

	public int GetRefWidgetID(string prefabName)
	{
		E2DWidget layout;
		if (this.uiRefMap.TryGetValue(prefabName, out layout))
		{
			return layout.id;
		}

		return -1;
	}

	public E2DWidget GetWidget(string prefabName)
	{
		E2DWidget widget;
		this.uiRefMap.TryGetValue(prefabName, out widget);
		return widget;
	}

	/// <summary>
	/// 分析Text组件，生成对应FontStyle缓存下来，最终导出为label类型配置
	/// </summary>
	/// <param name="text"></param>
	/// <returns></returns>
	public E2DFontStyle AddFontStyle(Text text)
	{
		var newFont = new E2DFontStyle(text);
		foreach (var oldFont in fontStyles)
		{
			if (newFont.Equals(oldFont))
			{
				return oldFont;
			}
		}

		//新样式字体
		RegisterCom(newFont);
		this.fontStyles.Add(newFont);
		return newFont;
	}

	public int RegisterCom(IE2DExport com)
	{
		com.id = registedComs.Count;
		registedComs.Add(com);
		return com.id;
	}

	public string Export()
	{
		var sb = new StringBuilder();
		sb.AppendFormat("texture({0})\n", this.textures.Count);

		foreach (var com in registedComs)
		{
			sb.Append(com.Export());
		}

		return sb.ToString();
	}
}