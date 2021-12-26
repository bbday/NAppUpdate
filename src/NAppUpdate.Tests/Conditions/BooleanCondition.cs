using NAppUpdateR.Conditions;
using NAppUpdateR.Tasks;
using NUnit.Framework;

namespace NAppUpdate.Tests.Conditions
{
    internal class MockCondition : IUpdateCondition
    {
        private bool _isMet;

        internal MockCondition(bool isMet)
        {
            _isMet = isMet;
        }

        public bool IsMet(IUpdateTask task)
        {
            return _isMet;
        }
    }

    public class BooleanConditionTests
    {
        [Test]
        public void ShortCircuitOR()
        {
            BooleanCondition bc = new BooleanCondition();
            bc.AddCondition(new MockCondition(true), BooleanCondition.ConditionType.OR);
            bc.AddCondition(new MockCondition(true), BooleanCondition.ConditionType.OR);
            bc.AddCondition(new MockCondition(false), BooleanCondition.ConditionType.OR);

            bool isMet = bc.IsMet(null);

            Assert.IsTrue(isMet, "Expected the second or to short circuit the condition list");
        }

        [Test]
        public void MultipleAND()
        {
            BooleanCondition bc = new BooleanCondition();
            bc.AddCondition(new MockCondition(true), BooleanCondition.ConditionType.AND);
            bc.AddCondition(new MockCondition(true), BooleanCondition.ConditionType.AND);

            bool isMet = bc.IsMet(null);

            Assert.IsTrue(isMet);
        }

        [Test]
        public void MultipleANDFail()
        {
            BooleanCondition bc = new BooleanCondition();
            bc.AddCondition(new MockCondition(false), BooleanCondition.ConditionType.AND);
            bc.AddCondition(new MockCondition(true), BooleanCondition.ConditionType.AND);

            bool isMet = bc.IsMet(null);

            Assert.IsFalse(isMet);
        }

        [Test]
        public void MultipleANDFail2()
        {
            BooleanCondition bc = new BooleanCondition();
            bc.AddCondition(new MockCondition(true), BooleanCondition.ConditionType.AND);
            bc.AddCondition(new MockCondition(false), BooleanCondition.ConditionType.AND);

            bool isMet = bc.IsMet(null);

            Assert.IsFalse(isMet);
        }

        [Test]
        public void LastORPass()
        {
            BooleanCondition bc = new BooleanCondition();
            bc.AddCondition(new MockCondition(false), BooleanCondition.ConditionType.AND);
            bc.AddCondition(new MockCondition(false), BooleanCondition.ConditionType.AND);
            bc.AddCondition(new MockCondition(true), BooleanCondition.ConditionType.OR);

            bool isMet = bc.IsMet(null);

            Assert.IsTrue(isMet);
        }

        [Test]
        public void MiddleORFail()
        {
            BooleanCondition bc = new BooleanCondition();
            bc.AddCondition(new MockCondition(false), BooleanCondition.ConditionType.AND);
            bc.AddCondition(new MockCondition(true), BooleanCondition.ConditionType.OR);
            bc.AddCondition(new MockCondition(false), BooleanCondition.ConditionType.AND);

            bool isMet = bc.IsMet(null);

            Assert.IsFalse(isMet);
        }

        [Test]
        public void Not()
        {
            BooleanCondition bc = new BooleanCondition();
            bc.AddCondition(new MockCondition(false), BooleanCondition.ConditionType.AND | BooleanCondition.ConditionType.NOT);

            bool isMet = bc.IsMet(null);

            Assert.IsTrue(isMet);
        }
    }
}
