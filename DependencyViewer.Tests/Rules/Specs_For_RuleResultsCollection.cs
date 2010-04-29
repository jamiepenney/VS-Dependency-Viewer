using DependencyViewer.Common.Rules;
using NUnit.Framework;

namespace Specs_For_RulesResultCollection
{
    public class When_A_Result_Is_Added_To_The_Parent_Result
    {
        [Test]
        public void It_Is_Added_To_The_Parent_Results_Collection()
        {
            CollectionAssert.Contains(result.Results, childResult);
        }

        [SetUp]
        public void Setup()
        {
            childResult = new RulesResultBase();
            childResult.AddFailure("failure");

            result = new RulesResultCollectionBase();
            result.AddResult(childResult);
        }

        private RulesResultCollectionBase result;
        private RulesResultBase childResult;
    }

    public class When_No_Results_Are_Added_To_The_Parent_Result
    {
        [Test]
        public void The_Parent_Results_Collection_Is_Empty()
        {
            CollectionAssert.IsEmpty(result.Results);
        }

        [Test]
        public void The_Result_Is_Marked_As_A_Pass()
        {
            Assert.That(result.AllRulesPassed, Is.True);
        }

        [SetUp]
        public void Setup()
        {
            result = new RulesResultCollectionBase();
        }

        private RulesResultCollectionBase result;
    }

    public class When_No_Failures_Are_Added_To_The_Child_Result
    {
        [Test]
        public void The_Colections_FailureMessages_Are_Empty()
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
            result = new RulesResultCollectionBase();
            result.AddResults(new []{new RulesResultBase()});
        }

        private RulesResultCollectionBase result;
    }

    public class When_A_Failure_Is_Added_To_The_Child_Result
    {
        [Test]
        public void It_Is_Added_To_The_Parent_FailureMessages_Collection()
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
            var childresult = new RulesResultBase();
            childresult.AddFailure("failure");

            result = new RulesResultCollectionBase();
            result.AddResult(childresult);
        }

        private RulesResultCollectionBase result;
    }
}