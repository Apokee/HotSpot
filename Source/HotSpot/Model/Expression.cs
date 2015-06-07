using System;
using System.Collections.Generic;
using System.Globalization;

namespace HotSpot.Model
{
    internal sealed class Expression
    {
        private double? _constant;
        private Variable? _variable;

        private Expression(double constant)
        {
            _constant = constant;
        }

        private Expression(Variable variable)
        {
            _variable = variable;
        }

        public double Evaluate(Dictionary<Variable, double> variables)
        {
            if (_constant != null)
            {
                return _constant.Value;
            }
            else if (_variable != null)
            {
                double value;
                if (variables.TryGetValue(_variable.Value, out value))
                {
                    return value;
                }
            }

            throw new InvalidOperationException($"Expression '{this}' could not be evaluated.");
        }

        public override string ToString()
        {
            if (_constant != null)
            {
                return _constant.Value.ToString(CultureInfo.InvariantCulture);
            }
            else if (_variable != null)
            {
                return _variable.Value.ToString();
            }

            throw new InvalidOperationException("Expression is not set.");
        }

        public static Expression Parse(string s)
        {
            Variable variable;
            double constant;

            if (s.TryParseEnum(out variable))
            {
                return new Expression(variable);
            }
            else if (double.TryParse(s, out constant))
            {
                return new Expression(constant);
            }
            else
            {
                throw new FormatException($"Invalid expression: '{s}'");
            }
        }
    }
}
