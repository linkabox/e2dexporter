using System.Text;
using UnityEngine;

public class E2DPanel : E2DUIComponent
{
	public bool scissor;
	public float width;
	public float height;

	public E2DPanel(RectTransform node, RectTransform root)
	{
		this.root = root;
		this.node = node;
		this.width = node.sizeDelta.x;
		this.height = node.sizeDelta.y;
		this.scissor = true;

		this.name = node.name;   //当前节点名
		E2DPackage.active.RegisterCom(this);
	}

	public override string ExportFrame(int index)
	{
		return string.Format("{{index = {0}, mat = {1}}},\n", index, E2DMatrix3x2.FromClip(this));
	}

	public override string Export()
	{
		var sb = new StringBuilder();
		sb.AppendLine("pannel {");
		sb.AppendFormat("\tid = {0},\n", this.id);
		sb.AppendFormat("\tscissor = {0},\n", this.scissor.ToString().ToLower());
		sb.AppendFormat("\twidth = {0}, height = {1},\n", this.width, this.height);
		sb.AppendLine("}");
		return sb.ToString();
	}
}