using System;
using System.Collections.Generic;
using System.Linq;
using DependencyViewer.Common.Interfaces;

namespace DependencyViewer.Common.Rules
{
    public class RulesResultBase : IRulesResult
    {
        protected readonly List<string> _failureMessages = new List<string>();

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

    public class RulesResultCollectionBase : IRulesResultCollection
    {
        protected readonly List<IRulesResult> _results = new List<IRulesResult>();
        public IEnumerable<IRulesResult> Results { get { return _results; } }

        public void AddResults(IEnumerable<IRulesResult> results)
        {
            _results.AddRange(results);
        }

        public virtual bool AllRulesPassed
        {
            get { return _results.All(r => r.AllRulesPassed); }
        }

        public virtual IEnumerable<string> FailureMessages
        {
            get { return _results.SelectMany(r => r.FailureMessages); }
        }

        public void AddResult(IRulesResult result)
        {
            _results.Add(result);
        }
    }
}