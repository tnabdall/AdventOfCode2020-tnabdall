using System;
using System.Collections.Generic;
using System.Linq;

namespace Day24
{
    class Program
    {
        static void Main(string[] args)
        {
            Part1(PROBLEM);
            Console.ReadLine();
        }

        private static void Part1(string input)
        {
            // Parse input to a list of moves
            var lines = input.Split(new char[] {'\n','\r' }, StringSplitOptions.RemoveEmptyEntries);

            var allMoves = new List<List<string>>();
            foreach(var line in lines)
            {
                var moves = new List<string>();
                for (int i = 0; i < line.Length; i++)
                {
                    if (line[i] == 'e' || line[i] == 'w')
                        moves.Add(line[i].ToString());
                    else
                    {
                        moves.Add(line[i..(i + 2)]);
                        i++;
                    }
                }
                allMoves.Add(moves);
            }

            Dictionary<(int x, int y), bool> flippedTiles = new Dictionary<(int x, int y), bool>();

            foreach(var moves in allMoves)
            {
                int x = 0; int y = 0;
                foreach(var move in moves)
                {
                    switch (move)
                    {
                        case "e":
                            x += 2;
                            break;
                        case "w":
                            x -= 2;
                            break;
                        case "ne":
                            x++; y++;
                            break;
                        case "nw":
                            x--; y++;
                            break;
                        case "se":
                            x++; y--;
                            break;
                        case "sw":
                            x--; y--;
                            break;
                        default:
                            throw new Exception();
                    }
                }

                if (flippedTiles.TryGetValue((x, y), out var flipped))
                    flippedTiles[(x, y)] = !flipped;
                else
                    flippedTiles[(x, y)] = true;
            }
            Console.WriteLine(flippedTiles.Count(x => x.Value));

            for (int days = 0; days < 100; days++)
            {
                var xMin = flippedTiles.Keys.Min(x => x.x);
                var xMax = flippedTiles.Keys.Max(x => x.x);
                var yMin = flippedTiles.Keys.Min(x => x.y);
                var yMax = flippedTiles.Keys.Max(x => x.y);

                List<(int x, int y)> tilesToFlip = new List<(int x, int y)>();

                // Check every tile from min - 2 to max + 2
                for (int i = xMin - 2; i <= xMax + 2; i++)
                {
                    for (int j = yMin - 2; j <= yMax + 2; j++)
                    {
                        bool isFlipped = flippedTiles.TryGetValue((i, j), out var flipped) && flipped;
                                                
                        var neighbourCoordinates = new (int x, int y)[]
                        {
                            (i - 2, j),
                            (i + 2, j),
                            (i - 1, j + 1),
                            (i - 1, j - 1),
                            (i + 1, j + 1),
                            (i + 1, j - 1)
                        };

                        var neighbourFlippedTilesCount = neighbourCoordinates.Count(x => flippedTiles.TryGetValue((x.x, x.y), out var flipped) && flipped);
                        if (isFlipped && (neighbourFlippedTilesCount == 0 || neighbourFlippedTilesCount > 2))
                            tilesToFlip.Add((i, j));
                        else if (!isFlipped && neighbourFlippedTilesCount == 2)
                            tilesToFlip.Add((i, j));
                    }
                }

                foreach(var tile in tilesToFlip)
                {
                    if (flippedTiles.TryGetValue((tile.x, tile.y), out var flipped))
                        flippedTiles[(tile.x, tile.y)] = !flipped;
                    else
                        flippedTiles[(tile.x, tile.y)] = true;
                }
            }
            Console.WriteLine(flippedTiles.Count(x => x.Value));

        }

        const string PROBLEM = @"weseswseswesewseseseswewswseswnenwsese
nwswwwnwwwswswseswseswwsewswnewsw
wnwnwsewwnwenwnwnwwwnwnwnewnwswne
nwnwnwewnwnwswwnenwsenenwnwnwnwenwnwnw
swswewswewneseeseswseewwsewnwne
swwnenenenwswsweneswswswswneewwsewnw
neswewseswswseswswswswneswwswsenewsese
nwwnenenwwnwnewnwsenenwsenwse
eswwnwswswswwneswwwwewnwnwswsewse
sewseneeesesewseseswnesesewseseeese
nwweneseeswnesewsesenwsese
swswswneswswswswwsw
nwnwnwnwnwswnwwnwnwnenwewnwsenwnwese
enwwneneseseswewneeswsenwneswneseenw
enwswesenwnwseenwseseeeswnwnwneseswnwe
eswwwswneneesenwenenenwswswnese
wnwweseswnwneenenewswwnweswwsesw
nwneswneeeswewwneseneseswswwwsenenw
nwweweeeswnwswesweneenweneeew
swseswswwwseswwwwwwwwnwewwne
nwsewsenwnwwwnenw
neswswneeeeeeneenwwnwneewseswwse
seesesewseswenwsesewneeenenwswnesew
eeenweeesweeeee
swnwnwnwswsewnenewswwwweswwsewswe
seneenwnwwnwswwnwnwnwwnwnwewwnwnw
swswneenenwwewswnwwswnewwnwswwww
neswenenwnwnenenenesenwswswnweswwsewnenw
eeseeeenweeseesesenwe
nenwneneneewneesenesenenenene
eenesewseseewe
nwnwwwwwwswswwneweeewswwwww
wwnesenweseneneeeneswswswnesenwnwe
swseseswenwsesesenweeswsesesewswnwnwnw
newswnweswswseswnenenene
neneeneneeeneeeswneeneesewwnwsenw
neeneswwnwnenwwneenwneseswneenewe
nenwswseswneswswswneswswswswwsewsweseww
sesewseeseesesesenewsenwnwswewsenew
swswneeseswswwwwswwneswswnwswswswsenw
swswswseswswneswswseswseswsweww
swwneswneswwseseenwswswswswswsenwswswsw
seswseesenwewsesesw
sweneswnwenewneseneenenwnwneseswseenw
nwnenenwnwnewenenwsweswne
wwnewwwwwwewewnwwwwnesewse
nwnwsewnwnwweswnenwnw
swneeneneeneneneneeenenee
eneweseeesenwwweeeeeeweene
nenwnwswnwnwnwnwnwnwnwnwnwnenwnwsese
nwneneesweneeeneenenene
eneesweneneenenwneesenwneneeneswnw
seswnwswneswswnwswswseseseswsesenwseesese
swwenwenwnwnwnwnwswwnwneswwnwneeenw
wseeseneneswwwnewnenenenwswwswswwsw
eneenwnwnenwwnwnwnwnwnwswnwnwnwnwsew
nwnwswswnwnwnwnwweweenwwnwnwnew
swenwsenenwswwneenenenwswnenenenenwnenenw
swnenwwneseswswwswneswwnewswswesenw
nenenenenwnwnesenwswnwenenwnenwenwneswne
wenenweeswsweswnwseswneeneneswswnw
seesesesesweseneswswswwnwwswsenewswsw
newwwsenwnwswwwnweswnwnewnwwsenese
senwwnwnenwnenenwnenenwnweenwnewwnw
eneswwseenenwnenwswswseeneenesesew
eseeeswwenwenweswseneeenenenene
seeswnweeswneeeeneswneneneneeeew
sewswwswwswneswseswneswnenwwswswseww
seseenwseswsesenwnwnwwseseesesese
swswseneswneswswswswsweswswswwswswnwsw
neeseeseneneenweewe
swwsenenenenwneneneneneneneeneneneswne
enwnenwnwneewswseenenenwswswnwnwenwnwsw
neswenwneeeeeeee
neesenesewswnwswsesewswseswsese
nwnwnwnewsenwnwsenesenwneswnewnenwnenw
eneswsenenenewene
wsenwnwnenwnwswwnwnwenwnenwswsenewwnw
nenewswneswsenwenwnwnesenwnwneeswneene
swneseseneswnesenesewwswesewseseswse
eneeewnewwwswnewesenwseseweswsene
neesweneswswneeeswneneeeeneenenw
wsenwnwwnwnwnwnwwwwww
sewwsewswwwnwwnwenwwnewwnwnwe
neeneeneeswneeeneee
swseneseneseseswnenwswneswenenwseswww
seseswswswwsewswswnewsesweneswnwswesw
seswswsenwswneswswswnwwsewnwwwswwse
swsenwswnwwswswnwsenwneneenesenwnenee
swnwswsewswswnewswswswswseswswswseswe
neeeeneneneneneweeene
sesweneseswnwneswswnwwnwneewenesee
sewswsesenesewseseseneswseseswnesesesw
nwswwswwswswswswswwneswswwswe
enwnewnwswnenenwwseswnwswsenwnenwene
esesesenweseeseseseeenwseseseswsenw
neeneneneswseewneswnwnenwnwnesenenesee
swswwwswswswswesww
wwwswnwswswenesewenweewneenwene
eseeenewwwwnenenewwneneneseewsese
wswsesweswswswnewswnw
neeeeneenesweeeswswneeeenwsenw
sesesenewnwswsenwnwnwwnwnwwwwseswsese
seeesesweeeseeenwenwsweneseewee
wneweswswwneenwwwsenewsewwwsw
nenewneneneneneenenenesewnenenene
seswswnwnenwnwnenwnenenenewnesweeswse
swesesesewnwseesenwswnwe
swewswwesesewswwnwweneenwswsww
seeswewswnwseeseseseesenwnwnwenwseese
neneswswwwseneswwneswswswswwswsesesw
seenwweeswenesweneeeneeeenweene
swwesenwswwenenwneewwsesww
nwwnwneeenwswnwenwnewwwnwnwswnwnw
neswneneneneneeneeenwneneswnenewnesene
eseseenesenwswsesenwseseeseseeswnese
neseswswenwnewneneswnenwneseswsweswsww
swsenwseneswswnesweeswswswswswswswswsww
eseeewseeeseneseeeseeewneseww
nwnwswwnwnwnwnwnenwwww
wwwwwnwnwwwneseww
seeswseneseeeseseseswwneeeseesesenw
wnwewnwnwwwnesewwnwnw
eesewsenwsesenesee
sesesewesweneewnweneesesese
nwseewseneswseseseeseewsenenweswese
wneswwswnenwwswseeswwwnw
nesesesewneseswswwswswswswesenwnwnwsesw
nwnwnenwnwnwnewnwwswnwewnwnwswenwnw
swseeswwnwnwsenenenwswwswseseswswsesesesw
swseseseeneewsesesewneesewseseeese
sewwsesenwwewnenenwneneenewnwnwe
nwwenwwnwnenwnenwnwnwsene
nwenwnwewnwwesesenwswwnwnenwwwne
senwsewwnwwwwwseenwswwnwwneese
wwwswwnwswsewewewwnewewswsww
swneeeenenwnwneswese
neswwwwwnweseenwneenwswswseeswnw
eswnenwswneeeswseneswneenenwnenwee
nwnewneswswwneeseseseswseneeneeseesw
neeneeeeweneswenweseswee
eseswnwwswnwsweseseswseswswsesesweswsw
seenwenwseswsesewweseseseweswse
eenweseswewneswwnwwenwwwenwsw
swseseswwwswwnenewwswswnwnesesw
wwnwswwswsewneneewnwnwwswnwwwwse
nenenesenenwnwnenenw
nenwwneneseneneswneswnenenweneneswne
nwsenewwnwnwnewwwswswswseeswwwswsee
swnwnwnwswneseeenesweeewesweeeee
swseseenwswswswwnwswwswsweswse
nwswnenenwneswswnenenwswnwneswswnweneneene
swswenwsenesewswswnwwenwswnwneswswswsw
wwswswnwwneswswswneeswsw
nwsweswwwwswwswwnewswweswsewswne
seewseswswnwwswsenenesweeswswwswnwnenw
neenwswseeswswsweseeneneeneewsewe
weenwsesesenwnwseseswseseseswnesesee
seswswseswswswnewsesw
nwnwnwwnwnwnwnwnwenwnwnwnw
neswneswseneswneneneneneneneewneneenw
swwnwneswnwswswwswswswsesewneeswesee
nenwnewnesenwswnwnenwnenwe
esenwswsesenesewseswseseswseswsw
eseweeeeewneeeeeeewene
seswnwneswswwseswsewsweseswswswseswse
nwwnwewwnwwnwswnww
nwseseswseswswswswswsweswswsw
enesenesenewswsenwneswnenwnewnwnwnenwnw
seswswnwneswsweswswswswswnwswswweswswesw
wseswswwwwsenenesweseeseswse
nwnwnewwenwnwswseew
nwwnwsenwnwnwnenesenwnenwnwnenwnwnwnwse
wwnwsewnewwsewnwnenwsenwwwwnwww
wwwewwwwwwwwwwwswnwe
seseesweenwenwsenwswsenwseeeseseswese
swwenwnwnwnwnwnwwnwnwnww
nwnenwneneseneneneswenenenenwnwsenwnew
nenwsenwnenenesenenenenenenwnwnwneseww
nwwnwnenenesenenenesenenwnenenenesewe
neneewneswnwsewnenenwnene
nenwneewswnwnwneneewneneneenwnenenw
nwwwnwwnwnwseswnwwwsenwwneswnewewne
esweneenenwsewwswswswswwseswsewnwne
wseenwseswnesenwse
eeeswenwesenesenwewswne
neswseseseneswseswseseneswseseswsenesesese
wsewewwnewwnenwsenewwwwswswsw
wneeeswewsesenenwsewnwsewnewseese
swnweewnwwseeswnenwseswneswwnesese
sesenwewsesewswsweneneseeseeesene
wnwenwwwnwewsewwwwswsewwww
seneenenwnweeeeeeneeseenwnwswse
wsenewwwwwnwwwnwnwnww
nenwnwenwwwnenewwneenenwesenene
seeeswsweseesewenweeseseeneenee
sesewenwseeeeseewswnweeeseeenw
swenwnwswnwnwseeneswnwnene
swswswwswneesewswwwneeneeweswsw
swwnweeswnesenenweseseeesesewseese
sweseswwwswwneenwwwswe
wneneneeesenenenwneseeneneneeneswne
swwesesewneswwenwwswwwwwwwwnwsw
swenwswswswswswwwwnwnwnwswewswswee
senwswnwseeeneeswse
swswnwesweswswsweswswswwswnwnwne
neswneeneneneneneneeneene
eneseeseeswenweeweseseswseeenese
seswnwseseswswenesenenesewsewswswswse
wsesenwneseseseew
swwwswwewswwswsww
nwswswnwswswsweswsweswswewsenwswnene
eeeeeewnweeeweeseeswnwneseee
nwwwnwnwwsenwnenwswenwnwnwnwnwnenwnwe
seswsenwnwwseeseseneeswswswswswswnenw
swwneeeeenenweswe
wwwwnwwwnwswnenwsewwewww
swnenwnenwnwnewswnwnwenenw
seewswsesenwswneeee
nwwwewwwenwwwwweswwwnwnwnwse
wwnwwwswnwewwwswwswnwenewww
seswnenwneswnwsweenwnweenwwwweswnw
neswneseenesweenenenew
nenenwwneneneeeneneenenwswewesweene
swswswswswneswswswnwswseswswwneeswesw
seeweeeeseseeneeswnwewnwesee
swnwewswswnwswneswswswsesweneswswswsw
swnewneswnwswseswswswswsweswswswneswsw
swnwswneneneewnwwenwse
sewwwnewwswwwwwsw
swnwswnewnwsesewsewnenwnwsenwsenwenw
nwnwnwnwsenwsenwnwnwnwnenwnenwnwswneswnwnw
senesesesesesesewsenwswseseneseeseswwse
sesenenweneseseseseswwnewseewsenww
wneneswnenenenenenesenenenenenenesenwnw
swswneenwnwnwnwwnwnwnwnwnenwnwsenwsenwnw
wwnwwneswswwwswswswswswesew
wwnwseswnwnewwswwwewsewwsewene
wswseeswswswswswnwwneeswswswwwswswsw
wwnewesesewwseenwnwswnwnwnwnwnwse
neeneswewwswneswnenenenweenewnenwsee
swenwnwsesweenweeeneneneswewseese
sewwseweseseeseseneseesesenwnewse
nenenwnwneswneneneneseeneneneswnenene
newseeeneneeeeneeneenenw
neeneneswswnwnwneneneneseneesewnwnene
seseenwseseeswseneseswsenwseesesesee
swswswsesenwswnwswseswneswseswswswseseswnw
eneneneneenwesweweeeeswwenese
enenwweeneneneneswswnenenenenenenene
sewswswnwswswneseseneneswswseswseswswsw
sesesenwwnwnwwnwwwwenenwwnww
seseseswswswseenwseseswsweseenwnwsesw
wnwswseswsenwsesenwnwesesweseesenenese
sesesenwseseseswseseseneseesewseswnww
seswseeseswnwnwseewenwswseeseswseswse
seseswseneswneenwwseesenewswnwswwswse
wnwneneseswnenewnwnenwneneenenenewsenene
neneseseeeewwewsweeenwnwneswee
nwwwewwewww
newnwswwesesewnenwneswnewnwseneesew
nwwwwewewwswwnenwsenwnwnwnwwww
neseneswnenwweeneseewswseeneeew
newwsenwsesesesweseewseseseneseseew
neneneswewneneneneeeewnenesene
enenwswseeneseseeseweseee
wnenewwswewseeswswwnwwswwwwsw
seswnwnweenwsweenwneseeneeenenwneswe
newwwwswswneswwwswsww
wwwwsenwswwnwewneswwww
neswnenwnenenwnenwswnenenwswnenwnw
wsesenenwnenwnwnwnwneswwnenenwnwnwene
neswswwwwnewswswwswswwnwsenewnwswee
eswwsenenwneeeseseweeeswswenese
newseenenweesweeseesewsesenwseesw
neswswsenesweseswwwwswsweeswsenwswsw
swnwneswnenewenenenenenwenwneneenesw
esweswswnwswswsewnweeewwwseeswnw
nwnwwseweswnwswnwwewnesewwnenenwnw
senwnwsenwnwnwnwseswwnwnenwnwnwnwenenw
swnewswwwwwnewwwwnwneewwwsew
neneneeeseneneneesewswewneeenee
wswsenewneenesewnenwnenwsenwneswswnene
neewwnwnenwnenesesenenwnenwsenenwneswnee
seswnwneenesenwnenenwswnewneneneenee
nwnwwnenwswwwnenwnwnwneswnwswnwnwsenwnwne
swwwnewsenesenwsenewseeswnenesenwwsw
senwseseseneswseenwwwse
seseswseseseseseseswswseenwswsesenw
nenesenenwswwsewnwswnwneneeewnwene
swswswwswswswswswswwneswsw
sweneseesenwesewnewnwseeseswnwesese
nwnwswswswseswswsweswsw
eswswwswswwnwswwswwswswneswswnwsesw
newwwwwwwewwsenwnenwewsenenwsw
nweswweenwnwswnwenwwnwswnwnwnwnwnwnw
senwwswnenwseswswswnewneeswwwww
senwseswswnweneeswseswswwseswneswswswsw
weswwsewnweeewweneswnesewsene
neeswswnwnwnwswnwswneewnwnwnwnwswnenww
nwswwneewnesenesenenwnenenenwnwnwnwnenw
sweseeneseeneswenwneswswnwesee
nesewnwnwwnwnwnwnwnwnwnwnenwnwsenwnw
newneeenweeeeneneseene
wesesenwneenenwneswwneneenweswsene
eswnwswnwsenesweneswswsweswswsenwswsesw
swsesenwseswswseswswewsenwswsesene
weseeseeswswweseeneseseneeseenwne
wswswswswwsenwswnewneswneswswwsewwsw
eseswnwesenwnwwnwswnwsweneeeneesesew
wnwwswswwwnewswsenwswswsesweswnww
eeeneeneswewseeeneeweneesww
wnenwswwseewwswnewwnewwsenwnwsew
nenwswenenwsenenwsenwnwnwnenewnenenwswsw
neswnewweeeeeeeneneenenenenwsee
ewnweeeeswswseeeenweeeeenwe";

        const string EXAMPLE = @"sesenwnenenewseeswwswswwnenewsewsw
neeenesenwnwwswnenewnwwsewnenwseswesw
seswneswswsenwwnwse
nwnwneseeswswnenewneswwnewseswneseene
swweswneswnenwsewnwneneseenw
eesenwseswswnenwswnwnwsewwnwsene
sewnenenenesenwsewnenwwwse
wenwwweseeeweswwwnwwe
wsweesenenewnwwnwsenewsenwwsesesenwne
neeswseenwwswnwswswnw
nenwswwsewswnenenewsenwsenwnesesenew
enewnwewneswsewnwswenweswnenwsenwsw
sweneswneswneneenwnewenewwneswswnese
swwesenesewenwneswnwwneseswwne
enesenwswwswneneswsenwnewswseenwsese
wnwnesenesenenwwnenwsewesewsesesew
nenewswnwewswnenesenwnesewesw
eneswnwswnwsenenwnwnwwseeswneewsenese
neswnwewnwnwseenwseesewsenwsweewe
wseweeenwnesenwwwswnew";
    }
}
