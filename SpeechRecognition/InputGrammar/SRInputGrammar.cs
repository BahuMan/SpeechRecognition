using System;
using System.Text.RegularExpressions;
using System.Xml;
using System.Speech.Recognition;
using System.Collections.Generic;

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
                else
                    throw new XmlException("unknown element inside InputString: " + xr.ToString());
            }
            return result;
        }

        public bool Matches(string teststring)
        {
            foreach (var m in matchSequence)
            {
                if (!m.Matches(teststring)) return false;
            }
            return true;
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
