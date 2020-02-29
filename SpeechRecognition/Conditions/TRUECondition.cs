
namespace bvba.cryingpants.SpeechRecognition.Conditions
{
    //this is a trivial condition that will always return TRUE
    public class TRUECondition : ISRCondition
    {
        public bool HasBeenMet(SRStatus status)
        {
            return true;
        }
    }
}
