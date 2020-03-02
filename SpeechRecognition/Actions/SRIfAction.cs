using System.Collections.Generic;
using bvba.cryingpants.SpeechRecognition.Conditions;

namespace bvba.cryingpants.SpeechRecognition.Actions
{
    public class SRIfAction : ISRAction
    {
        private ISRAction _action;
        private ISRCondition _condition;
        private ISRAction _elseAction;

        public SRIfAction(ISRCondition condition, ISRAction action)
        {
            _action = action;
            _condition = condition;
            _elseAction = null;
        }

        public void PerformAction(SRStatus status, string inputstring)
        {
            if (_condition == null || _condition.HasBeenMet(status)) _action.PerformAction(status, inputstring);
            else if (_elseAction != null) _elseAction.PerformAction(status, inputstring);
        }

        public void SetElseAction(ISRAction elseAction)
        {
            _elseAction = elseAction;
        }
    }
}
