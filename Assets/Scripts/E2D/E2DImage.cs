using UnityEngine;
using UnityEngine.UI;

public class E2DImage : E2DUIComponent
{
	public E2DSprite e2DSprite;
	public Image image;

	public E2DImage(E2DSprite e2DSprite, Image image, RectTransform root)
	{
		this.root = root;
		this.node = image.rectTransform;
		this.e2DSprite = e2DSprite;
		this.image = image;
		this.id = e2DSprite.id;
		this.name = image.name;
	}

	public override string ExportFrame(int index)
	{
		string extraStr;
		if (image.color == Color.white)
		{
			extraStr = "";
		}
		else
		{
			extraStr = string.Format(" color={0}", image.color.ToBGRA());
		}

		var mat = E2DMatrix3x2.FromE2DImage(this, this.e2DSprite);
		return string.Format("{{index = {0}, mat = {1},{2}}},\n", index, mat, extraStr);
	}
}