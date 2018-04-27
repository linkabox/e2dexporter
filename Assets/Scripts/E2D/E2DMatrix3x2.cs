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
		var mat4x4 = Matrix4x4.TRS(com.e2dPos, com.node.rotation, com.node.lossyScale);
		var scale = Matrix4x4.Scale(new Vector3(com.node.sizeDelta.x / e2DSprite.w, com.node.sizeDelta.y / e2DSprite.h, 1));
		var mat = new E2DMatrix3x2(mat4x4 * scale);
		return mat;
	}

	public static E2DMatrix3x2 FromUICom(E2DUIComponent com)
	{
		var mat4x4 = Matrix4x4.TRS(com.e2dPos, com.node.rotation, com.node.lossyScale);
		return new E2DMatrix3x2(mat4x4);
	}

	public static E2DMatrix3x2 FromText(E2DUIComponent com)
	{
		var fixPos = new Vector3(-com.node.sizeDelta.x * com.node.lossyScale.x / 2, com.node.sizeDelta.y * com.node.lossyScale.y / 2, 0);
		fixPos = com.node.rotation * fixPos;
		var mat4x4 = Matrix4x4.TRS(com.e2dPos + fixPos, com.node.rotation, com.node.lossyScale);
		return new E2DMatrix3x2(mat4x4);
	}

	public static E2DMatrix3x2 FromTRS(Vector3 pos, Quaternion rot, Vector3 scale)
	{
		var mat4x4 = Matrix4x4.TRS(pos, rot, scale);
		return new E2DMatrix3x2(mat4x4);
	}

	public override string ToString()
	{
		return string.Format("{{{0}, {1}, {2}, {3}, {4}, {5}}}", m00, m01, m10, m11, m20, m21);
	}
}