

namespace bvba.cryingpants.SpeechRecognition.Conditions
{
    //this condition will only be met if the command was spoken (rather than typed)
    public class SpokenCondition : ISRCondition
    {
        public bool HasBeenMet(SRStatus status)
        {
            return false; //so far, we haven't implemented speech recognition
        }
    }
}
