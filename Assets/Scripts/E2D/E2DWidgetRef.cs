using UnityEngine;

public class E2DWidgetRef : E2DUIComponent
{
	public E2DWidget refWidget;

	public E2DWidgetRef(E2DWidget widget, RectTransform node, RectTransform root)
	{
		this.root = root;
		this.node = node;
		this.id = -1;
		this.refWidget = widget;
		this.name = node.name;
	}

	public override string ExportCom()
	{
		return string.Format("\t\t{{id = {0}, name = \"{1}\"}},\n", this.refWidget.id, this.name);
	}

	public override string ExportFrame(int index)
	{
		return string.Format("{{index = {0}, mat = {1}}},\n", index, E2DMatrix3x2.FromUICom(this));
	}
}