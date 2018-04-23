using System.Collections.Generic;
using System.Text;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using VisualDesignCafe.Editor.Prefabs;

public class E2DWidgetRef : E2DUIComponent
{
	public E2DWidget refWidget;

	public E2DWidgetRef(E2DWidget widget, RectTransform node, RectTransform root)
	{
		this.root = root;
		this.node = node;
		this.id = -1;
		this.refWidget = widget;
		this.name = node.name;
	}

	public override string ExportCom()
	{
		return string.Format("\t\t{{id = {0}, name = \"{1}\"}},\n", this.refWidget.id, this.name);
	}

	public override string ExportFrame(int index)
	{
		return string.Format("{{index = {0}, mat = {1}}},\n", index, E2DMatrix3x2.FromUICom(this));
	}
}

public class E2DWidget : E2DUIComponent
{
	public List<E2DUIComponent> components = new List<E2DUIComponent>();
	public E2DAnimator animator;

	public E2DWidget(Transform root)
	{
		this.name = root.name;
		this.root = root as RectTransform;
		this.node = this.root;
		this.animator = root.GetComponent<E2DAnimator>();
	}

	public void Convert()
	{
		ParseComponent(root, root);
	}

	/// <summary>
	/// 解析Prefab结构提取相应组件
	/// </summary>
	/// <param name="node"></param>
	/// <param name="root"></param>
	private void ParseComponent(RectTransform node, RectTransform root)
	{
		var nestedPrefab = node.GetComponent<NestedPrefab>();
		if (nestedPrefab != null)
		{
			E2DWidget refWidget = E2DPackage.active.GetWidget(nestedPrefab.Asset.name);
			//这是一个引用外部Prefab的按钮控件
			this.components.Add(new E2DWidgetRef(refWidget, node, root));
			return;
		}

		bool isAnchor = true;
		var btn = node.GetComponent<Button>();
		if (btn != null)
		{
			isAnchor = false;
			//这是一个按钮组件
			if (btn.image != null)
			{
				//if (node != root)
				//{
				//	Debug.LogError("不支持此种方式布局，Button应该制作为独立控件：" + E2DHelper.PrintNodePath(node, root));
				//}

				E2DSprite e2DSprite;
				if (E2DPackage.active.spriteRefMap.TryGetValue(btn.image.sprite, out e2DSprite))
				{
					var e2dBtn = new E2DButton(e2DSprite, btn, root);
					this.components.Add(e2dBtn);
				}
				else
				{
					Debug.LogError("该按钮图片没有引用图集内资源");
				}
			}
			else
			{
				Debug.LogError("Button必须要有目标图片：" + E2DHelper.PrintNodePath(node, root));
			}
		}

		var image = node.GetComponent<Image>();
		if (image != null && btn == null)
		{
			isAnchor = false;
			E2DSprite e2DSprite;
			if (E2DPackage.active.spriteRefMap.TryGetValue(image.sprite, out e2DSprite))
			{
				var e2dImage = new E2DImage(e2DSprite, image, root);
				this.components.Add(e2dImage);
			}
			else
			{
				Debug.LogError("引用到不在图集内的Sprite:" + E2DHelper.PrintNodePath(node, root), image.sprite);
			}
		}

		var text = node.GetComponent<Text>();
		if (text != null)
		{
			isAnchor = false;
			var e2dText = new E2DText(text, root);
			this.components.Add(e2dText);
		}

		if (isAnchor && node != root && node.childCount == 0)
		{
			var e2dAnchor = new E2DAnchor(node, root);
			this.components.Add(e2dAnchor);
		}

		//遍历其子节点
		foreach (RectTransform child in node)
		{
			ParseComponent(child, root);
		}
	}

	public override string ExportFrame(int index)
	{
		var sb = new StringBuilder();
		sb.AppendLine("\t\t{");
		for (var i = 0; i < components.Count; i++)
		{
			var com = components[i];
			if (com.node.gameObject.activeInHierarchy)
			{
				sb.AppendFormat("\t\t\t{0}", com.ExportFrame(i));
			}
		}
		sb.AppendLine("\t\t},");
		return sb.ToString();
	}

	public override string Export()
	{
		var sb = new StringBuilder();
		sb.AppendLine("animation {");
		sb.AppendFormat("\tid = {0},\n", this.id);
		sb.AppendFormat("\texport = \"{0}\",\n", this.name);

		//导出component列表
		sb.AppendLine("\tcomponent = {");
		foreach (var com in components)
		{
			sb.Append(com.ExportCom());
		}
		sb.AppendLine("\t},");

		//导出默认帧，就是UI界面的默认布局信息
		sb.AppendLine("\t{");
		sb.Append(ExportFrame(0));
		sb.AppendLine("\t},");

		if (animator != null)
		{
			foreach (var clip in animator.exportClips)
			{
				sb.AppendLine("\t{");
				sb.AppendFormat("\t\taction = \"{0}\",\n", clip.name);
				int frameCount = clip.GetClipFrameCount();
				Debug.LogFormat("Export AnimationClip:{0} {1}", clip.name, frameCount);
				for (int i = 0; i < frameCount; i++)
				{
					this.animator.SampleAnimation(clip, (float)i / frameCount);
					sb.Append(ExportFrame(0));
				}
				//重置初始状态，采样下一个Clip
				this.animator.SampleAnimation(clip, 0);
				sb.AppendLine("\t},");
			}
		}

		sb.AppendLine("}");
		return sb.ToString();
	}
}