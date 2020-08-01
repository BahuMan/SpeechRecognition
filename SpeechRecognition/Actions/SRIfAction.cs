using System.Collections.Generic;
using System.Xml;
using bvba.cryingpants.SpeechRecognition.Conditions;

namespace bvba.cryingpants.SpeechRecognition.Actions
{
    public class SRIfAction : ISRAction
    {
        private ISRAction _action;
        private ISRCondition _condition;
        private ISRAction _elseAction;

        public SRIfAction(ISRCondition condition, ISRAction action)
        {
            _action = action;
            _condition = condition;
            _elseAction = null;
        }

        public void PerformAction(SRStatus status, string inputstring)
        {
            if (_condition == null || _condition.HasBeenMet(status)) _action.PerformAction(status, inputstring);
            else if (_elseAction != null) _elseAction.PerformAction(status, inputstring);
        }

        public void SetElseAction(ISRAction elseAction)
        {
            _elseAction = elseAction;
        }

        public static ISRAction ParseXML(XmlReader xr)
        {
            ISRCondition condition = SRConditions.ParseXML(xr, null);
            if (condition is null)
                throw new XmlException("if action should start with a condition");

            if (!xr.Read() || xr.NodeType != XmlNodeType.Element || xr.Name.ToLower() != "then")
                throw new XmlException("condition should have 1 condition operator followed by actionsequence; found: " + xr.Name);

            SRActionSequence action = SRActionSequence.ParseXML(xr, "then");

            SRIfAction result = new SRIfAction(condition, action);

            xr.Read();
            if (xr.NodeType == XmlNodeType.Element && xr.Name.ToLower() == "else")
            {
                result.SetElseAction(SRActionSequence.ParseXML(xr, "else"));
                if (!xr.Read() || xr.NodeType != XmlNodeType.EndElement || xr.Name.ToLower() != "if")
                    throw new XmlException("expected </condition>; found: " + xr.Name);
            }
            else if (xr.NodeType != XmlNodeType.EndElement || xr.Name.ToLower() != "if")
                throw new XmlException("expected </condition>; found: " + xr.Name);

            return result;
        }

    }
}
