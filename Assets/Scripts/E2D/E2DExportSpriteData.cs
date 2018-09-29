using System.Collections.Generic;
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

#if UNITY_EDITOR
	[UnityEditor.MenuItem("E2D/GenExportSpriteData")]
	static void ResetData()
	{
		_instance = GenData();
	}
#endif

	static E2DExportSpriteData GenData()
	{
#if UNITY_EDITOR
		var data = UnityEditor.AssetDatabase.LoadAssetAtPath<E2DExportSpriteData>(cachedPath);
		if (data == null)
		{
			data = ScriptableObject.CreateInstance<E2DExportSpriteData>();
			UnityEditor.AssetDatabase.CreateAsset(data, cachedPath);
		}

		return data;
#else
		return null;
#endif
	}

	public bool Contain(Sprite sprite)
	{
		return sprites.Contains(sprite);
	}
}