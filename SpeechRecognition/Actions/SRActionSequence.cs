using System.Collections.Generic;
using System.Xml;
using bvba.cryingpants.SpeechRecognition.Conditions;
using bvba.cryingpants.SpeechRecognition.Expressions;

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

        public static SRActionSequence ParseXML(XmlReader xr, string endTag)
        {
            SRActionSequence actionSequence = new SRActionSequence();

            while (xr.Read())
            {
                string elname = xr.Name.ToLower();
                if (xr.NodeType == XmlNodeType.EndElement && elname == endTag)
                    break;
                else if (xr.NodeType == XmlNodeType.Element && elname == "if")
                    actionSequence.AddAction(SRIfAction.ParseXML(xr));
                else if (xr.NodeType == XmlNodeType.Element && elname == "response")
                    actionSequence.AddAction(SRResponseAction.ParseXML(xr));
                else if (xr.NodeType == XmlNodeType.Element && elname == "setvar")
                    actionSequence.AddAction(new SRSetVariableAction(xr.GetAttribute("name"), CompoundExpression.ParseXML(xr, "setvar")));
                else
                    throw new XmlException("expected action but found: " + xr.Name);
            }

            return actionSequence;
        }
    }

}
