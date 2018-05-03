using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class CocoSpriteRenderer : MonoBehaviour
{
	public int spriteId;
	private MeshRenderer _renderer;
	public MeshRenderer renderer
	{
		get
		{
			if (_renderer == null)
			{
				_renderer = this.GetComponent<MeshRenderer>();
			}

			return _renderer;
		}
	}

	private MeshFilter _meshFilter;

	public MeshFilter meshFilter
	{
		get
		{
			if (_meshFilter == null)
			{
				_meshFilter = this.GetComponent<MeshFilter>();
			}

			return _meshFilter;
		}
	}
	private CocoSpritaMeta _spriteMeta;
	private Texture2D _spriteTex;

	void Start()
	{
		Rebuild();
	}

	public void SetSpriteId(int id)
	{
		this.spriteId = id;
		Rebuild();
	}

	[ContextMenu("Rebuild")]
	public void Rebuild()
	{
		if (spriteId == -1) return;

		_spriteMeta = CocoResMgr.GetSpriteMeta(spriteId);
		_spriteTex = CocoResMgr.GetTexture(_spriteMeta.texId);
		if (_spriteTex == null)
		{
			Debug.LogError("GetTexture Error:" + _spriteMeta.texId);
			return;
		}
		renderer.material.mainTexture = _spriteTex;
		meshFilter.mesh = CocoResMgr.CreateSpriteMesh(_spriteMeta, _spriteTex);
	}
}
