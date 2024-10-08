using m039.Common.Pathfindig;
using NUnit.Framework;
using UnityEngine;

public class PathfindingTest
{
    [Test]
    public void PriorityQueuePasses()
    {
        PriorityQueue<int> queue = new();

        for (int i = 0; i < 100; i++)
        {
            for (int j = 0; j < 100; j++)
            {
                queue.Enqueue(Random.Range(0, int.MaxValue));
            }

            while (queue.Count > 0)
            {
                var front = queue.Dequeue();

                foreach (var item in queue.GetEnumerable())
                {
                    Assert.IsTrue(item >= front);
                }
            }
        }
    }
}
