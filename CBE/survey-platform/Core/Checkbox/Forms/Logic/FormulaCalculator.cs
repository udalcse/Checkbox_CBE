using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;

namespace Checkbox.Forms.Logic
{
    /// <summary>
    /// Calculates the value of the arithmetic expression, that contains constants only
    /// 
    /// </summary>
    class FormulaCalculator
    {
        #region Public methods
        /// <summary>
        /// Calculates the expression result
        /// </summary>
        /// <param name="expression">Expression to calculate</param>
        /// <returns>Result</returns>
        public static double Calculate(string expression)
        {
            return parseAndCalc(expression);
        }

        private static CultureInfo _ConversionCI = new CultureInfo("en-US");
        public static CultureInfo ConversionCI
        {
            get
            {
                return _ConversionCI;
            }
        }

        #endregion Public methods

        #region Calculator's logic
        enum Status { Start, OperatorNeeded, OperationNeeded }

        /// <summary>
        /// Recursive function based on FSM that performs parsing and calculations
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        private static double parseAndCalc(string expression)
        {
            if (string.IsNullOrEmpty(expression))
                return default(double);

            int pos = 0;

            Status status = Status.Start;
            Stack<object> terms = new Stack<object>();
            
            while (pos < expression.Length)
            {
                string term = getNextTerm(expression, ref pos);
                if (string.IsNullOrEmpty(term))
                    continue;

                //finite-state machine based algorithm
                switch (status)
                {
                    case Status.Start:
                        switch (term)
                        {
                            case "-":
                                terms.Push(-1.0);
                                terms.Push('*');
                                status = Status.OperatorNeeded;
                                break;
                            case "(":
                                int begin = pos;
                                findCloseBrace(expression, ref pos);
                                string subExpr = expression.Substring(begin, pos - begin - 1);
                                terms.Push(parseAndCalc(subExpr));
                                status = Status.OperationNeeded;
                                break;
                            default:
                                if (char.IsDigit(term[0]))
                                {
                                    terms.Push(calcConst(term));
                                    status = Status.OperationNeeded;
                                }
                                else
                                if (term[0] == '@')
                                {
                                    throw new ApplicationException(string.Format("Wrong expression member {0} at position {1}.", term, pos - term.Length));
                                }
                                else
                                {
                                    throw new ApplicationException(string.Format("Cannot parse expression. Unknown term \"{0}\" at position {1} was found.", term, pos - term.Length));
                                }
                                break;
                        }
                        break;
                    case Status.OperatorNeeded:
                        switch (term)
                        {
                            case "(":
                                int begin = pos;
                                findCloseBrace(expression, ref pos);
                                string subExpr = expression.Substring(begin, pos - begin - 1);
                                double op2 = parseAndCalc(subExpr);

                                //if we have a high-priority operation -- do the operation right now and save result to the stack
                                if ((char)terms.Peek() == '*')
                                {
                                    terms.Pop();
                                    double op1 = (double)terms.Pop();
                                    terms.Push(op1 * op2);
                                }
                                else if ((char)terms.Peek() == '/')
                                {
                                    terms.Pop();
                                    double op1 = (double)terms.Pop();
                                    terms.Push(op1 / op2);
                                }
                                else
                                    terms.Push(op2);
                                
                                status = Status.OperationNeeded;
                                break;
                            default:
                                double op22 = default(double);
                                if (char.IsDigit(term[0]))
                                {
                                    op22 = calcConst(term);
                                }
                                else
                                    if (term[0] == '@')
                                {
                                    throw new ApplicationException(string.Format("Wrong expression member {0} at position {1}.", term, pos - term.Length));
                                }
                                else
                                {
                                    throw new ApplicationException(string.Format("Cannot parse expression. Unknown term \"{0}\" at position {1} was found.", term, pos - term.Length));
                                }

                                //if we have a high-priority operation -- do the operation right now and save result to the stack
                                if ((char)terms.Peek() == '*')
                                {
                                    terms.Pop();
                                    double op1 = (double)terms.Pop();
                                    terms.Push(op1 * op22);
                                }
                                else if ((char)terms.Peek() == '/')
                                {
                                    terms.Pop();
                                    double op1 = (double)terms.Pop();
                                    terms.Push(op1 / op22);
                                }
                                else
                                    terms.Push(op22);

                                status = Status.OperationNeeded;

                                break;
                        }
                        break;
                    case Status.OperationNeeded:
                        if (term == "+" || term == "-" || term == "*" || term == "/")
                            terms.Push(term[0]);
                        status = Status.OperatorNeeded;
                        break;
                }
            }

            if (status == Status.OperatorNeeded)
                throw new ApplicationException("Cannot parse expression: an operand was expected");

            //process low-priority operations
            while (terms.Count > 1)
            {
                double op2 = (double)terms.Pop();
                char operation = (char)terms.Pop();
                double op1 = (double)terms.Pop();
                switch (operation)
                {
                    case '+':
                        terms.Push(op1 + op2);
                        break;
                    case '-':
                        terms.Push(op1 - op2);
                        break;
                    default:
                        throw new ApplicationException(string.Format("Cannot parse expression: unexpected operation \"{0}\"", operation));
                }
            }

            //result is the thing that remains in the stack
            return (double)terms.Pop();
        }
        private static double calcConst(string expr)
        {
            return double.Parse(expr, ConversionCI);
        }

        private static string getConstant(string expr, ref int pos)
        {
            int begin = pos;
            bool dotFound = false;
            while (pos < expr.Length && (char.IsDigit(expr[pos]) || expr[pos] == '.' && !dotFound))
            {
                if (expr[pos] == '.')
                    dotFound = true;
                pos++;
            }

            return expr.Substring(begin, pos - begin);
        }

        /// <summary>
        /// Returns next significant term
        /// </summary>
        /// <param name="expr"></param>
        /// <param name="pos"></param>
        /// <returns></returns>
        private static string getNextTerm(string expr, ref int pos)
        {

            if (string.IsNullOrEmpty(expr) || pos >= expr.Length)
                return string.Empty;

            //bypass spaces
            while (pos < expr.Length && expr[pos] == ' ')
                pos++;

            if (pos >= expr.Length)
                return string.Empty;

            if (expr[pos] == '(' || expr[pos] == ')' || expr[pos] == '+' || expr[pos] == '-' || expr[pos] == '*' || expr[pos] == '/')
            {
                pos++;
                return expr[pos - 1].ToString();
            }

            if (char.IsDigit(expr[pos]))
            {
                return getConstant(expr, ref pos);
            }

            if (pos < expr.Length - 1 && expr[pos] == '@' && expr[pos + 1] == '@')
            {
                //read variable
                int begin = pos;
                while (pos < expr.Length && expr[pos] != ' ' && !(expr[pos] == '(' || expr[pos] == ')' || expr[pos] == '+' || expr[pos] == '-' || expr[pos] == '*' || expr[pos] == '/'))
                    pos++;

                return expr.Substring(begin, pos - begin);
            }

            pos++;

            return string.Empty;
        }

        /// <summary>
        /// Finds a close brace
        /// </summary>
        /// <param name="expr"></param>
        /// <param name="pos"></param>
        private static void findCloseBrace(string expr, ref int pos)
        {
            int counter = 1;
            while (pos < expr.Length && counter > 0)
            {
                if (expr[pos] == ')')
                    counter--;
                else if (expr[pos] == '(')
                    counter++;
                pos++;
            }
            if (counter == 0)
                return;

            throw new ApplicationException("Cannot parse expression: close brace was not found.");
        }
        #endregion Calculator's logic
    }
}
