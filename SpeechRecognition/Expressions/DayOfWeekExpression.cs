using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bvba.cryingpants.SpeechRecognition.Expressions
{
    public class DayOfWeekExpression : ISRExpression
    {
        public string Evaluate(SRStatus status)
        {
            return DateTime.Now.DayOfWeek.ToString();
        }
    }
}
