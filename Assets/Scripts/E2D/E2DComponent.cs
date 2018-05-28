
public abstract class E2DComponent : IE2DExport
{
	public int id { get; set; }
	public string name { get; set; }

	public virtual string Export()
	{
		throw new System.NotImplementedException();
	}
}

public interface IE2DExport
{
	int id { get; set; }
	string name { get; set; }

	string Export();
}