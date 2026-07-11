Dictionary库
Procedure节点编辑器

将Optionalkits移到其他项目，这个项目就是单纯的框架。

神人TriggerDectorEditor的更新速度修复


文档：
如果你只引用一个Entity，没必要新增依赖，可以引用guid
依赖的规则：持有一个Root或者它内部人额和Enityt
DomainEntity只是Domain内部的概念，外部不需要在意它是什么
On语义：正在进行这个操作过程，需要其他操作参入其中，才能完成完整的操作。
管理Domain，就像管理一个普通的类一样
DomainRoot可以依赖其他DomainRoot，但是不能出现环形引用
同一个Domain内，尽量不要订阅内部发出的事件
- 事件是解耦用的，然而Domain内部的少量耦合是合理的
对Domain依赖的定义：A以任何方式访问了DomainB的成员，认为是依赖
Domain成员：组成一个Domain的 DomainRoot / DomainEntity （不过外部不关注DomainEntity的概念，这里的表达还需要改进）


# 1