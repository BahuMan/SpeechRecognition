using System.Speech.Recognition;

namespace bvba.cryingpants.SpeechRecognition.InputGrammar
{
    class SRTextMatcher : ISRMatch
    {
        private string _literal;

        public SRTextMatcher(string literal)
        {
            _literal = literal.ToLower();
        }

        public bool Matches(string tomatch)
        {
            return _literal.Equals(tomatch);
        }

        public GrammarBuilder ToSpeechGrammar()
        {
            return new GrammarBuilder(_literal);
        }
    }
}
