using System.Collections.Generic;
using System.Text;

namespace bvba.cryingpants.SpeechRecognition.Expressions
{
    public class CompoundExpression : ISRExpression
    {
        private List<ISRExpression> _expressions = new List<ISRExpression>();

        public int Count { get { return _expressions.Count; } }

        public ISRExpression this[int i] {
            get { return _expressions[i]; }
        }

        public void AddExpression(ISRExpression expr)
        {
            _expressions.Add(expr);
        }

        public string Evaluate(SRStatus status)
        {
            StringBuilder sb = new StringBuilder();
            foreach (var e in _expressions) sb.Append(e.Evaluate(status));
            return sb.ToString();
        }
    }

}
