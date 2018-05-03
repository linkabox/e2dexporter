using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PerfTest : MonoBehaviour
{
	public string anim_name;
	public int count = 100;

	public GameObject animPrefab;

	public List<CocoRenderer> animators;

	private CocoAnimMeta _animMeta;
	// Use this for initialization
	void Start()
	{
		_animMeta = CocoResMgr.GetAnimMetaByName(anim_name);
		animators = new List<CocoRenderer>(count);
		Gen();
	}

	// Update is called once per frame
	void Update()
	{

	}

	void OnGUI()
	{
		if (GUILayout.Button("Gen"))
		{
			Gen();
		}
	}

	void Gen()
	{
		if (_animMeta == null) return;
		for (int i = 0; i < animators.Count; i++)
		{
			Destroy(animators[i].cachedGo);
		}
		animators.Clear();

		for (int i = 0; i < count; i++)
		{
			var go = Instantiate(animPrefab);
			go.name = "anim_" + _animMeta.id;
			var animator = go.GetComponent<CocoAnimatior>();
			animator.anim_id = _animMeta.id;
			animators.Add(animator);
			go.transform.localPosition = new Vector3(Random.Range(-9, 10) * 30f, Random.Range(-12, 13) * 40f, 0f);
		}
	}
}
