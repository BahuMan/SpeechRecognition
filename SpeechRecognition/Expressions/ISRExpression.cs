namespace bvba.cryingpants.SpeechRecognition.Expressions
{
    public interface ISRExpression
    {
        string Evaluate(SRStatus status);
    }
}
