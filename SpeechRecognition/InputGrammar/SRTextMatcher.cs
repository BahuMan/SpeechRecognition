using System.Speech.Recognition;

namespace bvba.cryingpants.SpeechRecognition.InputGrammar
{
    class SRTextMatcher : ISRMatch
    {
        private string _literal;

        public SRTextMatcher(string literal)
        {
            _literal = literal.ToLower().Trim();
        }

        public bool Matches(string tomatch, ref int pos)
        {
            SRInputGrammar.SkipWhiteSpaceAndPunctuation(tomatch, ref pos);

            //if by now, we're at the end of the string, this can't be a match...
            //@TODO: ... unless one of the choices is empty? 
            if (pos >= tomatch.Length) return false;

            if (tomatch.IndexOf(_literal, pos) == pos)
            {
                pos += _literal.Length; //move the position up, so the next match starts after this one
                return true;
            }
            return false;
        }

        public GrammarBuilder ToSpeechGrammar()
        {
            return new GrammarBuilder(_literal);
        }
    }
}
