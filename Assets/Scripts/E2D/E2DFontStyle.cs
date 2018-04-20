using System;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class E2DFontStyle : E2DComponent
{
	public int size;
	public int align;
	public bool noedge;
	public Color color;
	public int width;
	public int height;
	public string langKey;

	public E2DFontStyle()
	{

	}

	public E2DFontStyle(Text text)
	{
		this.size = text.fontSize;
		this.noedge = true;
		this.color = text.color;
		this.width = (int)text.rectTransform.sizeDelta.x;
		this.height = (int)text.rectTransform.sizeDelta.y;
		switch (text.alignment)
		{
			case TextAnchor.UpperLeft:
				this.align = 0;
				break;
			case TextAnchor.UpperCenter:

				this.align = 2;
				break;
			case TextAnchor.UpperRight:
				this.align = 1;
				break;
			case TextAnchor.MiddleLeft:
			case TextAnchor.LowerLeft:
			case TextAnchor.MiddleCenter:
			case TextAnchor.LowerCenter:
			case TextAnchor.MiddleRight:
			case TextAnchor.LowerRight:
				this.align = 2;
				Debug.LogError("ejoy2d不支持此种字体对齐方式：" + text.name);
				break;
			default:
				this.align = 2;
				break;
		}

		var localize = text.GetComponent<E2DUILocalize>();
		if (localize != null)
		{
			this.langKey = localize.key;
		}
	}

	public override string Export()
	{
		var sb = new StringBuilder();
		sb.AppendLine("label {");
		sb.AppendFormat("\tid = {0},\n", this.id);
		sb.AppendFormat("\tsize = {0}, align = {1}, noedge = {2}, color = {3},\n", this.size, this.align, this.noedge.ToString().ToLower(), this.color.ToARGB());
		sb.AppendFormat("\twidth = {0}, height = {1},\n", this.width, this.height);
		int text_id = E2DLocalization.GetKeyIndex(this.langKey);
		if (text_id != -1)
		{
			sb.AppendFormat("\ttext_id = {0},\n", text_id + 1);
		}
		sb.AppendLine("}");
		return sb.ToString();
	}

	public bool Equals(E2DFontStyle other)
	{
		return size == other.size && align == other.align && noedge == other.noedge && color.Equals(other.color) && width == other.width && height == other.height;
	}

	public override int GetHashCode()
	{
		unchecked
		{
			var hashCode = size;
			hashCode = (hashCode * 397) ^ align;
			hashCode = (hashCode * 397) ^ noedge.GetHashCode();
			hashCode = (hashCode * 397) ^ color.GetHashCode();
			hashCode = (hashCode * 397) ^ width;
			hashCode = (hashCode * 397) ^ height;
			return hashCode;
		}
	}
}