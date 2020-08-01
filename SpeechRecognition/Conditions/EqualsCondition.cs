using bvba.cryingpants.SpeechRecognition.Expressions;
using System.Xml;

namespace bvba.cryingpants.SpeechRecognition.Conditions
{
    public class EqualsCondition: ISRCondition
    {
        private ISRExpression _expr1, _expr2;

        public EqualsCondition(ISRExpression expr1, ISRExpression expr2)
        {
            _expr1 = expr1;
            _expr2 = expr2;
        }

        public bool HasBeenMet(SRStatus status)
        {
            return _expr1.Evaluate(status).ToLower() == _expr2.Evaluate(status).ToLower();
        }

        public static EqualsCondition ParseXML(XmlReader xr)
        {
            xr.Read();
            if (xr.NodeType != XmlNodeType.Element || xr.Name.ToLower() != "operand1")
                throw new XmlException("expected <operand1> but read " + xr.ToString());
            ISRExpression op1 = CompoundExpression.ParseXML(xr, "operand1");

            xr.Read();
            if (xr.NodeType != XmlNodeType.Element || xr.Name.ToLower() != "operand2")
                throw new XmlException("expected <operand2> but read " + xr.ToString());
            ISRExpression op2 = CompoundExpression.ParseXML(xr, "operand2");

            xr.Read();
            if (xr.NodeType != XmlNodeType.EndElement || xr.Name.ToLower() != "equals")
                throw new XmlException("expected <equals> but read " + xr.ToString());

            return new EqualsCondition(op1, op2);
        }
    }
}
