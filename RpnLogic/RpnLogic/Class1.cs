using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using static System.Net.Mime.MediaTypeNames;

namespace RpnLogic
{
    public class ExpRpn
    {
        private double value;
        public ExpRpn(string input)
        {
            
            var rpnTokens = ConvertToRpn(input);
            EvaluateRpn(rpnTokens);
           

        }


        private List<Token> ConvertToRpn(string input)
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
                else if (IsOperator(currentChar) || IsParenthesis(currentChar))
                {
                    ProcessOperatorOrParenthesis(currentChar, rpn, stack);
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
                while (stack.Count > 0 && stack.Peek().Symbol != '(' && GetPriority(symbol) <= GetPriority(stack.Peek().Symbol))
                {
                    rpn.Add(stack.Pop());
                }
                stack.Push(new Operator(symbol));
            }
        }

        private void EvaluateRpn(List<Token> rpnTokens)
        {
            Stack<double> numbers = new Stack<double>();
            Stack<Token> operations = new Stack<Token>();

            foreach (var token in rpnTokens)
            {
                if (token is Number numberToken)
                {
                    numbers.Push(numberToken.Value);
                }
                else if (token is Operator operatorToken)
                {
                    while (operations.Count > 0 && operations.Peek() is Operator && operatorToken.Priority <= ((Operator)operations.Peek()).Priority)
                    {
                        numbers.Push(((Operator)operations.Pop()).Apply(numbers.Pop(), numbers.Pop()));
                    }
                    operations.Push(operatorToken);
                }
            }

            while (operations.Count > 0)
            {
                numbers.Push(((Operator)operations.Pop()).Apply(numbers.Pop(), numbers.Pop()));
            }

            if (numbers.Count == 1)
            {
                value = numbers.Pop();
            }
            else
            {
                throw new InvalidOperationException("Ошибка");
            }
        }

        public double Value => value;

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

        private int GetPriority(char symbol)
        {
            switch (symbol)
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
                    return operand2 + operand1;
                case '-':
                    return operand2 - operand1;
                case '*':
                    return operand2 * operand1;
                case '/':
                    if (operand1 != 0)
                    {
                        return operand2 / operand1;
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

    class LeftParenthesis : Token
    {
        public LeftParenthesis() : base('(') { }
    }

    class RightParenthesis : Token
    {
        public RightParenthesis() : base(')') { }
    }

    class RpnCalculate
    {
        
        public static List<object> ConvertToRpn(string str)
        {
            Dictionary<object, int> prioretyDictionary = new Dictionary<object, int>
            {
                {'+', 1},
                {'-', 1},
                {'*', 2},
                {'/', 2},
                {'(', 0},
                {')', 5},
            };
            List<object> prn = new List<object>();
            Stack<object> stack = new Stack<object>();
            string num = string.Empty;
            for (int i = 0; i < str.Length; i++)
            {
                if (prioretyDictionary.ContainsKey(str[i]))
                {
                    if (num != string.Empty)
                    {
                        prn.Add(num);
                        num = string.Empty;
                    }

                    if (str[i] == ')')
                    {
                        while ((Char)stack.Peek() != '(')
                        {
                            prn.Add(stack.Pop());
                        }
                        stack.Pop();
                    }
                    else if (stack.Count == 0
                        || str[i] == '('
                        || prioretyDictionary[str[i]] > prioretyDictionary[stack.Peek()])
                    {
                        stack.Push(str[i]);
                    }
                    else if (prioretyDictionary[str[i]] <= prioretyDictionary[stack.Peek()])
                    {
                        while (stack.Count > 0 && (Char)stack.Peek() != '(')
                        {
                            prn.Add(stack.Pop());
                        }
                        stack.Push(str[i]);
                    }
                }
                else
                {
                    num += str[i];
                }
            }
            prn.Add(num);
            while (stack.Count > 0)
            {
                prn.Add(stack.Pop());
            }
            return prn;
        }
    }
}
