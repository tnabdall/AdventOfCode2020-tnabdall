using System;
using System.Collections.Generic;
using System.Linq;

namespace Day23
{
    class Program
    {
        static void Main(string[] args)
        {
            // Part1
            var firstNodePart1 = ParseInput(PROBLEM);
            DoAction(firstNodePart1, 100, out var cupsListPart1);
            PrintAnswerPart1(cupsListPart1);
            Console.WriteLine();
            //Console.ReadLine();

            // Part 2
            var firstNode = ParseInput(PROBLEM, addTo: 1000000);
            DoAction(firstNode, 10000000, out var cupsList);
            PrintAnswerPart2(cupsList);
            Console.ReadLine();
        }


        private static void PrintAnswerPart1(List<Node<int>> cupsList)
        {
            var node = cupsList.First(x => x.Value == 1);
            do
            {
                node = node.Next;
                Console.Write(node.Value);
            } while (node.Next.Value != 1);
        }

        private static void PrintAnswerPart2(List<Node<int>> cupsList)
        {
            var node = cupsList.First(x => x.Value == 1);
            Console.WriteLine((long)node.Next.Value * (long)node.Next.Next.Value);
        }

        private static void DoAction(Node<int> firstNode, int rounds, out List<Node<int>> cupsList)
        {
            cupsList = new List<Node<int>>();
            var firstVal = firstNode.Value;
            var iter = firstNode;
            cupsList.Add(iter);
            do
            {
                iter = iter.Next;
                if (iter.Value != firstVal)
                    cupsList.Add(iter);
            } while (iter.Value != firstVal);


            var nodeByVal = cupsList.ToDictionary(x => x.Value, x => x);

            var maxValue = cupsList.Max(x => x.Value);
            var minValue = cupsList.Min(x => x.Value);

            var currentCup = firstNode;

            for (int i = 0; i < rounds; i++)
            {
                // Pick up cups
                var pickedUpCups = new Node<int>[] { currentCup.Next, currentCup.Next.Next, currentCup.Next.Next.Next };

                // Find destination cup
                Node<int> destCup = currentCup;

                int destCupVal = currentCup.Value;
                do
                {
                    destCupVal--;
                    if (destCupVal < minValue)
                        destCupVal = maxValue;
                } while (pickedUpCups[0].Value == destCupVal || pickedUpCups[1].Value == destCupVal || pickedUpCups[2].Value == destCupVal);
                destCup = nodeByVal[destCupVal];               

                // Remove picked up cups and place them after destination cup
                if (pickedUpCups.First().Previous != destCup)
                {
                    var prevCupBeforeFirst = pickedUpCups[0].Previous;
                    var nextCupAfterLast = pickedUpCups[2].Next;

                    // Removes our 3 cups from the sequence
                    prevCupBeforeFirst.Next = nextCupAfterLast;
                    nextCupAfterLast.Previous = prevCupBeforeFirst;

                    pickedUpCups[0].Previous = destCup;
                    pickedUpCups[2].Next = destCup.Next;

                    // Fixes sequencing of other cups
                    destCup.Next.Previous = pickedUpCups[2];
                    destCup.Next = pickedUpCups[0];
                }

                //Console.Write($"{i + 1}: ");
                //PrintAnswerPart1(cupsList);
                //Console.WriteLine();
                currentCup = currentCup.Next;
            }
            
        }

        private static Node<int> ParseInput(string input, int? addTo = null)
        {
            Node<int> first = null;
            Node<int> prev = null;
            int max = -1;
            for (int i = 0; i < input.Length; i++)
            {
                Node<int> newNode = new Node<int>() { Value = int.Parse(input[i].ToString()), Previous = null, Next = null };
                if (newNode.Value > max)
                    max = newNode.Value;
                if (prev != null)
                {
                    prev.Next = newNode;
                    newNode.Previous = prev;                     
                }

                if (i == 0)
                    first = newNode;
                else if (i == input.Length - 1)
                {
                    Node<int> last = newNode;
                    last.Next = first;
                    first.Previous = last;
                }

                prev = newNode;
            }

            if (addTo.HasValue)
            {
                var current = first.Previous;
                for (int i = input.Length; i < addTo.Value; i++)
                {
                    Node<int> newNode = new Node<int>() { Value = max + i + 1 - input.Length, Previous = null, Next = null };
                    newNode.Previous = current;
                    current.Next = newNode;

                    if (i == addTo.Value - 1)
                    {
                        newNode.Next = first;
                        first.Previous = newNode;
                    }

                    current = newNode;
                }
            }

            return first;
        }

        class Node<T>
        {
            public T Value { get; set; }
            public Node<T> Previous { get; set; }
            public Node<T> Next { get; set; }

            public override string ToString()
            {
                var nextVal = Next != null ? Next.Value.ToString() : "None";
                var prevVal = Previous != null ? Previous.Value.ToString() : "None";

                return $"Value {Value}, Next {nextVal}, Prev {prevVal}";
            }
        }

        const string PROBLEM = @"394618527";
        const string EXAMPLE = @"389125467";
    }
}
