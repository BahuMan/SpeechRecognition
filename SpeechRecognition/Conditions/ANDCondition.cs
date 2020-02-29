using System.Collections.Generic;

namespace bvba.cryingpants.SpeechRecognition.Conditions
{
    public class ANDCondition : ISRCondition
    {

        private List<ISRCondition> _conditions;

        public ANDCondition()
        {
            _conditions = new List<ISRCondition>();
        }

        public ANDCondition(List<ISRCondition> initConditions)
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
                if (!c.HasBeenMet(status)) return false;
            }
            return true;
        }
    }
}
