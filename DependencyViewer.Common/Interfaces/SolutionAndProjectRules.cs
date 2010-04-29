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

    public interface IRulesResultCollection : IRulesResult
    {
        IEnumerable<IRulesResult> Results { get; }

    }
}