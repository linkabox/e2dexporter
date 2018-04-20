using UnityEngine;

public class E2DAnchor : E2DUIComponent
{
	public E2DAnchor(RectTransform node, RectTransform root)
	{
		this.id = -1;
		this.name = node.name;
		this.node = node;
		this.root = root;
	}

	public override string ExportCom()
	{
		//没有指定id只有name字段的时候会被当作是一个Anchor
		return string.Format("\t\t{{name = \"{0}\"}},\n", this.name);
	}

	public override string ExportFrame(int index)
	{
		return string.Format("{{index = {0}, mat = {1}}},\n", index, E2DMatrix3x2.FromUICom(this));
	}
}