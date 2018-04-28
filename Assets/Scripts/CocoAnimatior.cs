using System.Collections;
using System.Collections.Generic;
using LitJson;
using UnityEditor;
using UnityEngine;

public class CocoAnimatior : MonoBehaviour
{
	public GameObject spritePrefab;
	public string anim_name;
	public int fps = 30;
	public List<CocoSpritaMeta> spriteMetas;
	public float timer;

	public int curFrame;
	private float interval;
	private List<SpriteRenderer> _spriteRenderers;
	private CocoAnimMeta _animMeta;

	// Use this for initialization
	void Start()
	{
		_animMeta = CocoResMgr.GetAnimMetaByName(anim_name);
		if (_animMeta != null)
		{
			spriteMetas = new List<CocoSpritaMeta>();
			foreach (int id in _animMeta.components)
			{
				spriteMetas.Add(CocoResMgr.GetSpriteMeta(id));
			}

			_spriteRenderers = new List<SpriteRenderer>(_animMeta.max_render_count);
			for (int i = 0; i < _animMeta.max_render_count; i++)
			{
				var go = Instantiate(spritePrefab, this.transform);
				_spriteRenderers.Add(go.GetComponent<SpriteRenderer>());
			}
		}
		curFrame = 0;
		Play(curFrame);
	}

	// Update is called once per frame
	void Update()
	{
		if (_animMeta == null) return;
		if (spriteMetas == null || spriteMetas.Count == 0) return;

		interval = 1f / fps;
		timer += Time.deltaTime;
		if (timer >= interval)
		{
			timer -= interval;
			NextFrame();
		}
	}

	void Play(int frame)
	{
		if (_animMeta == null || frame >= _animMeta.frames.Count) return;

		var frameTrans = _animMeta.frames[frame];

		for (int i = 0; i < frameTrans.Count; i++)
		{
			var ft = frameTrans[i];
			var renderer = _spriteRenderers[i];
			var meta = spriteMetas[ft.index];
			renderer.enabled = true;
			renderer.sprite = CocoResMgr.GetSprite(meta.id);
			E2DMatrix3x2.SetTransformFromMatrix(renderer.transform, ft.mat);
			var scale = renderer.transform.localScale;
			renderer.transform.localScale = new Vector3(scale.x * meta.scaleX, scale.y * meta.scaleY, 1);
		}

		//禁用当前帧多余Renderer
		for (int i = frameTrans.Count; i < _spriteRenderers.Count; i++)
		{
			_spriteRenderers[i].enabled = false;
		}
	}

	void NextFrame()
	{
		int nextFrame = curFrame + 1;
		if (nextFrame < _animMeta.frames.Count)
		{
			curFrame = nextFrame;
			Play(curFrame);
		}
		else
		{
			curFrame = 0;
		}
	}
}
