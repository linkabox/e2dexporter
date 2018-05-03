
using System.Collections.Generic;

public class CocoMetaData
{
	public int id;
	public string name;
}

public class CocoAnimMeta : CocoMetaData
{
	public List<int> components;

	//ejoy2d每帧可能包含多个子Sprite对象的变换
	public List<List<CocoFrameTrans>> frames;
	public int max_render_count;
}

public class CocoFrameTrans
{
	public int index;
	public float[] mat; //matrix3x2
}

public class CocoSpritaMeta : CocoMetaData
{
	public int texId;

	public float[] src;
	public float[] screen;
}

public class CocoPackage
{
	public string name;
	public int texCount;

	public List<CocoSpritaMeta> sprites;
	public List<CocoAnimMeta> animations;
}
