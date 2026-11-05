using System;
using System.Text.RegularExpressions;
using NUnit.Framework;
using SoyoFramework.Utils;
using UnityEngine;
using UnityEngine.TestTools;

namespace SoyoFramework.Tests
{
    public sealed class AggregateRelationsTests
    {
        private class TestRoot : AggregateRoot
        {
            public Action OnRemove = () => { };
            protected override void OnUnregister() => OnRemove();
        }

        private sealed class DependencyRoot : TestRoot { }

        [Requires(typeof(DependencyRoot))]
        private sealed class RequiredRoot : TestRoot { }

        [Manages(typeof(DependencyRoot))]
        private sealed class ManagerRoot : TestRoot { }

        [Manages(typeof(DependencyRoot))]
        private sealed class OtherManagerRoot : TestRoot { }

        [Observes(typeof(DependencyRoot))]
        private interface IObserver : IAggregateRoot { }

        private sealed class ObserverRoot : TestRoot, IObserver { }

        [Observes(typeof(DependencyRoot)), Requires(typeof(DependencyRoot))]
        private sealed class ConflictingRoot : TestRoot { }

        [Test]
        public void RequiresNeedsTargetAndBlocksItsUnregister()
        {
            var registry = new AggregateRootRegistry();
            LogAssert.Expect(LogType.Error, new Regex("需要确保 DependencyRoot 已注册"));
            Assert.IsFalse(registry.TryRegister(new RequiredRoot(), null));

            var dependency = new DependencyRoot();
            Assert.IsTrue(registry.TryRegister(dependency, null));
            var required = new RequiredRoot();
            Assert.IsTrue(registry.TryRegister(required, null));
            Assert.AreSame(dependency, registry.GetRoot<DependencyRoot>(required));
            LogAssert.Expect(LogType.Error, new Regex("仍被已注册"));
            Assert.IsFalse(registry.RequestUnregister<DependencyRoot>(null));
            Assert.IsTrue(registry.RequestUnregister<RequiredRoot>(null));
            Assert.IsTrue(registry.RequestUnregister<DependencyRoot>(null));
        }

        [Test]
        public void ManagerTakesOverAndReturnsAuthorityToExternalCaller()
        {
            var registry = new AggregateRootRegistry();
            var dependency = new DependencyRoot();
            Assert.IsTrue(registry.TryRegister(dependency, null));
            var manager = new ManagerRoot();
            Assert.IsTrue(registry.TryRegister(manager, null));
            Assert.AreSame(dependency, registry.GetRoot<DependencyRoot>(manager));

            LogAssert.Expect(LogType.Error, new Regex("已有其他管理者"));
            Assert.IsFalse(registry.TryRegister(new OtherManagerRoot(), null));
            LogAssert.Expect(LogType.Error, new Regex("外部调用无权操作"));
            Assert.IsFalse(registry.RequestUnregister<DependencyRoot>(null));
            Assert.IsTrue(registry.RequestUnregister<ManagerRoot>(null));
            Assert.IsTrue(registry.RequestUnregister<DependencyRoot>(null));
        }

        [Test]
        public void ReadPermissionsAreExclusiveAndRequireRegisteredRequester()
        {
            var registry = new AggregateRootRegistry();
            var dependency = new DependencyRoot();
            Assert.IsTrue(registry.TryRegister(dependency, null));
            var observer = new ObserverRoot();
            Assert.IsTrue(registry.TryRegister(observer, null));
            Assert.AreSame(dependency, registry.GetHandle<DependencyRoot>(observer).Current);
            LogAssert.Expect(LogType.Error, new Regex("无权直接读取"));
            Assert.IsNull(registry.GetRoot<DependencyRoot>(observer));

            var manager = new ManagerRoot();
            Assert.IsTrue(registry.TryRegister(manager, null));
            LogAssert.Expect(LogType.Error, new Regex("没有对 DependencyRoot 的 Observes"));
            Assert.IsNull(registry.GetHandle<DependencyRoot>(manager));
            LogAssert.Expect(LogType.Error, new Regex("外部调用者不能直接读取"));
            Assert.IsNull(registry.GetRoot<DependencyRoot>(null));
            Assert.AreSame(dependency, registry.GetHandle<DependencyRoot>(null).Current);

            var unregistered = new ObserverRoot();
            LogAssert.Expect(LogType.Error, new Regex("尚未注册"));
            LogAssert.Expect(LogType.Error, new Regex("没有对 DependencyRoot 的 Observes"));
            Assert.IsNull(registry.GetHandle<DependencyRoot>(unregistered));
        }

        [Test]
        public void CallbackCanManageChildWhileStillRegistered()
        {
            var registry = new AggregateRootRegistry();
            var manager = new ManagerRoot();
            var dependency = new DependencyRoot();
            Assert.IsTrue(registry.TryRegister(manager, null));
            Assert.IsTrue(registry.TryRegister(dependency, manager));
            manager.OnRemove = () =>
            {
                Assert.AreSame(dependency, registry.GetRoot<DependencyRoot>(manager));
                Assert.IsTrue(registry.RequestUnregister<DependencyRoot>(manager));
                Assert.AreSame(dependency, registry.GetHandle<DependencyRoot>(null).Current);
            };

            Assert.IsTrue(registry.RequestUnregister<ManagerRoot>(null));
            Assert.IsNull(registry.GetHandle<DependencyRoot>(null).Current);
        }

        [Test]
        public void CallbackExceptionKeepsRootAndDiscardsQueuedWork()
        {
            var registry = new AggregateRootRegistry();
            var manager = new ManagerRoot();
            var dependency = new DependencyRoot();
            Assert.IsTrue(registry.TryRegister(manager, null));
            Assert.IsTrue(registry.TryRegister(dependency, manager));
            manager.OnRemove = () =>
            {
                Assert.IsTrue(registry.RequestUnregister<DependencyRoot>(manager));
                throw new InvalidOperationException("callback failed");
            };

            Assert.Throws<InvalidOperationException>(() => registry.RequestUnregister<ManagerRoot>(null));
            Assert.AreSame(manager, registry.GetHandle<ManagerRoot>(null).Current);
            Assert.AreSame(dependency, registry.GetRoot<DependencyRoot>(manager));
        }

        [Test]
        public void RejectsMultipleKeysAndConflictingRelations()
        {
            var registry = new AggregateRootRegistry();
            var root = new TestRoot();
            Assert.IsTrue(registry.TryRegister(root, null));
            LogAssert.Expect(LogType.Error, new Regex("已使用其他 key"));
            Assert.IsFalse(registry.TryRegister<IAggregateRoot>(root, null));
            LogAssert.Expect(LogType.Error, new Regex("同时声明"));
            Assert.IsFalse(registry.TryRegister(new ConflictingRoot(), null));
        }
    }
}
