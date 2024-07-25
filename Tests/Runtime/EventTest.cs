using System;
using System.Collections;
using m039.Common;
using NUnit.Framework;
using UnityEngine.TestTools;

public class EventTest
{
    static int s_EventPassesCounter = 0;

    [Test]
    public void EventByInterfacePasses()
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

    interface IEvent1 : IEventSubscriber
    {
        void AddNumbers(int x);
    }

    interface IEvent2 : IEventSubscriber
    {
        void AddStrings(string x);
    }

    interface IEvent3 : IEventSubscriber
    {
        void Unsub();
    }

    interface IEvent4 : IEventSubscriber
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

    static int s_Number1;

    [UnityTest]
    public IEnumerator EventByArgumentPasses()
    {
        s_Number1 = 1;

        var bus = new EventBusByArgument();
        var counter = new EventNumberSubscriber();
        bus.Subscribe<int>(counter.Count);
        Assert.AreEqual(1, s_Number1);
        bus.Raise(10);
        Assert.AreEqual(11, s_Number1);

        var number2 = 2;
        var number3 = 3;
        Assert.AreEqual(2, number2);
        void subscriber1(int n)
        {
            number3 += n;
        }

        bus.Subscribe<int>(subscriber1);

        void subscriber2(int n)
        {
            number2 += n;
            bus.Unsubscribe((Action<int>)subscriber2);
        }

        bus.Subscribe((Action<int>)subscriber2);

        bus.Raise(2);
        Assert.AreEqual(5, number3);
        Assert.AreEqual(4, number2);
        bus.Raise(2);
        Assert.AreEqual(4, number2);
        Assert.AreEqual(15, s_Number1);

        System.GC.Collect();

        yield return null;

        System.GC.Collect();

        yield return null;

        bus.Raise(2);
        Assert.AreEqual(15, s_Number1);

        var number4 = 4;

        Action<int> subscriber3 = (n) => number4 += n;
        bus.Subscribe(subscriber3);
        bus.Raise(2);
        Assert.AreEqual(6, number4);
        bus.Unsubscribe(subscriber3);
        bus.Raise(2);
        Assert.AreEqual(6, number4);
    }

    public class EventNumberSubscriber
    {
        public void Count(int n)
        {
            s_Number1 += n;
        }
    }
}
