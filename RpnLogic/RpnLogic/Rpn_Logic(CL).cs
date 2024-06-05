using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using static System.Net.Mime.MediaTypeNames;


namespace RpnLogic
{

    public class ExpRpn
    {
        private double value;

        public double Value => value;

        public ExpRpn(string input, double varX)
        {
            var rpnTokens = ConvertToRpn(input,varX);
            EvaluateRpn(rpnTokens);
        }

        private List<Token> ConvertToRpn(string input, double varX)
        {
            List<Token> rpn = new List<Token>();
            Stack<Token> stack = new Stack<Token>();

            for (int i = 0; i < input.Length; i++)
            {
                char currentChar = input[i];

                if (IsDigit(currentChar))
                {
                    string num = currentChar.ToString();

                    while (i + 1 < input.Length && (Char.IsDigit(input[i + 1]) || input[i + 1] == '.'))
                    {
                        num += input[i + 1];
                        i++;
                    }

                    rpn.Add(new Number(num));
                }
                else if(currentChar == 'x')
                {
                    rpn.Add(new Number(varX.ToString()));
                }
                else if (IsOperator(currentChar) || IsParenthesis(currentChar))
                {
                    ProcessOperatorOrParenthesis(currentChar, rpn, stack);
                }
                else if (IsFunction(currentChar, input, i, out int newIndex))
                {
                    string functionName = GetFunctionName(input, i, out newIndex);
                    stack.Push(new Function(functionName));
                    i = newIndex;
                }
            }
            while (stack.Count > 0)
            {
                rpn.Add(stack.Pop());
            }

            return rpn;
        }

        private void ProcessOperatorOrParenthesis(char symbol, List<Token> rpn, Stack<Token> stack)
        {
            if (symbol == ')')
            {
                while (stack.Peek().Symbol != '(')
                {
                    rpn.Add(stack.Pop());
                }
                stack.Pop();
            }
            else
            {
                if (symbol == '(')
                {
                    stack.Push(new LeftParenthesis());
                }
                else
                {
                    while (stack.Count > 0 && stack.Peek().Symbol != '(' && GetPriority(symbol) <= GetPriority(stack.Peek().Symbol))
                    {
                        rpn.Add(stack.Pop());
                    }
                    stack.Push(new Operator(symbol));
                }
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
                    double operand2 = numbers.Pop();
                    double operand1 = numbers.Pop();
                    numbers.Push(operatorToken.Apply(operand1, operand2));
                }
                else if (token is Function functionToken)
                {
                    List<double> operands = new List<double>();

                    for (int i = 0; i < functionToken.FuncOperand; i++)
                    {
                        operands.Insert(0, numbers.Pop());
                    }

                    numbers.Push(functionToken.ApplyFunc(operands.ToArray()));
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

       

        private bool IsDigit(char symbol)
        {
            return Char.IsDigit(symbol) || symbol == '.';
        }

        private bool IsOperator(char symbol)
        {
            return symbol == '+' || symbol == '-' || symbol == '*' || symbol == '/';
        }

        private bool IsParenthesis(char symbol)
        {
            return symbol == '(' || symbol == ')';
        }
        private bool IsFunction(char symbol, string input, int index, out int newIndex)
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
                default:
                    return 0;
            }
        }
       
    }

    class Token
    {
        public char Symbol { get; }

        public Token(char symbol)
        {
            Symbol = symbol;
        }
    }

    class Number : Token
    {
        public Number(string value) : base(value[0]) { Value = double.Parse(value); }

        public double Value { get; }
    }

    class Operator : Token
    {
        public Operator(char symbol) : base(symbol) { }

        public int Priority
        {
            get
            {
                switch (Symbol)
                {
                    case '+':
                    case '-':
                        return 1;
                    case '*':
                    case '/':
                        return 2;
                    default:
                        return 0;
                }
            }
        }
      
        public double Apply(double operand1, double operand2)
        {
            switch (Symbol)
            {
                case '+':
                    return operand1 + operand2;
                case '-':
                    return operand1 - operand2;
                case '*':
                    return operand1 * operand2;
                case '^':
                    return Math.Pow(operand1, operand2);
                case '/':
                    if (operand2 != 0)
                    {
                        return operand1 / operand2;
                    }
                    else
                    {
                        throw new DivideByZeroException("На ноль делить нельзя");
                    }
                default:
                    throw new ArgumentException("Недопустимый оператор " + Symbol);
            }
        }
       

    }
    class Function : Token
    {
        public static readonly List<string> FunctionNames = new List<string>
        {
        "log", "sqrt", "rt", "sin", "cos", "tg", "ctg"
        };

        public Function(string name) : base(name[0]) { Name = name; }

        public string Name { get; }

        public int FuncOperand
        {
            get
            {
                switch (Name)
                {
                    case "log":
                    case "rt":
                        return 2;
                    case "sqrt":
                    case "sin":
                    case "cos":
                    case "tg":
                    case "ctg":
                        return 1;
                    default:
                        throw new ArgumentException("Недопустимая функция " + Name);
                }
            }
        }
        public double ApplyFunc(double[] operands)
        {
            switch (Name)
            {
                case "log":
                    return Math.Log(operands[1], operands[0]);
                case "sqrt":
                    return Math.Sqrt(operands[0]);
                case "rt":
                    return Math.Pow(operands[1], 1.0 / operands[0]);
                case "sin":
                    return Math.Sin(operands[0]);
                case "cos":
                    return Math.Cos(operands[0]);
                case "tg":
                    return Math.Tan(operands[0]);
                case "ctg":
                    return 1.0 / Math.Tan(operands[0]);
                default:
                    throw new ArgumentException("Недопустимая функция " + Name);
            }
        }
    }



        class LeftParenthesis : Token
    {
        public LeftParenthesis() : base('(') { }
    }

    class RightParenthesis : Token
    {
        public RightParenthesis() : base(')') { }
    }

    
}
