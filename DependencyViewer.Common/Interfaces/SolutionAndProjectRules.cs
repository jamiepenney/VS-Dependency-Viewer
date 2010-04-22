using System;
using System.Collections.Generic;
using DependencyViewer.Common.Model;

namespace DependencyViewer.Common.Interfaces
{
    public interface ISolutionRuleProcessor
    {
        IRulesResult RunRules(Solution solution);
    }

    public interface IProjectRuleProcessor
    {
        IRulesResult RunRules(Project project);
    }

    public interface IRulesResult
    {
        bool AllRulesPassed { get; }
        IEnumerable<string> FailureMessages { get; }
    }

    public class RulesResultBase : IRulesResult
    {
        private readonly List<string> _failureMessages = new List<string>();

        public void AddFailure(string message)
        {
            _failureMessages.Add(message);
        }

        public virtual bool AllRulesPassed
        {
            get { return _failureMessages.Count == 0; }
        }

        public virtual IEnumerable<string> FailureMessages
        {
            get { return _failureMessages; }
        }
    }
}