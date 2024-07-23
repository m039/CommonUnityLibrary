using System.Collections;
using System.Collections.Generic;
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
}
