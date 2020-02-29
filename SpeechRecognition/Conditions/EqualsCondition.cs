using bvba.cryingpants.SpeechRecognition.Expressions;

namespace bvba.cryingpants.SpeechRecognition.Conditions
{
    public class EqualsCondition: ISRCondition
    {
        private ISRExpression _expr1, _expr2;

        public EqualsCondition(ISRExpression expr1, ISRExpression expr2)
        {
            _expr1 = expr1;
            _expr2 = expr2;
        }

        public bool HasBeenMet(SRStatus status)
        {
            return (_expr1.Evaluate(status) == _expr2.Evaluate(status));
        }
    }
}
