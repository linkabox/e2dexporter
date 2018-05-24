using System.Collections;
using System.Collections.Generic;
using LitJson;
using UnityEditor;
using UnityEngine;

public abstract class CocoRenderer : MonoBehaviour
{
	public GameObject cachedGo
	{
		get
		{
			if (_mGo == null)
			{
				_mGo = this.gameObject;
			}

			return _mGo;
		}
	}
	private GameObject _mGo;

	public void SetActive(bool active)
	{
		cachedGo.SetActive(active);
	}
}

public class CocoAnimatior : CocoRenderer
{
	public int anim_id;
	public int fps = 30;
	public List<CocoMetaData> metaDatas;
	public float timer;

	public int curFrame;
	private float interval;
	private List<CocoRenderer> _cocoRenderers;
	private CocoAnimMeta _animMeta;

	// Use this for initialization
	void Start()
	{
		_animMeta = CocoResMgr.GetAnimMeta(anim_id);
		if (_animMeta != null)
		{
			metaDatas = new List<CocoMetaData>();
			foreach (int id in _animMeta.components)
			{
				metaDatas.Add(CocoResMgr.GetMetaData(id));
			}

			_cocoRenderers = new List<CocoRenderer>(_animMeta.components.Count);
			for (int i = 0; i < _animMeta.components.Count; i++)
			{
				var comId = _animMeta.components[i];
				var meta = CocoResMgr.GetMetaData(comId);
				if (meta != null)
				{
					if (meta is CocoSpritaMeta)
					{
						var spritePrefab = Resources.Load<GameObject>("CocoSpriteRenderer");
						var go = Instantiate(spritePrefab, this.transform);
						go.name = "sprite_" + meta.id;
						var spriteRenderer = go.GetComponent<CocoSpriteRenderer>();
						spriteRenderer.SetSpriteId(meta.id);
						_cocoRenderers.Add(spriteRenderer);
					}
					else
					{
						var animPrefab = Resources.Load<GameObject>("CocoAnimator");
						var go = Instantiate(animPrefab, this.transform);
						go.name = "anim_" + meta.id;
						var animator = go.GetComponent<CocoAnimatior>();
						animator.anim_id = meta.id;
						animator.fps = fps;
						_cocoRenderers.Add(animator);
					}
				}
			}
		}
		curFrame = 0;
		Play(curFrame);
	}

	// Update is called once per frame
	void Update()
	{
		if (_animMeta == null) return;
		if (metaDatas == null || metaDatas.Count == 0) return;

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

		//播放当前帧前先隐藏所有Renderer
		for (int i = 0; i < _cocoRenderers.Count; i++)
		{
			_cocoRenderers[i].SetActive(false);
		}

		for (int i = 0; i < frameTrans.Count; i++)
		{
			var ft = frameTrans[i];
			var renderer = _cocoRenderers[ft.index];
			renderer.SetActive(true);
			var mat = CocoMatrix3x2.ToMatrix4x4(ft.mat, -i * 1.0f);
			CocoMatrix3x2.SetTransformFromMatrix(renderer.transform, ref mat);
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
