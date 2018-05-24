using UnityEngine;
using UnityEngine.UI;

public class E2DImage : E2DUIComponent
{
	public Image image;
	public E2DWidget container;

	public E2DImage(E2DSprite e2DSprite, Image image, E2DWidget container, RectTransform root)
	{
		this.root = root;
		this.node = image.rectTransform;
		this.image = image;
		this.id = e2DSprite.id;
		this.name = image.name;
		this.container = container;
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
			extraStr = string.Format(", color={0}", image.color.ToBGRA());
		}

		E2DSprite e2DSprite;
		if (E2DPackage.active.spriteRefMap.TryGetValue(this.image.sprite, out e2DSprite))
		{
			var mat = E2DMatrix3x2.FromE2DImage(this, e2DSprite);
			return string.Format("{{index = {0}, mat = {1}{2}}},\n", container.spriteIndexMap[this.image][this.image.sprite], mat, extraStr);
		}
		else
		{
			Debug.LogError("引用到不在图集内的Sprite:" + E2DHelper.PrintNodePath(node, root), this.image.sprite);
		}
		return "";
	}
}