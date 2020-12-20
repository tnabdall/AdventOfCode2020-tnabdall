using System;
using System.Collections.Generic;
using System.Linq;

namespace Day20
{
    class Program
    {
        static void Main(string[] args)
        {
            var tiles = ParseInput(PROBLEM_INPUT).ToArray();

            // Let's maybe try and find the 4 tiles that cannot connect to more than 2 tiles in any permutation
            var maxTileConnectionsByTileId = new Dictionary<long, (int, Tile)>();

            foreach(var tile in tiles)
            {
                maxTileConnectionsByTileId[tile.ID] = (0, null);
                var tilePerms = tile.GetPerms();

                foreach (var perm1 in tilePerms)
                {
                    var permMatches = 0;
                    foreach (var otherTile in tiles)
                    {
                        if (otherTile == tile)
                            continue;
                        var otherTilePerms = otherTile.GetPerms();

                        foreach (var perm2 in otherTilePerms)
                        {
                            if (perm1.CanConnectToOtherTile(perm2))
                            {
                                permMatches++;
                                break;
                            }
                        }
                    }

                    if (permMatches > maxTileConnectionsByTileId[tile.ID].Item1)
                    {
                        maxTileConnectionsByTileId[tile.ID] = (permMatches, perm1);
                    }
                }
            }


            var cornerTiles = maxTileConnectionsByTileId.Where(e => e.Value.Item1 == 2);

            Console.WriteLine(cornerTiles.Select(e => e.Key).Aggregate((e, g) => e * g));

            // Assemble the tiles
            // Start with top left
            var topLeft = cornerTiles.First().Value.Item2;
            List<Tile> remainingTiles;
            Tile rightNeighbour = null;
            Tile bottomNeighbour = null;
            foreach (var tile in cornerTiles.Skip(1))
            {
                topLeft = tile.Value.Item2;
                remainingTiles = tiles.Where(e => e.ID != topLeft.ID).ToList();
                var rightNeighbours = remainingTiles.SelectMany(e => e.GetPerms())
                    .Where(e => topLeft.CanConnectToOtherTile(e, Tile.EdgeEnum.Right));
                var bottomNeighbours = remainingTiles.SelectMany(e => e.GetPerms())
                    .Where(e => topLeft.CanConnectToOtherTile(e, Tile.EdgeEnum.Bottom));
                if (rightNeighbours.Any() && bottomNeighbours.Any())
                {
                    rightNeighbour = rightNeighbours.First();
                    bottomNeighbour = bottomNeighbours.First();
                    break;
                }
            }

            remainingTiles = tiles.Where(e => !new long[] {topLeft.ID, rightNeighbour.ID, bottomNeighbour.ID }.Contains(e.ID)).ToList();

            List<TileInGrid> tilesInGrid = new List<TileInGrid>()
            {
                new TileInGrid(){PositX = 0, PositY = 0, Tile = topLeft},
                new TileInGrid(){PositX = 1, PositY = 0, Tile = rightNeighbour},
                new TileInGrid(){PositX = 0, PositY = 1, Tile = bottomNeighbour},
            };
            tilesInGrid[0].BottomNeighbour = tilesInGrid[2];
            tilesInGrid[2].TopNeighbour = tilesInGrid[0];
            tilesInGrid[0].RightNeigbour = tilesInGrid[1];
            tilesInGrid[1].LeftNeighbour = tilesInGrid[0];

            var maxTilesPerXY = (int)Math.Sqrt(tiles.Length);
            while (tilesInGrid.Count < tiles.Length)
            {
                // Iteration 1 (choose the tile that works)
                foreach(var tile in tilesInGrid.ToArray())
                {
                    if (tile.RightNeigbour == null && tile.PositX < maxTilesPerXY - 1)
                    {
                        var potentialTopTile = tilesInGrid.FirstOrDefault(e => e.PositX == tile.PositX + 1 && e.PositY == tile.PositY - 1);

                        var tilesThatCanConnect = remainingTiles.SelectMany(e => e.GetPerms())
                            .Where(e => tile.Tile.CanConnectToOtherTile(e, Tile.EdgeEnum.Right)
                            && (potentialTopTile?.Tile?.CanConnectToOtherTile(e, Tile.EdgeEnum.Bottom) ?? true));
                        if (tilesThatCanConnect.Count() >= 1)
                        {
                            var rNeighbour = new TileInGrid() { Tile = tilesThatCanConnect.First(), PositX = tile.PositX + 1, PositY = tile.PositY, LeftNeighbour = tile };
                            tile.RightNeigbour = rNeighbour;
                            if (potentialTopTile != null)
                            {
                                potentialTopTile.BottomNeighbour = tile.BottomNeighbour;
                                tile.TopNeighbour = potentialTopTile;
                            }
                            remainingTiles.RemoveAll(t => t.ID == rNeighbour.Tile.ID);
                            tilesInGrid.Add(rNeighbour);
                            break;
                        }
                    }

                    if (tile.BottomNeighbour == null && tile.PositY < maxTilesPerXY - 1)
                    {

                        var potentialLeftTile = tilesInGrid.FirstOrDefault(e => e.PositX == tile.PositX - 1 && e.PositY == tile.PositY + 1);

                        var tilesThatCanConnect = remainingTiles.SelectMany(e => e.GetPerms())
                            .Where(e => tile.Tile.CanConnectToOtherTile(e, Tile.EdgeEnum.Bottom)
                            && (potentialLeftTile?.Tile?.CanConnectToOtherTile(e, Tile.EdgeEnum.Right) ?? true));
                        if (tilesThatCanConnect.Count() >= 1)
                        {
                            var bNeighbour = new TileInGrid() { Tile = tilesThatCanConnect.First(), PositX = tile.PositX, PositY = tile.PositY + 1, TopNeighbour = tile };
                            tile.BottomNeighbour = bNeighbour;
                            if (potentialLeftTile != null)
                            {
                                potentialLeftTile.RightNeigbour = tile.BottomNeighbour;
                                tile.LeftNeighbour = potentialLeftTile;
                            }
                            remainingTiles.RemoveAll(t => t.ID == bNeighbour.Tile.ID);
                            tilesInGrid.Add(bNeighbour);
                            break;
                        }
                    }
                }
            }

            var arrLength = tiles.First().GetEdges().First().Value.Length;
            bool[,] puzzleArr = new bool[(maxTilesPerXY) * (arrLength - 2) , (maxTilesPerXY) * (arrLength - 2)];

            // Let's make the rows first
            var iCounter = 0;
            var jCounter = 0;
            for (int i = 0; i < maxTilesPerXY; i++)
            {
                var sorted = tilesInGrid.Where(e => e.PositY == i).OrderBy(e => e.PositX).ToArray();
                jCounter = 0;
                for (int j = 0; j < sorted.Count(); j++)
                {
                    var tileInGrid = sorted[j];
                    var imgArray = tileInGrid.Tile.ImageArray;
                    for (int i2 = 0; i2 < arrLength; i2++)
                    {
                        // Trim borders
                        if (i2 == 0)
                        {
                            continue;
                        }
                        if (i2 == arrLength - 1)
                            continue;
                        for (int j2 = 0; j2 < arrLength; j2++)
                        {
                            if (j2 == 0)
                            {
                                continue;
                            }
                            if (j2 == arrLength - 1)
                                continue;

                            puzzleArr[iCounter + i2 -1, jCounter + j2 -1] = imgArray[i2, j2];
                        }                        
                    }

                    jCounter += arrLength - 2;                    
                }
                iCounter += arrLength - 2;                
            }
                

            Tile assembledPuzzle = new Tile() { ID = 01, ImageArray = puzzleArr, FlipOrientation = Tile.FlipOrientationEnum.Regular, Rotation = Tile.RotationEnum.Regular };

            var seaMonsterStr = @"                  # 
#    ##    ##    ###
 #  #  #  #  #  #   ";

            var seaMonsterLines = seaMonsterStr.Split("\r\n", StringSplitOptions.RemoveEmptyEntries);
            // bool[,] seaMonsterArr = new bool[seaMonsterLines.Length, seaMonsterLines[0].Length];
            var seaMonsterCoordinatesMap = new HashSet<(int i, int j)>();

            for (int i = 0; i < seaMonsterLines.Length; i++)
            {
                for (int j = 0; j < seaMonsterLines[i].Length; j++)
                {
                    //seaMonsterArr[i, j] = seaMonsterLines[i][j] == '#';
                    if (seaMonsterLines[i][j] == '#')
                        seaMonsterCoordinatesMap.Add((i, j));
                }
            }


            var maxSeaMonsters = 0;
            foreach(var perm in assembledPuzzle.GetPerms())
            {
                maxSeaMonsters = Math.Max(maxSeaMonsters, perm.CountSeaMonsters(seaMonsterCoordinatesMap));
            }

            Console.WriteLine($"Count is { (from bool item in assembledPuzzle.ImageArray where item == true select item).Count() - seaMonsterCoordinatesMap.Count * maxSeaMonsters}");
            Console.ReadLine();
        }

        class TileInGrid
        {
            public int PositX { get; set; }
            public int PositY { get; set; }
            public Tile Tile { get; set; }
            public TileInGrid TopNeighbour { get; set; }
            public TileInGrid BottomNeighbour { get; set; }
            public TileInGrid LeftNeighbour { get; set; }
            public TileInGrid RightNeigbour { get; set; }
        }

        private static IEnumerable<Tile> ParseInput(string input)
        {
            return input
                .Split("\r\n\r\n", StringSplitOptions.RemoveEmptyEntries)
                .Select(e =>
                {
                    var lines = e
                    .Split('\n', StringSplitOptions.RemoveEmptyEntries)
                    .Select(e => e.Trim()).ToArray();
                    long id = long.Parse(lines[0].Split(" ")[1].Split(":")[0]);
                    var length = lines.Length - 1;
                    var arr = new bool[length, length];
                    for (int i = 1; i < lines.Length; i++)
                    {
                        for (int j = 0; j < lines[i].Length; j++)
                        {
                            arr[i-1, j] = lines[i][j] == '#';
                        }
                    }
                    return new Tile { ID = id, ImageArray = arr, FlipOrientation = Tile.FlipOrientationEnum.Regular, Rotation = Tile.RotationEnum.Regular};
                });
        }

        class Tile
        {
            public enum FlipOrientationEnum { Regular, FlipX, FlipY, FlipBoth }
            public enum RotationEnum { Regular, R90, R180, R270 }

            public FlipOrientationEnum FlipOrientation { get; set; }
            public RotationEnum Rotation { get; set; }
            public long ID { get; set; }
            public bool[,] ImageArray { get; set; }

            private Dictionary<EdgeEnum, bool[]> edges = null;

            public Tile[] GetPerms()
            {
                return new Tile[]{
                    this,
                    Rotate90(),
                    Rotate180(),
                    Rotate270(),
                    FlipHoriz(),
                    FlipHoriz().Rotate90(),
                    FlipHoriz().Rotate180(),
                    FlipHoriz().Rotate270(),
                    FlipVert(),
                    FlipVert().Rotate90(),
                    FlipVert().Rotate180(),
                    FlipVert().Rotate270(),
                    FlipHorizVert(),
                    // FlipHorizVert().Rotate90(), 
                    // FlipHorizVert().Rotate180(), (duplicate case of regular)
                    // FlipHorizVert().Rotate270()
                };
            }

            public bool CanConnectToOtherTile(Tile other)
            {
                var thisEdges = GetEdges();
                var otherEdges = other.GetEdges();

                if (thisEdges[EdgeEnum.Left].SequenceEqual(otherEdges[EdgeEnum.Right]))
                    return true;
                if (thisEdges[EdgeEnum.Right].SequenceEqual(otherEdges[EdgeEnum.Left]))
                    return true;
                if (thisEdges[EdgeEnum.Top].SequenceEqual(otherEdges[EdgeEnum.Bottom]))
                    return true;
                if (thisEdges[EdgeEnum.Bottom].SequenceEqual(otherEdges[EdgeEnum.Top]))
                    return true;

                return false;
            }

            public bool CanConnectToOtherTile(Tile other, EdgeEnum onEdge)
            {
                var thisEdges = GetEdges();
                var otherEdges = other.GetEdges();

                if (onEdge == EdgeEnum.Left && thisEdges[EdgeEnum.Left].SequenceEqual(otherEdges[EdgeEnum.Right]))
                    return true;
                if (onEdge == EdgeEnum.Right && thisEdges[EdgeEnum.Right].SequenceEqual(otherEdges[EdgeEnum.Left]))
                    return true;
                if (onEdge == EdgeEnum.Top && thisEdges[EdgeEnum.Top].SequenceEqual(otherEdges[EdgeEnum.Bottom]))
                    return true;
                if (onEdge == EdgeEnum.Bottom && thisEdges[EdgeEnum.Bottom].SequenceEqual(otherEdges[EdgeEnum.Top]))
                    return true;

                return false;
            }

            public Tile Rotate90()
            {
                bool[,] rotatedArray = new bool[ImageArray.GetLength(0), ImageArray.GetLength(1)];
                for (int i = 0; i < ImageArray.GetLength(0); i++)
                {
                    for (int j = 0; j < ImageArray.GetLength(1); j++)
                    {
                        rotatedArray[i, j] = ImageArray[j, ImageArray.GetLength(1) - i - 1];
                    }
                }

                return new Tile { ID = ID, ImageArray = rotatedArray, FlipOrientation = FlipOrientation, Rotation = RotationEnum.R90 };
            }

            public Tile Rotate180()
            {
                bool[,] rotatedArray = new bool[ImageArray.GetLength(0), ImageArray.GetLength(1)];
                for (int i = 0; i < ImageArray.GetLength(0); i++)
                {
                    for (int j = 0; j < ImageArray.GetLength(1); j++)
                    {
                        rotatedArray[i, j] = ImageArray[ImageArray.GetLength(1) - i - 1, ImageArray.GetLength(1) - j - 1];
                    }
                }

                return new Tile { ID = ID, ImageArray = rotatedArray, FlipOrientation = FlipOrientation, Rotation = RotationEnum.R180 };
            }

            public Tile Rotate270()
            {
                bool[,] rotatedArray = new bool[ImageArray.GetLength(0), ImageArray.GetLength(1)];
                for (int i = 0; i < ImageArray.GetLength(0); i++)
                {
                    for (int j = 0; j < ImageArray.GetLength(1); j++)
                    {
                        rotatedArray[i, j] = ImageArray[ImageArray.GetLength(1) - j - 1, i];
                    }
                }

                return new Tile { ID = ID, ImageArray = rotatedArray, FlipOrientation = FlipOrientation, Rotation = RotationEnum.R270 };
            }

            public Tile FlipHoriz()
            {
                bool[,] flippedArray = new bool[ImageArray.GetLength(0), ImageArray.GetLength(1)];
                for (int i = 0; i < ImageArray.GetLength(0); i++)
                {
                    for (int j = 0; j < ImageArray.GetLength(1); j++)
                    {
                        flippedArray[i, j] = ImageArray[i, ImageArray.GetLength(1) - j - 1];
                    }
                }

                return new Tile { ID = ID, ImageArray = flippedArray, FlipOrientation = FlipOrientationEnum.FlipX, Rotation = Rotation };
            }

            public Tile FlipVert()
            {
                bool[,] flippedArray = new bool[ImageArray.GetLength(0), ImageArray.GetLength(1)];
                for (int i = 0; i < ImageArray.GetLength(0); i++)
                {
                    for (int j = 0; j < ImageArray.GetLength(1); j++)
                    {
                        flippedArray[i, j] = ImageArray[ImageArray.GetLength(1) - i - 1, j];
                    }
                }

                return new Tile { ID = ID, ImageArray = flippedArray, FlipOrientation = FlipOrientationEnum.FlipY, Rotation = Rotation };
            }

            public Tile FlipHorizVert()
            {
                bool[,] flippedArray = new bool[ImageArray.GetLength(0), ImageArray.GetLength(1)];
                for (int i = 0; i < ImageArray.GetLength(0); i++)
                {
                    for (int j = 0; j < ImageArray.GetLength(1); j++)
                    {
                        flippedArray[i, j] = ImageArray[ImageArray.GetLength(1) - i - 1, ImageArray.GetLength(1) - j - 1];
                    }
                }

                return new Tile { ID = ID, ImageArray = flippedArray, FlipOrientation = FlipOrientationEnum.FlipBoth, Rotation = Rotation };
            }

            public enum EdgeEnum { Top, Bottom, Left, Right };

            public Dictionary<EdgeEnum, bool[]> GetEdges()
            {
                if (this.edges != null)
                    return this.edges;
                Dictionary<EdgeEnum, bool[]> edges = new Dictionary<EdgeEnum, bool[]>();

                var edge = new bool[ImageArray.GetLength(0)];
                for (int i = 0; i < ImageArray.GetLength(0); i++)
                {
                    edge[i] = ImageArray[i,0];
                }
                edges.Add(EdgeEnum.Left, edge);

                edge = new bool[ImageArray.GetLength(0)];
                for (int i = 0; i < ImageArray.GetLength(0); i++)
                {
                    edge[i] = ImageArray[i, ImageArray.GetLength(0) - 1];
                }
                edges.Add(EdgeEnum.Right, edge);

                edge = new bool[ImageArray.GetLength(0)];
                for (int i = 0; i < ImageArray.GetLength(0); i++)
                {
                    edge[i] = ImageArray[0, i];
                }
                edges.Add(EdgeEnum.Top, edge);

                edge = new bool[ImageArray.GetLength(0)];
                for (int i = 0; i < ImageArray.GetLength(0); i++)
                {
                    edge[i] = ImageArray[ImageArray.GetLength(0) - 1, i];
                }
                edges.Add(EdgeEnum.Bottom, edge);

                this.edges = edges;
                return edges;
            }

            internal int CountSeaMonsters(HashSet<(int i, int j)> seaMonsterMap)
            {
                var count = 0;
                var arrCopy = new bool[ImageArray.GetLength(0), ImageArray.GetLength(1)];
                for (int i = 0; i < ImageArray.GetLength(0); i++)
                    for (int j = 0; j < ImageArray.GetLength(1); j++)
                        arrCopy[i, j] = ImageArray[i, j];

                // Need to check for both all .s or all #
                var invalidSearchSpace = new HashSet<(int i, int j)>();
                var maxSeaMonsterI = seaMonsterMap.Max(e => e.i);
                var maxSeaMonsterJ = seaMonsterMap.Max(e => e.j);

                for (int i = 0; i < arrCopy.GetLength(0) - maxSeaMonsterI; i++)
                    for (int j = 0; j < arrCopy.GetLength(1) - maxSeaMonsterJ; j++)
                    {
                        if (seaMonsterMap.Any(co => invalidSearchSpace.Contains((i + co.i, j + co.j))))
                            continue;
                        if (seaMonsterMap.All(co => arrCopy[i + co.i, j + co.j] == true))
                        {
                            count++;
                            // Invalidate those spaces for other monsters
                            foreach (var co in seaMonsterMap)
                            {
                                invalidSearchSpace.Add((i + co.i, j + co.j));
                            }
                        }                      

                    }

                return count;                
            }
        }

        const string EXAMPLE_INPUT = @"Tile 2311:
..##.#..#.
##..#.....
#...##..#.
####.#...#
##.##.###.
##...#.###
.#.#.#..##
..#....#..
###...#.#.
..###..###

Tile 1951:
#.##...##.
#.####...#
.....#..##
#...######
.##.#....#
.###.#####
###.##.##.
.###....#.
..#.#..#.#
#...##.#..

Tile 1171:
####...##.
#..##.#..#
##.#..#.#.
.###.####.
..###.####
.##....##.
.#...####.
#.##.####.
####..#...
.....##...

Tile 1427:
###.##.#..
.#..#.##..
.#.##.#..#
#.#.#.##.#
....#...##
...##..##.
...#.#####
.#.####.#.
..#..###.#
..##.#..#.

Tile 1489:
##.#.#....
..##...#..
.##..##...
..#...#...
#####...#.
#..#.#.#.#
...#.#.#..
##.#...##.
..##.##.##
###.##.#..

Tile 2473:
#....####.
#..#.##...
#.##..#...
######.#.#
.#...#.#.#
.#########
.###.#..#.
########.#
##...##.#.
..###.#.#.

Tile 2971:
..#.#....#
#...###...
#.#.###...
##.##..#..
.#####..##
.#..####.#
#..#.#..#.
..####.###
..#.#.###.
...#.#.#.#

Tile 2729:
...#.#.#.#
####.#....
..#.#.....
....#..#.#
.##..##.#.
.#.####...
####.#.#..
##.####...
##..#.##..
#.##...##.

Tile 3079:
#.#.#####.
.#..######
..#.......
######....
####.#..#.
.#...#.##.
#.#####.##
..#.###...
..#.......
..#.###...";


        const string PROBLEM_INPUT = @"Tile 3167:
.##..#...#
##.#......
.##......#
#.....#..#
........##
.#.......#
###.#.....
###.....##
#..#....#.
..####..##

Tile 2411:
...#####..
#.#.......
#..#..##.#
####...###
#..#....##
#..##.#.##
#...#.#..#
.#..#..#..
..#..##.##
#....#.##.

Tile 2909:
.#.#..##.#
##...###..
#.#..#...#
........##
#.#####.##
#..##..#..
...#.#.#.#
.........#
.##..#...#
....#....#

Tile 1571:
...#.#....
...##...#.
#...#.#..#
#..#..#...
#.......#.
....#.##..
...#.#.###
#.#.#...##
....#.....
##..#..###

Tile 1499:
..##.#.##.
##.....#..
#....#.#.#
#.#..#....
#....###.#
.#.#..#..#
##..#....#
.#..##....
#.##..#..#
..###....#

Tile 3299:
....#...##
.....#..##
##........
##..#.#.#.
.....###.#
#.....#...
....##.###
##..#.....
#.........
.##..#....

Tile 1021:
#.#.#...##
#..##.#...
##..##...#
###..##.##
..#..#....
##....#..#
........#.
##.#.###.#
.###..###.
####.##...

Tile 1231:
..##..#.##
...###..#.
##........
#....#....
...#.##..#
#..#..#..#
###....###
....#..#..
.#......#.
###.#.####

Tile 2467:
..##......
##........
.....#....
###..#.#.#
.#.......#
.#....#...
###.#.....
#....#..#.
..#.#.....
#.##......

Tile 3797:
.#.####...
........#.
#....#..##
#..#...#.#
..#.......
.##..#..##
#.#....#.#
..........
....#.#.#.
..##..###.

Tile 3691:
.###....##
.....###.#
##.#.#.#.#
.....##...
.#........
#####...##
##...#...#
..#.......
#........#
..#...##..

Tile 1669:
###.#####.
..#......#
.#...##..#
#.#......#
...#.#...#
##.......#
#.#....##.
..#......#
.#..#.....
##.###.#..

Tile 3637:
..#.......
....###.##
#..#.#..#.
.....#...#
#....#....
###....###
.#......##
........#.
#.........
#..#.#...#

Tile 3203:
.##.####.#
#..###....
.....##..#
#....#...#
#.....##..
#..##..#..
....#.#...
...###...#
#.....#...
...###.###

Tile 2477:
..####....
#..#...#.#
...#...###
..#.#.#...
#.##...##.
#.##...##.
##.....###
###....#.#
#.#..##...
..#####.#.

Tile 1657:
#...#...##
##..#..#.#
#..#..####
#.##......
##..#....#
#.##.....#
#..###...#
#......#..
###....###
.####.#...

Tile 3083:
...#.##.#.
#.#......#
..........
..####.#..
......#.#.
....#...#.
#...#..#..
...#..#...
....#.....
##..#.....

Tile 1409:
..#.#.#.#.
....#....#
#.........
#..#.##..#
........##
.....#...#
.......##.
......#...
#.##....#.
.#..#..##.

Tile 3391:
..#.#..###
##........
##..#.....
........#.
..#....#.#
#.....#...
..##.#..##
#...##.#..
...###.#..
...##.##.#

Tile 3491:
#.#..###.#
..####.#..
......#.#.
#....##.##
.#..#....#
....#....#
###.#.....
......##..
..#....#..
.......#..

Tile 3331:
####..##..
##........
...##.....
#.#.......
.#.......#
....##..##
#.#....#..
#..##...#.
###.#.....
###.##..##

Tile 2687:
#....#..##
###..##..#
####..#.#.
..###..###
#.......##
#.....#...
#.##.....#
.#....#.#.
.#...##...
#########.

Tile 3769:
####.###..
.#.#.#.##.
.#.......#
..........
#....#.###
........##
..........
.....#..##
#.#......#
##.#..#..#

Tile 2579:
##......#.
.##....###
##........
#.......#.
#.#.#...##
###...#.##
..#.##....
.##..#..##
###...####
.#...###..

Tile 1607:
.#.....#..
.........#
###.......
.#...##..#
...#.#....
###......#
..#....#..
#.........
....#..#.#
.##.#.#...

Tile 2383:
..#.....##
...#.#...#
......##..
.........#
......#.#.
#...##.##.
#....#..#.
.#..##.#..
.........#
...#.....#

Tile 2153:
.###..##.#
##.#####.#
#..#......
#...##....
......#...
##...#.#..
#.#..#....
##.#.....#
..#.###...
.##.#.###.

Tile 3761:
#.#.......
#..#...#.#
......#...
.........#
...#....##
#...#....#
..#.#.....
#.........
##.##.###.
##.#...#.#

Tile 1097:
..#..#.###
...#.#....
#..#.....#
###.......
#....#.#.#
#..#..####
#.....#..#
.....#....
..#..#....
#..#.#..##

Tile 2131:
.#.....###
#......#.#
.##.......
#.....#..#
.......##.
#...#.#...
#.#.....##
#..#.#....
.#.##.##..
.#.#....##

Tile 2777:
.######..#
.##..#..##
........##
#.......#.
###.#.##.#
#..##..#.#
..###....#
##.#....##
......##..
#....##...

Tile 3373:
.##.#.....
..#....###
.##.......
#.......##
..#...#..#
.#.#.##...
.##.....##
..#...#..#
....#....#
....##.#..

Tile 2351:
#...#..#.#
#......###
#...#.....
.##..#...#
#.##.#..#.
.##...#...
##...#...#
..#.....##
#.#.......
.#...###..

Tile 1871:
###..#....
#.........
#......#..
#...#.....
#.#.......
#......#..
#..#.##.##
.....#..##
.#..#####.
.##..####.

Tile 1847:
.#..#..#.#
#....#.#..
.#..#.#..#
#...#....#
...#.#....
..#.#.#.##
..........
##......##
......#..#
##.##.#.##

Tile 1283:
###.....#.
#..#.#...#
#...#.....
.....#.###
#....##...
##........
##..#....#
..........
###..#..#.
..#.#.#...

Tile 2357:
.......#.#
..#.##....
#....#....
#........#
#...#.....
#....#....
....#.....
#...#.....
..#..#.#..
#....#.###

Tile 1609:
.###.##...
#....#.#.#
#..##.....
##.#..##..
.#.#......
....#...##
.....#..##
#.........
..........
#.####..#.

Tile 1601:
#.#..###.#
#.#...#...
#..#.....#
#.#..##.##
#.#.#....#
#.#..##...
......#...
#...##..##
#....#.##.
#.####.#..

Tile 2393:
##.##.#.##
#...##.#..
##......#.
...#....#.
.#.....#..
##........
#....#...#
#.........
...#...#..
###..#....

Tile 2677:
#...#..###
.........#
####......
....#.#...
#.#......#
#........#
##........
#........#
#........#
..####.#.#

Tile 2557:
......##.#
##...###.#
#.......##
#.#...####
.#..#..#..
#.........
.......#..
#.##...###
##.#....#.
...#..#.##

Tile 1913:
..#..#..#.
.#....#...
..#......#
.#..##...#
..#..##...
#.#.#.##..
##.......#
.#..#..#..
........##
#.#.##.#..

Tile 1229:
####..#.#.
...#.....#
...#.....#
#.###....#
#.....#..#
#..#...#.#
.#.#.##.##
....##....
..........
.#.###.##.

Tile 3067:
#.#..##...
#.##.#..##
.#....###.
#.........
.#....##..
......#...
.#..#...#.
.##.###..#
##...###.#
......#.#.

Tile 1031:
..###.##..
.#.#..#.##
#...#.#..#
........#.
..........
#..##.#..#
#..#......
.##.#...##
#.......##
.#.###..#.

Tile 2753:
.##.##.#.#
........##
##...#....
.......#.#
##.#.....#
#.........
....#.....
..###.#..#
...###....
.#.###....

Tile 2437:
..#.#.##..
........##
#.....####
#.#.......
#........#
##..#....#
..#......#
.#.##....#
#....#.#..
###.......

Tile 2399:
.####.#..#
#..#.#...#
#....#...#
##.#..##..
##..#.....
......#...
###..##...
..#..#.##.
..#.##....
.....##.#.

Tile 3169:
#...#.####
..###.....
.....#....
#.####...#
###.###...
.#.###.#..
#.......#.
#..##...##
.......##.
#....#.##.

Tile 1091:
####.#.#.#
.##...##..
..#...#..#
#........#
#......##.
......#.##
..#.....##
.....###.#
#..#.....#
.#..##.#..

Tile 2083:
#..#####..
...#.#...#
....#.#...
..#.#...#.
#..##..#..
##.#......
#....#....
#.....#.##
...#..#.#.
##..#..###

Tile 1999:
..#..##...
...#.....#
#.#.#..#..
#..#...##.
..##.#...#
##.....#..
#..##...##
....#.##..
##.#..#.#.
##..####.#

Tile 1741:
#.#..#.#.#
#.....#...
...###...#
..##.#.#..
.....##..#
##..##.#..
###.......
#....#..#.
#...##...#
##.##..#.#

Tile 2297:
#...#...#.
..#.......
##...#...#
##..##...#
##.#.....#
#..#.....#
..........
#.....#.##
#...#.#..#
#..##.##.#

Tile 1721:
#####...#.
#.#.#....#
..#.##.#.#
#.........
##.......#
.........#
.........#
#......#.#
....#.....
.....###..

Tile 1873:
.#.....#.#
.##...#..#
##........
#.........
........##
.#........
.....###.#
.#.......#
#.........
##...##.#.

Tile 1567:
.#.#.##.#.
..........
..........
##........
.#..###..#
#.........
#....#....
....##....
.##.#....#
##.##.####

Tile 2251:
##.###....
...#.#...#
#.#...#..#
#........#
...###.#.#
...#...#.#
..##......
....#.....
...#.....#
#.#..#...#

Tile 1399:
#..#...##.
..##......
..........
##.......#
........#.
.#........
#.#..#.#.#
#.#..#....
#..####..#
##..#.####

Tile 2699:
#.####.#.#
#.#.......
##...#.##.
##...#..##
#####.....
...#.....#
#.#.#...##
#.....##.#
..#..##.##
##.##.#...

Tile 2017:
.....#..#.
..#...#...
.#..####..
....#.#..#
.....#....
....#...##
#........#
..#.##...#
##.##.#..#
.##.#.####

Tile 3917:
.####..##.
#.##..####
#.....#.##
......#...
#.##.#....
#..###..##
....##.#.#
........#.
...##.##.#
.#.##.##..

Tile 3301:
.#...#..##
##..###...
#.......##
..####.#.#
.#...#....
#.......#.
.#.......#
..........
#.........
##..#####.

Tile 2731:
.#.##....#
#...#.....
##..##...#
...#...#.#
#.##.....#
...#...#..
.#........
#...#..#..
#...#...#.
.#####.###

Tile 3389:
###.#.####
#.##.#..#.
.#...###.#
#.#.....#.
.....##...
....#.#...
.#.#.....#
..#.##..##
##....###.
...##.#.##

Tile 2837:
#..#......
....#.##..
...#......
#.#.#....#
..###...##
...#.....#
....#.....
.......###
...#.##..#
###...##..

Tile 1511:
....#.#..#
..###.....
#..##...#.
#..#..#...
.......#.#
####..#...
#..#..#...
..#.#....#
....#.#.##
...###...#

Tile 2069:
..###..#.#
.###.#..#.
#.#.....##
.....#....
#.........
#.#..#...#
.#.##..#.#
..##...#.#
#####...##
..#.##.###

Tile 3181:
#####.##.#
#........#
#.##......
#.##....##
#......#.#
#....#..##
###...#...
...##..#.#
#.......#.
..#..#..##

Tile 1627:
.#.#....#.
.###....##
.##.#..#.#
.#.....#..
##..#....#
##......#.
#.#.......
###...##.#
###....##.
.####..#..

Tile 2971:
...####.#.
#..#.#....
#...#.#.#.
#..#...###
.#........
#.#.......
..#.#.#...
##...#....
.#...#..#.
..#.#.#.#.

Tile 3847:
...######.
.#..#.....
.........#
##.##..###
...#..#.##
..##....##
###......#
#####....#
#.......##
....#.#.##

Tile 3463:
.#####.#.#
#..#..##.#
......#...
#.#...###.
#...#..##.
#....#..##
.........#
..#....#.#
....#.#...
#...#.#.#.

Tile 1181:
##.#..####
..........
....#.#...
.#......#.
#....##.#.
.#.......#
#...#.....
#...###..#
..#...#..#
#.##....##

Tile 3851:
##.......#
.....#..#.
#........#
#........#
##.....#..
#..#.....#
#..#...#.#
.....#....
.#....##.#
.###......

Tile 3229:
..#......#
........##
.##....#..
.#....#.#.
...#..#.##
#...#...#.
.#.##..#..
..#.#.....
.#....##.#
.#####....

Tile 1061:
..###....#
.....##...
......#..#
#..#..#..#
#...#.#...
...#.##..#
##..##.#..
#........#
.#..#....#
#...##.#..

Tile 2377:
.#.....##.
#........#
.....#...#
#..#..#...
#....##..#
..#......#
#.......##
##....#..#
.........#
#....##...

Tile 3793:
....#.#..#
##.#...###
#......#..
#...#...#.
...#..#...
#...##...#
....##...#
#...#..###
..........
.##...###.

Tile 3919:
.#.##.....
..##.#..#.
#....###.#
#..#...#..
#..##..###
....#..#..
.#.......#
..#.##....
..#...#...
.##..#..#.

Tile 2389:
#....##.#.
...#....#.
##.#.#....
#.#...#..#
...#.##..#
...#.#...#
.#....#...
.###.....#
.##..#...#
#...###..#

Tile 2689:
#.#.#.#...
#....#....
#.....#..#
.#..#....#
.......#..
......#.#.
#...#..###
...#..#...
#.....#..#
.####.##..

Tile 1579:
#..#...#..
...#......
#....#...#
##..##...#
#...#.....
#....#....
#...#.....
....#....#
#....#...#
#...#.#...

Tile 1747:
..#####.##
....#.....
.......##.
....#..##.
###....#..
.....#....
#....##...
...#.##..#
#.#...#...
#.###.##..

Tile 2633:
.#...####.
....##...#
.......#..
##..#..#..
#....###..
#......#.#
..........
..#..#...#
.....#..#.
#...##....

Tile 3581:
####.###.#
....#....#
#...##...#
....##...#
#...##..#.
..........
#....#....
.#..##...#
#...###...
##.#..##.#

Tile 2879:
....#.####
....#...#.
##.#..#..#
#....#..##
#.......#.
.#.#####..
....#....#
#...##..##
#..##....#
..#.....#.

Tile 1297:
..#.######
..#.....##
..#.##.#..
.#........
..#....#.#
#.........
.##.......
#..#..##..
###.####..
..##..#...

Tile 1069:
..#.#..#..
...#...###
#.#.....##
.........#
..#.......
#........#
..#.#..#.#
..#..##.##
##....##..
#..#...##.

Tile 2857:
##.##.....
..#....##.
....#.....
#.##..#..#
..##.##...
###....###
.#........
#...#.....
..#...#...
.#.##.#..#

Tile 2963:
....###.##
..#.#.....
##.......#
......####
.........#
..........
...##...#.
#....#....
#..##.....
.#..###..#

Tile 2111:
......#..#
#..#.....#
#....###..
.#...###..
...#.#....
......#.##
.#........
#........#
..#...####
#.#.###..#

Tile 3187:
####.#.##.
#..#...#.#
..#.##...#
..#.......
....#....#
.#.##....#
#.##.....#
..##.....#
....##...#
#...#...#.

Tile 3209:
.....####.
.##.#.#...
...##.##..
#..#...##.
#....#....
...#.....#
#....#.#..
#.#.....#.
#.#..#....
###..#.##.

Tile 2243:
#.....##..
.#....###.
#.......#.
##..#..#..
.###.#..##
#.#...##..
..#....#.#
#..##....#
###...#.##
##....#.##

Tile 3323:
##.##..#.#
#.#..#...#
...#.###.#
.....##.#.
##.#.....#
.....#.#..
#.#.......
#..#...#.#
#.##......
#.##..#.##

Tile 3697:
##.##.####
.##......#
#.#.....#.
#.#...#..#
#.........
#.#.#....#
#...#.#.##
....#...#.
.........#
#...####..

Tile 2767:
##.####...
.#.#.#..##
...#....##
#...#....#
...#.#..#.
##.#.....#
.#..##....
...###..##
#..#...#.#
##.#...#.#

Tile 1307:
......###.
..#......#
..#....#..
#..#....#.
#.#.#.#.##
..#.#..###
...#..#.#.
#...#.#..#
..#......#
#..##.##..

Tile 1117:
.###.#...#
##.......#
##.......#
###......#
...####...
#...##....
#.##.#.#..
...#....##
..#.#....#
#...#####.

Tile 2713:
..####.#..
#.#.....##
.#....##.#
...#....##
..#..#..##
..........
...#..#...
....#.....
......#..#
..##..###.

Tile 3449:
##.......#
#....#....
..#..#.#.#
.....#....
.....#...#
#..#..#..#
..#.#....#
#..#......
....#.....
.....#.#..

Tile 3499:
...#..#.#.
#.......##
..#.......
...#..#...
.....##..#
......#..#
#.........
#..##.##..
......#...
.##...###.

Tile 1861:
####.###..
....#..#.#
..#..#....
#.#.....#.
#..##.#...
#.##.#...#
.....#.#.#
##.##.....
#.........
#.#..#.#.#

Tile 3833:
.###...#.#
#..#.....#
.#........
##.......#
###.#....#
.....#...#
.....##..#
.....#.#..
##..####.#
####...##.

Tile 1237:
##..#.##..
..##.#.###
##.#.....#
###..#####
.##....##.
.#......##
.....#...#
#.##..##..
.....#.##.
..#..#.##.

Tile 2003:
.####.....
.##.......
#..#..#...
#.##...#.#
..#...#...
#..#.#..##
#..#.#...#
#.......#.
..##......
##........

Tile 2113:
.#.#.#....
#...#.#..#
.........#
.#..#..#..
#....###.#
#.#.......
#........#
..#.....##
........#.
#..#.##..#

Tile 2447:
#.##.####.
#...#.....
#.......#.
.....#..##
..###...#.
#..##.##.#
.#........
#....#..#.
#..#..#...
.####..#..

Tile 2347:
.###.#..#.
.......#..
..#.......
..#.###.##
#...##....
....#.....
#....#.#.#
...#.....#
#........#
...#....#.

Tile 1447:
..#..#.###
..##.#...#
#####...#.
#...##..##
...#......
.#.#####..
....#.....
.....#...#
#......#..
#...#.###.

Tile 3671:
....##..#.
##.#..#.##
.##..#....
###...####
..#....##.
#...#....#
.........#
..#......#
..#...#...
###.#.###.

Tile 2851:
.#..####..
.....#....
.#.#......
.#.##.#...
#........#
.#..###...
#.#...##..
#.#.#.....
.#........
##...##..#

Tile 2161:
#.###.#.##
......#.#.
.#...##.##
#.#....###
#.#...#..#
....#..#.#
.#........
...#..#.#.
......####
.###..####

Tile 3469:
##.#.###.#
#...#.##.#
.......##.
#......###
##.#......
#.#..#..##
..#.#.....
..##....##
..#.#...##
#....##..#

Tile 2293:
#.###...##
.##....##.
##..##.#.#
....#.....
#.#..#....
##.#..#..#
...###....
....#.....
#..#..#...
.#.######.

Tile 3907:
...#.#.##.
....#....#
...#.##...
#..#...#.#
.....#...#
#...#...##
.##....#.#
.#.....#.#
#..#...###
.##..#...#

Tile 2143:
.###....##
#.#.#...##
.....#.#..
......#.##
...#..#.##
###..#...#
......##..
.#...#..##
.....#...#
.##..#####

Tile 1583:
...###.##.
..#......#
##..#..##.
#.##...##.
#.#....#.#
..........
#.#......#
.#.##..##.
#.####.##.
#...###.##

Tile 2141:
#....####.
.###.#.#.#
.........#
..##.##..#
..#..#..#.
##..#.##.#
#.....#.##
..#..##...
..#####...
##...###.#

Tile 3511:
.#..##.#.#
..#.#..##.
#.##.##...
..##.##...
#..#.##..#
.##....###
.....#...#
........##
#........#
####......

Tile 2789:
#.#..#....
#.#..#....
#.#...##..
..#..##...
#.##.#...#
.#.....#..
####.##...
.....##..#
#.....#.#.
#.#.##.#..

Tile 1051:
.....#.#..
##........
......#...
##......##
#........#
#....#.#.#
#.#..#...#
#..#....##
#.#.....#.
.#..###...

Tile 1151:
.....##.##
.#..#...##
.#.......#
..##...#.#
#......###
#....#.##.
#..#......
#.#.....##
.##..#..##
.#.#...###

Tile 3527:
##..##.#..
.#..#....#
.....#.#..
..###....#
###......#
#....##..#
...##...#.
##..#.#.##
...#.##..#
.###.#..#.

Tile 1223:
#####..##.
...##.#...
...##...#.
....#.#.##
...#......
##.#..#..#
.##.###...
#..#.#.#..
..##..##.#
.#.##.#.##

Tile 3109:
#.##..#...
###...##..
...#.#...#
#......##.
###.......
#.....#.#.
#....#....
#..#.##..#
....#....#
.#....#...

Tile 1787:
........##
....#.####
#...#..#.#
#....##..#
.#.##.#..#
.#.#..#..#
......##.#
#..#.#.#.#
......###.
#.#..#.#..

Tile 2833:
.#..#.##..
#.#.#....#
#.........
...#.#..##
.......##.
###.....##
..##.....#
#.#....#..
.#......##
.#.#.####.

Tile 1783:
#.#..#.#..
...#...#.#
##....##..
##...#..##
......#...
#.....##..
.....#...#
.#..#.##.#
.#.......#
..#..###.#

Tile 2711:
#.#.###.##
......#.##
..##......
#.#.....#.
.#.#.....#
.....#....
#..##...#.
###..#.##.
#..##.....
#####...##

Tile 2203:
#.####...#
......#..#
#..#......
..####...#
.#...##.##
#........#
#.#...##.#
##........
###..####.
##....#.#.

Tile 1901:
.##...##.#
....##....
#..#.#...#
..#..#...#
..........
#.##.#....
##........
...#......
.....#..##
..##...###

Tile 2591:
.....#..#.
.......##.
...#.#....
#........#
##.#.#..#.
.#.......#
##...#...#
.....#...#
#.##......
###.##.##.

Tile 2371:
##.####.#.
#.#..#....
.##..###..
..#..#....
...#.#....
...#..#...
......#.##
.#.......#
#....#.#.#
...#####.#

Tile 1907:
..#.###...
...#..####
#..#.#..##
#....##.#.
.#.#.....#
...#....##
.....##...
#.....#..#
..#.#..#.#
.##.####.#

Tile 1531:
###.#.#.#.
#.........
.....#..#.
..#.#.#..#
#.#.#.#.##
......##..
..........
##........
#.#......#
.#.#....#.

Tile 1619:
..#..#....
..#.#..#.#
#.#.#.....
#....#....
..#......#
#####....#
##.......#
#...#.....
#.....#...
.#..#####.

Tile 3541:
#.#....##.
#...#..#..
...#..#.##
#.####...#
..#.#....#
.#........
.#.......#
#....###..
...###...#
.####....#

Tile 1523:
#.#...##..
#..#..#.#.
...#..#...
..#.#.#...
.....#....
###.#....#
#.......##
.......###
........#.
#.#.#.#.##

Tile 1777:
#.#..#.###
..#..#..#.
##..####.#
#...##...#
#...#...#.
........##
......##..
...##...##
.##.#...#.
...##.#...

Tile 3019:
.#..####..
....#....#
#....#....
..##.....#
##.......#
#.#...#.#.
#..#....##
#....#..##
....#...#.
#......##.

Tile 2053:
.##..#....
.....#....
#.#..##...
....#.#.#.
#..#.#..##
#.#.......
#.#.##.#..
#.#..#....
##...##..#
..##.###..";
    }
}
