using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using DependencyViewer.Common.Interfaces;
using DependencyViewer.Common.Model;

namespace DependencyViewer.Console
{
    public class RuleRunner
    {
        [ImportMany]
        public IEnumerable<ISolutionRuleProcessor> SolutionProcessors { get; set; }

        [ImportMany]
        public IEnumerable<IProjectRuleProcessor> ProjectProcessors { get; set; }

        public IEnumerable<IRulesResult> RunRules(Solution solution)
        {
            var solutionResults = SolutionProcessors.Select(processor => processor.RunRules(solution));
            var projectResults =
                ProjectProcessors.SelectMany(processor => solution.Projects.Select(project => processor.RunRules(project)));

            return solutionResults.Concat(projectResults);
        }
    }
}