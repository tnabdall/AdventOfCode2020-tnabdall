using System;
using System.Collections.Generic;
using System.Linq;

namespace Day13
{
    class Program
    {
        static void Main(string[] args)
        {
            Part1(PROBLEM_INPUT);
            Part2(PROBLEM_INPUT);
            Console.ReadLine();
        }

        private static void Part1(string input)
        {
            var lines = input.Split('\n', StringSplitOptions.RemoveEmptyEntries);
            var timeMin = long.Parse(lines[0]);
            var availableBuses = lines[1].Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Where(e => e != "x").Select(e => long.Parse(e));
            var waitTimePerBus = availableBuses.Select(e => new { BusId = e, WaitTime = (timeMin % e) == 0 ? 0 : (e - (timeMin % e)) });
            var closestBus = waitTimePerBus.OrderBy(e => e.WaitTime).First();
            Console.WriteLine(closestBus.WaitTime * closestBus.BusId);
        }

        private static void Part2(string input)
        {
            var line = input.Split('\n', StringSplitOptions.RemoveEmptyEntries)[1];
            var buses = line.Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(e => {
                    if (long.TryParse(e, out long longE))
                        return longE;
                    else
                        return -1; // Its an x means no required bus
                }).ToArray();
             
            List<(long Bus, long Offset)> busesAndOffsets = new List<(long Bus, long Offset)>();
            for (int i = 0; i < buses.Length; i++)
            {
                if (buses[i] != -1)
                    busesAndOffsets.Add((buses[i], i));
            }

            long currentTime = 0;
            long increment = busesAndOffsets[0].Bus;
            busesAndOffsets.RemoveAt(0);

            while (busesAndOffsets.Count > 0)
            {
                currentTime += increment;

                List<int> indicesToRemove = new List<int>();
                for (int i = 0; i < busesAndOffsets.Count; i++)
                {
                    if ((currentTime + busesAndOffsets[i].Offset) % busesAndOffsets[i].Bus == 0)
                    {
                        increment *= busesAndOffsets[i].Bus;
                        indicesToRemove.Add(i);
                    }
                }

                foreach (var index in indicesToRemove.OrderByDescending(e => e))
                {
                    busesAndOffsets.RemoveAt(index);
                }
            }

            Console.WriteLine(currentTime);           

        }

        const string PROBLEM_INPUT = @"1005162
19,x,x,x,x,x,x,x,x,41,x,x,x,x,x,x,x,x,x,823,x,x,x,x,x,x,x,23,x,x,x,x,x,x,x,x,17,x,x,x,x,x,x,x,x,x,x,x,29,x,443,x,x,x,x,x,37,x,x,x,x,x,x,13";

        const string EXAMPLE_INPUT = @"939
7,13,x,x,59,x,31,19";
    }
}
