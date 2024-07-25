using System.Collections;
using System.Collections.Generic;
using m039.Common;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class BasicTest
{
    [Test]
    public void CollectionPasses()
    {
        var list = new List<int>
        {
            44,
            13
        };

        Assert.IsTrue(list.Contains(x => x == 13));
        Assert.IsFalse(list.Contains(x => x == 30));
    }

    [Test]
    public void LogPasses()
    {
        var log = Log.Get<BasicTest>();
        var str = string.Empty;
        log.SetPrinter((level, message) =>
        {
            if (level == Log.LogLevel.Info)
            {
                str = "INFO:" + message;
            } else
            {
                str = "OTHER:" + message;
            }
        });

        Assert.IsTrue(string.IsNullOrEmpty(str));
        log.Info("1");
        Assert.AreEqual("INFO:[BasicTest] 1", str);

        str = string.Empty;
        log.Warning("2");
        Assert.AreEqual("OTHER:[BasicTest] 2", str);
    }

    static int s_EventPassesCounter = 0;

    [Test]
    public void EventPasses()
    {
        EventBus.Global.Logger.SetEnabled(false);

        EventPassesTest test1 = new();
        test1.Test();

        {
            EventPassesTestGC test2 = new();
            test2.Test();
        }

        System.GC.Collect();

        EventBus.Global.Raise<IEvent4>(a => a.Count());
        Assert.AreEqual(1, s_EventPassesCounter);
    }

    [Test]
    public void ServiceLocatorPasses()
    {
        var serviceLocator = ServiceLocator.Global;
        serviceLocator.Logger.SetEnabled(true);

        Assert.IsFalse(serviceLocator.TryGet<IncService>(out _));
        var service = new IncService();
        Assert.AreEqual(10, service.number);
        serviceLocator.Register<IIncService>(service);
        serviceLocator.Get<IIncService>().Inc();
        Assert.AreEqual(11, service.number);
    }

    interface IIncService
    {
        int number { get; }
        void Inc();
    }

    class IncService : IIncService
    {
        public int number { get; set; } = 10;

        public void Inc() => number++;
    }

    [UnityTest]
    public IEnumerator WaitForSecondsPasses()
    {
        var completed = false;

        Coroutines.WaitForSeconds(0.5f, () => completed = true);

        Assert.IsTrue(!completed);

        yield return new WaitForSeconds(0.6f);

        Assert.IsTrue(completed);
    }

    interface IEvent1 : ISubscriber
    {
        void AddNumbers(int x);
    }

    interface IEvent2 : ISubscriber
    {
        void AddStrings(string x);
    }

    interface IEvent3 : ISubscriber
    {
        void Unsub();
    }

    interface IEvent4 : ISubscriber
    {
        void Count();
    }

    class EventPassesTestGC : IEvent4
    {
        public void Nothing()
        {

        }

        public void Test()
        {
            Assert.AreEqual(0, s_EventPassesCounter);

            EventBus.Global.Subscribe(this);

            EventBus.Global.Raise<IEvent4>(a => a.Count());

            Assert.AreEqual(1, s_EventPassesCounter);
        }

        public void Count()
        {
            s_EventPassesCounter++;
        }
    }

    class EventPassesTest : IEvent1, IEvent2, IEvent3
    {
        int _number;

        string _string = "";

        int _unsubscribed = 0;

        public void AddNumbers(int x)
        {
            _number += x;
        }

        public void AddStrings(string x)
        {
            _string += x;
        }

        public void Unsub()
        {
            _unsubscribed++;
            EventBus.Global.Unsubscribe(this);
        }

        public void Test()
        {
            Assert.AreEqual(_number, 0);
            Assert.AreEqual(_string, "");

            EventBus.Global.Raise<IEvent1>(a => a.AddNumbers(1));

            Assert.AreEqual(0, _number);

            EventBus.Global.Subscribe(this);

            EventBus.Global.Raise<IEvent1>(a => a.AddNumbers(1));
            EventBus.Global.Raise<IEvent1>(a => a.AddNumbers(1));

            Assert.AreEqual(2, _number);

            EventBus.Global.Raise<IEvent2>(a => a.AddStrings("a"));
            EventBus.Global.Raise<IEvent2>(a => a.AddStrings("b"));

            Assert.AreEqual("ab", _string);
            Assert.AreEqual(0, _unsubscribed);

            EventBus.Global.Raise<IEvent3>(a => a.Unsub());
            EventBus.Global.Raise<IEvent1>(a => a.AddNumbers(1));
            EventBus.Global.Raise<IEvent2>(a => a.AddStrings("a"));

            Assert.AreEqual(1, _unsubscribed);

            EventBus.Global.Raise<IEvent3>(a => a.Unsub());

            Assert.AreEqual(1, _unsubscribed);

            Assert.AreEqual(2, _number);
            Assert.AreEqual("ab", _string);
        }
    }
}
