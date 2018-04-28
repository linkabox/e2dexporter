using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CocoSpritaMeta
{
	public int id;
	public string name;
	public int texId;
	public Rect src;
	public Rect screen;

	public float scaleX
	{
		get { return (screen.width / 16) / src.width; }
	}

	public float scaleY
	{
		get { return (screen.height / 16) / src.height; }
	}

}

public class CocoPackage
{
	public string name;
	public int texCount;

	public List<CocoSpritaMeta> sprites;
	public List<CocoAnimMeta> animations;
}
