using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class E2DAnimator : MonoBehaviour
{
	public AnimationClip curClip;
	public AnimationClip[] exportClips;

	[Range(0, 1)]
	public float slider;

	public void SetCurClip(string clipName)
	{
		if (exportClips == null) return;

		foreach (var clip in exportClips)
		{
			if (clip != null && clip.name == clipName)
			{
				this.curClip = clip;
				return;
			}
		}
	}

	// Update is called once per frame
	void Update()
	{
		//if (curClip != null)
		//{
		//	curClip.SampleAnimation(gameObject, curClip.length * slider);
		//}
	}

	public void SampleAnimation(AnimationClip clip, float time)
	{
		clip.SampleAnimation(this.gameObject, time);
	}
}
