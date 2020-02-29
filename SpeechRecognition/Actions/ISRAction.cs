using bvba.cryingpants.SpeechRecognition.Conditions;

namespace bvba.cryingpants.SpeechRecognition.Actions
{
    public interface ISRAction
    {
        void PerformAction(SRStatus status, string inputstring);

        //if a condition is set and performAction is called, the condition should be checked first
        void SetCondition(ISRCondition condition);
    }
}
