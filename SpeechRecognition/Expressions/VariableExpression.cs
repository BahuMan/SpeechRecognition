using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bvba.cryingpants.SpeechRecognition.Expressions
{
    public class VariableExpression : ISRExpression
    {
        private string _variableName;
        public VariableExpression(string variableName)
        {
            _variableName = variableName;
        }

        public string Evaluate(SRStatus status)
        {
            if (status.IsVariableDefined(_variableName))
                return status.GetVariable(_variableName);
            else
                return "(undefined: " + _variableName + ")";
        }
    }
}
