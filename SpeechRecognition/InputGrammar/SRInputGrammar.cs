using System;
using System.Text.RegularExpressions;
using System.Xml;
using System.Speech.Recognition;

namespace bvba.cryingpants.SpeechRecognition.InputGrammar
{
    /**
     * This class is a home-brewn parser for the typed/spoken input
     * because I'm too lazy to learn ANTLR.
     * I may regret this...
     */
    public class SRInputGrammar
    {

        private string literal = null;

        public static SRInputGrammar ParseXML(XmlReader xr, string endTag)
        {
            SRInputGrammar result = new SRInputGrammar();
            result.literal = ParseSRProfile.FoundElement(xr, "inputstring").ToLower();
            return result;
        }

        public bool Matches(string teststring)
        {
            return literal.Equals(teststring.ToLower());
        }

        public Grammar BuildSpeechGrammar()
        {
            return new Grammar(new GrammarBuilder(literal));
        }
    }
}
