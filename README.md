# SoyoFramework

这是一个由ThePlayer571制作的，适用于Unity游戏开发的渐进式框架，模仿了QFramework。

## 核心框架介绍

如果你不是MVC架构的熟练使用者，看起来可能会有点吃力。

### 设计哲学

1. 尽量不使用单例
    - 首先：不使用框架，只使用单例，仍然能完成项目
    - 但是：所有单例混在一起，代码混乱
    - 所以：设计**层级**，每个层级只负责特定的任务
2. 使用**ECA**结构: Event-Condition-Action
    - 解释：
        - Event: 输入信息 = 何时执行
        - Condition: 条件 = 是否执行
        - Action: 输出信息 = 执行什么
    - 框架的ECA解决的是**层级之间的信息传递问题**，而不是**处理层级内部的逻辑**
    - 对层级之间的通信做了极大限制，增大代码可控性

3. 符合自然的制作流程
    - 制作早期优势
        - 允许层级违反规范，但提供标记手段，以助重构
    - 制作后期优势
        - 层级划分细颗粒度，框架清晰
        - 支持层级拓展，适配不同项目的特殊需求
        - 不符合规范的层级有明显标记，方便重构

### 层级介绍

- 采用常规的三层架构
    - Model(数据层) < System(数据逻辑层) < ViewController(表现层)
    - (创新点) **对层级之间的通信做了极大限制**
        - 提出**职能**的概念：每个层级的"接受信息的能力"+"发送信息的能力"，叫做它的职能
- (创新点) **允许使用者添加Tool层**
    - Tool层：依赖于某个层级，用于解决逻辑共享问题：
        - Tool不是以工作内容作为区分，而是以职能作为区分
        - Tool层的职能不能超越它所依赖的层级
            - 基于以上规则，Tool层不会破坏原有的通信规范
      > 看了内置的Tool例子就能理解为什么设计Tool层了
- (创新点) 鼓励敏捷开发

    - **允许层级违反规范**，但提供标记手段，以助重构

#### 层级划分

##### 数据层

**Model**：**存储全局数据，提供数据变更事件**。

- 设计意图：
    - 作为数据的容器，存放全局共享的数据
    - 提供数据的加载存储方法
- 职能：
    - SendEvent
- 内置Tool：
    - Utility：解决数据处理的逻辑共享问题，比如可以封装持久化存储的方案
        - 职能：无

> 在Model里提供Action，本质上属于SendEvent的一种语法糖。
> 但是可能有"不能RegisterEvent的层级"能获取Model，因此提供Action是种非常危险的行为。

##### 数据逻辑层

**System**：**处理数据层面的逻辑**。

- 设计意图：
    - 程序逻辑的处理中心
- 职能：
    - GetModel
    - SendEvent
    - RegisterEvent
- 内置Tool：
    - Service：解决逻辑共享问题，比如可以封装某个操作的*唯一入口*，如种植植物的逻辑
        - 职能：
            - GetModel
            - SendEvent

##### 表现层

**ViewController**：**将数据呈现给玩家，接受玩家输入**。

分为两种：

- MonoVController：继承自MonoBehavior，使用Unity生命周期，无需注册到框架
- ViewController：注册到框架使用


- 设计意图：
    - 将数据转化为表现
    - 接受玩家输入
- 职能：
    - GetModel (但不能修改Model数据)
    - RegisterEvent
    - SendCommand
- 内置Tool：无

> 这里的Command是一种通信方式，马上会提到。

#### 层级通信方式

**Event**：

- 通信方向：低->高 or 同级
- 基于TypeEventSystem实现

**Command**：

- 通信方向：ViewController -> Model ＆ System
- 职能：
    - GetModel
    - SendEvent
    - SendCommand

> Command 类似一个“没有状态的System”。看职能，它和System唯一区别是“Command不能RegisterEvent”。
> 因此Command层经常会做出破坏规范的行为，写的时候要注意。

> 引入Command层的好处多多，比如：
> - 确保Controller和System都不会乱
> - Command代表了玩家向底层发送的所有操作，因此可以方便地做出“操作记录”“操作回放”等功能。

#### 违反规范

Unity的很多功能，和SoyoFramework的设计理念是冲突的。
比如有一个实体Entity，它继承了MonoBehavior，有血量，会走路，还会把自己渲染出来。

- 有血量：应该用Model存储，通过实体Id访问
- 会走路：应该用System处理走路逻辑
- 渲染出来：应该用ViewController处理

但是实践告诉我们，如果拆分了，代码会又乱又臭，还不如只写在ViewController里。这时候是**违反规范优于符合规范**。
SoyoFramework鼓励这种行为：**实践经验 > 教条主义**。

但为了防止引起混乱，建议使用SuperLayerAttribute标记。例如:

```csharp
    [SuperLayer("Model + System + ViewController")]
    public class Entity : ViewController
    {
        public int Id;
        public int Health;

        public void Move()
        {
            Debug.Log("Move");
        }
    }
```

> 这也是为什么View层要叫做ViewController😂

##### 超巨层 SuperLayer

我们可以自定义新的层级，把多个层级的职能耦合在一起，称为SuperLayer。例如：

**SuperSystem**：**耦合：Model, System, ViewController**。

- 职能：
    - GetModel
    - SendEvent
    - RegisterEvent
    - SendCommand
- 使用情景举例
    - PVZ游戏，我想管理手持状态，需要HandModel存储数据，HandSystem处理手持逻辑，HandViewController处理手持表现。
    - 但是HandModel和HandSystem加起来一共才100行代码，没必要分。
    - 这时候我可以建一个HandSuperSystem，综合Model和System的职能。

```csharp
    [SuperLayer("System + Model")]
    public class HandSystem : AbstractSystem, IModel
    {
        // 写逻辑
    }
    
    public class OtherSystem : AbstractSystem
    {
        protected override void OnInit()
        {
            // 可以通过GetModel获取到HandSystem
            var handSystem = this.GetModel<HandSystem>();
        }
    }

```

### 杂项

本部分介绍一些零碎的内容。

#### Command分析工具

Command是以类为单位的，每次发送Command，都会创建一个Command对象。
因此频繁发送Command，可能会引起GC压力。
Command分析工具正是为解决这个问题而设计。

- 功能：
    - 以Command类为单位，统计总发送次数
    - 以Command类为单位，统计每1s内发送次数峰值
- 使用方式：
  - 打开窗口：SoyoFramework -> CommandProfiler
  - 点击Start Analysis开始分析（注意）

## 杂项 / 随笔

### EasyEvent V.S. TypeEventSystem

EasyEvent 特指存储在Model里的Event，TypeEventSystem 指全局的事件系统。

从“代码能跑”的角度来看，它们其实是等价的：

- 能RegisterEvent的层级，都能获取Model
- 能获取Model的层级，都能RegisterEvent

因此如果禁止Model存储EasyEvent，反而会让框架更规范。

本质上说，Model里的EasyEvent算是一个语法糖，用于区分事件的设计意图：

- EasyEvent 更倾向是：数据变化的客观反映
- TypeEventSystem 更倾向是：某个行为的发生

而BindableProperty则是EasyEvent的语法糖，本质是**属性+EasyEvent**的结合体。

此外，EasyEvent的性能更好一些。

## 该部分建设中

### UIKit

新增功能：

- 自动创建测试场景
- 自定义加载方式

TODO：Command追踪器：看Command发送次数，辅助使用者进行对象池重构

### 新手入门

### 框架外内容整合

框架的设计精妙且“恰到好处”，原因是满足“尽量不使用单例”原则，但其他库不会管这一点。因此将其他库整合进框架时，需要做一些调整。

例如使用Unity的InputSystem时，InputControl是全局可获取的，算是违反规范。
这个时候可以建一个GameInputSystem，利用TypeEventSystem订阅InputControl的事件，然后将事件转发出去，这样就能避免其他层级直接使用InputControl。
虽然GameInputSystem算是使用了规范外的信息来源，但其他层级都在规范内。

当然，如果你对框架足够熟练，也可以不做这个封装。
前提是要确保自己不会做出**在Model里订阅InputControl事件**这种诡异行为。

反例：Addressable应该属于Service层级，因为它是一个逻辑操作的入口。但是通常不会做封装。

### 信息传递(ECA)理解

金句：**没有接收到信息，那就不该有任何代码执行**。

- Event: 输入信息 = 何时执行
- Condition: 条件 = 是否执行
- Action: 输出信息 = 执行什么
- 好处：
- 层级之间的所有通信方式都在预测之内，代码逻辑清晰
- 还有很多其余的好处，但是说起来就像流水账一样，这里就不说了

TODO: EasyEvent提供扩展：IEasyEvent，封装Trigger，实现类似event的语法糖

TODO：Procedure允许方便地自定义排序，还有给每个阶段添加注释
还有确认底层用int，然后提供注册文件，将int注册进去，只有注册了的int才能用。需要提供多个接口，其余的接口给外部用

### 框架层级定义 新版

通信的定义：
提到副作用的理论。
层级高度：Model < System < ViewController。通信方式：
解决层级的创建和销毁问题：开两个Architecture

- 高->低 高级向低级通信是非常危险的行为，违反规范的行为通常在此处诞生。
    - System -> Model: 接口调用
    - ViewController -> System: Command (Command实际处于System层级内。为了防止副作用过大，Command不能存在状态)
- 低->高 or 同级
    - System -> System: Event
    - System -> ViewController: Event
    - Model -> System: Event
    - Model -> ViewController: Event

每个层级有自己的工具GetTool（工具的职能不能超过当前层级）：（用户可以自定义新类型的工具，来适配不同需求）
Model - Utility: 解决数据存取等逻辑共享问题
System - Service: 解决逻辑共享问题，不存在逻辑共享问题时不要写Service
ViewController - 暂未设计

Command的理解：
玩家操作只能由ViewController接收，因此Command代表了玩家的所有操作，这样好处有非常多。
缺点：Command发送数量太过，比如InputCommand每帧发送一次，可能会性能问题，因此要支持对象池。可能要做一个Command创建频率分析工具，来指引设计者重构

Tool的理解：
不同功能的代码其实可以用同一个类型的Tool来实现
允许自定义新的Tool，不应该存在职能完全相同的Tool类型

```

EasyEvent, BindableProperty: Event

```


### ProcedureKit

```text

我打算为我的ProcedureKit的“切换规则配置面板”写一个基于xNode的节点式编辑器

ProcedureKit是游戏流程管理工具，可以设置硬性切换规则让游戏流程切换更严谨。
切换规则的信息非常少，只包括“阶段A能切换到阶段B”这种信息，xNode的基础类我已经全部写好。
xNode的配置界面是独立于原本的编辑器的，你需要新写一个编辑器完成

我希望你做如下工作：
1. 完成ProcedureChangeRuleGraphEditor，包括以下步骤
在左上角添加一个面板，包括以下：
[field] ProcedureKitConfigSO （赋值/存储在graph中）
(如果ProcedureKitConfigSO有值，显示以下内容，否则不显示)
[Button] 创建/销毁节点 以同步SO文件 （按下后读取SO文件的Procedures，遍历Node，根据EnumValue，销毁和新建节点，节点位置是(0,0,0)即可）
[Button] 写入SO文件 （将节点图的设置写入SO文件）

你需要做的步骤：
1. 确认清楚需求和UI表现效果
2. 跟我说说实现思路，由于这个编辑器会比较大，所以我希望你能先把实现思路说清楚
3. 开始写代码

如果你有不确定的点或需要的文件，请及时告诉我，请确定清楚需求后再开始写代码。


```