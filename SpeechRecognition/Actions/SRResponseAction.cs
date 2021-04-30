using System;
using System.Xml;
using bvba.cryingpants.SpeechRecognition.Conditions;
using bvba.cryingpants.SpeechRecognition.Expressions;
using System.Speech.Synthesis;

namespace bvba.cryingpants.SpeechRecognition.Actions
{
    class SRResponseAction : ISRAction
    {
        private SpeechSynthesizer tts;
        private ISRExpression _expression;

        public SRResponseAction(ISRExpression expression)
        {
            _expression = expression;
            tts = new SpeechSynthesizer();
            tts.SetOutputToDefaultAudioDevice();
            foreach (var v in tts.GetInstalledVoices())
            {
                Console.WriteLine(v.VoiceInfo.Name + "(" + v.VoiceInfo.Gender + "," + v.VoiceInfo.Age + ")" + v.VoiceInfo.Culture + " -> " + v.VoiceInfo.AdditionalInfo.Count);
            }
        }

        public void PerformAction(SRStatus status, string inputstring)
        {
            //@TODO: decide whether to use console or speech synthesis

            String evaluated = _expression.Evaluate(status);
            Console.WriteLine(evaluated);
            tts.Speak(evaluated);

        }

        public static SRResponseAction ParseXML(XmlReader xr)
        {
            return new SRResponseAction(CompoundExpression.ParseXML(xr, "response"));
        }
    }
}
