using DependencyViewer.Common.Rules;
using NUnit.Framework;

namespace Specs_For_RuleResultBase
{
    public class When_No_Failures_Are_Added_To_The_Result
    {
        [Test]
        public void The_FailureMessages_Collection_Is_Empty()
        {
            CollectionAssert.IsEmpty(result.FailureMessages);
        }

        [Test]
        public void The_Result_Is_Marked_As_A_Pass()
        {
            Assert.That(result.AllRulesPassed, Is.True);
        }

        [SetUp]
        public void Setup()
        {
            result = new RulesResultBase();
        }

        private RulesResultBase result;
    }

    public class When_A_Failure_Is_Added_To_The_Result
    {
        [Test]
        public void It_Is_Added_To_The_FailureMessages_Collection()
        {
            CollectionAssert.Contains(result.FailureMessages, "failure");
        }

        [Test]
        public void The_Result_Is_Marked_As_A_Failure()
        {
            Assert.That(result.AllRulesPassed, Is.False);
        }

        [SetUp]
        public void Setup()
        {
            result = new RulesResultBase();
            result.AddFailure("failure");
        }

        private RulesResultBase result;
    }
}