using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class E2DBitmapFont : MonoBehaviour, IE2DExport
{
	public int id { get; set; }

	// The dictionaries can be accessed throught a property
	[SerializeField]
	SymbolsSpriteMap m_symbols;
	public IDictionary<string, Sprite> Symbols
	{
		get { return m_symbols; }
		set { m_symbols.CopyFrom(value); }
	}

	void Reset()
	{
		// access by property
		Symbols = new Dictionary<string, Sprite>();
	}

	public string Export()
	{
		var sb = new StringBuilder();
		sb.AppendLine("animation {");
		sb.AppendFormat("\tid = {0},\n", this.id);
		sb.AppendFormat("\texport = \"{0}\",\n", this.name);

		//导出component列表
		var indices = new List<string>();
		sb.AppendLine("\tcomponent = {");
		foreach (var pair in Symbols)
		{
			var sprite = pair.Value;
			if (sprite != null)
			{
				E2DSprite e2DSprite;
				if (E2DPackage.active.spriteRefMap.TryGetValue(sprite, out e2DSprite))
				{
					sb.Append(string.Format("\t\t{{id = {0}}},\n", e2DSprite.id));
					indices.Add(pair.Key);
				}
				else
				{
					Debug.LogError("引用到不在图集内的Sprite:" + sprite.name, this.gameObject);
				}
			}
		}
		sb.AppendLine("\t},");

		foreach (var pair in Symbols)
		{
			string action = pair.Key;
			sb.AppendFormat("\t{{action = \"{0}\", {{{{ index = {1},mat = {{1024,0,0,1024,0,0}} }}}} }},\n", action, indices.IndexOf(action));
		}

		sb.AppendLine("}");
		return sb.ToString();
	}
}
