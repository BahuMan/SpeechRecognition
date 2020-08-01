using bvba.cryingpants.SpeechRecognition.Expressions;
using System.Xml;

namespace bvba.cryingpants.SpeechRecognition.Actions
{
    public class SREnableInputByTagAction : ISRAction
    {
        private static readonly string[] FALSE = new string[] { "no", "false", "0", "disabled", "inactive"};
        private ISRExpression _expression;
        private bool _enable = true;

        public SREnableInputByTagAction(ISRExpression expr, bool enable)
        {
            _enable = enable;
            _expression = expr;
        }

        public static SREnableInputByTagAction ParseXML(XmlReader xr)
        {
            bool toEnable = true;
            if (xr.GetAttribute("enable") != null && System.Array.Exists(FALSE, element => element == xr.GetAttribute("enable").ToLower()))
                toEnable = false;

            return new SREnableInputByTagAction(CompoundExpression.ParseXML(xr, "enableinputbytag"), toEnable);
        }

        public void PerformAction(SRStatus status, string inputstring)
        {
            status.EnableInputByTag(_expression.Evaluate(status), _enable);
        }
    }
}
