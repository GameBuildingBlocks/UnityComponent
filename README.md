# UnityComponent

这里整合了Unity资源管理上的一些解决方案。包括资源统计，资源格式化，资源打包以及资源加载等等。

###ResourceOverview
从几个不同的维度对资源进行统计，并生成Markdown报告文档，目前仅输出贴图和模型资源。

###ResourceFormat
按资源根路径以及通配符匹配的方式配置资源格式，同时可以展示项目中各资源格式细节。

- [Unity项目中的资源管理](https://zhuanlan.zhihu.com/p/27779619)

###BundleBuildTool
一个使用Unity 4.x接口打包的AssetBundle打包工具。同样使用路径加通配符的方式匹配资源，资源之间的依赖关系由规则顺序决定，优先匹配后添加的规则。

- [一个灵活的AssetBundle打包工具](https://zhuanlan.zhihu.com/p/27876042)

###ResourceManager
一套支持Resources和AssetBundle并存的资源管理代码，以强引用的方式管理资源，可以方便的统计实时资源资源使用情况。

- [Unity项目中资源管理（续）](https://zhuanlan.zhihu.com/p/28324190)
