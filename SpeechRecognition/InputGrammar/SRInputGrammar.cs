using System;
using System.Text.RegularExpressions;
using System.Xml;
using System.Speech.Recognition;
using System.Collections.Generic;
using System.IO;

namespace bvba.cryingpants.SpeechRecognition.InputGrammar
{
    /**
     * This class is a home-brewn parser for the typed/spoken input
     * because I'm too lazy to learn ANTLR.
     * I may regret this...
     */
    public class SRInputGrammar
    {

        private List<ISRMatch> matchSequence = new List<ISRMatch>();

        public static SRInputGrammar ParseXML(XmlReader xr, string endTag)
        {
            SRInputGrammar result = new SRInputGrammar();
            while (xr.Read())
            {
                string elname = xr.Name.ToLower();
                if (xr.NodeType == XmlNodeType.EndElement && elname == endTag)
                    break;
                else if (xr.NodeType == XmlNodeType.Text)
                    result.matchSequence.Add(new SRTextMatcher(xr.Value.ToLower()));
                else if (xr.NodeType == XmlNodeType.Element && elname == "choice")
                    result.matchSequence.Add(SRChoiceMatcher.ParseXML(xr));
                else
                    throw new XmlException("unknown element inside InputString: " + xr.ToString());
            }
            return result;
        }

        public bool Matches(string teststring)
        {
            int pos = 0;

            foreach (var m in matchSequence)
            {
                //when a match occurs, the position will have moved forward, and we check the next requirement in the sequence
                if (!m.Matches(teststring, ref pos)) return false;
            }
            return true;
        }

        //utility method for all kinds of matchers to use:
        public static void SkipWhiteSpaceAndPunctuation(string tomatch, ref int pos)
        {
            while (pos < tomatch.Length && " \t\r\n.;:".IndexOf(tomatch[pos]) >= 0) pos++;
        }

        public GrammarBuilder ToSpeechGrammar()
        {
            GrammarBuilder gb = new GrammarBuilder();
            foreach (var m in matchSequence)
            {
                gb.Append(m.ToSpeechGrammar());
            }
            return gb;
        }
    }
}
