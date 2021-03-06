﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
[RequireComponent(typeof(Graphic))]
public class UILocalize : MonoBehaviour
{
	[SerializeField]
	private string _key = "";
	public string key
	{
		get { return _key; }
		set
		{
			if (value == _key) return;
			_key = value;

			string val;
			if (Localization.TryGet(value, out val))
			{
				this.val = val;
			}
		}
	}

	private string _val;
	public string val
	{
		get { return _val; }
		private set
		{
			_val = value;

			Text lbl = GetComponent<Text>();
			if (lbl != null)
			{
				lbl.text = value;
#if UNITY_EDITOR
				if (!Application.isPlaying) UnityEditor.EditorUtility.SetDirty(lbl);
#endif
			}
		}
	}

	bool mStarted = false;

	/// <summary>
	/// Localize the widget on enable, but only if it has been started already.
	/// </summary>

	void OnEnable()
	{
#if UNITY_EDITOR
		if (!Application.isPlaying) return;
#endif
		if (mStarted) OnLocalize();
	}

	// Use this for initialization
	void Start()
	{
#if UNITY_EDITOR
		if (!Application.isPlaying) return;
#endif
		mStarted = true;
		OnLocalize();

	}

	void OnLocalize()
	{
		this.val = Localization.Get(key);
	}
}
