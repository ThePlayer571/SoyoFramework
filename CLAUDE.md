# CLAUDE.md

你正在开发一个适用于unity单机游戏开发的学术性框架。

作者希望他能完全掌握整个框架。编写代码时，你被推荐详细地询问作者"怎么做"，涉及不清楚的地方，确保你完全理解作者的意图。

## 项目结构

### [Framework](Assets/SoyoFramework/Framework/README.md) — 核心架构
框架的核心，提供了基于 Domain 的架构设计思路。所有架构指引和核心抽象都在这里。

### [ToolKits](Assets/SoyoFramework/ToolKits) — 工具包
提供常用的功能实现。通常建议导入，包含比较重要的内容。

### [UIKit](Assets/SoyoFramework/OptionalKits/UIKit/README.md) — UI 面板管理工具（可选）
按 UIPage + UIView 的形式管理跨场景、可复用的 UI 面板。

### [SoyoUGUIKit](Assets/SoyoFramework/OptionalKits/SoyoUGUIKit) — UGUI 拓展组件（可选）
提供一些 UGUI 的拓展组件。

### [ProcedureKit](Assets/SoyoFramework/OptionalKits/ProcedureKit) — 游戏流程管理工具（可选）
管理游戏流程（如启动流程、场景加载流程等）。
