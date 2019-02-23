using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TwentyFour
{
    class Program
    {
        public const bool NegativesAllowed = false;
        static void Main(string[] args)
        {
            
            using (System.IO.StreamWriter file =
                new System.IO.StreamWriter(@".\24.txt"))
            {
                int countWins = 0;
                int countFails = 0;
                for (var a = 0; a < 10; a++)
                for (var b = a; b < 10; b++)
                for (var c = b; c < 10; c++)
                for (var d = c; d < 10; d++)
                {
                    var result = Get24Result(a, b, c, d);
                    file.WriteLine($"{a} {b} {c} {d}");
                    file.WriteLine(result.Result);
                    if (result.IsWinner) countWins++;
                    else countFails++;
                    file.WriteLine();
                }

                file.WriteLine("\r\n\r\n");
                file.WriteLine("Total Combos With a Victory: "+countWins);
                file.WriteLine("Total Combos Without a Victory: " + countFails);
                var percentage = (decimal) countWins / (countWins + countFails);
                file.WriteLine("Percent of Winners: "+percentage*100+"%");
            }

            using (System.IO.StreamWriter file =
                new System.IO.StreamWriter(@".\24winners.txt"))
            {

                for (var a = 0; a < 10; a++)
                for (var b = a; b < 10; b++)
                for (var c = b; c < 10; c++)
                for (var d = c; d < 10; d++)
                {
                    var result = Get24Result(a, b, c, d);
                    if(!result.IsWinner) continue;                    
                    file.WriteLine($"{a} {b} {c} {d}");
                    file.WriteLine(result.Result);
                    file.WriteLine();
                }
            }

            using (System.IO.StreamWriter file =
                new System.IO.StreamWriter(@".\24summary.txt"))
            {
                for (var a = 0; a < 10; a++)
                for (var b = a; b < 10; b++)
                for (var c = b; c < 10; c++)
                for (var d = c; d < 10; d++)
                {
                    var result = Get24Result(a, b, c, d);
                    var character = result.IsWinner ? "✓" : string.Empty;
                    file.WriteLine($"{a} {b} {c} {d} {character}");                    
                }
            }

            //Console.ReadLine();
        }

        public static Result24 Get24Result(int a, int b, int c, int d)
        {
            var nums = new[] {a, b, c, d};
            for (var x = 0; x < 4; x++)
            {
                for (var y = x+1; y < 4; y++)
                {
                    for (var op = 0; op < 4; op++)
                    {
                        for (int flip = 0; flip < 2; flip++)
                        {
                            int i = flip == 0 ? nums[x] : nums[y];
                            int j = flip == 0 ? nums[y] : nums[x];
                            var operation = (Operation) op;
                            if (!CanDoOp(i, j, operation))
                            {
                                continue;
                            }

                            int k = GetFirstOtherFromFour(nums, x, y);
                            int l = GetSecondOtherFromFour(nums, x, y);
                            var result = Op(i, j, operation);
                            if (result < 0 && !NegativesAllowed)
                            {
                                continue;
                            }
                            var nextResult = Get24Result(k, l, result);
                            if (nextResult.IsWinner)
                            {
                                nextResult.Result = GetStepString(i, j, operation, result) + "\r\n" + nextResult.Result;
                                return nextResult;
                            }
                        }
                    }
                }
            }
            return new Result24 { IsWinner = false, Result = $"No winning combos for {a} {b} {c} {d}" };
        }

        public static Result24 Get24Result(int a, int b, int c)
        {
            var nums = new[] { a, b, c };
            for (var x = 0; x < 3; x++)
            {
                for (var y = x+1; y < 3; y++)
                {
                    for (var op = 0; op < 4; op++)
                    {
                        for (int flip = 0; flip < 2; flip++)
                        {
                            int i = flip == 0 ? nums[x] : nums[y];
                            int j = flip == 0 ? nums[y] : nums[x];
                            var operation = (Operation) op;
                            if (!CanDoOp(i, j, operation))
                            {
                                continue;
                            }

                            int k = GetOtherFromThree(nums, x, y);
                            var result = Op(i, j, operation);
                            if (result < 0 && !NegativesAllowed)
                            {
                                continue;
                            }
                            var nextResult = Get24Result(k, result);
                            if (nextResult.IsWinner)
                            {
                                nextResult.Result = GetStepString(i, j, operation, result) + "\r\n" + nextResult.Result;
                                return nextResult;
                            }
                        }
                    }
                }
            }
            return new Result24{IsWinner = false};
        }

        public static Result24 Get24Result(int a, int b)
        {
            for (var op = 0; op < 4; op++)
            {
                for (int flip = 0; flip < 2; flip++)
                {
                    int i = flip == 0 ? a : b;
                    int j = flip == 0 ? b : a;
                    var operation = (Operation) op;
                    if (!CanDoOp(a, b, operation))
                    {
                        continue;
                    }

                    var result = Op(a, b, operation);

                    if (result == 24)
                    {
                        return new Result24 {IsWinner = true, Result = GetStepString(a, b, operation, result)};
                    }
                }
            }
            return new Result24 { IsWinner = false };
        }

        public static string GetStepString(int a, int b, Operation op, int result)
        {
            return $"{a} {OpString(op)} {b} = {result}";
        }

        public static string OpString(Operation op)
        {
            switch (op)
            {
                case Operation.Addition:
                {
                    return "+";
                }
                case Operation.Subtraction:
                {
                    return "-";
                }
                case Operation.Multiplication:
                {
                    return "×";
                }
                case Operation.Division:
                {
                    return "÷";
                }
            }
            throw new Exception("Invalid operation for OpString");
        }

        public static int GetFirstOtherFromFour(int[] nums, int ind1, int ind2)
        {
            for (int i = 0; i < 4; i++)
            {
                if (ind1 != i && ind2 != i)
                {
                    return nums[i];
                }
            }
            throw new Exception("Couldn't find first other index");
        }

        public static int GetSecondOtherFromFour(int[] nums, int ind1, int ind2)
        {
            for (int i = 3; i >= 0; i--)
            {
                if (ind1 != i && ind2 != i)
                {
                    return nums[i];
                }
            }
            throw new Exception("Couldn't find first other index");
        }

        public static int GetOtherFromThree(int[] nums, int ind1, int ind2)
        {
            for (int i = 0; i < 3; i++)
            {
                if (ind1 != i && ind2 != i)
                {
                    return nums[i];
                }
            }
            throw new Exception("Couldn't find first other index");
        }

        public static bool CanDoOp(int a, int b, Operation op)
        {
            if (op != Operation.Division) return true;
            return b != 0 && a % b == 0;
        }

        public static int Op(int a, int b, Operation op)
        {
            switch (op)
            {
                case Operation.Addition:
                {
                    return a + b;
                }
                case Operation.Subtraction:
                {
                    return a - b;
                }
                case Operation.Multiplication:
                {
                    return a * b;
                }
                case Operation.Division:
                {
                    return a / b;
                }
            }
            throw new Exception("Invalid operation");
        }
    }

    enum Operation
    {
        Addition=0, Subtraction=1, Multiplication=2, Division=3
    }

    class Result24
    {
        public bool IsWinner { get; set; }

        public string Result { get; set; }
    }
}
