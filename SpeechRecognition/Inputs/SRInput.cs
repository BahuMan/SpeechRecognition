using System.Collections.Generic;
using System.Speech.Recognition;
using System.Xml;
using bvba.cryingpants.SpeechRecognition.Actions;
using bvba.cryingpants.SpeechRecognition.InputGrammar;

namespace bvba.cryingpants.SpeechRecognition.Inputs
{
    public delegate void InputActivationHandler(SRInput cmd, bool active);

    public class SRInput
    {
        public string Name { get; set; }
        public string[] Tags { get; private set; }
        private SRActionSequence _actions;
        public int ActionCount { get { return _actions.Count; } }
        private List<SRInputGrammar> _inputStrings = new List<SRInputGrammar>();
        private bool _active = true;
        public event InputActivationHandler InputActivated;

        public static SRInput ParseXML(XmlReader xr)
        {

            SRInput input = new SRInput();
            input.Tags = new string[0];
            while (xr.Read())
            {
                string elname = xr.Name.ToLower();
                if (xr.NodeType == XmlNodeType.EndElement && elname == "input")
                    break;
                else if (xr.NodeType == XmlNodeType.Element && elname == "id")
                    input.Name = ParseSRProfile.FoundElement(xr, "id");
                else if (xr.NodeType == XmlNodeType.Element && elname == "tags")
                    input.SetTags(ParseSRProfile.FoundElement(xr, "tags"));
                else if (xr.NodeType == XmlNodeType.Element && elname == "actionsequence")
                    input.SetActionSequence(SRActionSequence.ParseXML(xr, "actionsequence"));
                else if (xr.NodeType == XmlNodeType.Element && elname == "inputstring")
                    input.AddInputString(SRInputGrammar.ParseXML(xr, "inputstring"));
            }

            if (input.Name == null) throw new XmlException("found an input without a name");

            //@TODO: some autoexec inputs might have no inputstrings, so this check may have to be refined
            if (input.GetAllInputGrammars().Count < 1) throw new XmlException("input '" + input.Name + "' has no inputstrings");

            if (input.ActionCount < 1) throw new XmlException("input '" + input.Name + "' has no actions");
            return input;
        }

        public void SetTags(string tags)
        {
            this.Tags = tags.Split(",;: \t".ToCharArray(), System.StringSplitOptions.RemoveEmptyEntries);
        }

        public void AddInputString(SRInputGrammar inputstring)
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

        public IReadOnlyCollection<SRInputGrammar> GetAllInputGrammars()
        {
            return _inputStrings;
        }

        public static Grammar ConvertInputStringToGrammar(string inputstring)
        {
            GrammarBuilder gb = new GrammarBuilder(inputstring);
            //@TODO: inputstring should be able to include synonyms, values for variables (like "use X on Y");
            //inputstring.Split("[|]".ToCharArray(), System.StringSplitOptions.RemoveEmptyEntries)
            return new Grammar(gb);
        }

        public bool isActive {
            set
            {
                if (value != _active)
                {
                    _active = value;
                    InputActivated?.Invoke(this, _active);
                }
            }
            get
            {
                return _active;
            }
        }
    }

}