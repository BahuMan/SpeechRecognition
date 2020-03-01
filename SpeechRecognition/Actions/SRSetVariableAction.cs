using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using bvba.cryingpants.SpeechRecognition.Conditions;
using bvba.cryingpants.SpeechRecognition.Expressions;

namespace bvba.cryingpants.SpeechRecognition.Actions
{
    public class SRSetVariableAction : ISRAction
    {
        private string _variableName;
        private ISRExpression _expression;

        public SRSetVariableAction(string variableName, ISRExpression expression)
        {
            _variableName = variableName;
            _expression = expression;
        }

        public void PerformAction(SRStatus status, string inputstring)
        {
            status.SetVariable(_variableName, _expression.Evaluate(status));
        }
    }
}
