using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace bvba.cryingpants.SpeechRecognition.Expressions
{
    public class CompoundExpression : ISRExpression
    {
        private List<ISRExpression> _expressions = new List<ISRExpression>();

        public int Count { get { return _expressions.Count; } }

        public ISRExpression this[int i] {
            get { return _expressions[i]; }
        }

        public void AddExpression(ISRExpression expr)
        {
            _expressions.Add(expr);
        }

        public string Evaluate(SRStatus status)
        {
            StringBuilder sb = new StringBuilder();
            foreach (var e in _expressions) sb.Append(e.Evaluate(status));
            return sb.ToString();
        }

        public static ISRExpression ParseXML(XmlReader xr, string endTag)
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
    }

}
