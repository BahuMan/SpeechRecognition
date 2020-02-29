using System.Collections.Generic;

using bvba.cryingpants.SpeechRecognition.Inputs;

namespace bvba.cryingpants.SpeechRecognition
{
    public class SRProfile
    {
        private Dictionary<string, SRInput> _inputsByName;

        public SRProfile(string name)
        {
            Name = name;
            _inputsByName = new Dictionary<string, SRInput>();
        }

        public string Name { get; }

        public void AddInput(SRInput input)
        {
            _inputsByName.Add(input.Name, input);
        }

        public SRInput GetInput(string name)
        {
            return _inputsByName[name];
        }

        public ICollection<SRInput> GetAllInputs()
        {
            return _inputsByName.Values;
        }
    }
}
