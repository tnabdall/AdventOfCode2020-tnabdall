﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace Day3
{
    class Program
    {
        static void Main(string[] args)
        {
            Part1(PROBLEM_INPUT, 3, 1);
            Part2(PROBLEM_INPUT);
            Console.ReadLine();
        }

        private static void Part2(string input)
        {
            (int X, int Y)[] listOfSlopes = 
            { 
                (1, 1),
                (3, 1), 
                (5, 1), 
                (7, 1), 
                (1, 2) 
            };

            var treesEncountered = listOfSlopes.Select(e => (long)Part1(input, e.X, e.Y));
            var multiplied = treesEncountered.Aggregate((e, f) => e * f);
            Console.WriteLine($"Trees encountered multiplied is {multiplied}");
        }

        private static int Part1(string input, int xIncrement, int yIncrement)
        {
            var charArray = ParseInputToCharArray(input);
            int row = 0;
            int column = 0;
            int endRow = charArray.GetLength(0);

            int treesEncountered = 0;

            while (row < (endRow - yIncrement))
            {
                row += yIncrement;
                column += xIncrement;
                if (column >= charArray.GetLength(1))
                    column -= charArray.GetLength(1);
                if (charArray[row, column] == '#')
                    treesEncountered++;
            }

            Console.WriteLine($"Trees encountered {treesEncountered}");
            return treesEncountered;
        }

        private static char[,] ParseInputToCharArray(string input)
        {
            var lines = input.Split('\n', StringSplitOptions.RemoveEmptyEntries).Select(e => e.Trim()).ToArray();
            var rows = lines.Length;
            var columns = lines.First().Length;
            var charArray = new char[rows, columns];
            for (int i = 0; i < rows; i++)
                for (int j = 0; j < columns; j++)
                    charArray[i, j] = lines[i][j];
            return charArray;
        }

        const string PROBLEM_INPUT = @"......#..........##......#.####
.##...###....#.....#...#.#.....
#..##..#.....#............#.#.#
##.#....#####..#....#..#.#.....
..#.#...##.##.......#.#..#..##.
##.#.......#.#.#..#...#.#...#..
...#...#..#.##....##..#.#......
.......##.#.#.#.##...#.........
..#...##.##...##..##.##...#....
.#.#...#.....####..#.#...#.##..
.#...#......##......##....#....
..#.....#.........##.#...#.#...
...#.#...#..##...#....#.....##.
..#.....#..#..#......###.......
...##.#....##..##...........#..
....#......#..#....###...#.....
.....#...#.#.....#..##........#
....#...#....##.#.##.#...#..#.#
.......##.#......##....#....#..
...#.#...##..#...#..#..#..##.#.
##.#...#..#..................##
##........#....##...#..#..#....
.#.#..............#######.##...
##..#..#.#.##..#...............
..#........#..#...##.......#...
............##.##.#..........##
.....##..#.....##..#.....#.....
..#.##.###.#..##.............#.
.........##...........#.....#..
..#....#.#.###.#.#.......##....
..###..##..#.#.##......#.#.##..
##......#.#....##.#..#.#..#.#.#
..##.#.###.#...#...............
..####.......#...#.##....#.....
..#....##...#.#.#.#....#.##..##
.#...####..###....#.###...##...
..#.#..........#.#..#..#.....##
.#....#.........###...#.....##.
..#.#.#.##........#.##.#.....#.
#....#....###...#..#.........#.
#..#.###....#..............#...
............#....##.#......#.#.
...#..#.####...............##..
....##......#.#.........####..#
.#....###..#.#..##........##...
#..##.....###..#...............
..#...........#........#...#..#
......................#.#..#...
.#.##.#..#.#....#...#...#.#....
..#..#.........#..#.#..........
.#......#####...#......#..#....
..........#.....#..#.##.####.##
##.##..#............#####...#.#
..##..#..###......#...#...#....
....#####........#.##...###....
......#...##..#..#............#
...#....##.##...#..#...#.......
....#####.#...............###..
.#....#..##....#.#.#..##.##...#
...#..#..#........#.#####.....#
......##.#...#..#..#.....#..###
###.......#.#........#......#.#
..#.#..#..#........#..#......#.
...##.........#..........#.....
...#..###.#.......#.#.........#
....#..#.##...##.....#.....##..
#.#.#.#.....##.##.###..#.#....#
..#....#.....##.####..#........
...#..#.##.....##.#..#....###..
.#..#.....#....#...#.#.......##
..#..#.......#.#.###......#.##.
.###.####....##............##.#
#....###.#......##.#......##.#.
.##...........#.#....#.........
#.##..##...#...........###....#
#.#..#...#.#..#..###.#.##...#..
..#...#.#..##....#..#..#.......
#..##..#.####...#...#..####.##.
###..#.##....#...#.###..##...##
##..#..#.#....#.....##.......#.
..#..##.##.#.......###.#.....#.
..........#.####....#.......#..
#...#.#..#.......##......##..##
##...##.##..###...............#
....##.#...#.......##...##..#..
.#.........#...#.#...##.#.....#
.#...#.#..#...#..##....#..#...#
.#.#...#..#..###...##....#.....
.........#.#...####..#...#.#...
...#.............#.#..........#
...#...#..##.#........#.#......
...#...#.....#....#..###.##.###
.#.#........#....#...#.###.#.#.
##.....#.......#..##.#....#..##
...###...#.#.#.#....#.#....#...
#...#.#.......##.#..#....#.#...
#...#......###.....###........#
..#.##...##....#...#....#.#....
#....#..###....##.#......##...#
##.#...#..........#.##....#..#.
.##....#............###.#...#..
###.##.#####.##.....##..#####..
..###.###.......#.#...#....#...
.#...#....####.........#.......
..##.#.#......#....#.#....#.#.#
#.####.....#....#..#.....#.##..
###.###.##...##.#.#.#.....#.#..
.......#.....#.......##.#.....#
#..#.##...#........#.#.......##
#.#........#...#....#..........
..#....##.#......#..#..........
#....##.....#.....#.##.#...#...
....#.#.....#....####...#.#.##.
......#.......##...##.#......#.
.#.........##...#..#..##..#....
.#...##.....##.#....#..........
....#.###..##..#...#..........#
......#...#.#.#........##......
.#..........#.#.....#..##..#.#.
.......###.#......#....#.#..#..
..##.......#....#....#.#...##.#
#.##.#.......#..###..##...#.#..
......####....#.#.....#...#..#.
#.##.###..#..#.#.....###..#.#.#
#.#.#..#.#..##...#...#..##.###.
....##..##.#...............#.#.
..###.#.#.##..#....##.......#..
#.#....#..........##......#####
.#.#.......##.#.#......##..#.#.
......#.###.##.#..#....#.##....
..###........#.......##.#.#....
.#..##.............#.##.###...#
.#####...#......#.......##.....
##..###.#...#....#..#....#.#..#
.#.........###.##.....##.....##
.##.#....#..#.#..##..#....##...
.#..#..#......###...#.......#..
#.#...#.....#..#.#.#..#..###...
....#....#..#..#....#..#.#.#...
......#.......#.#.#.#.....#....
###...#...#......#..#.#.#..#.#.
#...##.##.##........##....#....
.....#.......#...#...#.#.#....#
...##.....##.#...#.#.#.#..#..#.
.#.......##...........#...#.##.
.##..........#......#.#...###..
.....##...#.....#...#......#...
...........#.....#..#...#..#.#.
#.....##..#...........##....#..
#.##...###.###....##..#..#....#
#.#.##...##....###....##.##....
.#..###.....#......#...#...#..#
..#...#....#.#.###.#..#......#.
......#.........#..#.##...#...#
..#.#....##.#..##..##...#....#.
#.....#....##.........##.#.....
...#...#..###.###......##...###
.##.###...##..#.##....##.#..#..
..#..#.......#................#
.....#..#.#.#..........##..#...
......###.#.#............#..#.#
..#.##.....##....#...#...#.#...
..#......##...#...##........#..
#.....#.....#..#......#.###...#
....#..#.#.....#...#....#.#...#
#.......#..#...##..#.#..#.##...
..#......###...#.........##...#
...#.......##.....#..##........
.#....#.#.....##.#.#...........
##..#..#...#.##.#.#.#.#.#..##.#
##...####.#.#.##...#..#......#.
#.##..####.##.#.........#...###
#...#.......#.#..####.#.#.#....
#....#........#........#.......
..#..####.....#....##...###.##.
...#.#..####.........#....#.##.
##.#...#...#..#.#..##.....##...
....#.........#.##........##.#.
##...#......#....#..#....#....#
###.....#......##...#...##...#.
#.##...............#.......#...
.##.#...#..#....#.#.....###..#.
.....##...#.##.....##...#....#.
#.#..#..........#####..##......
..#.........##...#.........#.##
...#..##.#.#..#......#..###.###
#..#...#.#...##..........#.....
.###..#....###.....#....#..###.
#..#....#...#........##.....#..
.#..###........#....#..####..##
.#..#.#.#.......##.#..##.#..##.
..#..###......##....#..#..#..#.
.......###..##....#......#...##
#........#.##.............##.#.
...#.#.#....##....##.###...#...
..#.....#..##..#.#.......#.####
.#......##.........##...#.....#
.#.###........##....###.#.#...#
##...#.#....#.....##.......#..#
#...........#...........####...
#..#.#..##..#...#....#.##....#.
................##.............
..##...#.#....##....#...#......
.#.....#....#....#..#..#.#..##.
.....######.#.#.##.###.#.......
..#####....#..#...........#.#..
.......#..#..##.#...###.#.#.###
###...#...#..##.#.##..#...#..#.
.#..#..............#...........
.#.....#.....##....#....##..#..
....#####.#....#......#.......#
.#.#.....##.####..#...#.......#
.#...##.#.......#.....##.#..##.
..........#...#....###....#...#
..#......#...#...#..#.#........
.......#.......#..####..##.....
.#..#.....###...#...#...#...#..
##..#.......#.#...#..#..#.##..#
#..#...#.#.....#.##.#........#.
......#......#.#..###.##..###..
.#..#..#.##.#...........#...##.
.#....#...#.#..#.#.#...##.#..#.
##.#....#..#..#.#...#......#.#.
..#.#............##...#........
...####...#...#.....##..#...###
....###.......###.##..#.###....
#......#.#....#.#.##.#.##..###.
.....##.....#..##.....##....#..
..#...#..##.#.##.#.#.#.......##
#....#..##.......#......#..#.##
#.....##...#..##......##.#.#..#
....#..##..#.##...#.#.##..#..##
#..#...##....##..#...#....#...#
.##.#.#....#.....#........##.#.
..##..#....#........#.....#....
.##.#..##...#.....#...###.....#
#..#..#........#..#.....#.#.#.#
..##..###.#..#...#.#......#..#.
#.....#.....#.###......##..#.#.
.........#...##.........#...#..
.##.#.##......#.#...###..#....#
...##.#..###........##......#..
...#.#...#......#.#.#....#..#..
..####.........#..#....#.......
#..#.........##.#.##....#.....#
..#..#..#.#........#.###.......
##.#..#..#....#...##.......#..#
..#.#.....#.............#...##.
..........#...##.....#..#.#..#.
....#..#...#..##..#...##.#.....
##....#......#..#.....#..#.....
...#.#.#.#...........##...#.#..
....#.###...#............#.....
.#.#.#.......#.#......#....#.#.
#.#.#.#..##.#..#..##...##.#..#.
.#.##....##..#........#....#...
####...#....#.#...#..#..###...#
.....#.#.##.......##..#.######.
.......#.#.#.....#.#..##....#..
..#....#.#..#.#.#..#..#........
.....##......#.........#.#...##
#....##.#.....#..........#.#...
#...#.#..#.#..#.#....#..#.#....
....##........#................
###.#.#...#..##...#...#.##...#.
...#....###..#..##..#..#.......
.....#..........#.#........##.#
.#........#.##.#..##..#...#...#
..##....#...#.#.........##.#...
......#...#......#.....#.......
....##.##..#.##...#.#.#.##.#.#.
..#...#.....#.#....##.#........
.#.#.......#.......###..#..#...
#...#..#..#..##....#...#.....#.
.#..####.##.....##.........#.#.
#...###.......#...####..##.....
#.##.#....#.#.##.......#...#...
..#.......#.#.##.##..#...##....
.#.......#.#..#.....#.....#.#..
..#..#.......##.....#.#.....#.#
#...###..#..#..##...#.....#..##
......#................#.......
..#.....##..#.......#...#...##.
...##...####.#..#...#.......##.
..#...#..#...#...#..#..#####...
#..#...#....#....#...........#.
..#.......#..#.##...##..###...#
.#..#..#......##...#....#......
...#..##....#..........#.....#.
###...#.#......#.#.....#.....##
#.#..#.....#........#.##.#.##..
....#...#.....#..#.......#.#...
#.#...##....#..#.....#...#.#.#.
.#......#...##..#.......#......
...#...#.#.#.###.#..#.#..#.....
###...#..###.#...#..##...####..
.#.#.#..#........#..#......#..#
.#..#....#......#....#.#...#...
.##..........###...##.....#.#..
.#...#.#.##.##..###.#...#..###.
......#......#......#.##......#
..#.##..#.#..#....##..##...#...
.#......#..#...##....#...#.....
.#.....#.##..........#..#......
###.#..#.##..#..##...#..#...#..
#.....###........#.#..##.#.....
.....#.......##.....##.....#.##
...##.#......####....##........
..#..#..#....#.##.....##.####..
...#..#....#.#..#.#..#.#.#..#..
#..........#....#.#.#.#...#..#.
...####.##...#..#.......#.#..##
#........#..#..................
.#..#....#.#.#..#..........#...
###...#....####....#......#..#.
#.........####..#..#...........
.....##..#..##.##.##.#..#.....#
.#..#.#.##..#..#.#.#.##.###....
......##......#...#.##....#..#.
.#.#....#..#......#..#...###...
.##...#......##...###...#.#...#
.......#.#....#............#..#
.#..##.#.######...#...#......#.";

        const string EXAMPLE_INPUT = @"..##.......
#...#...#..
.#....#..#.
..#.#...#.#
.#...##..#.
..#.##.....
.#.#.#....#
.#........#
#.##...#...
#...##....#
.#..#...#.#";
    }
}