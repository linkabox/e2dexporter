using UnityEngine;
using UnityEngine.UI;

public class E2DButton : E2DUIComponent
{
	public Button btn;
	public E2DSprite e2DSprite;

	public E2DButton(E2DSprite e2DSprite, Button btn, RectTransform root)
	{
		this.root = root;
		this.node = btn.image.rectTransform;
		this.e2DSprite = e2DSprite;
		this.id = e2DSprite.id;
		this.btn = btn;
		this.name = btn.name;
	}

	public override string ExportFrame(int index)
	{
		string extraStr;
		if (btn.colors.normalColor == Color.white)
		{
			extraStr = "";
		}
		else
		{
			extraStr = string.Format(" color={0}", btn.colors.normalColor.ToBGRA());
		}

		var mat = E2DMatrix3x2.FromE2DImage(this, this.e2DSprite);
		return string.Format("{{index = {0}, touch = true, mat = {1},{2}}},\n", index, mat, extraStr);
	}
}