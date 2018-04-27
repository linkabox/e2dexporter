using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CocoSpritaMeta
{
	public int id;
	public string name;
	public int texId;
	public Rect rect;
}

public class CocoPackage
{
	public string name;
	public int texCount;

	public List<CocoSpritaMeta> sprites;
	public List<CocoAnimMeta> animations;
}
