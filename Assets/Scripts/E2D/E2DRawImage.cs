using UnityEngine;
using UnityEngine.UI;

public class E2DRawImage : E2DUIComponent
{
	public RawImage rawImage;
	public E2DWidget container;

	public E2DRawImage(RawImage rawImage, E2DWidget container, RectTransform root)
	{
		this.root = root;
		this.node = rawImage.rectTransform;
		this.rawImage = rawImage;
		//引用外部资源时ID为特定值
		this.id = 0xFFFF - 1;
		this.name = string.Format("{0}/{1}.png@{2}", E2DPackage.RawImageDir, rawImage.texture.name, rawImage.name);
		this.container = container;
	}

	public override string ExportCom()
	{
		return string.Format("\t\t{{id = {0}, name = \"{1}\"}},\n", this.id, this.name);
	}

	public override string ExportFrame(int index)
	{
		string extraStr;
		if (rawImage.color == Color.white)
		{
			extraStr = "";
		}
		else
		{
			extraStr = string.Format(", color={0}", rawImage.color.ToBGRA());
		}

		var mat = E2DMatrix3x2.FromE2DRawImage(this, rawImage.texture);
		return string.Format("{{index = {0}, mat = {1}{2}}},\n", index, mat, extraStr);
	}
}