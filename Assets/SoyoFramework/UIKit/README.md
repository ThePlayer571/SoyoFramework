# SoyoFramework UIKit（施工中）

## 快速简介

这是由ThePlayer571开发的UI面板管理方案，基于UGUI，依赖SoyoFramework。

亮点：

- 借鉴MVVM思路，实现UI与数据分离
- 与SoyoFramework无缝集成
- 调试友好
- 简单易上手 + 高度可自定义

## 教程 初见必看

> 该部分会录制视频教程

### 安装

将UIKit.unitypackage导入到你的Unity项目中。

依赖于这些插件运行：

- SoyoFramework
- UniTask
- Addressables

对这些插件做了支持：

- URP

### 工作流预览

1. 创建UISettings文件（ScriptableObject），注册UI页面（注册Key和UIPrefab）
2. 使用测试场景创建器，创建测试场景
3. 创建UIPage类，负责数据中转和逻辑处理（对应ViewModel）
4. 创建UIView类，负责显示界面和接受输入（对应View）
5. UIView通过UIViewHost与UIPage交互（使用Context获取数据，Command传递输入）
6. 在测试场景中制作UI界面
7. 利用UITestManager测试UI页面，可以实时调试Context
8. UI制作调试完毕，保存UISettings
9. 在游戏中，使用UIKit系列api，加载UIPrefab，打开UI页面

### 核心概念：UIPage

UIPage是UI面板打开的基本单位。同一个UIPage只能同时存在一个实例。

> 我们注册的UIPrefab是UIPage的Prefab，用UIKit打开的面板也是UIPage。

UIPage的职责与ViewModel类似：

1. 提供数据给UIView显示
2. 处理UIView传递的输入

> 总结：UIPage有两个作用：
> 1. 作为UI页面的实例
> 2. 作为ViewModel

### 核心概念：UIView

UIView负责显示UI界面和接受用户输入。

需要与UIPage绑定，一个UIView只能绑定到一个UIPage。


### 核心概念：IUIViewHost

UIPage实现了IUIViewHost接口，UIView通过获取绑定的UIPage的IUIViewHost接口与UIPage交互。

IUIViewHost接口定义如下：

```csharp
public interface IUIViewHost : IGameObject
{
    T GetContext<T>() where T : class, IUIContext;
    T GetModule<T>() where T : UIModule;
    void SubmitCommand(ICommand command);
    void SubmitCommand<TResult>(ICommand<TResult> command, out TResult result);
}
```

这其中涉及到一些概念：
- Context：数据上下文，UIView通过Context获取数据
- Command：命令，UIView通过Command传递输入
- Module：功能模块，为了解决逻辑共享问题而设计（之后会细说）

### 核心概念：UIKit 静态类

UIKit封装了一系列实用api，例如：
```csharp
// 仅示意
public static async UniTask<T> OpenPageAsync<T>(
    string pageName, 
    PageOpenSettings openSettings = null)
    where T : UIPage
{
    // ...
}
public static async UniTask OpenPageAsync(
    string pageName, 
    PageOpenSettings openSettings = null)
{
    // ...
}
public static T GetPage<T>(string pageName) where T : UIPage

    // ...
}
public static UIPage GetPage(string pageName)
{
    // ...
}
public static void ClosePage(string pageName)
{
    // ...
}


```

### 实现最基础工作流

学习完其上的知识，我们已经可以做最基础的实践了，请跟着我做一遍。



### 解决逻辑共享问题

### UI堆栈支持

### 解决UI复用问题

当有UI复用需求时（比如暂停界面复用），不建议单独创建一个UIPage，而是利用 UIPageLogic + UIViewPrefab组合实现。




## 进阶教程

### 自定义UIRoot

### 自定义测试场景