using System;
using System.Collections.Generic;
using System.Linq;

namespace Day15
{
    class Program
    {
        static void Main(string[] args)
        {
            // Part1(PROBLEM_INPUT);
            Part2(PROBLEM_INPUT);
            Console.ReadLine();
        }

        static void Part1(string input)
        {
            Helper(input, 2020);
        }

        static void Part2(string input)
        {
            Helper(input, 30000000);
        }

        private static void Helper(string input, long count)
        {
            var startingNumbers = input.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(x => long.Parse(x));

            var counter = 1;
            // For each number keeps the last time it was spoken and the time before that as well.
            var lastTimeNumbersSpoken = new Dictionary<long, long[]>();

            foreach (var num in startingNumbers)
            {
                AddNewNumber(num);
            }

            var lastNumber = startingNumbers.Last();
            var currentNumber = (long)-1;
            do
            {
                var lastTimeSpokenMemory = lastTimeNumbersSpoken[lastNumber];

                if (lastTimeSpokenMemory[1] == -1)
                    currentNumber = 0;
                else
                    currentNumber = lastTimeSpokenMemory[0] - lastTimeSpokenMemory[1];

                Console.WriteLine($"{counter}: {currentNumber}");
                AddNewNumber(currentNumber);
                lastNumber = currentNumber;

            } while (counter <= count);

            void AddNewNumber(long num)
            {
                if (lastTimeNumbersSpoken.TryGetValue(num, out var memory))
                {
                    memory[1] = memory[0];
                    memory[0] = counter++;
                }
                else
                {
                    // -1 Means no second number encountered
                    lastTimeNumbersSpoken[num] = new long[2] { counter++, -1 };
                }
            }
        }

        const string EXAMPLE_INPUT = "0,3,6";
        const string PROBLEM_INPUT = "1,0,18,10,19,6";
    }
}
