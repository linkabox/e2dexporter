using UnityEngine;
using UnityEngine.UI;

public class E2DText : E2DUIComponent
{
	public E2DFontStyle style;
	public Text text;

	public E2DText(Text text, RectTransform root)
	{
		this.root = root;
		this.node = text.rectTransform;
		this.text = text;
		this.style = E2DPackage.active.AddFontStyle(text);

		this.id = this.style.id; //text引用ID与fontStyle一致
		this.name = text.name;   //当前节点名
	}

	public override string ExportFrame(int index)
	{
		//ejoy2d的字体对齐方式，在这里修正最终位置
		var fixPos = new Vector3(-node.sizeDelta.x / 2, node.sizeDelta.y / 2, 0);
		var mat = E2DMatrix3x2.FromUIComOffset(this, fixPos);
		return string.Format("{{index = {0}, mat = {1}}},\n", index, mat);
	}
}