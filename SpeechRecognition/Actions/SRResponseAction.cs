using System;
using System.Xml;
using bvba.cryingpants.SpeechRecognition.Conditions;
using bvba.cryingpants.SpeechRecognition.Expressions;

namespace bvba.cryingpants.SpeechRecognition.Actions
{
    class SRResponseAction : ISRAction
    {
        private ISRExpression _expression;

        public SRResponseAction(ISRExpression expression)
        {
            _expression = expression;
        }

        public void PerformAction(SRStatus status, string inputstring)
        {
            //@TODO: decide whether to use console or speech synthesis

            Console.WriteLine(_expression.Evaluate(status));
        }

        public static SRResponseAction ParseXML(XmlReader xr)
        {
            return new SRResponseAction(CompoundExpression.ParseXML(xr, "response"));
        }
    }
}
