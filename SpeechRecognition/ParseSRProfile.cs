﻿using System;
using System.Xml;
using System.Speech.Recognition;
using bvba.cryingpants.SpeechRecognition.Actions;
using bvba.cryingpants.SpeechRecognition.Conditions;
using bvba.cryingpants.SpeechRecognition.Inputs;

namespace bvba.cryingpants.SpeechRecognition
{
    public class ParseSRProfile
    {
        public static void Main(string[] args)
        {
            SRProfile profile = ParseXML("D:\\Projects\\Unity3D\\SpeechRecognition\\TestConfiguration.xml");
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
                Console.WriteLine("Ready...");

                Console.Write(">");
                string typed = Console.ReadLine().ToLower();
                while (typed != "quit" && typed != "exit")
                {
                    status.ProcessInput(typed);

                    Console.Write(">");
                    typed = Console.ReadLine().ToLower();
                }
            }

        }

        static void sre_speechRecognized(object sender, SpeechRecognizedEventArgs e, SRStatus status)
        {
            Console.WriteLine("Recognized text: " + e.Result.Text);
            status.ProcessInput(e.Result.Text);
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
                    ReadActionSequence(xr, input);
            }

            if (input.Name == null) throw new XmlException("found an input without a name");

            //@TODO: some autoexec inputs might have no inputstrings, so this check may have to be refined
            if (input.GetAllInputStrings().Count < 1) throw new XmlException("input '" + input.Name + "' has no inputstrings");

            if (input.ActionCount < 1) throw new XmlException("input '" + input.Name + "' has no actions");
            return input;
        }

        private static void ReadActionSequence(XmlReader xr, SRInput input)
        {
            
            while (xr.Read())
            {
                string elname = xr.Name;
                if (xr.NodeType == XmlNodeType.EndElement && elname == "actionSequence")
                    return;
                else if (xr.NodeType == XmlNodeType.Element && elname == "action")
                    input.AddAction(ReadSingleAction(xr));
            }
        }

        private static ISRAction ReadSingleAction(XmlReader xr)
        {
            SRCompoundAction action = new SRCompoundAction();

            while (xr.Read())
            {
                string elname = xr.Name.ToLower();
                if (xr.NodeType == XmlNodeType.EndElement && elname == "action")
                    break;
                else if (xr.NodeType == XmlNodeType.Element && elname == "condition")
                    action.SetCondition(ReadCondition(xr));
                else if (xr.NodeType == XmlNodeType.Element && elname == "response")
                     action.AddAction(new SRResponseAction(FoundElement(xr, "response")));
                else
                    throw new XmlException("unknown action '" + xr.Name + "'");
            }

            if (action.ActionCount < 1) throw new XmlException("expected <condition> or <response> inside actionsequence");
            return action;
        }

        //@TODO: will become a recursive method to read composite conditions
        private static ISRCondition ReadCondition(XmlReader xr)
        {
            ISRCondition result = null;

            xr.Read();
            string elname = xr.Name.ToLower();
            if (xr.NodeType == XmlNodeType.Element) {
                if (elname == "true") result = new TRUECondition();
                else if (elname == "false") result = new FALSECondition();
                else throw new XmlException("unknown condition '" + elname + "'");
            }
            else
                throw new XmlException("expected a condition tag <true/> or <false/> instead of " + elname);

            xr.Read();
            if (xr.NodeType != XmlNodeType.EndElement || xr.Name.ToLower() != "condition")
                throw new XmlException("expected </condition>");

            return result;
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