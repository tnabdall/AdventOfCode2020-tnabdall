﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace Day19
{
    class Program
    {
        static void Main(string[] args)
        {
            var rules = GetRuleSet(PROBLEM_RULES);
            var firstRule = rules.First();
            PrintMatches(PROBLEM_INPUT, firstRule);
            Console.ReadLine();
        }

        static void PrintMatches(string input, Rule rule)
        {
            var matchSet = new HashSet<string>(rule.MatchStr);
            var count = input.Split('\n', StringSplitOptions.RemoveEmptyEntries).Select(e => e.Trim()).Count(e => matchSet.Contains(e));
            Console.WriteLine(count);

        }

        static IEnumerable<Rule> GetRuleSet(string input)
        {
            var lines = input.Split('\n', StringSplitOptions.RemoveEmptyEntries);
            var rules = lines.Select(x =>
            {
                var parts = x.Split(":", StringSplitOptions.RemoveEmptyEntries);
                var idx = int.Parse(parts[0]);
                var ruleStr = parts[1];
                return new Rule() { Index = idx, RuleStr = ruleStr };
            }).OrderBy(e => e.Index).ToList();
            rules.ForEach(e => e.RuleSet = rules.ToArray());
            return rules;
        }

        class Rule
        {
            public Rule[] RuleSet { get; set; }
            public int Index { get; set; }
            // String to parse rule to get the match string
            public string RuleStr { get; set; }
            public string[] matchStr = null;
            public string[] MatchStr => matchStr ?? ResolveRuleStr();

            private string[] ResolveRuleStr()
            {
                var str = RuleStr.Split('|', StringSplitOptions.RemoveEmptyEntries);
                if (str.Length == 1)
                {
                    var strTrim = str[0].Trim();
                    if (strTrim.StartsWith("\"") && strTrim.EndsWith("\""))
                    {
                        matchStr = new string[] { strTrim.Replace("\"", "") };
                        return matchStr;
                    }
                }

                List<string> allStrMatches = new List<string>();
                foreach(var set in str)
                {
                    var indices = set.Split(" ", StringSplitOptions.RemoveEmptyEntries);
                    var matches = indices.Select(e => RuleSet[int.Parse(e)].MatchStr).ToList();
                    allStrMatches.AddRange(GetAllMatches(matches, null));
                }

                matchStr = allStrMatches.ToArray();

                return matchStr;
            }

            private static IEnumerable<string> GetAllMatches(IEnumerable<string[]> input, string[] head)
            {
                
                if (head == null)
                {
                    head = input.First();
                    input = input.Skip(1).ToList();
                }                

                if (input.Count() == 0)
                {
                    return head;
                }

                var newHead = new List<string>();

                foreach (var h in head)
                {
                    foreach (var firstStr in input.First())
                    {
                        newHead.Add(h + firstStr);
                    }                   
                }

                return GetAllMatches(input.Skip(1), newHead.ToArray());                
            }
        }


        const string PROBLEM_RULES = @"29: 116 82 | 119 24
45: 116 69 | 119 124
24: 119 116
79: 102 119 | 117 116
52: 94 119 | 25 116
116: ""a""
60: 48 116 | 27 119
39: 119 41 | 116 132
14: 119 119
80: 116 116 | 119 134
46: 119 14 | 116 61
129: 30 116 | 61 119
133: 119 2 | 116 122
68: 119 3 | 116 33
105: 119 47 | 116 104
107: 62 119 | 50 116
76: 119 100 | 116 127
125: 119 111 | 116 86
74: 24 119 | 61 116
128: 119 76 | 116 99
59: 87 119 | 7 116
88: 62 116 | 50 119
103: 119 97 | 116 118
17: 116 119 | 119 116
32: 116 134 | 119 116
56: 119 106 | 116 60
71: 115 119 | 10 116
4: 119 118 | 116 61
132: 119 17 | 116 97
12: 67 116 | 78 119
70: 119 24 | 116 30
83: 17 116 | 30 119
90: 116 105 | 119 12
84: 119 87 | 116 14
134: 116 | 119
119: ""b""
49: 4 119 | 132 116
50: 134 119 | 119 116
99: 119 21 | 116 46
48: 61 119 | 32 116
54: 119 85 | 116 58
122: 116 7 | 119 82
53: 123 119 | 91 116
75: 43 119 | 79 116
66: 116 52 | 119 93
13: 97 119 | 14 116
23: 82 119 | 7 116
95: 90 116 | 28 119
98: 119 32 | 116 50
118: 134 134
2: 116 97 | 119 82
62: 119 116 | 119 119
89: 73 116 | 108 119
96: 24 116 | 17 119
131: 116 110 | 119 15
123: 64 116 | 98 119
78: 81 116 | 15 119
0: 8 11
87: 116 119
18: 116 24
41: 119 62 | 116 30
28: 9 116 | 128 119
33: 24 116 | 97 119
58: 107 116 | 84 119
20: 116 130 | 119 45
109: 116 5 | 119 112
55: 118 116 | 17 119
11: 42 31
30: 116 119 | 116 116
104: 36 116 | 57 119
67: 101 119 | 29 116
106: 46 119 | 38 116
10: 49 119 | 44 116
121: 116 103 | 119 96
111: 116 97 | 119 50
92: 116 61 | 119 80
35: 116 37 | 119 107
26: 119 32 | 116 62
64: 119 118 | 116 97
86: 17 116 | 82 119
130: 119 19 | 116 66
25: 116 50
110: 87 116 | 118 119
61: 116 116
1: 116 56 | 119 65
40: 116 118 | 119 61
22: 116 17 | 119 24
63: 119 68 | 116 6
94: 119 24 | 116 14
100: 119 24 | 116 80
3: 14 119 | 30 116
81: 24 119 | 80 116
126: 30 116 | 17 119
7: 116 116 | 119 116
44: 40 119 | 92 116
16: 116 18 | 119 88
102: 116 17 | 119 7
51: 1 119 | 71 116
5: 119 54 | 116 53
124: 119 72 | 116 39
108: 50 119 | 118 116
65: 119 77 | 116 16
114: 116 61 | 119 62
15: 118 119 | 50 116
19: 116 133 | 119 121
21: 50 119 | 61 116
101: 82 119 | 24 116
31: 95 119 | 51 116
115: 34 119 | 131 116
117: 7 119 | 24 116
27: 17 119 | 32 116
6: 119 120 | 116 127
85: 116 55 | 119 94
93: 119 64 | 116 74
69: 116 125 | 119 113
47: 70 119 | 114 116
42: 109 119 | 20 116
72: 119 114 | 116 64
82: 134 119 | 116 116
9: 35 119 | 89 116
112: 116 75 | 119 63
36: 119 32 | 116 87
57: 119 87 | 116 50
38: 119 24 | 116 61
77: 22 119 | 13 116
91: 73 116 | 92 119
8: 42
113: 59 116 | 129 119
97: 116 119 | 119 119
73: 97 119 | 30 116
37: 62 119 | 24 116
120: 119 61 | 116 61
34: 126 119 | 83 116
127: 116 30 | 119 14
43: 23 116 | 26 119";

        const string PROBLEM_INPUT = @"babaaaabbbaaaabbbaaaaaba
bababbaababaabbbabbaabaa
aabbbbaaaaaabbbbaababaab
bbaabbaabbaaaaabbbbbababbbaaabbababaaaababbbbaab
baaaabaabaabbbbbbbbbaaba
bbaabbaabbbbbbbbabbbabbbbbbabbbbaaabaabb
babbaaaaaabaaaaabaabbabbbaabaaaababbbaaa
aababbbbbbbbaaaabbabbaabaabbbaaaaababbaa
bbbbbbbbbbbbaaababbaabbbbbbbbaaa
bbbabbbbbbbabbaaaaaaabababaaaaab
baababbbabbbabbbbaaabbbb
baabbbbaaaabaabbabbbbbaa
bbbabaabbaaaabbabaababaa
aaaabaaaabababbbaabaabbbababaaaa
baababaabbaababbbbbbaabb
abababbababbababbbbabaaabbaababb
abbaababaaaabaaabaaaaaab
ababababababababbabaababbaabaabababbabbbbbabbabbbaabbaab
aabbababbaaaaabaabaaaabb
bbaaaaabaabbaabbaababaaa
aaabbaaaaababbabaababaaabbbbabbababbbbbb
bbbbbbbbabbababbabaaaaab
bbabbaabbbbababbbabbbabaaabaaaabbabbbbaabbbababb
aaabbaabbaaaabbabaabbbbaababababbabbbaabbaaaaaaaababbbaa
baaabbbabaaaabaabaababbbbbaaaaabbaaaabab
bbbbaaabbbbabbaaaabbabbaabbaaaabaaaaabaaaabaaaab
ababaaabababaaabbbaaaabbbaaabbbabbbbaaba
bbabbbbaaabaabbbbaaabbbabbaaaababbbbababbbaababb
aaabbabbabbbabbbaabbbaabbbabbbaaaaabaaba
baabbbbabaaaabaababbaaaabbbaababaaabaaaa
ababbbbbaabbbabbbabaaabaaabbabbaaababbbbaaabaabbaaaababa
aaaaabababbaababaaaaaabb
abbabbabaabbbbabbabaaaaaababaaababbababaabababbbabababaa
abaabbbbbbbbbbaababbabbbbababaababaaaaba
abaabaabbbaaaababaaaabbb
aabbaababbbaabbbabbaabaaabaabbabbbaabbba
baabbbaaabbbbbababbabbbaaaabaabb
aabbbbabbaabbabbababbababaabaaaaabbbaaab
aaababbbbabbbabbaabbbabbaabababbabbbaaaabaababaa
abbaaaaaabbaabababaabababaaaabab
bbabbbaabaabbabaaabababbbaaaabaaababbbbbababbbaababbbaba
babaabbaababbabaababbbababbaabbbaaaababbabbaabbabbaaaaaa
abaabaaababaaababbbbbabaabbaaaaabbbabbbaabbbaabaaabaabba
abaabaaabaaabbababaaaaab
aaaabbbaabbbabbbbaababababababbbabaabababaaaabbababbbaba
bbbabaaabbbbaaaaaaabbaaa
abaabbaaabbbabaabbbaaaab
aabababbabaabaabbbbbbbbbbbabbaba
baabbababaabbabbaabaabba
aabababbaaabbababaaabbabbabbabba
abbabbbabaaaaabababaaabbbbbaababbabbbbbb
baabbbbaabbaabaabaaabbbaabbabbbbabbbbabb
aabbbabbababbaaaaabbabbaaaabaaab
ababbaaabbbabbbaabaaaaaababbbbbb
bbbabaabababbbabbbbbababbbababbb
aabbbaabaabaaaaabbabbaaa
aabaabbbbbaabbaabbaabaab
bbabbbaaababbabaaaaababa
bbbabbaabbabbbaabbababba
bbbaaabaabababbbbbbbbbbaaaabbabbaababaaaaababaaaaababaaa
abababbbbbbabbbbbaabbbab
bbaaaaabaaaaabababbbbbba
bbabaaaabbbbbbbbaaabbabaaabbbbbbabbbabaa
aabbbbaaabaaabaaaababaaa
ababababaabbbabaaababaab
abbaaaaabababbaabbbaabbaaaabbbba
bbbbababaaaabbaabbbbbaba
ababbaabbbabbaabbbbbaaababbaaaabaabbabbaababbbbabaabaaab
aabbbbaaaaaabbbbbbbbbbab
aaaabaaaaaabbbbbbbbabbab
babaaabbabbababbbbbababb
abaabbbabbaaabbaaabbbbbb
baaabbbabbbbaaaabbababbb
abbaaaababbaabbbaaaaaaabaaaaaaabbbbbabababbbaaaa
bbbaabaaaabbbbbbabbbaaabbaaaaaaa
abbabaabbaaabaababaabaabbbbbaabb
aaababbbabbaabaaabababbabbabaaabbbaabbba
babaabbababaaabbabaabaaaabbbabaa
bbaaabbabaaaabbaaaaaaaabbabbaaaaababaabb
ababaabaababbbabababbaaaaaababba
baabbbbaaabbababbaaababbbabbabbbbbaabaaa
babbaaaaabbaaaababbbbbba
bbbabaabaababbbbabaabaababaabaabbaabbaabbbabababbbbbbbab
aaabbabaabbaabababaabaaabaaaabaabaaabaaabbaabbaaaaabaabbaabaabaa
ababbaabbabaababbbbbbbbabaaabaababaabbaa
bbaaaaabaabbbbbaababaabaaabbbaabbbabbabaaaaaabbbaaaabaab
aaaaabaaaabbbabbbaabaabb
aabbabbabababaababbbbabaaabbbbabbaabbbaababbbbba
baaabbbabbbbababbbbabaaaaabbbbaabaaabaabaababbba
abaaabbbbabbaaaabbabaaaabababaabbabaabaabaabaaaa
abaaabaaaaabaaabababbbbaaabbbbbb
abaabbaabaabbaabaababbab
abaaaaaaabbaaaaababbabbbbbababbaabbabbbb
bbbaaabaababaaabbaaababbabaabbbaaaabaaba
babbabaaaababbabbbbabbbbbabbbbbabbaaabaababbbabbabbbaaaaaabababaaaaaabab
aaabbabaabbaabbbbaaaabbb
babaabbabbabbaabbaabababaabbbabbbbbbbaaababbaaba
bbabbaabababaabaababbbabaaaaaaabbbbbbbaaabaababb
aaaabaaaabbbbababaaaabbabbbbbbbbaababababaabaabb
abaaabaaaabaaabbaabaabbbbaabbbab
aaaaabaaaaababbbababaaaa
aabbabbbaabbbbaaaabbbbaabbabbaabababbbbb
aaabbbbabaaaabbbbbababbb
abaaaaaabaababbbabaabbbaababbaabbbabbabaaaaabaab
aabaaaaabbabaaaabaaaabbaaabbbbababaaaaba
baaabaabaaabbabaabbaabaabbaaabaa
bbabbbbabbbabaaaaaaababa
aaaabaaababbabbbaaababab
baabbbbbababbaabbbbbbbbabaababbbbbbababaabbaaabb
baabbbbbaabbbabbabaabbbbabaaabba
bbabaaabababbababbbbbbbbbabaaaaababaabab
baaaabaaaaabaaabbbbbabbb
babbabbbbbbbbbbbababababababaaababbbabbaaaaabbababaaabbb
aaabbaabaabbbaaababbaaab
aabbbabbbaaabaaaabbbabaa
aabbbbaabbabaabaabaaaaab
bbbaabbbabbababaababbbababaaaaaaaaaaabbb
bababbaaababbaabbbbbbbbbaaaabbbbabaabbbaababababaababbabaabbbbbb
aabbbaaaababababaaaabaaabaabbaaa
ababbbbbbbbbbbbbabbbabbaababaabaabaaababbabababbbbbaaabbbbbbbaaaaabbbbab
abbaaaaabbabbaababbaabbbbabaaaabbabbbaba
babbabbbabbaabbababbbbba
abbaabbbaabbabbbbbaabbaabbbaaabbbbbaabab
ababaabbaaabbabbbbaaababbbaaabbabaaaaaab
bbbababaaaaaaaabababababbababbba
aaabbbbbbbbabaabbababbaababbababaabaabaa
babaaaabbbaabbbbababbbbb
bbbaabbbbabbabbbabbbbbabaaaabbaaaaaaaabaaabaaaab
bbaabbbbbaabaabaaaaabbaaaabbabbaaaabaaabbbaabbbbabbbbabbbbbbbbabbbbaaaab
baababbbabbabbabbababbba
ababbabababaabbbababbbabaababaab
baabababbbabaaabbaaabaaaaabbbabbabbbabbbbabaabaaaaaaabbaaaabaaaabbbaabaa
abaabbbbaaaabaaaabbabbbaabbbabab
aaaaabababbbabbbabbaaabaabbbababbbababbb
bababbaaaababbbbababaaababbbabab
bbbbbbbbbbbabbaaabaabaaaabbabababbbababb
abbaababbbbaaabaaabbbbbaaabbbaaabbabaabbabbbbbbb
abbaaaababbbbaaaabbabaabbaaaaaababbbabaa
aabbbaabababbaabbbaababaabbbbabb
babababaaabbbbbaabbababaabbbabbbbabbbaaabbbbaabbbbababba
ababbaabbbaaaaababbaaabb
aabbaabbaabbabbbbbbbaaabbbbbbbbaabaabbbaaaababbaabababaaaaabaabbababbabbaabbaaaa
ababbbabbbabaaababbbabab
bbaabbaaabbababbaabbbbaaabbbbaaaaaabaaabbabbaaabbbbbaababbbbaabaaaaabaab
baaaabaabbbaabbaaaaaaaba
baababbbbbbabbbabbbabbaabaaababbabbaaabaaabaaaab
aabaaaaaabbabbbabaaaabab
abbaaabaaabbaabababaababbbbbaababbbaaabb
aabbbabbababbbbabababababbbbbaba
aaabbabbbababaabbabaabbabababbaabbabbbbb
abbbbbabbaabbbabbababaaabbbbbababbbbaaaaaabaaabbbabaabbbbbbabaaaabbabbbabbabaaaa
aabbaababbaaaaabaaabbbba
aabaabbbabbbabbaabbabbaa
bbaaaaababababbbababbbababbbabbbabbbaaabbaaaaaaabaabaaab
babababaabbaabbabbbaabbbbaaabbbb
babaaaaababaabbbbabaababbbabbaabbbaabbaabbabbbaaaaaabbabbbbbaabb
aaababbbabbaabaaaaabbabaaaaaabbbbbbabbab
aabbababaabbbaaabbbabaabbbaababaaabaabbbababbabb
babbaaaabaaaabbabaabbbbaaabbaabaabababaa
abababbaaababbbbbaabbabaabbbbabb
bbbabaabababaababbbbbbaa
bbbbabababbbbaaabbaaaabaaaaabbbabbbbbabb
aabaabbbbbabaaaabbaaaabbababaabbaaababba
babaaaabbbbbbbaababaabbabbbabababbbbaaba
ababababaabbbbbabbaaabbb
babbabbbbbbabaaabbababbb
babbbabbababaaababaaaaab
abaabbbbabbaabababbbaaaa
bbbabaabbaaababbabbaababbaaaaabaaabbabaabbbbbaaa
aababbaaababaabbaaabbbabbababbaaaabaaaaabbababbaabbaaabbbabaabba
bbabaabaaabaaabbbabaabba
aabbbaabaababbbbbaababbababbbbbbbaaabbbb
aaabbababbaababaaabbbbaababbbbaaabbbaabb
babaabbbababbbabbbbbbbbbbbabbaabaaababba
bbabaaabbbaabaaaaababbba
baaababbabbaabbabaaabbabaabbabaa
bbabaaaaaaabbabababaaabaaabbaabbababbaababbabbabbababbabbbababba
baaabbbbaaaaabaabababababbaababbbbabbbababaaabbbabbaabbb
bbbabaaaaabababbaaabbaabbaabaaabaabbabaa
bbaaaabaaaabaabbaaabaabbabbbbbbabbababbaaabbaaabbaabbaab
babbaaaaaaabbaabbbbbabaa
bbaabbaaabbbbbabaaaababb
aabbbbbabbbababababbbabbbaaabbbabaabbaabaaababba
abaaaabaabbabaaaaaaabbabbbbaababbabaabbbbbbaaababbababab
abbbbaaaaaaabbaabaabaababaabaabaabbbabbaaabbabaabbaababb
bbbbbbaabbaababababaaabbbbabbaaa
babaaabbbaaaaabaabaabaaabbbbbbbabbbbbabb
aaabbaabbbabbaabbbbabbbbaababababbbbbaaa
bbbabaabababbbabbaaabbabbaabbabbabbaaababbbbbaabaaabbbbabbbaaabbbabbbaab
ababbbbabaabababbbabaaaabbbaabbabbababaabbababba
bbabaaaaaaabbabbbabbaabb
ababaababbbbbbbaaaabaaba
bbbabbbbaaaababbbbaaaaaa
bbbababaaabaababbbbabbbbaabababbbabbababbaaaaaaaaaabbbaabaaabbaa
abaabaabbbaaaababbaaabaa
babbabbbbabaababaaaaabaaaaaabaaaabbaaaabaabaabba
bababbbbaabbaababbabbaabbaababbaabbbaabbaaabbbaa
bbbbbbaabbaaaabbabaabaaaaababaab
bbaabaabbaaaaabbababaabbbbbabbaababbaaaababbaabbaabaabaabbabbaabbbababab
bbbabbbabbaabbaabaaabbaa
bbbabbbabaaaabaaabbbbbbb
aabbbabababbaaaaabababbbabbbaaab
baaababbabbbabbbbbaabbbbbbbaabab
aaaabbaaababbaabbbababaa
bbaaaabbbaaababbabbbbabb
babaaabaababababaabbabbaaaabaabbabaaaaab
baaabaabababaabaaaabaaabaabaabaa
abbababbbbbbababbaababbabbaabbbaabbabbbb
aaaaaaabaabaababbbabbbbababaaaaabbbaabab
baabbbbbabaabababbaababaababbbabababaabbabbbbbba
aabbabbaabbaaaaaabbbaaaa
aaaabbbbbaabababbabbabbbababbbbb
babaababbbaabbbbabbaaababaabababbaaabbbbababaabb
babbababbaaaaabaaabaaabbababbaab
bbaabbaaaabbbbabaaabbbbbbaaababa
abbbabbabaabaababaabababbbbbbbaaababbabbbbabbaaa
aabaaaaaaabbaabbaabbaaab
aabaaaaabaaaaabbaaaabbab
ababaabaabbbbaaabbbbaabb
abbaabbbbbbbaaabbaabbbbbbbaabbaaabaabaaaabaaabaabbbbbabbbbababbb
babaabbabaabaabaabbaabbabaaabbaa
bbaabababababbbbbbaabbab
aabbbbaababaaabaaaaaabba
aaabbbbbabaababaabaabaababbabaabbaaaaaaa
bbaababaaaabaaabaabbabababababbbabbaaaabaaaaaaaaaabaabaaaabbaaabbaaabbaa
aabbbbbaaabaaabbbbbaabaa
abbbaabbaaabbabaabbbababaaaaabbaabaabaabbbbaababababaabaaabaabbabbaabbbbbbbbbbabbabbaaaa
baaabaaaabbaabbaabababbbaabaaabbabbaabbaabbababaaaababab
aaaaaaabbbabbbaaababbbabaaabbbaa
baaaaabaabaabababbbaabbb
aababbbbbbaaaabbaabbbbabaaaabbbabbbbabbb
baaaabaabbbabaaaaabaaaba
bbbaaabaababaaababbbabaa
bbaaaabbbbbaaabaaabbbaaaaaabbabbbbbbbaab
baaaabaabaabbabaaaabbbbbaaaabaaababbaaaabababbbabaabbbabbababbab
ababbbaabbabaabaabaabbbbababbaababbababbabbaaabbabbaaaaaabaabbaaabbaaaaaaabaabbb
ababaaabbaabbabaabaabbbbabaaabaaaababbab
baaabaabbabbaaaabbbbbbaabbbbaabb
bbabaaaaaaabbbbbabbabbbaabbaaababbababba
bbbbaaaaabbaababbabbbaaa
bbbbbbbbaababbbbaababaab
aaaaabaabbabbbaabbabaaaaababbababbaaaababaabaabaaaaabbab
babaaaaababbbabbbbaabbaaabbbaaab
bbaaaababbbaabbabaaabbbb
baabbabbbbaabbbbaabbbbbb
aabbaababbaabbbbbaaababbaabaaaba
aaaabbbaababbaabaaabbbaa
bbbaabbbbabbababbaabbabaaabaabbabbabbaaa
abaababaabaaabaaaabaabababaaaaaaababaaabbabababb
bababababaaaaabbaaaaababbbbabbbabababaabbabbbaab
abbaabbbaabaababababbbbb
babaaaabababbababbaaaababaaaaababbbbbbbabbbaaabbaaabaaaa
aabbaabbbabaaaaabaabaaab
aabbbaabbabbabbbbaaababa
baaaaabbabbaababbaabbabbaaabbbaaabaabbab
ababaababbaabbaaabbbbabaaaaabbaaaababbabbbabbaba
aaaaabaaaabbbabbaabbaaaa
abbaaabababaabbaabaaaaab
abaaabaaaabbbababbbbbaaa
bbbbbbaabababbaabbbbbbaababbbaab
bababaabbaababbbbaabbababbbaaabababbaaaaaaabbbab
bbbbaabaaaabababbaabbbab
abaabbbbaabaaaaaabbaaabababbbbbbaaaaaaaa
abbaabbaaaaabbbbbabaaabababbbbba
aabbaabbababababbaaababbabbabbabaabaaaba
bbaaaabbababbbabaabbbbbababbbbaaaaabaaaa
abababbaababbbbaaababbbbababaaaabbbaaaab
abbabbababbaaaababbaabbbbabbbbbb
abbbbbababaabbbabbaaaaabaaaaabbb
abbaabbbbaaabbabbabbabbbabbabbbaaabbababbbbbabaabaababaa
aabbbaaabaabababbababbbbabbbbbaa
bbbaabbbaabbbaaabbaaaaabbaabaabaaaababbbabbabbbaaabbaaaa
baaaabbabaabbabbaaaabbbaaaaaabaabbaaaaabbabbbbba
aabbabababababbababababaababbbbababbaaba
ababbbbaabbabbbabaaaabab
aababaaaaaaabbabbabbaaba
bbaaaaabbabaabbaaaaabbaa
aabaaaaabaaabbbaaaabbabababbbbbb
abbababbaabbbaaababbababaaaaababbbaabbbbbabaaaaaabaaaabb
baababbaabbaaaababbabaaa
abaaaaaababbbabbaabbbbaabbbabbaaabbabbbaabbbbbabaaabaaaabbaabaabaaabbaaabbbaababbaababaa
bbbabbbbabbbabbabaabbababbaabaaaaabaaaab
abbaabbbbbabbbbabaaabbaa
bbbaabbaabbabaabaaabaaba
babbabababababbabababaaa
abababbbbbbabbbbababbbaa
aaababbbabbaaaaaaababaab
aaaabbaabaaaabaabbbababababaaabaabbabaabababbabb
ababbbabbaaabbabbbaaaaaa
abbaaabaaababbbaaabababbbbbbbabb
bbbbbbbaababababababbabaabaabbaa
abababbbbaababbbaabbbabbbbabbaba
baabababaabbbaabbabaaabbbaabaaaaabbbbbbb
aabbbbaababaaababaaababa
bbabbbaababaaabbabbbbbaa
ababbbababbbbaaabbbaabaa
baaaabaaabababbbaaababba
bbbabbbababaaaabaababaaa
babbabbbaaabaaabbbabaabaaaaaabbb
ababaaababbaabaaaabbbbbaabbbbaaaaaabaabaaaaabbab
bbbabaaababbaaaababbababaaaaabbabbbbbaba
babaabbabaabbabaaababbbbababababbaabbbaaaaabaaba
baabbbbaaabaababaaaabaaaaaaabaab
abbbabbaababbaaaaabbaabababbbabbaababbabbbbabbaaabbaaaabbbbbbabb
bbaabbbbaaababbbbaaabaaabaaabbbaabbbbaabbaabbabbabababababbbbbaababbabbbaaaababb
abbaabbabbbabbbaaabbabbaaaabaaabaabaaaba
bbbabaabaabaabbbbabbaaba
abbababbbbabaabababaabaa
aabbbabbbbabaaabbabababbbbbaabaaabaababbbbbabbaa
babaabbaababbaaaaaaabababbbbabaa
abbabaabbbbbbbbbbaabbbbbabababbaaababbba
ababaababbaabbaabbaaaaabbaaaabaaabbbbaabaaabaabb
bbabbbaabbaabbaabaabbbbabaabbaaa
abaabbbbaabbaabbbbbabaaaabbaaabababbbaba
bbaaaabbbabbbbabbbbbabbababbbbbaabbbabab
ababbbbabbabaabababbbbab
abababbaaaabbabbaaaabbbb
abaabbbaabbaaaaabababaabbaaaaabababbbbaabaaaaaab
abbaaaaaababbaabbbbbbaaa
bbbabaabbabaabbbababababbaabbaababababaa
abbabbabbaabbbbaabbaabbabbbbbbababbbbaab
bbbbbbbababababaaabaabaa
bbabbbbabaabbababbababaa
abababaabbababbbbbabbabaabbbaaabaaaabbbabaaaaaabababaabbabbabaabaaabbabb
bbaaaabbbababbbbaaabaaabbaabbbaabbbaabbabbbaaaaababbbbabbabbaabb
aabbbbabaaaabbbabaababab
babaabbabbabaabaaaaaaaabbbaababb
aabbbaabbabaaabbaaabaaabbabbaabb
aabaaabbbbabaaaabbbabbaaababbbabbbbbbbbabaabaaaa
ababaabaabbaabaaabbababbababaabbabaaaabaaabaabaaaaabbbaa
babaaaaaaabbbaabbabaabbbbbaaaabbababaaaabbbaabab
babababaaaaaababbbabbaabababaaabbabbbbba
abbbbaaaababbaaaababbbbb
abababbaaaaaaaabbaaabaabaaababab
aabbbabbbbbaabbbaaababab
aabbbbabbbaabbbbaababbbbbbbaabaa
bbaabbaababaaaaabbaaaaaa
bababbaaabaabbaabbbbbabbbbabaabaababaaaabbbabbbabbabbbbaaaaaabbabaabbabaabaaaaabbbbabbbb
bbbaabbabbabbbbaaaababba
bababbaaaaabaaabbbbbbabb
ababbaabbaaaabbaabbaabbbbaaaabbb
babaabbababaaaaababbaabb
aaabbbbbbbabbbaabbbbbaaa
abaabaaaabbaababbaabaabb
aabbabbbbabaaabbabbabbbabbabbbbabbbabaabaababaab
abaabaabbababaababbbbbaa
aababbbbbbaaaaaaabbaaaaaaabababaababaaaaaababbababbbbabababaabab
abbabbababbbbaaabaaabaabaaabbbba
aababbbbbbbabaabaabbaaaa
aabbbaabbaabbabbbaaababaaabababaabbaaaababaaababaabbbabbbbbbaababbaababb
aabbaabbbaabbababbabaabaabbbabbabaaaaaaa
babaaabbaabbbbaaaabbbbabaaababbaabaaabba
babaaababbaababaabaaabbb
bbabaaaaababbaaaaababbbbbaaaaaab
aaaabbbbbbaaabbababbaaba
abbaabbaaabbbaabbbbaabab
abaabbbaaabbbaaabbababababbbbbbb
bbabbaababbaabbabbabbbbb
abbaabababababbaaabbaaab
aaaaaaabaabababbabbaababbabbabbbbabababbbbbbbabbaaaaaaaa
abaabbbbbbbaabbaababbabb
baabaababbabaababaabaaaabbabbabbabbbbaab
bababbaaaaaabbbaaaabbabbaababbaa
babaaaaaaabaaaaaaababababababbab
bbaababaaabbbaababbaabbbaabbaababaaaaabaabaababb
aabaabbbbaabbababbaabbbbabaaabaabbabbbaaabbbaabb
abaabbbbbaaabbbababbbabbbbaaaabbbaabaabbbbaabaaa
aabaaaaaaabababbbbbbbaba
abbaababababbaabbbbaabbbababaaaababbbaab
bbaaabbababaabbbaababaaa
abbbabbbaabbabbabababababbaaabaa
baabbbbabbabbbaabbaaaaababaaaabb
abaabbbaaabbabababbbbbabbbbabaaabbbabbbaaababbaa
aabaababbabaaabbbbabbbaaaabbbbbbaababaab
abbaaabbaababbabbbbaaaabbbbabbab
aaabbabaaabbbaaaabaaabaaaaabbbbbaaaabbbbaabbaaaa
babbbabbabaabaabaabaaaaaababbabababaabbbaaababbaababbbbb
aaaabbaabbbabbbaabbbaabb
aabababbabbabbbabbbbaaabbbbbaabb
aabaaabbabababbbaabbbaaaaababbbbaaaaabaaaaabaabb
aabbababababbaaabaabbaaabbbaaaaababbbaabbababaaaabaabbaabbbabaab
bbbabaabaabbbaaaababababbbaabaaa
bbbbbbaaaaabbabaabbbabaa
aaaaabbbababbbaabaaaabbbabaaababaaababbbaaaabaaa
aabbbabbabbbbabaaaabbbbbababbbababbbbababbbbbaba
ababaaabbbabaaababababbbabbabbaa
abbaabbababaaaabbabaaaaabbabbbbaaaabaaaaabaaabba
abbaababaabaaabbbbbbabaa
aaabaaabaaabaaabaaaabaaabbbbaaaaaabbababbbbaabaabbbbaaba
ababaabaabaababaaaabbaababbbaaba
aabbbbabbbaaaabaabaabaabbbaabbab
aabbbbaababaabbababbbaba
aaabbaabababbbabbbbbaaabbbaabaababaabbaa
babababaababbbabbaababbabbaaabaaabaaaaab
aaaabbbaaabbaabbaaaaabba
bbaaabaababaaaaabbbaabbbbabaaabbaaabbbbaaabbbabbbbbbabbb
abbbabbbbababbaabbaabababbbbbaba
babbaaaaabababbbbaabababbbbaabab
baaabaaaababaaabaababbbbababababbabaaabaabaaaabaaaabaaaa
baaabbababbbbbabbabaabbabaaababbbbbbbbbbabbabbbbaaaababa
ababbaabaaabbabbabbabbbb
aabbababaaaababbbbabbbbb
babaaababbabaaaababaababbbaaabbababaaabaabbbbabb
bbaaabbbabaaaabbbaaaaaab
aabaaaaabbaaaabbabbaaababbaabbbbbaabbbaaaaaaabba
baabababaabababbaaaaaaaa
abbbaabaaabbaabbbaabbbbabaaabbbbbaaaaabaaababbbaabbababb
aaabbabbbababaabaabbaabaabbabbbaaabbbaaabbabaaaaaababbbaaababbabbaabbaaabbabaabb
ababaaabbbabaaaabaabbbaababaabbbbbaaaabbbabaabbbabbbbbbabbaabbabaaababbabbbbaabb
bbbbbbbbbaabaabababbbaaa
bbaabbaaabbabbbababbabaa
abbaabaabbbbabbabbbaabababbbabaaabbbbabbbbbabaabbbabbbbabaaaabbbbabbababaababbba
aaabaaaaabababbaabaaababaababbabababbbaaaabbaaaabbabaabaaababbaababbabbabbaaaaba
aabababbabaabbbbbbbbaabb
babaabbbbaabbababaaabaaaaabaaabbaababbaa
aaabbbbbaaaabaaaababbbbabaabaaaaaaaabaab
aabbaabbaabababababbbbba
abbbabbabaabbabbbaaaabab
bbabaaaabbbababababaabababbaababbaaaababbabababbabaabbab
babaabbbaabaabbbabaabbbaaaababbbbabbaaababbbaabaabaaaaab
aabbaabaabbaabaaaabbbbaabbaabaaa
aaabbaababbaaaaaabbbbababaaaabbbabaabbab
bbaabbbbabaababaababbaabbaaabbbaaaaabaaaabbabbbb
baaaabaabbabbbbaaaabbabbbababbbbbbababab
ababbaababbaabaaabaabaababaaaabbaaababab
bbbbaaaaaabbbbabbaabaaab
aabbbabababbabbbaababaaa
baaababbbaababbaabaaabaaaabbabbabbababbb
aabaababbabaaaabbabaaabaaaaaaabababbbbaa
abbabbabaabaaaaaaaaabbbbbaabaabaaabbaabbabaaabbaaabaaaababaaabbaabaaabbb
baababbabaabaababaaaaaab
baaaabaaaaaababbabbabbbabbabaabb
bbaaabbaabaabaaaabbababaaabbbbabbabbbbaa
abababbaababbbbababababababababaabaaabba
aabaabababbaabbabbababbb
aabbbbabbabaababaaababab
bbabbbbbbaabbbbaabbbbabbabaaaababaabbbbb";

        const string EXAMPLE_RULES = @"0: 4 1 5
1: 2 3 | 3 2
2: 4 4 | 5 5
3: 4 5 | 5 4
4: ""a""
5: ""b""";

        const string EXAMPLE_INPUT = @"ababbb
bababa
abbbab
aaabbb
aaaabbb";
    }
}
