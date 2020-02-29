
namespace bvba.cryingpants.SpeechRecognition.Expressions
{
    //this class represents constant text expressions, with no more variable names or string operations to evaluate
    public class TextExpression: ISRExpression
    {
        private string _text;

        public TextExpression(string txt)
        {
            _text = txt;
        }

        public string Evaluate(SRStatus status)
        {
            return _text;
        }
    }
}
