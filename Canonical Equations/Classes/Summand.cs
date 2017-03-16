using System;

namespace Canonical_Equations.Classes
{
    class Summand
    {
        public double Coefficient { get; set; }
        public string Variable { get; set; }
        public int Order { get; set; }
        public bool IsRhSide { get; set; }

        public Summand(string coefficient, string order, string variable, char operation, bool isRhSide)
        {
            string coeff = coefficient;
            string ord = order;
            string var = variable;
            if (String.IsNullOrEmpty(coeff))
            {
                coeff = "1";
            }
            if (String.IsNullOrEmpty(ord))
            {
                ord = "1";
            }
            if (String.IsNullOrEmpty(coefficient) && String.IsNullOrEmpty(variable))
            {
                coeff = "0";
                var = String.Empty;
            }
            double parsedCoefficient = Double.Parse(coeff);
            int parsedOrder = int.Parse(ord);
            if (OperationConstants.MINUS_SIGN.Equals(operation))
            {
                parsedCoefficient = parsedCoefficient * -1;
            }
            if (isRhSide)
            {
                parsedCoefficient = parsedCoefficient * -1;
            }
            Coefficient = parsedCoefficient;
            Variable = var;
            Order = parsedOrder;
            IsRhSide = isRhSide;
        }
    }
}
