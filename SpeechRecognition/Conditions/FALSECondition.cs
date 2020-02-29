
namespace bvba.cryingpants.SpeechRecognition.Conditions
{
    public class FALSECondition : ISRCondition
    {
        public bool HasBeenMet(SRStatus status)
        {
            return false;
        }
    }
}
