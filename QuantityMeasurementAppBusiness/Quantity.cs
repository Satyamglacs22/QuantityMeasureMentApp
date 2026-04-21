using QuantityMeasurementAppBusiness.Interfaces;
using QuantityMeasurementAppModels.Enums;

namespace QuantityMeasurementAppBusiness
{
    public class Quantity<U> where U : IMeasurable
    {
        private readonly double Value;
        private readonly U Unit;

        private const double epsilon = 0.0001;

        public Quantity(double value, U unit)
        {
            if (unit == null)
                throw new ArgumentException("Unit cannot be null");

            if (double.IsNaN(value) || double.IsInfinity(value))
                throw new ArgumentException("Invalid numeric value");

            Value = value;
            Unit  = unit;
        }

        // Convert to target unit
        public Quantity<U> ConvertTo(U targetUnit)
        {
            if (targetUnit == null)
                throw new ArgumentException("Invalid target unit");

            double baseValue     = Unit.ConvertToBaseUnit(Value);
            double convertedValue = targetUnit.ConvertFromBaseUnit(baseValue);
            return new Quantity<U>(convertedValue, targetUnit);
        }

        // Validate arithmetic operands
        private void ValidateArithmeticOperands(Quantity<U> second, U? targetUnit, bool targetRequired)
        {
            if (second == null)
                throw new ArgumentException("Second operand cannot be null");

            if (Unit.GetType() != second.Unit.GetType())
                throw new ArgumentException(
                    "Cannot perform arithmetic between different measurement categories: "
                    + Unit.GetMeasurementType() + " and " + second.Unit.GetMeasurementType());

            if (double.IsNaN(Value) || double.IsInfinity(Value) ||
                double.IsNaN(second.Value) || double.IsInfinity(second.Value))
                throw new ArgumentException("Invalid numeric value");

            if (targetRequired && targetUnit == null)
                throw new ArgumentException("Invalid target unit");
        }

        // Perform arithmetic in base units
        private double PerformBaseArithmetic(Quantity<U> second, ArithmeticOperation operation)
        {
            double firstBase  = Unit.ConvertToBaseUnit(Value);
            double secondBase = second.Unit.ConvertToBaseUnit(second.Value);

            return operation switch
            {
                ArithmeticOperation.ADD      => firstBase + secondBase,
                ArithmeticOperation.SUBTRACT => firstBase - secondBase,
                ArithmeticOperation.DIVIDE   => firstBase / secondBase,
                _                             => throw new ArgumentException("Invalid arithmetic operation")
            };
        }

        // Add with explicit target unit
        public Quantity<U> Add(Quantity<U> second, U targetUnit)
        {
            Unit.ValidateOperationSupport("Addition");
            ValidateArithmeticOperands(second, targetUnit, true);
            double baseResult  = PerformBaseArithmetic(second, ArithmeticOperation.ADD);
            double resultValue = targetUnit.ConvertFromBaseUnit(baseResult);
            return new Quantity<U>(resultValue, targetUnit);
        }

        // Add using current unit
        public Quantity<U> Add(Quantity<U> second)
        {
            Unit.ValidateOperationSupport("Addition");
            ValidateArithmeticOperands(second, Unit, true);
            double baseResult  = PerformBaseArithmetic(second, ArithmeticOperation.ADD);
            double resultValue = Unit.ConvertFromBaseUnit(baseResult);
            return new Quantity<U>(resultValue, Unit);
        }

        // Subtract with explicit target unit
        public Quantity<U> Subtract(Quantity<U> second, U targetUnit)
        {
            Unit.ValidateOperationSupport("Subtraction");
            ValidateArithmeticOperands(second, targetUnit, true);
            double baseResult  = PerformBaseArithmetic(second, ArithmeticOperation.SUBTRACT);
            double resultValue = targetUnit.ConvertFromBaseUnit(baseResult);
            return new Quantity<U>(Math.Round(resultValue, 2), targetUnit);
        }

        // Subtract using current unit
        public Quantity<U> Subtract(Quantity<U> second)
        {
            Unit.ValidateOperationSupport("Subtraction");
            ValidateArithmeticOperands(second, Unit, true);
            double baseResult  = PerformBaseArithmetic(second, ArithmeticOperation.SUBTRACT);
            double resultValue = Unit.ConvertFromBaseUnit(baseResult);
            return new Quantity<U>(Math.Round(resultValue, 2), Unit);
        }

        // Divide (returns ratio — dimensionless)
        public double Divide(Quantity<U> second)
        {
            Unit.ValidateOperationSupport("Division");
            ValidateArithmeticOperands(second, default, false);

            if (second.Value == 0)
                throw new DivideByZeroException("Cannot divide by zero quantity");

            return PerformBaseArithmetic(second, ArithmeticOperation.DIVIDE);
        }

        public override bool Equals(object? obj)
        {
            if (this == obj) return true;
            if (obj == null || obj.GetType() != GetType()) return false;

            Quantity<U> other = (Quantity<U>)obj;

            if (Unit.GetType() != other.Unit.GetType()) return false;

            double firstBase  = Unit.ConvertToBaseUnit(Value);
            double secondBase = other.Unit.ConvertToBaseUnit(other.Value);

            return Math.Abs(firstBase - secondBase) <= epsilon;
        }

        public override int GetHashCode()
        {
            double baseValue = Unit.ConvertToBaseUnit(Value);
            double rounded   = Math.Round(baseValue / epsilon) * epsilon;
            return rounded.GetHashCode();
        }

        public override string ToString() => $"{Value} {Unit.GetUnitName()}";

        public double GetValue() => Value;
    }
}
