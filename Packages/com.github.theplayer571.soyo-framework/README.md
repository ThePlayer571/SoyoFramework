# SoyoFramework

[![Unity 2022.3+](https://img.shields.io/badge/unity-2022.3%2B-blue.svg)](https://unity3d.com/get-unity/download)
[![License: MIT](https://img.shields.io/badge/License-MIT-brightgreen.svg)](https://github.com/ThePlayer571/SoyoRuntimeConsole/blob/master/LICENSE)

## 快速简介

SoyoFramework 是一个适用于单机游戏开发的实验性 Unity 框架。

- 提供了架构设计指引，而不是打包功能。
- 使用新颖的架构理念。

### 设计哲学：

---

- **代码结构描述业务需求**

---

- **跨层级的信息传递应当显式**

---

- **自然制作流程优先于规范**

---

## 安装

推荐使用 Unity Package Manager 安装：
https://github.com/ThePlayer571/SoyoFramework.git?path=Packages/com.github.theplayer571.soyo-framework

## 架构介绍

#### 架构运行速览

1. 用户与 `ViewController`交互，产生输入。
2. `ViewController`将输入封装为`Command`，发送`Command`。
3. `Command`处理逻辑，与`Aggregate`交互。
4. `Aggregate`进行本地的逻辑运算，修改自己的数据。
5. `ViewController`察觉到`Aggregate`的数据变化，更新表现。

**特别地有：**

1. `ViewController`已经处理了无需进入`Aggregate`的逻辑，`Aggregate`得以保持简洁。
2. `Command`对输入进行验证、二次封装，安全且复用性好。

### 层级介绍

一共有以下概念：

- 后端：Aggregate
- 前端：ViewController
- 通信方式：Command, Event

架构运行如下图所示：

> 注意：箭头方向是信息传输方向，而不是调用方向。

![系统架构图](Packages/com.github.theplayer571.soyo-framework/Documentation~/Architecture.drawio.svg)

---

想详细学习SoyoFramwork？请移步 [SoyoFramework 教程](Packages/com.github.theplayer571.soyo-framework/Documentation~/教程.md)。

## 目录

| 内容               | 地址                                                                                       |
|--------------------|--------------------------------------------------------------------------------------------|
| 教程（初学者必看） | [点此跳转](Packages/com.github.theplayer571.soyo-framework/Documentation~/教程.md)         |
| 层级职能速查       | [点此跳转](Packages/com.github.theplayer571.soyo-framework/Documentation~/层级职能速查.md) |
| 代码规范           | [点此跳转](Packages/com.github.theplayer571.soyo-framework/Documentation~/代码规范.md)     |
