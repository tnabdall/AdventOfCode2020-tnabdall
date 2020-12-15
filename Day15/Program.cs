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
            var lastTimeNumbersSpoken = new Dictionary<long, long>();

            foreach (var num in startingNumbers.Take(startingNumbers.Count()-1))
            {
                lastTimeNumbersSpoken[num] = counter++;
            }

            var lastNumber = startingNumbers.Last();
            var delayedCommitToMemory = ((long)lastNumber, (long)counter++);          

            var currentNumber = (long)-1;
            do
            {
                if (lastTimeNumbersSpoken.TryGetValue(lastNumber, out var mem))
                {
                    currentNumber = counter - 1 - mem;
                }
                else
                {
                    currentNumber = 0;
                }

                lastTimeNumbersSpoken[delayedCommitToMemory.Item1] = delayedCommitToMemory.Item2;
                // Note to self: This adds so much time per loop
                //Console.WriteLine($"{counter}: {currentNumber}");
                lastNumber = currentNumber;
                delayedCommitToMemory = (lastNumber, counter++);

            } while (counter <= count);
            lastTimeNumbersSpoken[delayedCommitToMemory.Item1] = delayedCommitToMemory.Item2;

            Console.WriteLine($"{delayedCommitToMemory.Item2}: {delayedCommitToMemory.Item1}");

        }

        const string EXAMPLE_INPUT = "0,3,6";
        const string PROBLEM_INPUT = "1,0,18,10,19,6";
    }
}
