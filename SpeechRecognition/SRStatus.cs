using bvba.cryingpants.SpeechRecognition.Inputs;
using System;
using System.Collections.Generic;
using System.Speech.Recognition;

namespace bvba.cryingpants.SpeechRecognition
{
    public class SRStatus
    {
        private List<SRProfile> _profiles = new List<SRProfile>();
        private Dictionary<string, string> _variableValuesByName = new Dictionary<string, string>();
        private Dictionary<string, SRInput> _activeInputsByInputString = new Dictionary<string, SRInput>();
        private Dictionary<string, Grammar> _grammarByInputString = new Dictionary<string, Grammar>();
        private Dictionary<string, ICollection<SRInput>> _inputsByTag = new Dictionary<string, ICollection<SRInput>>();
        SpeechRecognitionEngine _sre;

        public SRStatus()
        {
        }

        public void SetSpeechRecognitionEngine(SpeechRecognitionEngine sre)
        {
            _sre = sre;
        }

        public void AddProfile(SRProfile pr)
        {
            _profiles.Add(pr);

            foreach (var i in pr.GetAllInputs())
            {
                if (i.isActive)
                {

                    foreach (var tag in i.Tags)
                    {
                        ICollection<SRInput> inputs = _inputsByTag.ContainsKey(tag) ? _inputsByTag[tag] : new List<SRInput>();
                        inputs.Add(i);
                        _inputsByTag[tag] = inputs;
                    }

                    foreach (var instring in i.GetAllInputStrings())
                    {
                        if (_activeInputsByInputString.ContainsKey(instring)) Console.WriteLine("warning: input string clash for " + instring);
                        _activeInputsByInputString.Add(instring, i);

                        Grammar g = new Grammar(new GrammarBuilder(instring));
                        _grammarByInputString[instring] = g;
                        _sre.LoadGrammar(g);
                    }
                }
            }

        }

        public void EnableInputByTag(string tagname, bool enabled = true)
        {
            if (!_inputsByTag.ContainsKey(tagname))
            {
                Console.WriteLine(" could not find tag '" + tagname + "' to " + (enabled?"enable":"disable"));
                return;
            }
            foreach (var input in _inputsByTag[tagname])
            {
                if (input.isActive != enabled) {
                    input.isActive = enabled;

                    foreach (var inputstring in input.GetAllInputStrings())
                    {
                        if (enabled)
                        {
                            _activeInputsByInputString[inputstring] = input;
                            _sre.LoadGrammar(_grammarByInputString[inputstring]);
                        }
                        else
                        {
                            _activeInputsByInputString.Remove(inputstring);
                            _sre.UnloadGrammar(_grammarByInputString[inputstring]);
                        }
                    }
                }
            }
        }

        public void RemoveProfile(SRProfile pr)
        {
            //@TODO: extract inputs, deactivate them, unregister them from speech engine
            _profiles.Remove(pr);
        }

        public void ProcessInput(string typed)
        {
            if (_activeInputsByInputString.ContainsKey(typed))
            {
                _activeInputsByInputString[typed].ProcessInputString(this, typed);
            }
        }

        public bool IsVariableDefined(string variableName)
        {
            return _variableValuesByName.ContainsKey(variableName);
        }

        public string GetVariable(string variableName)
        {
            return _variableValuesByName[variableName];
        }

        public void SetVariable(string variable, string value)
        {
            _variableValuesByName[variable] = value;
        }
    }
}
