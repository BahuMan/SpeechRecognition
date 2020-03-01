using System.Collections.Generic;
using bvba.cryingpants.SpeechRecognition.Conditions;

namespace bvba.cryingpants.SpeechRecognition.Actions
{
    public class SRActionSequence : ISRAction
    {

        private List<ISRAction> _actions = new List<ISRAction>();

        public int Count { get { return _actions.Count; } }
        public ISRAction this[int i] { get { return _actions[i]; } }

        public void AddAction(ISRAction action)
        {
            _actions.Add(action);
        }

        public void PerformAction(SRStatus status, string inputstring)
        {
            foreach (var a in _actions) a.PerformAction(status, inputstring);
        }
    }
}
