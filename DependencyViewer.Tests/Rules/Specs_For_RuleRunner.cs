using System;
using DependencyViewer.Common.Interfaces;
using DependencyViewer.Common.Model;
using DependencyViewer.Common.Rules;
using NUnit.Framework;
using Rhino.Mocks;

namespace Specs_For_RuleRunner
{
    [TestFixture]
    public class When_The_RulesRunner_Runs
    {
        [Test]
        public void The_Rule_Runner_Should_Return_The_Results_From_The_Solution_Processor()
        {
            CollectionAssert.Contains(results.Results, solutionResult);
        }

        [Test]
        public void The_Rule_Runner_Should_Return_The_Results_From_The_Project_Processor()
        {
            CollectionAssert.Contains(results.Results, projectResult);
        }

        [Test]
        public void The_Rule_Runner_Should_Run_The_Solution_Processor()
        {
            solutionRuleProcessor.AssertWasCalled(r => r.RunRules(solution));
        }

        [Test]
        public void The_Rule_Runner_Should_Run_The_Project_Processor()
        {
            projectRuleProcessor.AssertWasCalled(r => r.RunRules(project));
        }

        [SetUp]
        public void Setup()
        {
            runner = new RuleRunner();
            solutionResult = new RulesResultBase();
            projectResult = new RulesResultBase();

            project = new Project("", new Guid());
            solution = new Solution("", new[] { project });

            solutionRuleProcessor = MockRepository.GenerateMock<ISolutionRuleProcessor>();
            solutionRuleProcessor.Expect(r => r.RunRules(solution)).Return(solutionResult);

            projectRuleProcessor = MockRepository.GenerateMock<IProjectRuleProcessor>();
            projectRuleProcessor.Expect(r => r.RunRules(project)).Return(projectResult);

            runner.SolutionProcessors = new[] { solutionRuleProcessor };
            runner.ProjectProcessors = new[] { projectRuleProcessor };

            results = runner.RunRules(solution);
        }

        protected RuleRunner runner;
        protected IRulesResultCollection results;
        protected IRulesResult solutionResult;
        protected IRulesResult projectResult;
        protected ISolutionRuleProcessor solutionRuleProcessor;
        protected IProjectRuleProcessor projectRuleProcessor;

        protected Solution solution;
        protected Project project;
    }
}