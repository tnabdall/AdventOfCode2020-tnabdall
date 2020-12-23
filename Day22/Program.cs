using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Day22
{
    class Program
    {
        static void Main(string[] args)
        {
            Part1(PROBLEM);
            Part2(PROBLEM);
            Console.ReadLine();
        }

        private static void Part2(string input)
        {
            HashableQueue<int> Player1Cards = new HashableQueue<int>(), Player2Cards = new HashableQueue<int>();
            ParseQueues(input, Player1Cards, Player2Cards);

            // Play the game
            int winner = PlayRecursiveGame(Player1Cards, Player2Cards);

            // Game is done. Get answer
            long sum = ScoreGame(winner == 1 ? Player1Cards : Player2Cards);

            Console.WriteLine(sum);
        }

        // Return 1 or 2 depending on who wins
        static int PlayRecursiveGame(HashableQueue<int> Player1Cards, HashableQueue<int> Player2Cards)
        {
            HashSet<HashableQueues<int>> gameMemory = new HashSet<HashableQueues<int>>();

            while (Player1Cards.Count > 0 && Player2Cards.Count > 0)
            {
                HashableQueues<int> set = new HashableQueues<int>() { Queue1 = Player1Cards, Queue2 = Player2Cards };
                if (gameMemory.Contains(set))
                {
                    return 1;
                }
                gameMemory.Add(set);


                int nextP1 = Player1Cards.Dequeue();
                int nextP2 = Player2Cards.Dequeue();

                int winner;
                if (nextP1 <= Player1Cards.Count && nextP2 <= Player2Cards.Count)
                {
                    winner = PlayRecursiveGame(Player1Cards.Copy(nextP1), Player2Cards.Copy(nextP2));
                }
                else
                {
                    winner = nextP1 > nextP2 ? 1 : 2;
                }

                if (winner == 1)
                {
                    Player1Cards.Enqueue(nextP1);
                    Player1Cards.Enqueue(nextP2);
                }
                else
                {
                    Player2Cards.Enqueue(nextP2);
                    Player2Cards.Enqueue(nextP1);
                }
            }

            return Player1Cards.Count > 0 ? 1 : 2;
        }

        class HashableQueues<T>
        {
            public HashableQueue<T> Queue1 {get; set;}
            public HashableQueue<T> Queue2 { get; set; }

            public override bool Equals(object obj)
            {
                if (obj is HashableQueues<T> other)
                {
                    return other.Queue1.Equals(Queue1) && other.Queue2.Equals(Queue2);
                }
                return false;
            }

            public override int GetHashCode()
            {
                return HashCode.Combine(Queue1.GetHashCode(), Queue2.GetHashCode());
            }

        }

        class HashableQueue<T> : Queue<T>
        {
            public HashableQueue(): base() { }

            public HashableQueue(IEnumerable<T> collection) : base(collection) { }
            public HashableQueue<T> Copy(int n)
            {
                if (Count < n)
                    throw new Exception();
                return new HashableQueue<T>(this.Take(n));
            }

            public override bool Equals(object obj)
            {
                if (obj is HashableQueue<T> other)
                {
                    if (other.Count != this.Count)
                        return false;
                    var thisCopy = new HashableQueue<T>(this);
                    var otherCopy = new HashableQueue<T>(other);
                    while(thisCopy.Count > 0)
                    {
                        if (!thisCopy.Dequeue().Equals(otherCopy.Dequeue()))
                            return false;
                    }                    
                    return true;
                }
                return false;
            }

            public override int GetHashCode()
            {
                StringBuilder contentsBuilder = new StringBuilder();
                var copy = new HashableQueue<T>(this);
                while(copy.Count > 0)
                {
                    contentsBuilder.Append(copy.Dequeue().ToString());
                }
                return HashCode.Combine(contentsBuilder.ToString());
            }
        }

        private static void Part1(string input)
        {
            Queue<int> Player1Cards = new Queue<int>(), Player2Cards = new Queue<int>();
            ParseQueues(input, Player1Cards, Player2Cards);

            // Play the game
            int turns = PlayGame(Player1Cards, Player2Cards);

            // Game is done. Get answer
            long sum = ScoreGame(Player1Cards.Count > 0 ? Player1Cards : Player2Cards);

            Console.WriteLine(sum);
        }

        private static void ParseQueues(string input, Queue<int> Player1Cards, Queue<int> Player2Cards)
        {            
            bool player2Parse = false;
            foreach (var line in input.Split("\r\n", StringSplitOptions.RemoveEmptyEntries))
            {
                if (line.Contains("Player 1"))
                    continue;
                else if (line.Contains("Player 2"))
                {
                    player2Parse = true;
                    continue;
                }
                if (player2Parse)
                    Player2Cards.Enqueue(int.Parse(line));
                else
                    Player1Cards.Enqueue(int.Parse(line));
            }
        }

        private static long ScoreGame(Queue<int> winner)
        {
            long sum = 0;
            while (winner.Count > 0)
            {
                int next = winner.Dequeue();
                sum += next * (winner.Count + 1);
            }
            return sum;
        }

        private static int PlayGame(Queue<int> Player1Cards, Queue<int> Player2Cards)
        {
            int turns = 0;
            while (Player1Cards.Count > 0 && Player2Cards.Count > 0)
            {
                turns++;
                int nextP1 = Player1Cards.Dequeue();
                int nextP2 = Player2Cards.Dequeue();
                if (nextP1 > nextP2)
                {
                    Player1Cards.Enqueue(nextP1);
                    Player1Cards.Enqueue(nextP2);
                }
                else
                {
                    Player2Cards.Enqueue(nextP2);
                    Player2Cards.Enqueue(nextP1);
                }
            }

            return turns;
        }

        const string PROBLEM = @"Player 1:
29
30
44
35
27
2
4
38
45
33
50
21
17
11
25
40
5
43
41
24
12
19
23
8
42

Player 2:
32
13
22
7
31
16
37
6
10
20
47
46
34
39
1
26
49
9
48
36
14
15
3
18
28";

        const string EXAMPLE = @"Player 1:
9
2
6
3
1

Player 2:
5
8
4
7
10";
    }
}
