using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Day11
{
    class Program
    {
        static void Main(string[] args)
        {
            var seatingLayout = ParseInputAndCreateSeatingLayout(PROBLEM_INPUT);

            // For part 1
            //seatingLayout.CanChangeSeatRules.Add(seatingLayout.CanChangeSeatsRule1);
            //seatingLayout.CanChangeSeatRules.Add(seatingLayout.CanChangeSeatsRule2);

            // For part 2
            seatingLayout.CanChangeSeatRules.Add(seatingLayout.CanChangeSeatsRule3);
            seatingLayout.CanChangeSeatRules.Add(seatingLayout.CanChangeSeatsRule4);

            var occupiedSeatsCount = seatingLayout.GetOccupiedSeatsCount();
            while (true) {
                seatingLayout.ChangeSeats();
                var newOccupiedSeatsCount = seatingLayout.GetOccupiedSeatsCount();
                if (occupiedSeatsCount == newOccupiedSeatsCount)
                    break;
                occupiedSeatsCount = newOccupiedSeatsCount;
            }

            Console.WriteLine(seatingLayout);

            Console.WriteLine($"Occupied Seats: {occupiedSeatsCount}");

            Console.ReadLine();
        }

        static SeatingLayout ParseInputAndCreateSeatingLayout(string input)
        {            
            var rows = input.Split(new char[] { '\n' }).Select(e => e.Trim()).ToArray();
            GridSpot[,] spots = new GridSpot[rows.Length, rows[0].Length];
            for (int i = 0; i < rows.Length; i++)
            {
                for (int j = 0; j < rows[0].Length; j++)
                {
                    GridSpot spot;
                    if (rows[i][j] == '.') 
                    {
                        spot = new Floor(i, j);
                    }
                    else if (rows[i][j] == 'L')
                    {
                        spot = new Seat(i, j);
                    }
                    else
                    {
                        throw new ArgumentException("Unrecognized character " + rows[i][j]);
                    }
                    spots[i, j] = spot;
                }
            }
            return new SeatingLayout(spots);
        }

        abstract class GridSpot
        {            
            public int SeatRow { get; }
            public int SeatColumn { get; }

            public GridSpot(int row, int column)
            {
                SeatRow = row;
                SeatColumn = column;
            }
        }

        class Floor : GridSpot
        {
            public Floor(int row, int column) : base(row, column)
            {            
            }
            public override string ToString()
            {
                return ".";
            }
        }

        class Seat : GridSpot
        {
            public bool IsOccupied { get; set; }

            public Seat(int row, int column):base(row, column)
            {
                IsOccupied = false;
            }

            public override string ToString()
            {
                return IsOccupied ? "#" : "L";
            }
        }

        class SeatingLayout
        {
            public GridSpot[,] Spots { get; }
            public int Columns { get; }
            public int Rows { get; }

            public List<Func<Seat, bool>> CanChangeSeatRules = new List<Func<Seat, bool>>();

            public SeatingLayout(GridSpot[,] spots)
            {
                Spots = spots;
                Rows = Spots.GetLength(0);
                Columns = Spots.GetLength(1);
            }

            public void ChangeSeats()
            {
                List<Seat> seatsToSwitch = new List<Seat>();
                for (int i = 0; i < Rows; i++)
                {
                    for (int j = 0; j < Columns; j++)
                    {
                        if(Spots[i, j] is Seat seat && CanChangeSeatRules.Any(rule => rule(seat)))
                        {
                            seatsToSwitch.Add(seat);
                        }
                    }
                }

                seatsToSwitch.ForEach(e => e.IsOccupied = !e.IsOccupied);
            }

            public bool CanChangeSeatsRule1(Seat seat)
            {
                if (seat.IsOccupied)
                    return false;
                int row = seat.SeatRow;
                int col = seat.SeatColumn;

                if ((row - 1) >= 0 && (Spots[row - 1, col] as Seat)?.IsOccupied == true)
                    return false;
                if ((row - 1) >= 0 && (col + 1) < Columns && (Spots[row - 1, col + 1] as Seat)?.IsOccupied == true)
                    return false;
                if ((col + 1) < Columns && (Spots[row, col + 1] as Seat)?.IsOccupied == true)
                    return false;
                if ((row + 1) < Rows && (col + 1) < Columns && (Spots[row + 1, col + 1] as Seat)?.IsOccupied == true)
                    return false;
                if ((row + 1) < Rows && (Spots[row + 1, col] as Seat)?.IsOccupied == true)
                    return false;
                if ((row + 1) < Rows && (col - 1) >= 0 && (Spots[row + 1, col - 1] as Seat)?.IsOccupied == true)
                    return false;
                if ((col - 1) >= 0 && (Spots[row, col - 1] as Seat)?.IsOccupied == true)
                    return false;
                if ((row - 1) >= 0 && (col - 1) >= 0 && (Spots[row - 1, col - 1] as Seat)?.IsOccupied == true)
                    return false;
                return true;
            }

            public bool CanChangeSeatsRule2(Seat seat)
            {
                if (!seat.IsOccupied)
                    return false;
                int row = seat.SeatRow;
                int col = seat.SeatColumn;

                int numChangeSeats = 0;
                if ((row - 1) >= 0 && (Spots[row - 1, col] as Seat)?.IsOccupied == true)
                    numChangeSeats++;
                if ((row - 1) >= 0 && (col + 1) < Columns && (Spots[row - 1, col + 1] as Seat)?.IsOccupied == true)
                    numChangeSeats++;
                if ((col + 1) < Columns && (Spots[row, col + 1] as Seat)?.IsOccupied == true)
                    numChangeSeats++;
                if ((row + 1) < Rows && (col + 1) < Columns && (Spots[row + 1, col + 1] as Seat)?.IsOccupied == true)
                    numChangeSeats++;
                if ((row + 1) < Rows && (Spots[row + 1, col] as Seat)?.IsOccupied == true)
                    numChangeSeats++;
                if ((row + 1) < Rows && (col - 1) >= 0 && (Spots[row + 1, col - 1] as Seat)?.IsOccupied == true)
                    numChangeSeats++;
                if ((col - 1) >= 0 && (Spots[row, col - 1] as Seat)?.IsOccupied == true)
                    numChangeSeats++;
                if ((row - 1) >= 0 && (col - 1) >= 0 && (Spots[row - 1, col - 1] as Seat)?.IsOccupied == true)
                    numChangeSeats++;
                return numChangeSeats >=4;
            }            

            public bool CanChangeSeatsRule3(Seat seat)
            {
                if (seat.IsOccupied)
                    return false;
                IEnumerable<Seat> nearestSeats = GetNearestSeatsInEachDirection(seat);

                return !nearestSeats.Any(e => e?.IsOccupied == true);
            }

            public bool CanChangeSeatsRule4(Seat seat)
            {
                if (!seat.IsOccupied)
                    return false;
                IEnumerable<Seat> nearestSeats = GetNearestSeatsInEachDirection(seat);
                return nearestSeats.Count(e => e?.IsOccupied == true) >= 5;
            }

            private Seat GetNearestSeatInDirection(Seat seat, int rowIncrementor, int colIncrementor)
            {
                if (rowIncrementor == 0 && colIncrementor == 0)
                    throw new ArgumentOutOfRangeException("Will be stuck at seat");

                int currentRow = seat.SeatRow;
                int currentCol = seat.SeatColumn;

                while ((currentRow + rowIncrementor) >= 0 && (currentRow + rowIncrementor) < Rows
                    && (currentCol + colIncrementor) >= 0 && (currentCol + colIncrementor) < Columns)
                {
                    currentRow += rowIncrementor;
                    currentCol += colIncrementor;
                    if (Spots[currentRow, currentCol] is Seat nearestSeat)
                    {
                        return nearestSeat;
                    }
                }

                return null; // If nothing found
            }

            private IEnumerable<Seat> GetNearestSeatsInEachDirection(Seat seat)
            {
                var directions = new List<(int rowDirection, int colDirection)>
                {
                    (-1, 0),
                    (-1, 1),
                    (0, 1),
                    (1, 1),
                    (1, 0),
                    (1, -1),
                    (0, -1),
                    (-1, -1)
                };

                var nearestSeats = directions.Select(e => GetNearestSeatInDirection(seat, e.rowDirection, e.colDirection));
                return nearestSeats;
            }

            public int GetOccupiedSeatsCount()
            {
                int count = 0;
                foreach (GridSpot spot in Spots)
                    if ((spot is Seat seat) && seat.IsOccupied)
                        count++;
                return count;
            }

            public override string ToString()
            {
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < Rows; i++) 
                {
                    for (int j = 0; j < Columns; j++)
                    {
                        builder.Append(Spots[i, j].ToString());
                    }
                    builder.Append('\n');
                }
                return builder.ToString();                
            }
        }

        const string PROBLEM_INPUT = @"LLLLLLLLLLLLLL...LLL.LLLLLLLLL.LLLLLLLLLLLLLLLLLLL.LLLLLLLL.LLL.LLLLLLLLLLLLLLLLLLL.LLLLLLLLLLLLLL
LLLLLL.LLLLLLLL.L.LL.LLLLLLLLLLLLLLLLLLLLLLLLLLLLL..LLLLLLLLL.LLLLLLL.LLLLLLL.LLLLL.LLLLLLLLLLL.LL
LLLLLL.LLLLLLLL.L.LL.LLLLLLLLLLLL.L.L.LL.LLLLLLLLL.LLLLLL.L.LLLL.LLLL.LLLLLLLLLLLLL.LLLLLLLLLLLLLL
L.LLLL.LLLLLLLL.LLLL.LLLLLL.LLLLLLLLL.LLLLLL.LLLLL.LLLLLLLLLLLLLLLLLLLLLL.LLL.LLLLL.LL.LLLLLLLLLLL
LLL.LL.LLLLLLLLLLLLL..LLLL.LLLLLLLL.L.LLL.LL.LLLLL.LLLLLLLL.LLLLLLLLL.LLLLLLLLLLLLL.LLLLLLLLLLLLLL
L.....LLLLL.LL.L.L..L..L.L..LL...LLLLL.L...L..LLL.L.L..L...LLL.LL.L..LL...L......L.L.L.L.LLLLL....
LLLLLL.LLLLLLLLLLLLL.LLLLLLLLL.LLLLLL.LLLLL.LLLL.LLLLLLLLLL.LL..L.LLL.LLLLLLLLLLLLL.LLLLLLLLLLLLLL
LLLLLL.LLLLLL.L.LLLL.LLLLLLLLL.LLLLLL.LLLLLLLLLLLLLLLLLLLLL.LLLLLLLLL.LLLLLLL.LLLLL.L.L.LLLLLLLLLL
LLLLLL.LLLLLLLLLLLLLLLLLLLLLLLLLLLLLL.LLLLLL.LLLLL.LLLLLLLLLLLLLLLLLLLLLLLLLL.LLLLL.LLLLLLLLLL.LLL
LLLLLL.LLLLLLLLLLLLL.LLLLLLLLL.LLLL.L.LLLLLL.LLLLLLLLLLLLLL.LLLLLLLLL.LLLLLLL.LLLLL.LLLLLLLLLLLLLL
.L.L...L.LL....L.......L...LLL.LL....L.LL.LLL...L........L..L.....L........L.LLL..L..LL.LL........
LLLLLL.LLLLLLLL.LLLL.LLLLLLLL...LLLLL.LLLLLL.LLLLL..LLLLLLLL.LLLLLLLL.L.LLLLL.LLLLL.LLLLLLLLLLLL.L
LLLLLL.LLL.LLLLLLLLL.LLLLLLLLLLLLLLLL.LLLLLL.LLLLL.LLLLLLLLLLLLLLLLLL.LLLLLLL.LLLLL.LLLLLLL.LLLLLL
LLLLLL.LLLLLLLLLLLLLLLLL.LLLLL.LLLLLL.LLLLLL.LLLL..LLLLLLLL.LLLLLLLLL.LLLLLLL.LLLLL.LLLLLLLLLLLLLL
LLLLLL.LLLLLLLL.L.LLLLLLLLLLLL.LLLLLLLLLLLLL.LLLLL.LLLLLLLLLL.LLLLLLL.LLLLLLL.LLLLL.LLLLLLLLLL.LLL
LL.LLL..LLLLLLLLLL.L.LLLLLLLLL.LLLL.L.LLLLLL.LLLLL..LLLLLLLLLLLLLLLLL.LLLLLLL.L.LLL.LLLLLLLLLLLLLL
LLLLLL.LLLLLLLL.LLLL.LLLLLLLLLLLLLLLL.LLLLLLLLLLLLLLLL.LLLLLLL..LLL.L.LLLLLLLLLLLLL.LLLLLLLLLLLLLL
LLLLL.LLLLLLLLLLLLLLLLLLLLLLLL.LLLLLLLLLLLLLLLLLLL.LLLLLLLL.LLLLLLLL.LLLLLLLL.LLLLL.LLLLLLLLLLLLLL
.L.........LL.LL..L..LLLL...L.........L.LLL..L....L....LLL.....L..L.L..L........LL..LL..L.....L...
L.LLLL.LLLLLLLL.LLL..LLLLLLLLLLL.LLL..LLLLLL.LLLLL.LLLLLLLL.L..LLLLLL.LLLLLLLLL.LLL.LLLLLLLLLLLLLL
LLLLLLLLLLLLLLL.LLLL.LLLLLLLLLLLLLLLLLLLLLLL.LLLLL.LLLLLLLL.LLLLLLLLLLLLLLLLLLLLLLL.LLLLLLLLLLLLL.
LLLLLLLLL.LLLLL..LLLLLLLLLLLLLLLLLLLL.LLLLLL.LLLLL.LLLLLLLL.LLLLLLLLLLLLLLLLL.LLLLL.LLLL.LLLLLLLLL
LLLLLL.LLLLLLLLLLLLLLLLLLLLLLL.LLLLLL.LLLLLL.LLLLL.LLLLLLLL.LLL.LLLLL.LLLLLLL.LLLLLLLLL.LLLLLLLLLL
LLLLLL.LLLLLLLLLLLLLLLLLLLLLLL.LLLLLLLLLLLLL.LLLLL.LLLLLLLLLL.LLLLLLL.LLLLLLL.LLLLL.LLLLLLLLLLLLLL
LLL.LL.LLLLLLLL.LLL.LLLLLLLLLLLLLLLLLLLLLLLL.LLL.L.LLLLL.LL.LLLLLLLLL.LLLLLLLLLLLLL.LLLLLLLLLLLLLL
.....L....L.L...LL...LL..L.LL.....L............LL...LL..LL.L.L......LL........L.L..L....L....L..L.
L.LLLLLLLLLLLLLLLLLLLLLLLLLLLL.LLLLLL.LLLLLLLLLLLL.LLLLLLLL.LLLLLLL.L.LLLLLLL.LLLLL.LLLLLLLLLLLLLL
LLLLLLLLLLLLLLL.LLLL.LLLLLLLLL.LLLLLLLLLLLLL.LLLLL.LLLLLLLL..LLLLLLLLLLLLLLLL.LLLLL.LLLLLLLLLLLLLL
LLLLLL.LLLLLLLL.LLLL.LLLLLLLLL.LLLLLL.LLLLLLLLLLLL.LLLLLLLL.LLLLLLLLL.LLLLLLLLLLLLL.L.LLLLLLLLLLLL
L.LLLL..LLLLLLLLLLLLLLLLLLLLLL.LLLLLL.LLLLLLLLLLLLLLLLLLLLL.LLLLLLLLL.LLLLLLL.LLLLL.LLLLLLLLLLLLLL
LLLLLLL..LLLLLL.LLLLLLLLLLLLLLLLLLLLL.LLLLLLLLL.LL.LLLLLLLL.LLLLLLLL.LLLLLLLLLL.LLL.LLLLLLLLLLLLLL
LLLLLL.LLLLL.LLLLLLL.LLLLLLLLL.LLLLLL.LLLLLLLLLLLL.LLLLLLLL.LLLLLLLLL.LLLLLLL.LLLLL.LLLLLLLL.LLLLL
LLLLLLLL.LLLLLL.LLLL.LLLL.LLLL.LLLLL.LLLLLLLLLLLLL.LLLLLLLL.LLLLLLLLL.LLLLL.L.LLLLL.LLLLLLL.LLLLLL
LLLLL..LLLLLLLLLLLLLLLLLLLLL.L.LLLLLL.LLL.LL.LLLLL.LLLLLLLLLLLLLLLLLL.LLLLLLLLLLLLL.LLLLLLLLLLLLLL
L..L.L.L.L........L...L.L..LL....L.....L.L......L..L........LL........L......LL.LLL.L..LL...LL...L
LLLLLL.LLLLLLLL.LLLL.LLLL.LLLL.LLLLLLLLL.LLLLLLLLL.LLLLL.LL.LLLLLLLLL.LLLLLLLLLLLLLLLLLLLLLLLLLLLL
LL.L.LLLLLLLLLL.LLLL.LLL.LLLLLLLLLLLL.LLLLLL.LLLLL..LLLLLLL.LLLLLLLLL.LLL.LLL.LLLLLLLLLLLLLLLLLLL.
LLLLLL.LLLLLLLL.LLLLLLLLLLLLLL.LLLLLL.LLLLLL.LLLLL.LLLLLLLL.LLLLL.LLL.LLLLLLL.LLLLL.LLLLLLLLLLLLLL
LLLLLL.LLLLLLLL.LLLLLLLLLLLLLL.LLLLLLLL.LLLLLLLLLLLLLLLL.LL.LLLLLLLLLLLLLLLLL.LLLLLLLLLLLLLLLLLLLL
LLLLLL.LLLLLLLL.LLLL.LLLLLLLLL.LLLLLLLLLLLLL.LL.LL.LLLLLLLL.LLLLLLLLL.LLLLLLL.LLLLL.LLLLLLLLLLLLLL
LLLLLLLLLLLLLLLLLLLL.LLLLLLLLL.LLLLLL.LLLLLL.LLLLL.LLLLLLLLLLLLLLLLLLLLLLLL.LLLLLLLLLLLLLLLLLLLLLL
LLLLLL.LLLLLLLL.LLLL.LLLLLLLLL.LLLLLLLLLLLLL.LLLLL.LLLLLLLL.LLLLLLLLL.LLLLLLLLLLLLL.LLLLLLLLLLLLLL
...LL.LL.L..LL..L.LLL........L...L.......L...L..L.LL.L.......L....L...L.....LLL.L..L.L..L.L..L....
LLLLLL.LLLLLLLL.LLLL.LLLLLLLLLLLLLLLLLLLLLLL.LLLLL.LLLLLL.L.LLLLLLLL..LLLLLLL.LLLLL.LLLLL.LLLLLLLL
LLLLLL.LLLLLLLLLLLLL.LLLLLLLLL.L.LLLL.LLLLLL.L.LLLLLLLLLLLLLLLLLLLL.L.LLLLL.L.LLLLLLLLL.LLLLLLLLLL
L..LLL.LLLLLLLLLLLLL.LLLLLLLLL.LLLLLL.LLLLLL.LLLLL.LLLLLLLL.LLL.LLLLL.LLLLLLLLLLLLLLLLLLLLL.LLLLLL
LLLLLL.LLLLLLLLLLLLL.LLLLLLLLL.LLLLLL.LLLLLL.LLLLL.LLLLLLLL.LLLLLLLLL.LLLLLLLLLLLLLLLLLLLLLLLLLLLL
LLLLLL.LLLLLLLL.LLLL.LLLLLLLLL.LLLLLL..LLLLL.LLLLL.LLLLLLLLLLLLLLLLLL.LLLLLLL.LLLLLLLLLLLLLLLLLLLL
.LLLLL.LLLLLLLL.LLLL.LLLLLLLLLLLLLLLLL.LLLLL.LLLLL.LLLLLLLLLLLLLLLLLL.LLLLLLL.LLLLL.LLLLLLLLLLLLLL
L.LLLL.LLLLLLLL.LLLL.LLLLLLL.L.LLLLL..LLLLLL.LLLLL.LLLLLLLLLL.LLLLLLL.LL.LL.L.LLLLL.LLLLLLLLLLLLLL
LL.L...L.........L..L...LLL......L......L...L..L.L..LL..L.LLL.LL..L.LL..L.L.LL.L.L..LL..........L.
LLLLLLLLLLLLLLL.LLLLLLLLLLLLLL.LLLLLL.LLLLLL.LLLLL.LLLLLLLL.LLLLLL.LLLLLLLLLL.LLLLL.LLLLLLLLLLLLLL
LL.LLL..LLLLLLLLLLLL..LLLLLLLL.LLLLLL.L.LLLL.LLLLLLLLLLLLLL.L.LLLLLLL.LLLLLLL.LLLLLLLLLLLLLLLLLLLL
LLLLLL.LLLLLLLL.LLLL.LLLLLLLLL.LL.LLL.LLLLLL.LLLLL.L.LLLLLL.LLLLLLLLLLLLLL.LLLLLLLL.LLLLLLLLLLLLLL
LLLLL...LLLLLLLLLLLLLLLLLLLLLL.LL.LLL.LLLLLL.LLLLL.LLLLLLLLLLLLLLLLLLLLLLLLLLLLL.LL.LLLLL.LLLLLLLL
LLLLLLLLLLLLLLL..LLL.LLLLLLLLL.LLLLLL.LLLLLLLLLLL..LLLLLLLL.LLLLLLLLL.LLLLLLL.LLLLL.LLLLLLLLLLLLLL
LLLLLLLLLLLLLL.LL.LLLLLLLLLLLL.LLLLLL.LLLLL..LLLLL.LLLLLLLL.L.LLLLL.L.LLLLLLL.LLLL..LLLLLLLLLLLLLL
LLLLLL.LL.LL.LL.LLLL.LLLLLLLLL.LLL.LL.LLL.LL.LLLLL.LLLLLLLL.LLLLLLLLL.LLLLLLL.LLLLL.LLLLLLLLLLLLLL
L.LLLL.L.LLLLLL.LLLLLLLLLLLLLL.L.LLLL.LLLLLL.LLLLL.LLLLLLLL.LLLLLLLLL.LLL.LL..LLLLL.LLLLLLLLLLLL.L
..L.....LLLL..L..L......LL.L.......L..LLL..L..L..LL.LLL....L..L.L..L...L.LL..L...L.L.......L......
LLLLLL.LLLLLLLL..L.LLLLLLLLLLLLLLLLLL.LLLLLL.LLLLL.LLLLLLLL.LL.LLLLLL.LLLLLLL.LL.LL.LLLLLLLLLLLLLL
LLLLLL.LLLLLLLLLLLLL.LLL.LLLLL.LLLLLLLLLLLLL.LLLLLLLLLLLLLL.LLLLLLLLLLLLLLLLL.LLLLLLLLLLLLLLLLLLLL
LLLLLL.LLLLLLLL.LLLL.LLLLLLL.L.LLLL.LL.LLLLL.LLLL..LLLLLLLL.LLLLLLLLL.LLLLLLL.L.LLL.LLLLLLLLLLLLLL
LLLLLLLLLLLLLLL.LLLLLLLLLLLLLL.LLLLLL.LLLLLL.LLLLL.LLLLLLLLLLLLLLLLLL.LLLLLL..LLLLL.LLL.LLLLLL.L.L
LLL.LLLLLLLLLLL.LLLLLLLLLLLLLL.LLLLLL.LLLLLLL.LLLL.LLLLLLLL.LLL.LL.LL.LLLLLLL.LLLLL.LLLLLLLLLLLLLL
LLLLL..LLLLLLLL.LL.LLLLLLLLLLL.LLLLLL.LLLLLL.L.LLL.LLLLLLLLLLLLLLLLLL.LLLLLLLLLLLLL.LLLLLLLLLLLLLL
L..L...L.L.L....L....L.LLLL.L.....L..LL...L...L..LLL.L......LL...L..LLL..L.L..L....L.L.L...L..LL.L
LLLLLLLLLLLLLLL.LLLLLLLLLLLLL.LLLLLLLLLLLLLL..LLLL.LL.LLLLL.LLLLLLLL.LLLLLLLL.LLLLLLLLLLLLLLLLL.LL
LLLLLLLLLLLLLLL.LLLL.LLL.LLL.LLLLLLLLLLLLLLL.LLLLL.LLL.LLLL.LLLLLLLLL.LLLLLLL.LLLLL.LLLLLLLLLL.L.L
LLLLLL.LLL.LLLLL.LLL.L.LLLLLLL.LLLLLL.LLLLLL.LLLLL.LLLLLLLLLL.LL.LLL..LLLLLLL.LLLLL.LLLLL.LLLLLLLL
LLLLLL.LLLLLLLLLLLLL.LLLL.L.L..LLLLLLLLLLLLL.LLLLL.LLLLLLLLL.LLLLLLLL.LLLLLLL.LLLLL.LLLLLLLLLLLLLL
LLLLLLLLLLLLLLL.LLLL.LLLLLLLLL.LLLLLL.LLL.LLLLLL.LLLLLLLLLL.LL.LLLLLL.LL.LLLL.LLLLL.LLLLLLLL.LLL.L
.LLL.LLLLLLLL.L.LLLLLLLLLLLLLL.LLLLLL.LLLLLL.LLLLLLLLLLLLLL.LLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLL
LLLLLL.LLLLLLLLLLLLL.LLLLLLLLL.LLLLLLLLLLLLL.LLLLL.LLLLLLLLLL.LLLLLLL..LLLLLL.LLLL.LLLLLLLLLLLLLLL
LL.LLL.L.LL.LLLL.....L...LL..L....LL.LLL.....L..L..L..L.............L.L.....L......LLL.L.....L.LL.
LLLLLL.LLLLLLLL.LLLLLLLLLLLLLLLLLLLLL.LLLLLL.LLLLL.LLLLLLLLLLLLLLLLLL.LLLLLLL.LLLLLLLLLLLLLL.LLLLL
LLLLLL.L.LLLLLL.LLLL.LLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLL.LLLL.LLLLLLLLLLLLLLL
LLLLLL.L.LLLLLL.LLLL.L.LLLLLLL.LLLLLLLLLLLLL.LLLLL.LLLLLLLL.LLLLLLLLL.LLLLLLL.LL.LLLLLLLLLLLLLLLLL
LLL.LLLLLLLL.LL.LLLL.LLLLLLLLL.LLLLLLLL..LLL.LLLLL.LLL.LLLL.LLLLLLLLL.LLLLLLL.LLLLL.LLLLLLLLLLLLLL
LLLLLLLLLLLLLLL.LLLLLLL.LLLLLL..LLLLLLLLLLLL.LLLLLLLLLLLLLL.LLL..LLLL.LLLLLLL.LLLLLLLLLLLLLLLLLLLL
.L....L...L.....LL.L..LL.L.L..LLL..L...L......LL..L..L.LL..L..LLLL..L...LLL.L.LLL.L.L...LL..L.L...
LLLLLL..LLLLLLLL.L.LLLLLLL.LLL..LLLLL.LLLLLL.LLLLLLLLLLLLLLLLLLLLLLLL.LLLLLL.LLLLLL.LLLLLLLLLL.LLL
LLLLLL.LLLLLLLL.LLLL.LLLLLLLLL.LLLLLL.LLLLLL.LLLLL.LLLLLLLL.LLLLLLLLL.LLLLLLL.LLLLL.LLLLLLLLLLLLLL
LLLLLL.LLLLLLLL.LLLL.LLLLLLLLLLLLLLLL.LL.L.LLLLLLL.LLLLLLLLLLLLLLLLLLL.LLLLLLLLLLLL.LLLLLLLLLLLLLL
LLLLLL.LLLLLLLL.LLLLLLLLLLLLLL.LLLLLL.LLLLLL.LL...LLL.LLLLL.LLLLLLLLLLLLLLLLL.LLLLL.LLLLLLLLLLLLL.
LLLL.L.LLLLLLLLLLL.L.LLLLL..LL.LLL.LL.LL.LLL.LLLLLLLLLLLLLLLLLLLLLLLL.LLLLLLL.LLLLL.LLLLLLLLLLLLLL
LLLLLL.LLLLLLLL.LLLL.LLLLLLLLL.LLLLLLLLLL.LL.LLLLL.LLLLLLLL.LLLLLLLLLLLL..LLL.LLLLL.LLLLLLLLLLL.LL
L.L.L.L.LL...L....L.L.L...L.L.....L....LL..L.L....LL..LL..L.......................L.L...L..L..L...
LLLLLL.LLLLLLLL.LLLLLLLLLLLL.L.LLLLLLLLLLLLL.L.LLLLLLLLLLLLLLLLLLLLLL.LLLLLL.LLLLLLLLLLLLLLLLLLLLL
LLLLLL.LL.LLLLL.LLL.LLL.LLLLL..LLL.LL.LLLLLL.LLLL..LLLLLL.L.LLLLLLLLLLLLLLLLL.LLLLL.LLLLLLLLLLLLLL
.LLLLL.LLLLLLLL.L.LL.LLLLL.LL.LLLLLLL.LLLLLLLLLLLL.LLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLL.LLL
LLLLLLLLLLLLLLLLLLLLLLLLLLLLLL.LLLLLL.LLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLL.LLLLLL.LLLLLLLLLLLLLL
LLLLLLLLLLLLLLLLLLLL..LLLLL.LL.LLLLLL..LLLLL.LLLL.LLLLLLL.L.LLLLLLLLL.LLLLLLLLLLLLL..LLLLLLLLLLLLL
LLLLLLLLLLLLL.LLLLLL..LLLLLLLL.LLLLLL.LLLLLL.LLLLL.LLLLLLLLLLLLLLLLL.LLLLLLL.LLLLLL.LL.LLLLLL.LLLL
LLLLLL.LLLLLLLL.LLLLLLLLLLLLLLLLLLLLL.LLLLLL.LLLLLLLLLLLLLLLLLLLLLLLL.LLLLLLL.LLLLL.LLLLLLLLLLLLLL
LLLLLL.LLLLLLLL.LLLL.LLLLLLLLLLLLLLLL.LLLLLLLLLLLL..LLLLLLL.LLLLLLLLL.LLLLLLLLLLLLL.LLLLLLLLLLLLLL
LLLLLL.LLLLLLLL.LLLLLLLLL.LLLL.LLLLLLLLLLLLL.LLLLLLLLLLL..L.LLLLLLLLL.LLLL.LL.L..LL.LLLLLLLLLLLLLL";

        const string EXAMPLE_INPUT = @"L.LL.LL.LL
LLLLLLL.LL
L.L.L..L..
LLLL.LL.LL
L.LL.LL.LL
L.LLLLL.LL
..L.L.....
LLLLLLLLLL
L.LLLLLL.L
L.LLLLL.LL";
    }
}
