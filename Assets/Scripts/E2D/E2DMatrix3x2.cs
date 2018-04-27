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

	public Matrix4x4 ToMatrix4x4()
	{
		var mat = new Matrix4x4();
		mat.SetColumn(0, new Vector4(this.m00 / 1024.0f, this.m10 / 1024.0f, 0, 0));
		mat.SetColumn(1, new Vector4(this.m01 / 1024.0f, this.m11 / 1024.0f, 0, 0));
		mat.SetColumn(2, new Vector4(0, 0, 1, 0));
		mat.SetColumn(3, new Vector4(this.m20 / 16f, -this.m21 / 16.0f, 0, 1));
		return mat;
	}

	public static Vector3 ExtractTranslationFromMatrix(ref Matrix4x4 matrix)
	{
		Vector3 translate;
		translate.x = matrix.m03 / 100f;
		translate.y = matrix.m13 / 100f;
		translate.z = matrix.m23 / 100f;
		return translate;
	}

	public static Quaternion ExtractRotationFromMatrix(ref Matrix4x4 matrix)
	{
		Vector3 forward;
		forward.x = matrix.m02;
		forward.y = matrix.m12;
		forward.z = matrix.m22;

		Vector3 upwards;
		upwards.x = matrix.m01;
		upwards.y = matrix.m11;
		upwards.z = matrix.m21;

		return Quaternion.LookRotation(forward, upwards);
	}

	public static Vector3 ExtractScaleFromMatrix(ref Matrix4x4 matrix)
	{
		Vector3 scale;
		scale.x = new Vector4(matrix.m00, matrix.m10, matrix.m20, matrix.m30).magnitude;
		scale.y = new Vector4(matrix.m01, matrix.m11, matrix.m21, matrix.m31).magnitude;
		scale.z = new Vector4(matrix.m02, matrix.m12, matrix.m22, matrix.m32).magnitude;
		return scale;
	}

	public static void SetTransformFromMatrix(Transform transform, ref Matrix4x4 matrix)
	{
		transform.localPosition = ExtractTranslationFromMatrix(ref matrix);
		transform.localRotation = ExtractRotationFromMatrix(ref matrix);
		transform.localScale = ExtractScaleFromMatrix(ref matrix);
	}

	public static void SetTransformFromMatrix(Transform transform, float[] mat23)
	{
		var matrix = ToMatrix4x4(mat23);
		transform.localPosition = ExtractTranslationFromMatrix(ref matrix);
		transform.localRotation = ExtractRotationFromMatrix(ref matrix);
		transform.localScale = ExtractScaleFromMatrix(ref matrix);
	}

	public static Matrix4x4 ToMatrix4x4(float[] mat23, float z = 0)
	{
		Matrix4x4 mat = new Matrix4x4();
		mat.SetColumn(0, new Vector4(mat23[0] / 1024.0f, mat23[2] / 1024.0f, 0, 0));
		mat.SetColumn(1, new Vector4(mat23[1] / 1024.0f, mat23[3] / 1024.0f, 0, 0));
		mat.SetColumn(2, new Vector4(0, 0, 1, 0));
		mat.SetColumn(3, new Vector4(mat23[4] / 16, -mat23[5] / 16.0f, z, 1));

		return mat;
	}
}