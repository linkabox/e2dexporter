using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
[RequireComponent(typeof(Graphic))]
public class E2DUILocalize : MonoBehaviour
{
	[SerializeField]
	private string _key = "";
	public string key
	{
		get { return _key; }
		set
		{
			if (value == _key) return;

			string val;
			if (E2DLocalization.TryGet(value, out val))
			{
				_key = value;
				this.val = val;
			}
		}
	}

	private string _val;
	public string val
	{
		get { return _val; }
		set
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
		this.val = E2DLocalization.Get(key);
	}
}
