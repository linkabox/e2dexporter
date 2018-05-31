# e2dexporter

## 介绍
ugui prefab => ejoy2d sprite config
- 支持UGUI布局导出
- 支持Animation导出
- 支持本地化配置
- @为前缀的节点忽略导出，可以在Unity预览效果，在游戏运行时动态mount
- 隐藏对应Prefab根节点，表示忽略导出该Prefab
- 带动效的组件最好制作为单独Prefab进行导出，导出前在根节点加入E2DAnimator组件指定哪些Clip需要导出

## 需要使用到的额外工具

- TexturePacker
- Nested Prefabs插件
