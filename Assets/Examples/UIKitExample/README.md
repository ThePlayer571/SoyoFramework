## UIKit 示例

UIKit的示例项目，包括以下需求：

- 主界面，可以选择两个小游戏
- 点点点小游戏
- 点泡泡小游戏
- 需要复用且有差异的设置界面

演示了以下功能：

- UIRoot自定义
    - 使用自定义UIRoot
    - 自动查找与自动生成Layer
- UIPage基础功能
    - Command
    - Context
    - Logic+Prefab 实现复用
    - 内置的ViewHelper的使用
- UIKit方法
    - 开/关/获取 UIPage
    - 其余方法
- 调试功能
    - UIPage调试工具

额外描述：
- 点点点小游戏演示了适合快速制作的项目组织方法，点泡泡小游戏演示了完美符合规范的项目组织方法
- UIPage的优势是跨场景使用，此处的示例其实更适合直接在场景里利用UI完成（仅将SettingsPanel做到UIKit里）