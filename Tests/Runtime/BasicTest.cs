using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace m039.Common
{
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

        [Test]
        public void EventPasses()
        {
            var test = new EventPassesTest();
            test.Test();
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
                EventBus.Unsubscribe(this);
            }

            public void Test()
            {
                EventBus.Logger.SetEnabled(false);

                Assert.AreEqual(_number, 0);
                Assert.AreEqual(_string, "");

                EventBus.Raise<IEvent1>(a => a.AddNumbers(1));

                Assert.AreEqual(0, _number);

                EventBus.Subscribe(this);

                EventBus.Raise<IEvent1>(a => a.AddNumbers(1));
                EventBus.Raise<IEvent1>(a => a.AddNumbers(1));

                Assert.AreEqual(2, _number);

                EventBus.Raise<IEvent2>(a => a.AddStrings("a"));
                EventBus.Raise<IEvent2>(a => a.AddStrings("b"));

                Assert.AreEqual("ab", _string);
                Assert.AreEqual(0, _unsubscribed);

                EventBus.Raise<IEvent3>(a => a.Unsub());
                EventBus.Raise<IEvent1>(a => a.AddNumbers(1));
                EventBus.Raise<IEvent2>(a => a.AddStrings("a"));

                Assert.AreEqual(1, _unsubscribed);

                EventBus.Raise<IEvent3>(a => a.Unsub());

                Assert.AreEqual(1, _unsubscribed);

                Assert.AreEqual(2, _number);
                Assert.AreEqual("ab", _string);
            }
        }
    }
}
