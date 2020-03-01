using System.Collections.Generic;
using bvba.cryingpants.SpeechRecognition.Conditions;

namespace bvba.cryingpants.SpeechRecognition.Actions
{
    public class SRConditionAction : ISRAction
    {
        private ISRAction _action;
        private ISRCondition _condition;

        public SRConditionAction(ISRCondition condition, ISRAction action)
        {
            _action = action;
            _condition = condition;
        }

        public void PerformAction(SRStatus status, string inputstring)
        {
            if (_condition == null || _condition.HasBeenMet(status)) _action.PerformAction(status, inputstring);
        }
    }
}
