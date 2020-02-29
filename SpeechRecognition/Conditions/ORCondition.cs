using System.Collections.Generic;

namespace bvba.cryingpants.SpeechRecognition.Conditions
{
    public class ORCondition : ISRCondition
    {

        private List<ISRCondition> _conditions;

        public ORCondition()
        {
            _conditions = new List<ISRCondition>();
        }

        public ORCondition(List<ISRCondition> initConditions)
        {
            _conditions = initConditions;
        }

        public void Add(ISRCondition condition)
        {
            _conditions.Add(condition);
        }

        public bool HasBeenMet(SRStatus status)
        {
            foreach (var c in _conditions)
            {
                if (c.HasBeenMet(status)) return true;
            }
            return false;
        }
    }
}
