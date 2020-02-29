using System;
using System.Speech.Recognition;
using System.Xml;

namespace bvba.cryingpants.SpeechRecognition
{
    public class ParseVAProfile
    {
        static void oldmain(string[] args)
        {
            try
            {
                VAProfile vp = new VAProfile("d:\\Projects\\Unity3D\\SpeechRecognition\\arma3.vap");
            }
            catch (XmlException xmle)
            {
                Console.WriteLine(xmle.Message);
                Console.WriteLine(xmle.StackTrace);
            }
            Console.WriteLine("Press <enter> when ready ...");
            Console.ReadLine();
        }
    }
}
