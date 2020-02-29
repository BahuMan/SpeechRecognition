using System.Collections.Generic;
using bvba.cryingpants.SpeechRecognition.Conditions;

namespace bvba.cryingpants.SpeechRecognition.Actions
{
    public class SRCompoundAction : ISRAction
    {

        private List<ISRAction> _actions = new List<ISRAction>();
        private ISRCondition _condition = null;

        public int ActionCount { get { return _actions.Count; } }

        public void AddAction(ISRAction action)
        {
            _actions.Add(action);
        }

        public void PerformAction(SRStatus status, string inputstring)
        {
            if (_condition == null || _condition.HasBeenMet(status))
                foreach (var a in _actions) a.PerformAction(status, inputstring);
        }

        public void SetCondition(ISRCondition condition)
        {
            _condition = condition;
        }
    }
}
