using System.Collections.Generic;
using System.Text;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using VisualDesignCafe.Editor.Prefabs;

public class E2DWidget : E2DUIComponent
{
	public List<E2DUIComponent> components = new List<E2DUIComponent>();
	public Dictionary<Image, Dictionary<Sprite, int>> spriteIndexMap = new Dictionary<Image, Dictionary<Sprite, int>>();
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
		if (animator != null)
		{
			foreach (var clip in animator.exportClips)
			{
				if (clip == null)
				{
					Debug.LogError("Clip is null:" + animator.name);
					continue;
				}

				var allObjBindings = AnimationUtility.GetObjectReferenceCurveBindings(clip);
				foreach (var binding in allObjBindings)
				{
					var image = AnimationUtility.GetAnimatedObject(this.root.gameObject, binding) as Image;
					if (image != null)
					{
						var keyFrames = AnimationUtility.GetObjectReferenceCurve(clip, binding);
						foreach (var frame in keyFrames)
						{
							var sprite = frame.value as Sprite;
							if (sprite != null)
							{
								E2DSprite e2DSprite;
								if (E2DPackage.active.spriteRefMap.TryGetValue(sprite, out e2DSprite))
								{
									Dictionary<Sprite, int> spriteIndexes;
									if (!this.spriteIndexMap.TryGetValue(image, out spriteIndexes))
									{
										spriteIndexes = new Dictionary<Sprite, int>();
										this.spriteIndexMap.Add(image, spriteIndexes);
									}

									if (!spriteIndexes.ContainsKey(sprite))
									{
										var e2dImage = new E2DImage(e2DSprite, image, this, root);
										e2dImage.ignoreFrame = true;
										this.components.Add(e2dImage);
										spriteIndexes.Add(sprite, this.components.Count - 1);
									}
								}
								else
								{
									Debug.LogError("引用到不在图集内的Sprite:" + E2DHelper.PrintNodePath(node, root), sprite);
								}
							}
						}
					}
				}
			}
		}
	}

	/// <summary>
	/// 解析Prefab结构提取相应组件
	/// </summary>
	/// <param name="node"></param>
	/// <param name="root"></param>
	private void ParseComponent(RectTransform node, RectTransform root)
	{
		//@前缀的忽略导出
		if (node.IsUnExportNode()) return;

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
					Debug.LogError("该按钮图片没有引用图集内资源：" + E2DHelper.PrintNodePath(node, root));
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
			if (image.sprite == null)
			{
				image.sprite = E2DPackage.active.defaultSprite;
			}
			E2DSprite e2DSprite;
			if (E2DPackage.active.spriteRefMap.TryGetValue(image.sprite, out e2DSprite))
			{
				Dictionary<Sprite, int> spriteIndexes;
				if (!this.spriteIndexMap.TryGetValue(image, out spriteIndexes))
				{
					spriteIndexes = new Dictionary<Sprite, int>();
					this.spriteIndexMap.Add(image, spriteIndexes);
				}

				if (!spriteIndexes.ContainsKey(image.sprite))
				{
					var e2dImage = new E2DImage(e2DSprite, image, this, root);
					this.components.Add(e2dImage);
					spriteIndexes.Add(image.sprite, this.components.Count - 1);
				}
			}
			else
			{
				Debug.LogError("引用到不在图集内的Sprite:" + E2DHelper.PrintNodePath(node, root), image.sprite);
			}
		}

		var rawImage = node.GetComponent<RawImage>();
		if (rawImage != null && rawImage.texture != null)
		{
			isAnchor = false;
			var e2DRawImage = new E2DRawImage(rawImage, this, root);
			this.components.Add(e2DRawImage);
			E2DPackage.active.AddRawTexture(rawImage.texture);
		}

		var text = node.GetComponent<Text>();
		if (text != null)
		{
			isAnchor = false;
			var e2dText = new E2DText(text, root);
			this.components.Add(e2dText);
		}

		var rectMask = node.GetComponent<RectMask2D>();
		if (rectMask != null)
		{
			isAnchor = false;
			var e2dPanel = new E2DPanel(node, root);
			this.components.Add(e2dPanel);
		}

		var gridLayoutGroup = node.GetComponent<GridLayoutGroup>();
		if (gridLayoutGroup != null)
		{
			LayoutRebuilder.ForceRebuildLayoutImmediate(node);
		}

		if (isAnchor && node != root)
		{
			int childCount = node.childCount;
			for (int i = 0; i < node.childCount; i++)
			{
				var child = node.GetChild(i);
				if (child.IsUnExportNode())
				{
					childCount -= 1;
				}
			}

			if (childCount == 0)
			{
				var e2dAnchor = new E2DAnchor(node, root);
				this.components.Add(e2dAnchor);
			}
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
			if (com.node.gameObject.activeInHierarchy && !com.ignoreFrame)
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
				if (clip == null)
				{
					Debug.LogError("Clip is null:" + animator.name);
					continue;
				}

				sb.AppendLine("\t{");
				sb.AppendFormat("\t\taction = \"{0}\",\n", clip.name);
				int frameCount = clip.GetClipFrameCount();
				Debug.LogFormat("Export AnimationClip:{0} {1}", clip.name, frameCount);
				for (int i = 0; i < frameCount; i++)
				{
					this.animator.SampleAnimation(clip, i * (1f / clip.frameRate));
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