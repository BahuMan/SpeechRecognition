using System.IO;
using System.Xml;
using System;

namespace bvba.cryingpants.SpeechRecognition
{
    public class VAProfile
    {
        public VAProfile(string filename)
        {
            XmlReaderSettings xs = new XmlReaderSettings
            {
                IgnoreComments = true,
                IgnoreWhitespace = true,
                IgnoreProcessingInstructions = true
                
            };
            //if I pass the filename as an URI instead of File.openText, I risk an XML exception about the encoding
            //this way, I force encoding to be UTF-8 and the XmlReader ignores the encoding tag of UTF-16 which is wrong anyway
            using (XmlReader xr = XmlReader.Create(File.OpenText(filename), xs))
            {
                ParseProfile(xr);
            }
        }

        private void ParseProfile(XmlReader xr)
        {
            try
            {
                xr.Read();
                if (xr.NodeType == XmlNodeType.XmlDeclaration) Console.WriteLine("xml, as expected");
            }
            catch (XmlException xmle)
            {
                Console.WriteLine("error on first element");
                foreach (var i in xmle.Data.Keys)
                {
                    Console.Write(i.ToString());
                    Console.Write(" = ");
                    Console.WriteLine(xmle.Data[i].ToString());
                }
                throw; //re-throw exception; we can't really handle it anyway
            }

            //read start profile
            xr.Read();
            if (xr.NodeType == XmlNodeType.Element && xr.Name.ToLower() == "profile") Console.WriteLine("start profile, as expected");
            else throw new XmlException("expected profile element instead of " + xr.Name);

            //first element is ID
            string id = ParseID(xr);

            while (xr.Read())
            {
                if (xr.NodeType == XmlNodeType.XmlDeclaration)
                {
                    Console.WriteLine("xml, as expected");
                }
                else if (xr.NodeType == XmlNodeType.Element)
                {
                    string name = xr.Name.ToLower();
                    if (name == "profile") Console.WriteLine("unexpected profile?");
                    if (name == "name") { Console.Write("profile name = "); Console.WriteLine(ParseText(xr)); }
                    else if (name == "id") { Console.Write("unexpected id = "); Console.WriteLine(ParseText(xr)); }
                    else if (name == "commands") ParseCommandList(xr);
                    //ignore everything else
                }
                else if (xr.NodeType == XmlNodeType.Text)
                {
                    //ignore
                }
                else if (xr.NodeType == XmlNodeType.EndElement)
                {
                    string name = xr.Name.ToLower();
                    if (name == "profile") Console.WriteLine("end of profile, as expected");
                    else if (name == "name") Console.WriteLine("end of name, as expected");
                    else if (name == "id") Console.WriteLine("end of id, ERROR");
                    else if (name == "commands") Console.WriteLine("end commandstring, ERROR");
                }
            }
            Console.WriteLine("finished parsing XML");
        }

        private string ParseID(XmlReader xr)
        {
            xr.Read();
            if (xr.NodeType != XmlNodeType.Element || xr.Name.ToLower() != "id") throw new XmlException("expected ID tag");
            xr.Read();
            if (xr.NodeType != XmlNodeType.Text) throw new XmlException("Expected text inside ID tag");
            string res = xr.Value;
            xr.Read();
            if (xr.NodeType != XmlNodeType.EndElement || xr.Name.ToLower() != "id") throw new XmlException("expected end of ID tag");
            return res;
        }
        private void ParseCommandList(XmlReader xr)
        {
            //for now, just swallow the whole actioncommand list:
            while (xr.Read())
            {
                if (xr.NodeType == XmlNodeType.EndElement && xr.Name.ToLower() == "commands") return;
            }
        }

        private string ParseText(XmlReader xr)
        {
            xr.Read();
            return xr.Value;
        }
    }
}
