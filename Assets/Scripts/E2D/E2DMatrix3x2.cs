using UnityEngine;
using UnityEngine.UI;

public struct E2DMatrix3x2
{
	public int m00;
	public int m01;
	public int m10;
	public int m11;
	public int m20;
	public int m21;

	private static readonly E2DMatrix3x2 identityMatrix = new E2DMatrix3x2(1024, 0, 0, 1024, 0, 0);
	public static E2DMatrix3x2 identity
	{
		get
		{
			return E2DMatrix3x2.identityMatrix;
		}
	}

	public E2DMatrix3x2(int m00, int m01, int m10, int m11, int m20, int m21)
	{
		this.m00 = m00;
		this.m01 = m01;
		this.m10 = m10;
		this.m11 = m11;
		this.m20 = m20;
		this.m21 = m21;
	}

	public E2DMatrix3x2(Matrix4x4 mat4x4)
	{
		this.m00 = (int)(mat4x4.m00 * 1024);
		this.m01 = (int)(mat4x4.m01 * 1024);
		this.m10 = (int)(mat4x4.m10 * 1024);
		this.m11 = (int)(mat4x4.m11 * 1024);
		this.m20 = (int)(mat4x4.m03 * E2DPackage.SCREEN_SCALE);
		this.m21 = -(int)(mat4x4.m13 * E2DPackage.SCREEN_SCALE);
	}

	public static E2DMatrix3x2 FromE2DImage(E2DUIComponent com, E2DSprite e2DSprite)
	{
		//根据图片组件的sizeDelta和scale计算最终缩放值
		var finalScale = new Vector3(com.node.sizeDelta.x / e2DSprite.w * com.e2dScale.x,
									 com.node.sizeDelta.y / e2DSprite.h * com.e2dScale.y, 1);
		return FromTRS(com.e2dPos, com.e2dRot, finalScale);
	}

	public static E2DMatrix3x2 FromE2DRawImage(E2DUIComponent com, Texture rawTex)
	{
		//根据图片组件的sizeDelta和scale计算最终缩放值
		var finalScale = new Vector3(com.node.sizeDelta.x / rawTex.width * com.e2dScale.x,
			com.node.sizeDelta.y / rawTex.height * com.e2dScale.y, 1);
		return FromTRS(com.e2dPos, com.e2dRot, finalScale);
	}

	public static E2DMatrix3x2 FromUICom(E2DUIComponent com)
	{
		return FromTRS(com.e2dPos, com.e2dRot, com.e2dScale);
	}

	public static E2DMatrix3x2 FromText(E2DUIComponent com)
	{
		var fixPos = new Vector3(-com.node.sizeDelta.x * com.e2dScale.x / 2, com.node.sizeDelta.y * com.e2dScale.y / 2, 0);
		fixPos = com.e2dRot * fixPos;
		return FromTRS(com.e2dPos + fixPos, com.e2dRot, com.e2dScale);
	}

	public static E2DMatrix3x2 FromClip(E2DUIComponent com)
	{
		var fixPos = new Vector3(-com.node.sizeDelta.x * com.e2dScale.x / 2, com.node.sizeDelta.y * com.e2dScale.y / 2, 0);
		return FromTRS(com.e2dPos + fixPos, com.e2dRot, com.e2dScale);
	}

	public static E2DMatrix3x2 FromTRS(Vector3 pos, Quaternion rot, Vector3 scale)
	{
		var scaleMat = Matrix4x4.Scale(scale);
		var rotMat = Matrix4x4.Rotate(rot);
		var transMat = Matrix4x4.Translate(pos);
		var finalMat = transMat * scaleMat * rotMat;

		return new E2DMatrix3x2(finalMat);
	}

	public override string ToString()
	{
		return string.Format("{{{0}, {1}, {2}, {3}, {4}, {5}}}", m00, m01, m10, m11, m20, m21);
	}
}