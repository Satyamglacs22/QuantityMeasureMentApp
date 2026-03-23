using QuantityMeasurementAppBusiness.Interfaces;

namespace QuantityMeasurementAppBusiness
{
    public class Quantity<U> where U : IMeasurable
    {
        public double Value { get; }
        public U Unit { get; }

        private const double EPSILON = 0.0001;

        public Quantity(double value, U unit)
        {
            if (unit == null)
                throw new ArgumentException("Unit cannot be null");

            Value = value;
            Unit = unit;
        }

        private double ToBase()
        {
            return Unit.ConvertToBaseUnit(Value);
        }

        private static double Round(double value)
        {
            return Math.Round(value, 2);
        }

        public Quantity<U> ConvertTo(U targetUnit)
        {
            if (targetUnit == null)
                throw new ArgumentException("Target unit cannot be null");

            double baseValue = Unit.ConvertToBaseUnit(Value);
            double converted = targetUnit.ConvertFromBaseUnit(baseValue);

            return new Quantity<U>(Round(converted), targetUnit);
        }

        public Quantity<U> Add(Quantity<U> other)
        {
            Unit.ValidateOperationSupport("ADD");

            if (other == null)
                throw new ArgumentException("Other quantity cannot be null");

            if (Unit.GetType() != other.Unit.GetType())
                throw new ArgumentException("Cannot add quantities of different measurement types");

            double resultBase = ToBase() + other.ToBase();

            double result = Unit.ConvertFromBaseUnit(resultBase);

            return new Quantity<U>(Round(result), Unit);
        }

        public Quantity<U> Subtract(Quantity<U> other)
        {
            Unit.ValidateOperationSupport("SUBTRACT");

            if (other == null)
                throw new ArgumentException("Other quantity cannot be null");

            if (Unit.GetType() != other.Unit.GetType())
                throw new ArgumentException("Cannot subtract quantities of different measurement types");

            double resultBase = ToBase() - other.ToBase();

            double result = Unit.ConvertFromBaseUnit(resultBase);

            return new Quantity<U>(Round(result), Unit);
        }

        public double Divide(Quantity<U> other)
        {
            Unit.ValidateOperationSupport("DIVIDE");

            if (other == null)
                throw new ArgumentException("Other quantity cannot be null");

            if (Unit.GetType() != other.Unit.GetType())
                throw new ArgumentException("Cannot divide quantities of different measurement types");

            double divisor = other.ToBase();

            if (Math.Abs(divisor) < EPSILON)
                throw new DivideByZeroException("Cannot divide by zero quantity");

            return Round(ToBase() / divisor);
        }

        public override bool Equals(object? obj)
        {
            if (obj == null || !(obj is Quantity<U> other))
                return false;

            if (Unit.GetType() != other.Unit.GetType())
                return false;

            double diff = Math.Abs(ToBase() - other.ToBase());

            return diff < EPSILON;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Value, Unit);
        }

        public override string ToString()
        {
            return $"Quantity({Value}, {Unit.GetUnitName()})";
        }
    }
}