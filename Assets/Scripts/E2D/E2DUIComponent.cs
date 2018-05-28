
using UnityEngine;

public abstract class E2DUIComponent : E2DComponent
{
	public RectTransform root;
	public RectTransform node;
	public bool ignoreFrame;

	public static readonly Vector2 PivotCenter = new Vector2(0.5f, 0.5f);
	public Vector3 e2dPos
	{
		get
		{
			Vector3 result = Vector3.zero;
			if (node.pivot != PivotCenter)
			{
				var rawPivot = node.pivot;
				node.SetPivot(PivotCenter);
				result = root.InverseTransformPoint(node.position);
				node.SetPivot(rawPivot);
				//Debug.LogError("Pivot Error:" + node.name + " ," + rawPivot);
			}
			else
			{
				result = root.InverseTransformPoint(node.position);
			}

			return result;
		}
	}

	public Quaternion e2dRot
	{
		get
		{
			return Quaternion.Euler(0, 0, node.eulerAngles.z);
		}
	}

	public Vector3 e2dScale
	{
		get { return node.lossyScale; }
	}

	public virtual string ExportCom()
	{
		string name = E2DHelper.PrintNodePath(node, root, false);
		if (string.IsNullOrEmpty(name))
			return string.Format("\t\t{{id = {0}}},\n", this.id);
		return string.Format("\t\t{{id = {0}, name = \"{1}\"}},\n", this.id, name);
	}

	public abstract string ExportFrame(int index);
	//{
	//	return string.Format("{{index = {0}, mat = {1}}},\n", index, E2DMatrix3x2.identity);
	//}
}