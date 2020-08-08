using System.Speech.Recognition;

namespace bvba.cryingpants.SpeechRecognition.InputGrammar
{
    interface ISRMatch
    {
        bool Matches(string tomatch);
        GrammarBuilder ToSpeechGrammar();
    }
}
