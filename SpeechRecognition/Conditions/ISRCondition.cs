
namespace bvba.cryingpants.SpeechRecognition.Conditions
{
    public interface ISRCondition
    {
        bool HasBeenMet(SRStatus status);
    }
}
