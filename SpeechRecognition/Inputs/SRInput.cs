using System.Collections.Generic;
using bvba.cryingpants.SpeechRecognition.Actions;

namespace bvba.cryingpants.SpeechRecognition.Inputs
{
    public delegate void InputActivationHandler(SRInput cmd, bool active);

    public class SRInput
    {
        public string Name { get; set; }
        public string GroupName { get; set; }
        private SRActionSequence _actions;
        public int ActionCount { get { return _actions.Count; } }
        private List<string> _inputStrings = new List<string>();
        private bool _active = true;
        public event InputActivationHandler InputActivated;

        public void AddInputString(string inputstring)
        {
            _inputStrings.Add(inputstring);
        }

        public void SetActionSequence(SRActionSequence actionSequence)
        {
            _actions = actionSequence;
        }

        public void ProcessInputString(SRStatus status, string inputstring)
        {
            _actions.PerformAction(status, inputstring);
        }

        public IReadOnlyCollection<string> GetAllInputStrings()
        {
            return _inputStrings;
        }

        public bool isActive {
            set
            {
                if (value != _active)
                {
                    isActive = value;
                    InputActivated.Invoke(this, _active);
                }
            }
            get
            {
                return _active;
            }
        }
    }

}