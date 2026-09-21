# 如何让AI会用SoyoFramework

将以下文本添加进 AGENTS.md / CLAUDE.md：

```markdown
## 框架

本项目使用 **SoyoFramework** 作为架构框架。

当需要查阅框架源码或文档时，请在本地已解析的 Unity 包缓存中查找（路径相对于项目根目录）：`Library/PackageCache/com.github.theplayer571.soyo-framework@*/`

在涉及架构分层、职责划分或约束规则时，必须阅读框架包内的：`Documentation~/层级职能速查.md`

该文档定义了各层级的职责与约束规则。相关实现应遵循这些规则。
```