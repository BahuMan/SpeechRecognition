using System;
using System.Xml;
using System.Speech.Recognition;
using bvba.cryingpants.SpeechRecognition.Actions;
using bvba.cryingpants.SpeechRecognition.Conditions;
using bvba.cryingpants.SpeechRecognition.Inputs;
using bvba.cryingpants.SpeechRecognition.Expressions;

namespace bvba.cryingpants.SpeechRecognition
{
    public class ParseSRProfile
    {
        public static void Main(string[] args)
        {
            SRProfile profile = ParseXML("F:\\Projects\\VisualStudio\\SpeechRecognition\\TestConfiguration.xml");
            Console.Write("profile ");
            Console.Write(profile.Name);
            Console.WriteLine(" read");
            SRStatus status = new SRStatus();


            using (SpeechRecognitionEngine sre = new SpeechRecognitionEngine())
            {
                sre.SpeechRecognized +=
                    new EventHandler<SpeechRecognizedEventArgs>((object e, SpeechRecognizedEventArgs a) => sre_speechRecognized(e, a, status));

                status.SetSpeechRecognitionEngine(sre);
                status.AddProfile(profile);

                sre.SetInputToDefaultAudioDevice();
                sre.RecognizeAsync(RecognizeMode.Multiple);

                Console.WriteLine("Ready...");

                Console.Write(">");
                string typed = Console.ReadLine().ToLower();
                while (typed != "quit" && typed != "exit")
                {
                    status.ProcessInput(typed);

                    Console.WriteLine();
                    Console.Write(">");
                    typed = Console.ReadLine().ToLower();
                }
            }

        }

        static void sre_speechRecognized(object sender, SpeechRecognizedEventArgs e, SRStatus status)
        {
            Console.WriteLine("Recognized text: " + e.Result.Text);
            status.ProcessInput(e.Result.Text);
            Console.WriteLine();
            Console.Write(">");
        }

        public static SRProfile ParseXML(string filename)
        {
            XmlReaderSettings xs = new XmlReaderSettings
            {
                IgnoreComments = true,
                IgnoreWhitespace = true,
                IgnoreProcessingInstructions = true

            };
            using (XmlReader xr = XmlReader.Create(filename, xs))
            //using (XmlReader xr = XmlReader.Create(File.OpenText(filename), xs))
            {
                return ParseXML(xr);
            }
        }

        public static SRProfile ParseXML(XmlReader xr)
        {
            SwallowXMLEncoding(xr);
            SRProfile profile = new SRProfile(GetProfileName(xr));

            while (xr.Read())
            {
                string elname = xr.Name.ToLower();
                if (xr.NodeType == XmlNodeType.EndElement && elname == "profile")
                {
                    return profile;
                }
                else if (xr.NodeType == XmlNodeType.Element && elname == "input")
                    profile.AddInput(ReadInput(xr));
            }

            return profile;
        }

        private static Inputs.SRInput ReadInput(XmlReader xr)
        {

            Inputs.SRInput input = new Inputs.SRInput();

            while (xr.Read())
            {
                string elname = xr.Name.ToLower();
                if (xr.NodeType == XmlNodeType.EndElement && elname == "input")
                    break;
                else if (xr.NodeType == XmlNodeType.Element && elname == "id")
                    input.Name = FoundElement(xr, "id");
                else if (xr.NodeType == XmlNodeType.Element && elname == "groupname")
                    input.GroupName = FoundElement(xr, "groupname");
                else if (xr.NodeType == XmlNodeType.Element && elname == "inputstring")
                    input.AddInputString(FoundElement(xr, "inputstring"));
                else if (xr.NodeType == XmlNodeType.Element && elname == "actionsequence")
                    input.SetActionSequence(ReadActionSequence(xr, "actionsequence"));
            }

            if (input.Name == null) throw new XmlException("found an input without a name");

            //@TODO: some autoexec inputs might have no inputstrings, so this check may have to be refined
            if (input.GetAllInputStrings().Count < 1) throw new XmlException("input '" + input.Name + "' has no inputstrings");

            if (input.ActionCount < 1) throw new XmlException("input '" + input.Name + "' has no actions");
            return input;
        }

        private static SRActionSequence ReadActionSequence(XmlReader xr, string endTag)
        {
            SRActionSequence actionSequence = new SRActionSequence();

            while (xr.Read())
            {
                string elname = xr.Name.ToLower();
                if (xr.NodeType == XmlNodeType.EndElement && elname == endTag)
                    break;
                else if (xr.NodeType == XmlNodeType.Element && elname == "if")
                    actionSequence.AddAction(ReadIfAction(xr));
                else if (xr.NodeType == XmlNodeType.Element && elname == "response")
                    actionSequence.AddAction(ReadResponse(xr));
                else if (xr.NodeType == XmlNodeType.Element && elname == "setvar")
                    actionSequence.AddAction(new SRSetVariableAction(xr.GetAttribute("name"), ReadExpression(xr, "setvar")));
                else
                    throw new XmlException("expected action but found: " + xr.Name);
            }

            return actionSequence;
        }

        private static ISRAction ReadIfAction(XmlReader xr)
        {
            ISRCondition condition = ReadConditionOperator(xr);

            if (!xr.Read() || xr.NodeType != XmlNodeType.Element || xr.Name.ToLower() != "then")
                throw new XmlException("condition should have 1 condition operator followed by actionsequence; found: " + xr.Name);

            SRActionSequence action = ReadActionSequence(xr, "then");

            SRIfAction result = new SRIfAction(condition, action);

            xr.Read();
            if (xr.NodeType == XmlNodeType.Element && xr.Name.ToLower() == "else")
            {
                result.SetElseAction(ReadActionSequence(xr, "else"));
                if (!xr.Read() || xr.NodeType != XmlNodeType.EndElement || xr.Name.ToLower() != "if")
                    throw new XmlException("expected </condition>; found: " + xr.Name);
            }
            else if (xr.NodeType != XmlNodeType.EndElement || xr.Name.ToLower() != "if")
                throw new XmlException("expected </condition>; found: " + xr.Name);

            return result;
        }

        private static SRResponseAction ReadResponse(XmlReader xr)
        {
            return new SRResponseAction(ReadExpression(xr, "response"));
        }

        private static ISRExpression ReadExpression(XmlReader xr, string endTag)
        {
            CompoundExpression ce = new CompoundExpression();
            while (xr.Read())
            {
                string elname = xr.Name.ToLower();
                if (xr.IsStartElement() && elname == "dayofweek")
                    ce.AddExpression(new DayOfWeekExpression());
                else if (xr.IsStartElement() && elname == "var")
                    ce.AddExpression(new VariableExpression(xr.GetAttribute("name")));
                else if (xr.NodeType == XmlNodeType.Text)
                    ce.AddExpression(new TextExpression(xr.Value));
                else if (xr.NodeType == XmlNodeType.EndElement && elname == endTag)
                    break;
                else
                    throw new XmlException("expected text, <dayofweek/>, <var/> or </" + endTag + "> instead of " + xr.Name);
            }
            if (ce.Count == 0) throw new XmlException("no expression found");
            if (ce.Count == 1) return ce[0];

            return ce;
        }

        //@TODO: will become a recursive method to read composite conditions
        private static ISRCondition ReadConditionOperator(XmlReader xr)
        {
            ISRCondition result = null;

            xr.Read();
            string elname = xr.Name.ToLower();
            if (xr.NodeType == XmlNodeType.Element) {
                if (elname == "true") result = new TRUECondition();
                else if (elname == "false") result = new FALSECondition();
                else if (elname == "equals")  result = ReadEqualsCondition(xr);
                else throw new XmlException("unknown condition '" + elname + "'");
            }
            else
                throw new XmlException("expected a condition tag <true/> or <false/> instead of " + elname);

            return result;
        }

        private static EqualsCondition ReadEqualsCondition(XmlReader xr)
        {
            xr.Read();
            if (xr.NodeType != XmlNodeType.Element || xr.Name.ToLower() != "operand1")
                throw new XmlException("expected <operand1> but read " + xr.ToString());
            ISRExpression op1 = ReadExpression(xr, "operand1");

            xr.Read();
            if (xr.NodeType != XmlNodeType.Element || xr.Name.ToLower() != "operand2")
                throw new XmlException("expected <operand2> but read " + xr.ToString());
            ISRExpression op2 = ReadExpression(xr, "operand2");

            xr.Read();
            if (xr.NodeType != XmlNodeType.EndElement || xr.Name.ToLower() != "equals")
                throw new XmlException("expected <equals> but read " + xr.ToString());

            return new EqualsCondition(op1, op2);
        }

        private static string FoundElement(XmlReader xr, string elname)
        {
            xr.Read();
            if (xr.NodeType != XmlNodeType.Text) throw new XmlException("expected text after " + elname + " tag");
            string result = xr.Value.ToLower();

            xr.Read();
            if (xr.NodeType != XmlNodeType.EndElement || xr.Name.ToLower() != elname) throw new XmlException("expected and of element after text in " + elname + " tag");

            return result;
        }

        private static void SwallowXMLEncoding(XmlReader xr)
        {
            try
            {
                xr.Read();
                if (xr.NodeType == XmlNodeType.XmlDeclaration) Console.WriteLine("xml, as expected");
            }
            catch (XmlException xmle)
            when (xmle.Message.Contains("Unicode byte order mark"))
            {
                throw new XmlException("the encoding specified in the XML document is different from the encoding actually used by the file. Simply removing the encoding in the first line of the xml fle might solve the problem");
            }


        }

        private static string GetProfileName(XmlReader xr)
        {
            xr.Read();
            if (xr.NodeType != XmlNodeType.Element || xr.Name.ToLower() != "profile")  throw new XmlException("expected tag <profile> instead of " + xr.Name);
            string result = xr.GetAttribute("name");

            return result;
        }

        private static string ExpectTextElement(XmlReader xr, string elemName)
        {
            xr.Read();
            if (xr.NodeType != XmlNodeType.Element || xr.Name.ToLower() != elemName) throw new XmlException("expected tag <" + elemName + "> instead of " + xr.Name);

            xr.Read();
            if (xr.NodeType != XmlNodeType.Text) throw new XmlException("expected text inside " + elemName);
            string res = xr.Value;

            xr.Read();
            if (xr.NodeType != XmlNodeType.EndElement || xr.Name.ToLower() != elemName) throw new XmlException("expected end tag </" + elemName + "> instead of " + xr.Name);

            return res;
        }
    }
}
