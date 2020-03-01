using System;
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

            Console.WriteLine("Response: " + _expression.Evaluate(status));
        }
    }
}
