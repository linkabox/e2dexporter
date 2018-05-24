using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class E2DSprite : E2DComponent
{
	public int texId;
	public int[] src;
	public int[] screen;

	public int w;
	public int h;

	public bool export;

	public E2DSprite(Sprite sprite, List<Texture2D> textures, bool export)
	{
		this.export = export;
		this.name = sprite.name;
		var atlasTex = sprite.texture;
		this.texId = textures.IndexOf(atlasTex);
		//ejoy2d里面y轴与unity相反
		//y1 = y2 = yMax = texHeight - sprite.rect.y
		//x3,y3 - x4,y4
		//  |       |
		//x2,y2 - x1,y1
		Rect rect = sprite.textureRect;
		w = (int)rect.width;
		h = (int)rect.height;
		int xMin = (int)rect.xMin;
		int xMax = (int)rect.xMax;
		int yMax = (int)(atlasTex.height - rect.y);
		int yMin = (int)(yMax - rect.height);
		src = new int[8];
		src[0] = xMax;
		src[1] = yMax;
		src[2] = xMin;
		src[3] = yMax;
		src[4] = xMin;
		src[5] = yMin;
		src[6] = xMax;
		src[7] = yMin;

		screen = new int[8];
		screen[0] = w * E2DPackage.SCREEN_SCALE / 2;
		screen[1] = h * E2DPackage.SCREEN_SCALE / 2;
		screen[2] = -w * E2DPackage.SCREEN_SCALE / 2;
		screen[3] = h * E2DPackage.SCREEN_SCALE / 2;
		screen[4] = -w * E2DPackage.SCREEN_SCALE / 2;
		screen[5] = -h * E2DPackage.SCREEN_SCALE / 2;
		screen[6] = w * E2DPackage.SCREEN_SCALE / 2;
		screen[7] = -h * E2DPackage.SCREEN_SCALE / 2;
		//if (sprite.uv.Length != 4)
		//{
		//	Debug.Log("!!! uv error:" + sprite.name);
		//}
	}

	public override string Export()
	{
		var sb = new StringBuilder();
		sb.AppendLine("picture {");
		sb.AppendFormat("\tid = {0},\n", this.id);
		if (export)
		{
			sb.AppendFormat("\texport = \"{0}\",\n", this.name);
		}
		sb.AppendFormat("\t{{ tex = {0},", this.texId);
		sb.AppendFormat(" src = {{{0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}}},", src[0], src[1], src[2], src[3], src[4], src[5], src[6], src[7]);
		sb.AppendFormat(" screen = {{{0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}}}", screen[0], screen[1], screen[2], screen[3], screen[4], screen[5], screen[6], screen[7]);
		sb.AppendLine(" }");
		sb.AppendLine("}");
		return sb.ToString();
	}
}