## SoyoFramework 简介

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

