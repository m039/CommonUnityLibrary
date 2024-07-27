using System.Collections;
using m039.Common.DependencyInjection;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class DependencyInjectionTest
{
    [UnityTest]
    public IEnumerator DITestWithEnumeratorPasses()
    {
        var provider = new GameObject("Provider").AddComponent<DIProvider>();
        var target = new GameObject("Target").AddComponent<DITarget>();
        var injector = new GameObject("Injector").AddComponent<InjectorMonoBehaviour>();

        yield return new WaitUntil(() => target.isStarted);
    }
}

class NumberCounter
{
    public int number = 100;
}

class DIProvider : MonoBehaviour, IDependencyProvider
{

    [Provide]
    NumberCounter GetNumberCounter() => new NumberCounter
    {
        number = 11
    };

}

class DITarget : MonoBehaviour
{
    [Inject]
    NumberCounter _counter1;

    [Inject]
    NumberCounter _counter2 { get; set; }

    NumberCounter _counter3;

    [Inject]
    void Init(NumberCounter counter)
    {
        _counter3 = counter;
    }

    public bool isStarted;

    void Start()
    {
        Assert.NotNull(_counter1);
        Assert.AreEqual(11, _counter1.number);
        _counter1.number++;

        Assert.NotNull(_counter2);
        Assert.AreEqual(12, _counter2.number);
        _counter2.number++;

        Assert.NotNull(_counter3);
        Assert.AreEqual(13, _counter3.number);

        isStarted = true;
    }
}
