using bvba.cryingpants.SpeechRecognition.InputGrammar;
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
        private Dictionary<SRInputGrammar, SRInput> _activeInputsByInputGrammar = new Dictionary<SRInputGrammar, SRInput>();
        private Dictionary<SRInputGrammar, Grammar> _grammarByInputGrammar = new Dictionary<SRInputGrammar, Grammar>();
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
                if (i.IsActive)
                {

                    foreach (var tag in i.Tags)
                    {
                        ICollection<SRInput> inputs = _inputsByTag.ContainsKey(tag) ? _inputsByTag[tag] : new List<SRInput>();
                        inputs.Add(i);
                        _inputsByTag[tag] = inputs;
                    }

                    foreach (var instring in i.GetAllInputGrammars())
                    {
                        _activeInputsByInputGrammar.Add(instring, i);

                        Grammar g = new Grammar(instring.ToSpeechGrammar());
                        _grammarByInputGrammar[instring] = g;
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
                if (input.IsActive != enabled) {
                    input.IsActive = enabled;

                    foreach (var igrammar in input.GetAllInputGrammars())
                    {
                        if (enabled)
                        {
                            _activeInputsByInputGrammar[igrammar] = input;
                            _sre.LoadGrammar(_grammarByInputGrammar[igrammar]);
                        }
                        else
                        {
                            _activeInputsByInputGrammar.Remove(igrammar);
                            _sre.UnloadGrammar(_grammarByInputGrammar[igrammar]);
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
            foreach (var pair in _activeInputsByInputGrammar)
            {
                if (pair.Key.Matches(typed))
                {
                    pair.Value.ProcessInputString(this, typed);
                    return; //we can't continue iterating the inputs anyway, because certain actions might change the collection
                }
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
