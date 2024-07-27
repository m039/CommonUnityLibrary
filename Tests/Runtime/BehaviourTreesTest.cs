using System.Collections;
using System.Collections.Generic;
using m039.Common.BehaviourTrees;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using static WaitUtils;

public class BehaviourTreesTest
{

    [UnityTest]
    public IEnumerator BehaviourTreesTestWithEnumeratorPasses()
    {
        var tester = new GameObject().AddComponent<BTTester>();
        var bt = new BehaviourTree();
        tester.BehaviourTree = bt;

        var sequenceCounter = 0;
        var counter12 = new CounterNode("CounterTo12", 12);
        var counter13 = new CounterNode("CounterTo13", 13);
        var counter7 = new CounterNode("CounterTo7", 7);
        var counter9 = new CounterNode("CounterTo9", 9);
        var counter3 = 0;

        var sequence1 = new SequenceNode("Sequence1");

        sequence1.AddChild(new ActionNode("Assert1", new(() =>
        {
            Assert.AreEqual(counter7.counter, 0);
            Assert.AreEqual(counter9.counter, 0);
        })));
        sequence1.AddChild(counter7);
        sequence1.AddChild(counter9);
        sequence1.AddChild(new ConditionNode("Fail3", new(() =>
        {
            if (counter3 < 3)
            {
                counter3++;
            }

            return counter3 >= 3;
        })));
        sequence1.AddChild(new ActionNode("Assert2", new(() =>
        {
            Assert.AreEqual(counter7.counter, 7);
            Assert.AreEqual(counter9.counter, 9);
            sequenceCounter++;
            Assert.AreEqual(counter7.timesReachedMax, sequenceCounter + 2);
            Assert.AreEqual(counter9.timesReachedMax, sequenceCounter + 2);
        })));

        var selector1 = new SelectorNode("Selector1");

        selector1.AddChild(counter12);
        selector1.AddChild(counter13);

        sequence1.AddChild(selector1);

        sequence1.AddChild(new ActionNode("Assert3", new(() =>
        {
            Assert.AreEqual(counter12.timesReachedMax, sequenceCounter);
            Assert.AreEqual(counter13.timesReachedMax, 0);
        })));

        bt.AddChild(sequence1);

        WaitUntilCooldown = 2;

        yield return WaitUntil(() => sequenceCounter == 3);
    }
}

class CounterNode : INode, IHasName
{
    public int counter { get; private set; } = 0;

    public readonly int maxCount;

    public int timesReachedMax = 0;

    public string name { get; }

    public CounterNode(string name, int maxCount)
    {
        this.maxCount = maxCount;
        this.name = name;
    }

    public Status Process()
    {
        if (counter >= this.maxCount)
        {
            timesReachedMax++;
            return Status.Success;
        }

        counter++;

        return Status.Running;
    }

    public void Reset()
    {
        counter = 0;
    }
}

class BTTester : MonoBehaviour
{
    public BehaviourTree BehaviourTree { get; set; }

    void Update()
    {
        BehaviourTree?.Update();
    }
}
