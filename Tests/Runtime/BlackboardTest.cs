using m039.Common.Blackboard;
using NUnit.Framework;

public class BlackboardTest
{
    [Test]
    public void BlackboardTestSimplePasses()
    {
        var blackboard = new Blackboard();
        var key11 = new BlackboardKey("key11");
        int value;

        if (blackboard.TryGetValue(key11, out int _))
        {
            Assert.Fail();
        }

        blackboard.SetValue(key11, 11);

        if (blackboard.TryGetValue(key11, out value))
        {
            Assert.AreEqual(11, value);
        } else
        {
            Assert.Fail();
        }

        if (blackboard.TryGetValue(key11, out bool _))
        {
            Assert.Fail();
        }
    }
}
