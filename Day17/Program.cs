using System;
using System.Collections.Generic;
using System.Linq;

namespace Day17
{
    class Program
    {
        static void Main(string[] args)
        {
            Part1(PROBLEM_INPUT);
            Console.ReadLine();
        }

        private static void Part1(string input, int cycles = 6)
        {
            var nodeGroup = new NodeGroup(input);
            for (int i = 0; i < cycles; i++)
            {
                nodeGroup.RunCycle();
            }

            var activeCount = nodeGroup.NodeStates.Values.Count(e => e);
            Console.WriteLine(activeCount);
        }

        class NodeGroup
        {
            public Dictionary<(long X, long Y, long Z), bool> NodeStates { get; }

            public long XMin { get; private set; }
            public long XMax { get; private set; }
            public long YMin { get; private set; }
            public long YMax { get; private set; }
            public long ZMin { get; private set; }
            public long ZMax { get; private set; }

            public void RunCycle()
            {
                // Expand once in each direction (for the neighbors)
                for (long i = XMin-1; i <= XMax + 1; i++)
                {
                    for (long j = YMin -1; j <= YMax + 1; j++)
                    {
                        for (long k = ZMin - 1; k<=ZMax +1; k++)
                        {
                            // New node
                            if (i < XMin || i > XMax || j < YMin || j > YMax || k < ZMin || k > ZMax)
                            {
                                NodeStates[(i, j, k)] = false;
                            }
                        }
                    }
                }
                // Update ranges
                XMin--; XMax++;
                YMin--; YMax++;
                ZMin--; ZMax++;
                var statesToChange = new Stack<(long X, long Y, long Z)>();
                // Loop thorugh all nodes
                for (long i = XMin; i<= XMax; i++)
                {
                    for (long j = YMin; j<= YMax; j++)
                    {
                        for (long k = ZMin; k<= ZMax; k++)
                        {
                            var countActiveNeighbours = 0;
                            // Count up active neighbors
                            for (long i2 = i -1; i2 <= i + 1; i2++)
                            {
                                for (long j2 = j - 1; j2 <= j + 1; j2++)
                                {
                                    for (long k2 = k - 1; k2 <= k + 1; k2++)
                                    {
                                        if ((i == i2) && (j == j2) && (k == k2))
                                            continue;
                                        countActiveNeighbours += IsActive(i2, j2, k2) ? 1: 0;
                                    }
                                }
                            }
                            // Apply switching criteria
                            var nodeActive = IsActive(i, j, k);
                            if (nodeActive && !(countActiveNeighbours == 2 || countActiveNeighbours == 3))
                            {
                                statesToChange.Push((i, j, k));
                            }
                            else if (!nodeActive && countActiveNeighbours == 3)
                            {
                                statesToChange.Push((i, j, k));
                            }
                        }
                    }
                }

                while (statesToChange.Count > 0)
                {
                    var nextNode = statesToChange.Pop();
                    NodeStates[nextNode] = !NodeStates[nextNode];
                }
            }

            private bool IsActive(long x, long y, long z)
            {
                if (x < XMin || x > XMax || y < YMin || y > YMax || z < ZMin || z > ZMax)
                    return false;
                return NodeStates[(x, y, z)];                
            }

            public NodeGroup(string input)
            {
                NodeStates = new Dictionary<(long X, long Y, long Z), bool>();
                var rows = input.Split('\n', StringSplitOptions.RemoveEmptyEntries);
                XMin = 0; YMin = 0; ZMin = 0;
                XMax = rows.First().Trim().Length -1;
                YMax = rows.Length -1;
                ZMax = 0;
                for(long j = YMin; j <= YMax; j++)
                {
                    var row = rows[j];
                    for (long i = XMin; i <= XMax; i++)
                    {
                        if (row[(int)i] == '#')
                        {
                            NodeStates[(i, j, 0)] = true;
                        }
                        else if(row[(int)i] == '.')
                        {
                            NodeStates[(i, j, 0)] = false;
                        }
                        else
                        {
                            throw new Exception("Unknown node state");
                        }
                    }
                }
            }
        }

        const string EXAMPLE_INPUT = @".#.
..#
###";

        const string PROBLEM_INPUT = @"#......#
##.#..#.
#.#.###.
.##.....
.##.#...
##.#....
#####.#.
##.#.###";
    }
}
