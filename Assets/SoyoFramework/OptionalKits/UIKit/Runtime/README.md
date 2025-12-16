## UIKit使用说明

### 1. 工作流速览

#### 1.1 默认工作流

1. 右键创建UIMainPanel Prefab：Create/SoyoFramework/UIKit/UIMainPanel
2. 编辑UI界面
    1. UIMainPanel是可以通过api打开的最小单位
    2. UIPanel可以被视为SubPanel，推荐作为UIKit的Controller层级；同时也是可进行UI堆栈的最小单位
3. 右键创建UISettingsSO文件：Create/SoyoFramework/UIKit/UISettings，配置UISettings（如何配置？一目了然）
4. 打开SoyoFramework/UIKit Editor窗口，将UISettings文件拖拽赋值

#### 1.2 进阶操作

1. 自定义UIRoot：你需要修改源代码，阅读完所有代码后，如何自定义一目了然
2. 自定义UIMainPanel的模板：打开SoyoFramework/UIKit Editor窗口，将UIMainPanelTemplate一项拖拽赋值

### 2. 层级介绍

- UIPanel层：只关注UI的显示和交互
- UIManager层：负责UI的打开关闭和堆栈管理