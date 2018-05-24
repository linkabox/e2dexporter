using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class E2DExportSpriteData : ScriptableObject
{
	private static E2DExportSpriteData _instance;

	public static E2DExportSpriteData Instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = GenData();
			}
			return _instance;
		}
	}
	public const string cachedPath = "Assets/Editor/E2D/E2DExportSprite.asset";
	public List<Sprite> sprites;

	[MenuItem("E2D/GenExportSpriteData")]
	static void ResetData()
	{
		_instance = GenData();
	}

	static E2DExportSpriteData GenData()
	{
		var data = AssetDatabase.LoadAssetAtPath<E2DExportSpriteData>(cachedPath);
		if (data == null)
		{
			data = ScriptableObject.CreateInstance<E2DExportSpriteData>();
			AssetDatabase.CreateAsset(data, cachedPath);
		}

		return data;
	}

	public bool Contain(Sprite sprite)
	{
		return sprites.Contains(sprite);
	}
}