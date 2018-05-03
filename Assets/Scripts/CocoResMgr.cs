using System.Collections.Generic;
using LitJson;
using UnityEditor;
using UnityEngine;

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

	private static Dictionary<int, Mesh> _allSpriteMeshes;
	public static Dictionary<int, Mesh> SpriteMeshDic
	{
		get
		{
			if (_allSpriteMeshes == null)
			{
				LoadRes();
			}

			return _allSpriteMeshes;
		}
	}

	private static List<Texture2D> _textures;
	public static List<Texture2D> SpriteTextures
	{
		get
		{
			if (_textures == null)
			{
				LoadRes();
			}

			return _textures;
		}
	}

	private static List<Material> _materials;
	public static List<Material> SpriteMaterials
	{
		get
		{
			if (_materials == null)
			{
				LoadRes();
			}

			return _materials;
		}
	}

	private static Dictionary<int, CocoMetaData> _metaDatas;
	public static Dictionary<int, CocoMetaData> MetaDatas
	{
		get
		{
			if (_metaDatas == null)
			{
				LoadRes();
			}

			return _metaDatas;
		}
	}


	public static void LoadRes()
	{
		if (_cocoPackage != null) return;

		string resDir = "Assets/coco_res/";
		//加载动画配置
		var ta = AssetDatabase.LoadAssetAtPath<TextAsset>(resDir + "characters2_pak.json");
		_cocoPackage = JsonMapper.ToObject<CocoPackage>(ta.text);

		_metaDatas = new Dictionary<int, CocoMetaData>();
		foreach (var spritaMeta in _cocoPackage.sprites)
		{
			_metaDatas.Add(spritaMeta.id, spritaMeta);
		}
		_allSpriteMeshes = new Dictionary<int, Mesh>(_metaDatas.Count);

		foreach (var animMeta in _cocoPackage.animations)
		{
			_metaDatas.Add(animMeta.id, animMeta);
		}

		//加载所有Sprite
		_textures = new List<Texture2D>();
		_materials = new List<Material>();
		var spriteMat = Resources.Load<Material>("spriteMat");
		string[] files = { "characters21.png", "characters22.png", "characters23.png", "characters24.png" };
		foreach (var file in files)
		{
			var tex = AssetDatabase.LoadAssetAtPath<Texture2D>(resDir + file);
			_textures.Add(tex);
			var mat = Object.Instantiate(spriteMat);
			mat.mainTexture = tex;
			_materials.Add(mat);
		}
	}

	public static CocoAnimMeta GetAnimMeta(int id)
	{
		var meta = GetMetaData(id);
		return meta as CocoAnimMeta;
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

	public static CocoMetaData GetMetaData(int id)
	{
		CocoMetaData meta;
		MetaDatas.TryGetValue(id, out meta);
		return meta;
	}

	public static CocoSpritaMeta GetSpriteMeta(int id)
	{
		var meta = GetMetaData(id);
		return meta as CocoSpritaMeta;
	}

	public static Texture2D GetTexture(int texId)
	{
		return SpriteTextures[texId];
	}

	public static Material GetMaterial(int texId)
	{
		return SpriteMaterials[texId];
	}

	public static void CacheSpriteMesh(int spriteId, Mesh mesh)
	{
		if (!SpriteMeshDic.ContainsKey(spriteId))
		{
			SpriteMeshDic.Add(spriteId, mesh);
		}
	}

	public static Mesh CreateSpriteMesh(CocoSpritaMeta meta, Texture tex)
	{
		Mesh mesh;
		if (!SpriteMeshDic.TryGetValue(meta.id, out mesh))
		{
			mesh = new Mesh();

			var vert = new Vector3[4];
			vert[0] = new Vector3(meta.screen[0] / 16, -meta.screen[1] / 16, 0);
			vert[1] = new Vector3(meta.screen[2] / 16, -meta.screen[3] / 16, 0);
			vert[2] = new Vector3(meta.screen[4] / 16, -meta.screen[5] / 16, 0);
			vert[3] = new Vector3(meta.screen[6] / 16, -meta.screen[7] / 16, 0);
			mesh.vertices = vert;

			var tri = new int[6];
			tri[0] = 0;
			tri[1] = 3;
			tri[2] = 1;

			tri[3] = 2;
			tri[4] = 1;
			tri[5] = 3;
			mesh.triangles = tri;

			var normals = new Vector3[4];
			normals[0] = -Vector3.forward;
			normals[1] = -Vector3.forward;
			normals[2] = -Vector3.forward;
			normals[3] = -Vector3.forward;
			mesh.normals = normals;

			Vector2[] uv = new Vector2[4];
			uv[0] = new Vector2(meta.src[0] / tex.width, 1 - meta.src[1] / tex.height);  // 0
			uv[1] = new Vector2(meta.src[2] / tex.width, 1 - meta.src[3] / tex.height);  // 1
			uv[2] = new Vector2(meta.src[4] / tex.width, 1 - meta.src[5] / tex.height);  // 2
			uv[3] = new Vector2(meta.src[6] / tex.width, 1 - meta.src[7] / tex.height);  // 3
			mesh.uv = uv;

			mesh.RecalculateBounds();
			CacheSpriteMesh(meta.id, mesh);
		}

		return mesh;
	}

}