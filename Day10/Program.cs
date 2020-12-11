using System;
using System.Collections.Generic;
using System.Linq;

namespace Day10
{
    class Program
    {
        static void Main(string[] args)
        {
            FirstPartSolution();

            SecondPartSolution();

            Console.ReadLine();
        }

        private static void FirstPartSolution()
        {
            IList<long> inputList = ConvertStringToListOfLong(PROBLEM_INPUT, new char[] { '\n' }).OrderBy(e => e).ToList(); // sorted

            Dictionary<int, int> countByDifferenceDistribution = new Dictionary<int, int>();

            // One added for the first adapter, and one added for the last adapter (which is 3 over)
            AddAndCreateIfKeyNotFound(countByDifferenceDistribution, (int)inputList[0], 1);
            AddAndCreateIfKeyNotFound(countByDifferenceDistribution, 3, 1);

            for (int i = 1; i < inputList.Count; i++)
            {
                int difference = (int)(inputList[i] - inputList[i - 1]);
                if (difference > 4)
                    throw new ArgumentException("Cant connect these 2 adapters");
                AddAndCreateIfKeyNotFound(countByDifferenceDistribution, difference, 1);
            }

            foreach (KeyValuePair<int, int> differenceCounts in countByDifferenceDistribution)
            {
                Console.WriteLine($"Found {differenceCounts.Value} occurrences of {differenceCounts.Key} differences");
            }
        }

        private static void SecondPartSolution()
        {
            IList<long> inputList = ConvertStringToListOfLong(PROBLEM_INPUT, new char[] { '\n' }).OrderBy(e => e).ToList(); // sorted
            // Add a node at 0 (don't need one at the end since thats always max + 3)

            IList<Node> sortedNodes = new Node[] { new Node() { Number = 0 } }.Concat(inputList.Select(e => new Node { Number = e })).ToList();

            // Determine what nodes each can connect to (going forwards)
            for (int i = 0; i < sortedNodes.Count - 1; i++) 
            {
                List<Node> forwardNodes = new List<Node>();
                // Only need to look at up to 3 nodes ahead (assuming none are repeated)
                for (int j = i + 1; j < Math.Min(i + 4, sortedNodes.Count); j++)
                {
                    if ((sortedNodes[j].Number - sortedNodes[i].Number) < 4)
                        forwardNodes.Add(sortedNodes[j]);
                }
                sortedNodes[i].ForwardNodes = forwardNodes;
            }
            sortedNodes[sortedNodes.Count - 1].ForwardNodes = new List<Node>(); // Just so its not null

            // Now, that we've built our list, call the get distinct paths method on the first node
            Console.WriteLine($"Distinct paths found {sortedNodes[0].GetDistinctPaths()}");

        }



        class Node
        {
            static Dictionary<long, long> CountedPathsMemo = new Dictionary<long, long>();

            public long Number;
            public IEnumerable<Node> ForwardNodes; // Forward nodes, eg. If this was 16, then 17, 18, and 19 could be here if available

            public long GetDistinctPaths()
            {
                if (CountedPathsMemo.TryGetValue(Number, out long paths))
                    return paths;

                long countedPaths;
                if (ForwardNodes.Count() == 0)
                    countedPaths = 1;
                else
                    countedPaths = ForwardNodes.Sum(e => e.GetDistinctPaths());

                CountedPathsMemo[Number] = countedPaths;
                return countedPaths;
            }
        }

        static void AddAndCreateIfKeyNotFound<TKey>(Dictionary<TKey, int> dict, TKey key, int value)
        {
            if (!dict.ContainsKey(key))
                dict[key] = 0;
            dict[key] += value;
        }

        static IList<long> ConvertStringToListOfLong(string input, char[] separators)
        {
            return input
                .Split(separators)
                .Select(e =>
                {
                    if (long.TryParse(e, out long converted))
                        return (long?)converted; // Necessary so that we can return nulls for failed parses
                    else
                        return null;
                })
                .Where(e => e != null)
                .Select(e => (long)e)
                .ToList();
        }

        const string EXAMPLE_INPUT = @"28
33
18
42
31
14
46
20
48
47
24
23
49
45
19
38
39
11
1
32
25
35
8
17
7
9
4
2
34
10
3";

        const string PROBLEM_INPUT = @"59
134
159
125
95
92
169
43
154
46
110
79
117
151
141
56
87
10
65
170
89
32
40
118
36
94
124
173
164
166
113
67
76
102
107
52
144
119
2
72
86
73
66
13
15
38
47
109
103
128
165
148
116
146
18
135
68
83
133
171
145
48
31
106
161
6
21
22
77
172
28
78
96
55
132
39
100
108
33
23
54
157
80
153
9
62
26
147
1
27
131
88
138
93
14
123
122
158
152
71
49
101
37
99
160
53
3
";
    }
}
