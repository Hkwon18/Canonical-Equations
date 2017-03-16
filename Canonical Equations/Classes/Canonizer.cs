using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Canonical_Equations.Classes
{
    public class Canonizer
    {
        private List<Summand> SummandList;

        public Canonizer()
        {
            SummandList = new List<Summand>();
        }

        public string ParseEquation(string equation)
        {
            ProcessSummands(equation, SummandList, OperationConstants.PLUS_SIGN);
            if (!IsSummandListValid(SummandList))
            {
                throw new Exception();
            }
            return CanonizedEquationToString();
        }

        private int FindBracketIndex(string leftBracketSubString)
        {
            Stack<char> stack = new Stack<char>();
            for (int i = 0; i < leftBracketSubString.Length; i++)
            {
                if (leftBracketSubString[i] == '(')
                {
                    stack.Push('(');
                }
                if (leftBracketSubString[i] == ')')
                {
                    stack.Pop();
                }
                if (stack.Count == 0)
                {
                    return i;
                }
            }
            return -1;
        }

        private void ProcessSummands(string inputString, List<Summand> list, char startingOperation)
        {
            string coefficient = String.Empty;
            string variable = String.Empty;
            string exponent = String.Empty;
            char operation = OperationConstants.PLUS_SIGN;
            bool isRhSide = false;
            bool isRaisedToExp = false;
            bool isNumExpected = false;
            bool safeToResolve = false;
            Summand summand;

            for (int i = 0; i < inputString.Length; i++)
            {
                char value = inputString[i];
                if (Char.IsWhiteSpace(value))
                {
                    continue;
                }
                if (IsBracket(value))
                {
                    int bracketIndex = FindBracketIndex(inputString.Substring(i));
                    string bracketSubString = inputString.Substring(i+1, bracketIndex-1);
                    if (startingOperation == operation)
                    {
                        ProcessSummands(bracketSubString, list, OperationConstants.PLUS_SIGN);
                    }
                    else
                    {
                        ProcessSummands(bracketSubString, list, OperationConstants.MINUS_SIGN);
                    }
                    i += bracketIndex;
                    continue;
                }
                if (IsOperation(value))
                {
                    if (isNumExpected)
                    {
                        throw new Exception();
                    }
                    isRaisedToExp = false;
                    if (value == OperationConstants.EXPONENT_SIGN)
                    {
                        if (!String.IsNullOrEmpty(variable))
                        {
                            isRaisedToExp = true;
                            isNumExpected = true;
                            safeToResolve = false;
                        }
                        else
                        {
                            throw new Exception();
                        }
                    }
                    if (value == OperationConstants.EQUAL_SIGN)
                    {
                        if (safeToResolve)
                        {
                            summand = new Summand(coefficient, exponent, variable, operation, isRhSide);
                            if (startingOperation.Equals(OperationConstants.MINUS_SIGN))
                            {
                                summand.Coefficient = summand.Coefficient * -1;
                            }
                            ResolveSummandToList(summand, list);
                            coefficient = String.Empty;
                            variable = String.Empty;
                            exponent = String.Empty;
                            operation = OperationConstants.PLUS_SIGN;
                            isRaisedToExp = false;
                            safeToResolve = false;
                            isNumExpected = false;
                        }
                        else if (list.Count.Equals(0))
                        {
                            throw new Exception();
                        }
                        isRhSide = true;
                    }
                    else if (value == OperationConstants.MINUS_SIGN)
                    {
                        if (safeToResolve)
                        {
                            summand = new Summand(coefficient, exponent, variable, operation, isRhSide);
                            if (startingOperation.Equals(OperationConstants.MINUS_SIGN))
                            {
                                summand.Coefficient = summand.Coefficient * -1;
                            }
                            ResolveSummandToList(summand, list);
                            coefficient = String.Empty;
                            variable = String.Empty;
                            exponent = String.Empty;
                            isRaisedToExp = false;
                            safeToResolve = false;
                            isNumExpected = false;

                            operation = OperationConstants.MINUS_SIGN;
                        }
                        else
                        {
                            if (String.IsNullOrEmpty(coefficient) && String.IsNullOrEmpty(variable)
                                && operation == OperationConstants.MINUS_SIGN)
                            {
                                operation = OperationConstants.PLUS_SIGN;
                            }
                            else
                            {
                                operation = OperationConstants.MINUS_SIGN;
                            }
                        }
                    }
                    else if (value == OperationConstants.PLUS_SIGN)
                    {
                        if (safeToResolve)
                        {
                            summand = new Summand(coefficient, exponent, variable, operation, isRhSide);
                            if (startingOperation.Equals(OperationConstants.MINUS_SIGN))
                            {
                                summand.Coefficient = summand.Coefficient * -1;
                            }
                            ResolveSummandToList(summand, list);
                            coefficient = String.Empty;
                            variable = String.Empty;
                            exponent = String.Empty;
                            isRaisedToExp = false;
                            safeToResolve = false;
                            isNumExpected = false;

                            operation = OperationConstants.PLUS_SIGN;
                        }
                        else
                        {
                            operation = OperationConstants.PLUS_SIGN;
                        }
                    }
                }
                else if (Char.IsNumber(value) || value == '.')
                {
                    if (String.IsNullOrEmpty(variable))
                    {
                        coefficient += value;
                        isNumExpected = false;
                    }
                    else if (isRaisedToExp)
                    {
                        exponent += value;
                        isNumExpected = false;
                    }
                    else
                    {
                        throw new Exception();
                    }
                }
                else if (Char.IsLetter(value))
                {
                    if (!isRaisedToExp)
                    {
                        variable += value;
                        isNumExpected = false;
                    }
                    else
                    {
                        throw new Exception();
                    }
                }
                if (!String.IsNullOrEmpty(coefficient) || !String.IsNullOrEmpty(variable))
                {
                    if (!isNumExpected)
                    {
                        safeToResolve = true;
                    }
                }
            }
            summand = new Summand(coefficient, exponent, variable, operation, isRhSide);
            if (startingOperation.Equals(OperationConstants.MINUS_SIGN))
            {
                summand.Coefficient = summand.Coefficient * -1;
            }
            ResolveSummandToList(summand, list);
        }

        private string CanonizedEquationToString()
        {
            StringBuilder sb = new StringBuilder();
            List<Summand> standardizedSummandList = 
                SummandList.OrderBy(x => x.Variable.Length==0).
                ThenBy(x => x.Variable.Length).
                ThenBy( x => x.Variable).
                ThenByDescending(x => x.Order).ToList<Summand>();
                
            foreach (Summand summand in standardizedSummandList)
            {
                if (!summand.Coefficient.Equals(0))
                {
                    if (sb.Length > 0)
                    {
                        sb.Append(" ");
                    }
                    if (summand.Coefficient > 0)
                    {
                        if (sb.Length > 0)
                        {
                            sb.Append("+ ");
                        }
                        if (summand.Coefficient.Equals(1))
                        {
                            if (String.IsNullOrEmpty(summand.Variable))
                            {
                                sb.Append(summand.Coefficient.ToString(CultureInfo.InvariantCulture));
                            }
                        }
                        else
                        {
                            sb.Append(summand.Coefficient.ToString(CultureInfo.InvariantCulture));
                        }
                    }
                    else if (summand.Coefficient < 0)
                    {
                        if (sb.Length == 0)
                        {
                            sb.Append("-");
                        }
                        else
                        {
                            sb.Append("- ");
                        }

                        if (summand.Coefficient.Equals(-1))
                        {
                            if (String.IsNullOrEmpty(summand.Variable))
                            {
                                double inverse = summand.Coefficient * -1;
                                sb.Append(inverse.ToString(CultureInfo.InvariantCulture));
                            }
                        }
                        else
                        {
                            double inverse = summand.Coefficient * -1;
                            sb.Append(inverse.ToString(CultureInfo.InvariantCulture));
                        }
                    }

                    if (!String.IsNullOrEmpty(summand.Variable))
                    {
                        sb.Append(summand.Variable);
                    }
                    if (summand.Order != 1)
                    {
                        sb.Append("^");
                        sb.Append(summand.Order.ToString());
                    }
                }
            }
            if (sb.Length.Equals(0))
            {
                sb.Append("0");
            }
            sb.Append(" = 0");
            return sb.ToString();
        }

        private void ResolveSummandToList(Summand summand, List<Summand> list)
        {
            Summand matchingVariable = list.Find(x =>
                x.Variable.Equals(summand.Variable) && x.Order == summand.Order);

            if (matchingVariable != null)
            {
                matchingVariable.Coefficient += summand.Coefficient;
                
                if (summand.IsRhSide)
                {
                    matchingVariable.IsRhSide = summand.IsRhSide;
                }
            }
            else
            {
                list.Add(summand);
            }
        }

        private static bool IsOperation(char item)
        {
            bool isOperator = item.Equals(OperationConstants.PLUS_SIGN) 
                || item.Equals(OperationConstants.MINUS_SIGN) 
                || item.Equals(OperationConstants.EQUAL_SIGN)
                || item.Equals(OperationConstants.EXPONENT_SIGN);

            return isOperator;
        }

        private static bool IsBracket(char item)
        {
            bool isBracket = item.Equals('(') || item.Equals(')');

            return isBracket;
        }

        private bool IsSummandListValid(List<Summand> summandList)
        {
            if (summandList.Count < 2)
            {
                if (summandList.Count.Equals(1))
                {
                    if (summandList[0].Coefficient.Equals(0))
                    {
                        return true;
                    }
                }
                return false;
            }
            bool rightValuesExist = false;
            bool success = false;
            foreach (Summand summand in summandList)
            {
                if (summand.IsRhSide)
                {
                    rightValuesExist = true;
                }
                if (!String.IsNullOrEmpty(summand.Variable))
                {
                    success = true;
                }
            }
            success = success && rightValuesExist;
            return success;
        }
    }
}
