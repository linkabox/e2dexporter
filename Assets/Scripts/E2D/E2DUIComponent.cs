
using UnityEngine;

public abstract class E2DUIComponent : E2DComponent
{
	public RectTransform root;
	public RectTransform node;

	public virtual string ExportCom()
	{
		return string.Format("\t\t{{id = {0}, name = \"{1}\"}},\n", this.id, this.name);
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