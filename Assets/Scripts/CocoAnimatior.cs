using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CocoAnimatior : MonoBehaviour
{
	public int fps = 30;
	public List<Sprite> sprites;
	public float timer;

	public int curFrame;
	private float interval;
	private SpriteRenderer renderer;

	// Use this for initialization
	void Start()
	{
		renderer = this.GetComponent<SpriteRenderer>();
		curFrame = 0;
		if (sprites.Count > 0)
		{
			renderer.sprite = sprites[0];
		}
	}

	// Update is called once per frame
	void Update()
	{
		if (sprites == null || sprites.Count == 0) return;

		interval = 1f / fps;
		timer += Time.deltaTime;
		if (timer >= interval)
		{
			timer -= interval;
			NextFrame();
		}
	}

	void NextFrame()
	{
		int nextFrame = curFrame + 1;
		if (nextFrame < sprites.Count)
		{
			renderer.sprite = sprites[nextFrame];
			curFrame = nextFrame;
		}
		else
		{
			curFrame = 0;
			renderer.sprite = sprites[curFrame];
		}
	}
}
