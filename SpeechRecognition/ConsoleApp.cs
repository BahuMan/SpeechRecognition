using System;
using System.Speech.Recognition;

namespace bvba.cryingpants.SpeechRecognition
{
    class ConsoleApp
    {
        static void Main(string[] args)
        {
            VAProfile vp = new VAProfile("d:\\Projects\\Unity3D\\SpeechRecognition\\testprofile.vap");
            Console.WriteLine("Press <enter> when ready ...");
            Console.ReadLine();
        }
    }
}
