using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using DependencyViewer.Common.Interfaces;
using DependencyViewer.Common.Model;

namespace DependencyViewer.Common.Rules
{
    public class RuleRunner
    {
        public RuleRunner()
        {
            SolutionProcessors = new List<ISolutionRuleProcessor>();
            ProjectProcessors = new List<IProjectRuleProcessor>();
        }

        [ImportMany]
        public IEnumerable<ISolutionRuleProcessor> SolutionProcessors { get; set; }

        [ImportMany]
        public IEnumerable<IProjectRuleProcessor> ProjectProcessors { get; set; }

        public IRulesResultCollection RunRules(Solution solution)
        {
            var solutionResults = SolutionProcessors.Select(processor => processor.RunRules(solution));
            var projectResults =
                ProjectProcessors.SelectMany(processor => solution.Projects.Select(project => processor.RunRules(project)));

            var results = new RulesResultCollectionBase();
            results.AddResults(solutionResults);
            results.AddResults(projectResults);
            
            return results;
        }
    }
}