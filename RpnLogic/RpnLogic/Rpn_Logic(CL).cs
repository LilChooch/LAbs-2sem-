using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using static RpnLogic.Operator;
using static System.Net.Mime.MediaTypeNames;


namespace RpnLogic
{

    public class ExpRpn
    {
        private double value;

        public double Value => value;

        public ExpRpn(string input, double varX)
        {
            var rpnTokens = ConvertToRpn(input, varX);
            EvaluateRpn(rpnTokens);
        }

        private List<Token> ConvertToRpn(string input, double varX)
        {
            List<Token> rpn = new List<Token>();
            Stack<Token> stack = new Stack<Token>();

            for (int i = 0; i < input.Length; i++)
            {
                char currentChar = input[i];

                if (Char.IsDigit(currentChar) || currentChar == '.')
                {
                    string num = currentChar.ToString();

                    while (i + 1 < input.Length && (Char.IsDigit(input[i + 1]) || input[i + 1] == '.'))
                    {
                        num += input[i + 1];
                        i++;
                    }

                    rpn.Add(new Number(double.Parse(num)));
                }
                else if (currentChar == 'x')
                {
                    rpn.Add(new Number(varX));
                }
                else if (IsOperator(currentChar))
                {
                    ProcessOperator(currentChar, rpn, stack);
                }
                else if (IsParenthesis(currentChar))
                {
                    ProcessParenthesis(currentChar, rpn, stack);
                }
                else if (IsFunction(input, i, out int newIndex))
                {
                    string functionName = GetFunctionName(input, i, out newIndex);
                    stack.Push(CreateFunction(functionName));
                    i = newIndex;
                }
            }

            while (stack.Count > 0)
            {
                rpn.Add(stack.Pop());
            }

            return rpn;
        }

        private void ProcessOperator(char symbol, List<Token> rpn, Stack<Token> stack)
        {
            while (stack.Count > 0 && stack.Peek() is Operator op && GetPriority(symbol) <= op.Priority)
            {
                rpn.Add(stack.Pop());
            }
            stack.Push(CreateOperator(symbol));
        }

        private void ProcessParenthesis(char symbol, List<Token> rpn, Stack<Token> stack)
        {
            if (symbol == '(')
            {
                stack.Push(new LeftParenthesis());
            }
            else
            {
                while (!(stack.Peek() is LeftParenthesis))
                {
                    rpn.Add(stack.Pop());
                }
                stack.Pop();
            }
        }

        private void EvaluateRpn(List<Token> rpnTokens)
        {
            Stack<double> numbers = new Stack<double>();

            foreach (var token in rpnTokens)
            {
                if (token is Number numberToken)
                {
                    numbers.Push(numberToken.Value);
                }
                else if (token is Operator operatorToken)
                {
                    double[] operands = new double[operatorToken.OperandCount];
                    for (int i = operatorToken.OperandCount - 1; i >= 0; i--)
                    {
                        operands[i] = numbers.Pop();
                    }
                    numbers.Push(operatorToken.Apply(operands));
                }
                else if (token is Function functionToken)
                {
                    double[] operands = new double[functionToken.OperandCount];
                    for (int i = functionToken.OperandCount - 1; i >= 0; i--)
                    {
                        operands[i] = numbers.Pop();
                    }
                    numbers.Push(functionToken.Apply(operands));
                }
            }

            if (numbers.Count == 1)
            {
                value = numbers.Pop();
            }
            else
            {
                throw new InvalidOperationException("Ошибка при вычислении выражения.");
            }
        }

        private bool IsOperator(char symbol)
        {
            return symbol == '+' || symbol == '-' || symbol == '*' || symbol == '/';
        }

        private bool IsParenthesis(char symbol)
        {
            return symbol == '(' || symbol == ')';
        }

        private bool IsFunction(string input, int index, out int newIndex)
        {
            string functionName = GetFunctionName(input, index, out newIndex);
            return Function.FunctionNames.Contains(functionName);
        }

        private string GetFunctionName(string input, int startIndex, out int newIndex)
        {
            newIndex = startIndex;
            while (newIndex + 1 < input.Length && char.IsLetter(input[newIndex + 1]))
            {
                newIndex++;
            }
            return input.Substring(startIndex, newIndex - startIndex + 1);
        }

        private int GetPriority(char symbol)
        {
            switch (symbol)
            {
                case '+':
                case '-':
                case '^':
                    return 1;
                case '*':
                case '/':
                    return 2;
                case 's':
                case 'c':
                case 't':
                    return 3;
                default:
                    return 0;
            }
        }

        private Operator CreateOperator(char symbol)
        {
            switch (symbol)
            {
                case '+':
                    return new Plus();
                case '-':
                    return new Minus();
                case '*':
                    return new Multiply();
                case '/':
                    return new Divide();
                default:
                    throw new ArgumentException("Недопустимый оператор " + symbol);
            }
        }

        private Function CreateFunction(string name)
        {
            switch (name)
            {
                case "log":
                    return new Log();
                case "sqrt":
                    return new Sqrt();
                case "sin":
                    return new Sin();
                case "cos":
                    return new Cos();
                case "tg":
                    return new Tg();
                case "ctg":
                    return new Ctg();
                default:
                    throw new ArgumentException("Недопустимая функция " + name);
            }
        }
    }

}
