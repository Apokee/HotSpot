using System;

namespace HotSpot.Configuration
{
    public struct AutoBoolean : IEquatable<AutoBoolean>
    {
        public static readonly AutoBoolean Auto = new AutoBoolean(null);
        public static readonly AutoBoolean False = new AutoBoolean(false);
        public static readonly AutoBoolean True = new AutoBoolean(true);

        public const string AutoString  = "Auto";
        public const string FalseString = "False";
        public const string TrueString  = "True";

        private readonly Value _value;

        public AutoBoolean(bool? value)
        {
            _value = value == null ? Value.Auto : (value.Value ? Value.True : Value.False);
        }

        public override string ToString()
        {
            switch (_value)
            {
                case Value.Auto:
                    return AutoString;
                case Value.False:
                    return FalseString;
                case Value.True:
                    return TrueString;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public static AutoBoolean Parse(string value)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            AutoBoolean result;
            if (TryParse(value, out result))
                return result;
            else
                throw new FormatException("String was not recognized as a valid AutoBoolean.");
        }

        public static bool TryParse(string value, out AutoBoolean result)
        {
            if (string.Equals(value, AutoString, StringComparison.OrdinalIgnoreCase))
            {
                result = new AutoBoolean(null);
                return true;
            }
            else if (string.Equals(value, FalseString, StringComparison.OrdinalIgnoreCase))
            {
                result = new AutoBoolean(false);
                return true;
            }
            else if (string.Equals(value, TrueString, StringComparison.OrdinalIgnoreCase))
            {
                result = new AutoBoolean(true);
                return true;
            }
            else
            {
                result = new AutoBoolean();
                return false;
            }
        }

        public bool Equals(AutoBoolean other)
        {
            return _value == other._value;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is AutoBoolean && Equals((AutoBoolean)obj);
        }

        public override int GetHashCode()
        {
            return (int)_value;
        }

        public static bool operator ==(AutoBoolean left, AutoBoolean right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(AutoBoolean left, AutoBoolean right)
        {
            return !left.Equals(right);
        }

        public static implicit operator AutoBoolean(bool b) => new AutoBoolean(b);

        public static explicit operator AutoBoolean(bool? b) => new AutoBoolean(b);

        public static explicit operator bool(AutoBoolean ab)
        {
            if (ab._value != Value.Auto)
                return (bool) ab;
            else
                throw new InvalidCastException("AutoBoolean with Auto value cannot be converted to a Boolean");
        }

        private enum Value : byte
        {
            Auto = 0,
            False = 1,
            True = 2,
        }
    }
}
