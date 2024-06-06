using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RpnLogic
{
    public abstract class Token
    {
       
    }

    public class Number : Token
    {
        public double Value { get; }
        public Number(double value)
        {
            Value = value;
        }

    }

    public abstract class Operator : Token
    {
        public abstract int Priority { get; }
        public abstract int OperandCount { get; }
        public abstract double Apply(params double[] operands);


        public class Plus : Operator
        {
            public override int Priority => 1;
            public override int OperandCount => 2;
            public override double Apply(params double[] operands)
            {
                return operands[0] + operands[1];
            }
        }
        public class Minus : Operator
        {
            public override int Priority => 1;
            public override int OperandCount => 2;
            public override double Apply(params double[] operands)
            {
                return operands[0] - operands[1];
            }
        }
        public class Multiply : Operator
        {
            public override int Priority => 2;
            public override int OperandCount => 2;
            public override double Apply(params double[] operands)
            {
                return operands[0] * operands[1];
            }
        }
        public class Divide : Operator
        {
            public override int Priority => 2;
            public override int OperandCount => 2;

            public override double Apply(params double[] operands)
            {
                if (operands[1] == 0)
                    throw new DivideByZeroException("На ноль делить нельзя");
                return operands[0] / operands[1];
            }
        }
        public class Power : Operator
        {
            public override int Priority => 2;
            public override int OperandCount => 2;
            public override double Apply(params double[] operands)
            {
                return Math.Pow(operands[0], operands[1]);
            }
        }
        public abstract class Function : Token
        {
            public static readonly List<string> FunctionNames = new List<string>
            {
                "log", "sqrt", "rt", "sin", "cos", "tg", "ctg"
            };
            public abstract int Priority { get; }
            public abstract string Name { get; }
            public abstract int OperandCount { get; }
            public abstract double Apply(params double[] operands);


        }
        public class Log : Function
        {
            public override int Priority => 3;
            public override string Name => "log";
            public override int OperandCount => 2;

            public override double Apply(params double[] operands)
            {
                return Math.Log(operands[1], operands[0]);
            }
        }
        public class Rt : Function
        {
            public override int Priority => 3;
            public override string Name => "rt";
            public override int OperandCount => 2;

            public override double Apply(params double[] operands)
            {
                return Math.Pow(operands[1], 1.0 / operands[0]);
            }
        }
        public class Sqrt : Function
        {
            public override int Priority => 3;
            public override string Name => "sqrt";
            public override int OperandCount => 1;

            public override double Apply(params double[] operands)
            {
                return Math.Sqrt(operands[0]);
            }
        }

        public class Sin : Function
        {
            public override int Priority => 3;
            public override string Name => "sin";
            public override int OperandCount => 1;

            public override double Apply(params double[] operands)
            {
                return Math.Sin(operands[0]);
            }
        }

        public class Cos : Function
        {
            public override int Priority => 3;
            public override string Name => "cos";
            public override int OperandCount => 1;

            public override double Apply(params double[] operands)
            {
                return Math.Cos(operands[0]);
            }
        }

        public class Tg : Function
        {
            public override int Priority => 3;
            public override string Name => "tg";
            public override int OperandCount => 1;

            public override double Apply(params double[] operands)
            {
                return Math.Tan(operands[0]);
            }
        }

        public class Ctg : Function
        {
            public override int Priority => 3;
            public override string Name => "ctg";
            public override int OperandCount => 1;

            public override double Apply(params double[] operands)
            {
                return 1.0 / Math.Tan(operands[0]);
            }
        }


        public  class LeftParenthesis : Token
        {

            public LeftParenthesis() { }
        }

        public     class RightParenthesis : Token
        {
            public RightParenthesis() { }
        }
    }
}
