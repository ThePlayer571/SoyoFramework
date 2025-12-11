## SoyoFramework 核心框架简介

### 框架设计哲学

1. 尽量不使用单例
    - 首先：不使用框架，只使用单例，仍然能完成项目
    - 但是：所有单例混在一起，代码混乱
    - 所以：设计**层级**，令层级只负责特定的任务，稀释单例的负影响
2. 使用**ECA**结构: Event-Condition-Action
    - 解释
        - ECA解决的是**层级之间的信息传递问题**，而不是**处理层级内部的逻辑**
        - Event: 输入信息 = 何时执行
        - Condition: 条件 = 是否执行
        - Action: 输出信息 = 执行什么
    - 原则："没有信息输入 => 没有代码执行 => 没有信息输出"
    - 优势：代码逻辑清晰，且能解耦
3. 符合自然的制作流程
    - 制作早期优势
        - 允许层级违反规范，但提供标记手段，以助重构
    - 制作后期优势
        - 层级划分细颗粒度，框架清晰
        - 不符合规范的层级有明显标记，方便重构

### 信息传递手段

信息传递是框架的关键。本框架内置了以下类型的信息传递方式：

从**信息使用者**角度来看，我可以**获取**这些信息：

1. Event: 通过EventSystem订阅事件
2. Model: 获取Model的实例，查询其存储的数据，订阅其提供的事件
3. Unity生命周期 / Unity组件: 通过MonoBehaviour的生命周期函数 / Unity组件提供的回调函数与事件等

从**信息提供者**角度来看，我可以**发送**这些信息：

1. Event: 通过EventSystem发送事件
2. Model: 获取Model的实例，修改其存储的数据
3. Service: 获取Service的实例，调用其提供的方法

### 框架层级介绍

总体上使用MVC架构，借鉴QFramework，做了一些细颗粒度的划分

##### 数据层：数据的存储与输出

**Model**：**存储全局数据，提供数据变更事件**。
目的是提供全局共享的数据。

- 输入
    - Model: 其他层级获取我的实例，修改我的数据
- 输出
    - Event: 我通过EventSystem发送事件，通知其他层级
    - Model: 我通过触发我的EasyEvent，通知其他层级

##### 逻辑层：数据的处理与输出

**Service**：**处理数据，作为某个操作的唯一入口**。
只能提供方法，禁止提供事件、数据等。
目的是提供共享的逻辑操作。

- 输入
    - Service: 其他层级获取我的实例，调用我的方法
    - Model TODO
- 输出
    - Event: 我通过EventSystem发送事件，通知其他层级
    - Model: 我通过获取Model的实例，修改其存储的数据

**System**：**分担附属的逻辑**。
目的是解决Service臃肿问题。

- 输入
    - Event: 我通过EventSystem订阅事件，接受其他层级的输入
    - Model TODO
- 输出
    - Event: 我通过EventSystem发送事件，通知其他层级
    - Service: 我通过获取Service的实例，调用其方法，通知其他层级
    - Model: 我通过获取Model的实例，修改其存储的数据

##### 表现层：将数据展示给用户

**ViewController**：**继承MonoBehaviour，负责处理表现、接受输入**。
推荐为所有逻辑的起源

- 输入
  - 
- 输出
  - 

##### 超巨层：耦合多个层级，为了写代码方便

**SuperSystem**：**耦合：Model, System, Service**。
当一个小功能不想拆成多个层级时，可以使用SuperSystem。
可以使用GetModel, GetSystem, GetService任意一个获取SuperSystem的实例。
这个层级相当于QFramework中的System。

**HasBetterArchAttribute**：**标记不符合规范的层级**。
如果有难以符合架构的单例或组件，可以使用这个标记。
允许传入string作为标注，建议描述清楚：1. 为什么不符合规范 2. 要想符合规范，应该属于什么层级

TODO：进行 ISuperLayer 标记，才让GetSystem能获取到System

TODO：MonoController和Controller的划分

TODO：目前System和Controller职责划分不清：
推荐Controller是处理外部的逻辑，发送请求给框架内，System只接受框架内部的逻辑。
然后Controller分为MonoController和Controller，MonoController处理Unity生命周期，Controller则是在框架内处理逻辑

TODO：Proxy机制的问题：其它层级获取了我的成员的引用，那我销毁时还会滞留在场上：这个问题没必要解决
UnRegister对应Deinit，这个对应关系不直观，需要改名

TODO: EasyEvent提供扩展：IEasyEvent，封装Trigger，实现类似event的语法糖

TODO：Procedure允许方便地自定义排序，还有给每个阶段添加注释，允许自定义有几个阶段\

TODO: 给 ProcudureKit添加程序集

### 框架外内容整合

框架的设计精妙且“恰到好处”，原因是满足“尽量不使用单例”原则，但其他库不会管这一点。因此将其他库整合进框架时，需要做一些调整。

例如使用Unity的InputSystem时，InputControl是全局可获取的，算是违反规范。
这个时候可以建一个GameInputSystem，利用TypeEventSystem订阅InputControl的事件，然后将事件转发出去，这样就能避免其他层级直接使用InputControl。
虽然GameInputSystem算是使用了规范外的信息来源，但其他层级都在规范内。

当然，如果你对框架足够熟练，也可以不做这个封装。
前提是要确保自己不会做出**在Model里订阅InputControl事件**这种诡异行为。

反例：Addressable应该属于Service层级，因为它是一个逻辑操作的入口。但是通常不会做封装。

### 层级注销

注销 = Deinit + 引用变为null
层级可以由System注册和注销

###

## 杂项 / 随笔

### EasyEvent V.S. TypeEventSystem

TODO：过时

EasyEvent 特指存储在Model里的Event； TypeEventSystem 指全局的事件系统

从“代码能跑”的角度来看，它们其实是等价的：

- **能RegisterEvent**的层级 都**能获取到Model的EasyEvent**
- **能获取到Model实例 但不推荐使用EasyEvent**的层级 都**不能RegisterEvent**

因此如果禁止Model存储EasyEvent，反而会让框架更规范（初见这个逻辑可能要捋一捋）

本质上说，Model里的EasyEvent算是一个语法糖，用于区分事件的设计意图

- EasyEvent 更倾向是：数据变化的客观反映
- TypeEventSystem 更倾向是：某个行为的发生

而BindableProperty则是EasyEvent的语法糖，本质是**属性+EasyEvent**的结合体

```


EasyEvent, BindableProperty: Event


```

### ProcedureKit

```
引入Procedure机制，这是本框架的核心

- Procedure机制
	所有层级都可以有依赖于Procedure的生命周期，只能在指定区间调用（解决问题: 关卡外调用关卡Model）
	todo 加一个IsChanging属性

- 框架部分(不依赖Unity生命周期)
[数据层]
Model IOC单例。负责数据增删改查，发送数据变更信息
[逻辑层]
System IOC单例。分担数据层面的逻辑
[超巨层]
Service 每个Service是一个类，功能单一，无状态。负责多个Model的数据操作，作为某个操作的唯一入口。全局可调用。Service禁止操作View

层级可以不符合规范(为了方便写代码)，但必须使用HasBetterArch标记不符合规范的层级
支持MonoSystem MonoModel MonoService，自动注册，用MonoBehavior的生命周期

- 框架部分(依赖Unity生命周期)
[表现层]
Controller 接受信息，操作View。可以将自己注册到Architecture里面: 通过protected RegisterSelf功能。
	Controller允许与View Model等层级耦合，请用[MetaLayer(Layer: View, Model)]特性标记

- 框架外
[表现层]
{废除: 使用partial class更佳，没必要独立出一个脚本}PassiveView 被别人调用，提供表现变更的方法，封装程度是"方便好用"。推荐只被Controller调用
AutoView 通过订阅Controller提供的事件，自主自动地更新View

- UIKit
	增加初始数据类
	移除SubPanel的设定
	添加类PanelAgent，这是一个组件，使用组合。用于管理Stack、Show状态等，方便使用

- SaveKit
	enum路径生成
	enum->filePathStr: 允许默认转换或自定义转换
		默认：填入另一个string，作为路径。可以选择在文件末尾添加当前时间
		自定义：允许自己写代码设置
	UI面板管理：默认使用AA包，需要配置好路径，自动生成字符串常量方便代码编写

- Proxy机制
所有层级通过Proxy获取，为了Procedure严谨
Proxy可以隐式转换为层级对象，方便使用
Proxy提供方法：UniTask WaitForValid: Proxy可以存空引用(Controller未注册时/受Procedure约束时)

EasyEvent/BindableProperty是客观的数据变化
EventSystem是具体事件行为的发生


- 随笔
原则上禁用所有单例
文档要写：框架+允许违反框架的点和需要做的标注
命名规则：
	TEMP_用于临时结构
	[HasBetterArch]
	Node 纯引用类
想要做的：
	给BetterToggle, MultiGroup这些组件给上自定义的图标
	统一的调试窗口，显示框架内当前所有Model, System, Controller，还能自定义发送Service，还能显示Procedure切换信息，当前Procedure
	ActionKit提供class UpdateHelper，允许提供OnEnter, OnExit, OnUpdate函数，提供开关，打开后先Enter，再一直Update
	
根框架不要使用DoTween, Naughty这些插件，有哪些插件需要说明清楚
```

```
请阅读以下文字，帮我完成editor。

Procedure是一种游戏框架中的流程状态系统，用于管理游戏中各个主要流程（如主菜单、战斗、结算等）的切换与生命周期。每个Procedure代表一个独立的游戏状态，拥有启动、更新和退出等逻辑。

我现在打算写一个全局的Procedure注册window，所有Procedure将在此处注册，并且会编辑额外信息、生成可用的类

工作流简介(使用者视角)：
1. 我打开目录 SoyoFramework/Procedure Managerment，打开编辑窗口
2. 我将SO文件拖入窗口顶侧的field，窗口显示出用户友好的配置界面。配置写入SO文件（当我点击保存按钮）
3. 这时候会显示出一个field，我通过这个field选择进入不同的region
3. [Region 1] 显示的是一张表格，第一列是每个ProcedureId，这是ProcedureId的注册入口，我使用string注册，点击左下角的[+]号新建行，也就是注册新的ProcedureId。后面的列是Tag，这个Tag即ProcedureMetaData.Tags里面的值，也是enum ProcedureTag里面的项。注册Tag是通过点击表格右上角的[+]号，第一行(除第一列外)是可以填入string的，这些string就是Tag的标识符。我点击表格中的项(空的方框)，这个框中间就会显示出一个✔图标，代表我选择了这个Tag。另外，有个常驻的ProcedureId叫Entrance，始终存在，不可去除
4. [Region 2] 然后我要配置ProcedureMetaData.AllowedPreviousProcedures数据。这是一个类似表格的结构，第一列是每个ProcedureId，后面的列都是“第一列的ProcedureId的ProcedureMetaData.AllowedPreviousProcedures数据”。当然实际上不会做成表格结构，因为不适合。操作方法是我点击一个[+]的图标，下拉框弹出来所有已注册的ProcedureId，我从中选择，进行注册。（这里的UI你可以自定发挥，好用即可）
5. 补充一下，顶部的UI排布为
"ProcedureKitConfig" [field] // 拖入SO文件的地方
// 未拖入文件不会显示下面的
[Button: "保存配置信息"] // 点击后将window内的数据转存到So文件里
[Button: "保存并生成C#类"] // 点击后保存配置信息，然后根据SO文件生成enum ProcedureId，enum ProcedureTag两个类。生成的路径是"Assets/SoyoFramework/Framework/Runtime/ProcedureKit/Classes"，格式要求：你模仿我给你的那个文件就行

生成的C#类要求：你需要利用Attribute给每个enum值标记Tag，这个Attribute你自己写，然后其他代码会通过反射获取Tag信息

此外，你还需要完成SO文件数据结构的设计和代码编写


你太厉害了，近乎完美地完成了我的需求。EditorWindow还有几个点需要优化
1. ProcedureId & Tags页面，如果出现了重名的ProcedureId或Tag，应该显示一个Error框提示
2. Allowed Previous Procedures页面，Entrance不应该出现在这里，因为它是入口，没有Previous Procedure


继续优化EditorWindow
1. Error框显示在底部而不是顶部，不然会导致表格整体往下移动。看起来难受
2. ProcedureId & Tags 是要生成为enum的，命名规则要和enum相符，比如不能数字开头

继续优化EditorWindow
1. ProcedureMetaData没必要生成，因为它是不变的
2. 生成的路径，最后的"Classes"改名为"GeneratedClasses"。然后不要生成SomeClassed.cs，而是ProcedureId.cs和ProcedureTag.cs

1. 请给So文件的引用增加记忆功能，记住对应文件的路径，方便使用
2. 每次生成完C#类后，表格一行都不会显示，重启 window后正常。这是个bug，你解决一下
```

