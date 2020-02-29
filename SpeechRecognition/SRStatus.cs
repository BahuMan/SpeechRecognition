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

            //@TODO: extract inputs from new profile, enable them and provide them to speech engine
            foreach (var i in pr.GetAllInputs())
            {
                if (i.isActive)
                {
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
    }
}
