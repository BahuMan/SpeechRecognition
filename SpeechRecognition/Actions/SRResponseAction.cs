using System;
using bvba.cryingpants.SpeechRecognition.Conditions;
using bvba.cryingpants.SpeechRecognition.Expressions;

namespace bvba.cryingpants.SpeechRecognition.Actions
{
    class SRResponseAction : ISRAction
    {
        private ISRCondition _condition = null;
        private ISRExpression _expression;

        public SRResponseAction(ISRCondition condition, ISRExpression expression)
        {
            _condition = condition;
            _expression = expression;
        }
        public SRResponseAction(ISRExpression expression)
        {
            _expression = expression;
        }

        //TODO: temporary. Normally, responses should use an expression (so you can substitute variables)
        public SRResponseAction(string constantText)
        {
            _expression = new TextExpression(constantText);
        }

        public void PerformAction(SRStatus status, string inputstring)
        {
            //@TODO: decide whether to use console or speech synthesis

            if (_condition == null || _condition.HasBeenMet(status)) Console.WriteLine("Response: " + _expression.Evaluate(status));
        }

        public void SetCondition(ISRCondition condition)
        {
            _condition = condition;
        }
    }
}
