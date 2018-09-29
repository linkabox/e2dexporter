using UnityEngine;

public static class E2DHelper
{
	public static bool IsUnExportNode(this Transform node)
	{
		return node.name.StartsWith("@");
	}

	public static string ToBGRA(this Color color)
	{
		Color32 c32 = color;
		return ToBGRA(c32);
	}

	public static string ToBGRA(this Color32 c32)
	{
		//BGRA
		return string.Format("0x{0:x2}{1:x2}{2:x2}{3:x2}", c32.b, c32.g, c32.r, c32.a);
	}

	public static string ToARGB(this Color color)
	{
		Color32 c32 = color;
		return ToARGB(c32);
	}

	public static string ToARGB(this Color32 c32)
	{
		//BGRA
		return string.Format("0x{0:x2}{1:x2}{2:x2}{3:x2}", c32.a, c32.r, c32.g, c32.b);
	}

	public static string PrintNodePath(Transform node, Transform root, bool printRoot = true)
	{
#if UNITY_EDITOR
		if (printRoot)
			return root.name + "/" + UnityEditor.AnimationUtility.CalculateTransformPath(node, root);
		return UnityEditor.AnimationUtility.CalculateTransformPath(node, root);
#else
		return "";
#endif
	}

	public static Transform FindRoot(Transform node)
	{
		while (node.parent != null && node.parent != Localization.UIRoot.transform)
		{
			node = node.parent;
		}
		return node;
	}

	public static int GetClipFrameCount(this AnimationClip clip)
	{
		return (int)(clip.length * clip.frameRate);
	}

	public static void SetPivot(this RectTransform rect, Vector2 newPivot)
	{
		if (rect.pivot == newPivot) return;

		Vector3 rectReferenceCorner = GetRectReferenceCorner(rect, true);
		rect.pivot = newPivot;

		Vector3 rectReferenceCorner2 = GetRectReferenceCorner(rect, true);
		Vector3 v = rectReferenceCorner2 - rectReferenceCorner;
		rect.anchoredPosition -= (Vector2)v;
		//Vector3 position = rect.transform.position;
		//position.z -= v.z;
		//rect.transform.position = position;
	}

	public static Vector3 GetPivotPos(this RectTransform rect, float px, float py)
	{
		Vector3 rectReferenceCorner = GetRectReferenceCorner(rect, true);
		Vector2 rawPivot = rect.pivot;
		rect.pivot = new Vector2(px, py);

		Vector3 rectReferenceCorner2 = GetRectReferenceCorner(rect, true);
		Vector3 v = rectReferenceCorner2 - rectReferenceCorner;
		Vector3 result = rect.anchoredPosition - (Vector2)v;

		//Reset Raw Pivot
		rect.pivot = rawPivot;

		return result;
	}

	private static readonly Vector3[] s_Corners = new Vector3[4];
	public static Vector3 GetRectReferenceCorner(RectTransform gui, bool worldSpace)
	{
		Vector3 result;
		if (worldSpace)
		{
			Transform transform = gui.transform;
			gui.GetWorldCorners(s_Corners);
			if (transform.parent)
			{
				result = transform.parent.InverseTransformPoint(s_Corners[0]);
			}
			else
			{
				result = s_Corners[0];
			}
		}
		else
		{
			result = (Vector3)gui.rect.min + gui.transform.localPosition;
		}
		return result;
	}
}