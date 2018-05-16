
using UnityEngine;

public abstract class E2DUIComponent : E2DComponent
{
	public RectTransform root;
	public RectTransform node;
	public bool ignoreFrame;

	public Vector3 e2dPos
	{
		get { return root.InverseTransformPoint(node.position); }
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

	public override string Export()
	{
		throw new System.NotImplementedException();
	}
}