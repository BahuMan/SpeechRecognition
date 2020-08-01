using System.Xml;

namespace bvba.cryingpants.SpeechRecognition.Conditions
{
    public class SRConditions
    {
        //@TODO: will become a recursive method to read composite conditions
        public static ISRCondition ParseXML(XmlReader xr, string endTag)
        {
            ISRCondition result = null;

            xr.Read();
            string elname = xr.Name.ToLower();
            if (xr.NodeType == XmlNodeType.EndElement && elname == endTag) return null; //inform caller: no more conditions left
            if (xr.NodeType == XmlNodeType.Element)
            {
                if (elname == "true") result = new TRUECondition();
                else if (elname == "false") result = new FALSECondition();
                else if (elname == "equals") result = EqualsCondition.ParseXML(xr);
                else if (elname == "and") result = ANDCondition.ParseXML(xr);
                else throw new XmlException("unknown condition '" + elname + "'");
            }
            else
                throw new XmlException("expected a condition tag <true/> or <false/> instead of " + elname);

            return result;
        }
    }
}
