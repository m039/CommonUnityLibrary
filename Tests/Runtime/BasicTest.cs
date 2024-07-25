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
}
