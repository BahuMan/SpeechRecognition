using System.Collections.Generic;
using System.Speech.Recognition;
using System.Xml;

namespace bvba.cryingpants.SpeechRecognition.InputGrammar
{
    class SRChoiceMatcher : ISRMatch
    {

        List<string> _choices;

        SRChoiceMatcher(string[] list)
        {
            _choices = new List<string>(list);
        }

        SRChoiceMatcher()
        {
            _choices = new List<string>();
        }

        public static SRChoiceMatcher ParseXML(XmlReader xr)
        {
            //@TODO: if choice tag contained attribute "listname", use existing choice instead of creating a new one

            if (!xr.Read()) throw new XmlException("unexpected EOF reading choice options");
            if (xr.NodeType != XmlNodeType.Text) throw new XmlException("Expected plain text inside choice tag");

            SRChoiceMatcher result = new SRChoiceMatcher(xr.Value.Split(";:,|".ToCharArray()));

            //skip the end tag: </choice>
            if (!xr.Read() || xr.NodeType != XmlNodeType.EndElement || !xr.Name.ToLower().Equals("choice"))
                throw new XmlException("Expected end tag for 'choice' but got " + xr.ToString());

            return result;
        }
        public bool Matches(string tomatch, ref int pos)
        {
            SRInputGrammar.SkipWhiteSpaceAndPunctuation(tomatch, ref pos);
            
            //if by now, we're at the end of the string, this can't be a match...
            //@TODO: ... unless one of the choices is empty? 
            if (pos >= tomatch.Length) return false;

            foreach (var c in _choices)
            {
                if (tomatch.IndexOf(c, pos) == pos)
                {
                    //update pos so next match will start from behind currently found choice
                    pos += c.Length;
                    return true;
                }
            }
            return false;
        }

        public GrammarBuilder ToSpeechGrammar()
        {
            return new Choices(_choices.ToArray());
        }
    }
}
