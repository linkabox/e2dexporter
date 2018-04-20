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

public class CocoAnimMeta
{
    public int id;
    public string name;
    public List<int> refIds;

    //ejoy2d每帧可能包含多个子Sprite对象的变换
    public List<List<CocoAnimFrame>> frames;
}

public class CocoAnimFrame
{
    public int index;
    public float[] mat; //matrix3x2
}

//public struct CocoMatrix3x2
//{

//}

public class CocoPackage
{
    public string name;
    public int texCount;

    public List<CocoSpritaMeta> sprites;
    public List<CocoAnimMeta> animations;
}
