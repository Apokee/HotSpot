using System;
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

        public double Evaluate(VariableBag variables)
        {
            if (_constant != null)
            {
                return _constant.Value;
            }

            if (_variable != null)
            {
                var value = variables.GetValue(_variable.Value);

                if (!double.IsNaN(value))
                    return value;
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

        public static Expression TryParse(string s)
        {
            var variable = s.TryParse<Variable>();

            if (variable != null)
            {
                return new Expression(variable.Value);
            }
            else
            {
                var constant = s.TryParse<double>();

                if (constant != null)
                {
                    return new Expression(constant.Value);
                }
            }

            return null;
        }
    }
}
