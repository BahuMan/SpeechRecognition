using bvba.cryingpants.SpeechRecognition.Conditions;

namespace bvba.cryingpants.SpeechRecognition.Actions
{
    public interface ISRAction
    {
        void PerformAction(SRStatus status, string inputstring);
    }
}
