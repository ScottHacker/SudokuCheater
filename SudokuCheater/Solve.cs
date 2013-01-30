using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace SudokuCheater
{
    public class Solve
    {
        //Singleton pattern, we only want one instance running at a time
        private static Solve _Instance;
        private Solve() { }

        //Declare new instances through GetInstance so we can control the number running
        public static Solve GetInstance()
        {
            if (_Instance == null)
            {
                _Instance = new Solve();
            }
            return _Instance;
        }

        /// <summary>
        /// Run the puzzle solver
        /// </summary>
        /// <param name="puzzle">Puzzle data formatted into 2D array</param>
        /// <returns>Finished puzzle</returns>
        public byte[,] Run(byte[,] puzzle)
        {
            //Keep solving until it's finished.
            while (Markup(puzzle)) { }

            return puzzle;
        }

        /// <summary>
        /// The main logic function, creates "pencilmarks" then does logic based on that.
        /// </summary>
        /// <param name="puzzle">Puzzle data formatted into 2D array</param>
        /// <returns>Returns false if it fails to solve puzzle</returns>
        private bool Markup(byte[,] puzzle)
        {
            Dictionary<string, List<byte>> pencilMarks = new Dictionary<string, List<byte>>();

            //For each row...
            for (int x = 0; x < 9; x++)
            {
                //..and each column...
                for (int y = 0; y < 9; y++)
                {
                    //...if it doesn't have a number listed...
                    if (puzzle[x, y] == 0)
                    {
                        //...Go through and try every number.
                        byte[,] tPuzzle = puzzle.Clone() as byte[,];
                        for (byte i = 1; i < 10; i++)
                        {
                            tPuzzle[x, y] = i;
                            //Check it's validity, if the number doesn't cause any problems...
                            if (!DuplicateCheck(GetBox(x, y, tPuzzle)) & !DuplicateCheck(GetColumn(y, tPuzzle)) & !DuplicateCheck(GetRow(x, tPuzzle)))
                            {
                                //...then add it to our "pencil marks" dictionary, or make a new one if it doesn't exist.
                                //We use the coordinates of the grid point as a key, i.e.: 12 or 05 meaning row 1, column 2; or row 0, column 5.
                                if (pencilMarks.ContainsKey(x.ToString() + y.ToString()))
                                    pencilMarks[x.ToString() + y.ToString()].Add(i);
                                else
                                    pencilMarks.Add(x.ToString() + y.ToString(), new List<byte> { i });
                            }
                        }
                    }
                }
            }
            //Do the logic that would affect the pencilmarks.
            FindDuplicatePencilMarks(ref pencilMarks);

            //All of the solving logic, returns true if it found a new number, false if it didn't (and therefore failed)
            return FindSingles(ref puzzle, pencilMarks) | FindNegativeSingles(ref puzzle, pencilMarks);
        }

        private void FindPencilMarkDoubles()
        {

        }

        /// <summary>
        /// Finds double pencil mark duplicates, i.e. pencilmarks for 2 and 3 twice in the same column.
        /// In Sudoku, this means that both those boxes must be 2 or 3 and can't be anything else.  So we can
        /// remove all other pencilmarks.
        /// </summary>
        /// <param name="pencilmarks">A reference to the full pencilmarks Dictionary</param>
        /// <returns>True if anything was changed, false if not</returns>
        private void FindDuplicatePencilMarks(ref Dictionary<string, List<byte>> pencilmarks)
        {
            List<Intersection> results = new List<Intersection>();

            //Check all the boxes for duplicate pencilmarks and add the results to a list of dictionaries
            foreach (string key in pencilmarks.Keys)
            {
                results.Add(DuplicatePMCheck(key, GetRow(int.Parse(key[0].ToString()), pencilmarks)));
                results.Add(DuplicatePMCheck(key, GetColumn(int.Parse(key[1].ToString()), pencilmarks)));
                results.Add(DuplicatePMCheck(key, GetBox(int.Parse(key[0].ToString()), int.Parse(key[1].ToString()), pencilmarks)));
            }

            //Go through our results and adjust the pencilmarks to fit
            foreach (Intersection i in results)
            {
                if (i.combo != null)
                {
                    pencilmarks[i.key1] = i.combo;
                    pencilmarks[i.key2] = i.combo;
                    //changed = true;
                }
            }
        }

        /// <summary>
        /// Looks for duplicate pencil mark groups.
        /// </summary>
        /// <param name="oKey">key for square sent in</param>
        /// <param name="pencilMarks">List of pencilmarks for square</param>
        /// <param name="numLists">Full list of pencilmarks for a column, row, or box</param>
        /// <returns>Dictionary with key as square names, and pencilmarks that matched</returns>
        private Intersection DuplicatePMCheck(string oKey, Dictionary<string, List<byte>> numLists)
        {
            //First we take the pencilmarks sent in and break them into every combination of 2 available
            //while not repeating.  i.e.: if 1, 3, and 5 was sent in, then we get 1 + 3, 1 + 5, and 3 + 5
            List<List<byte>> combinations = new List<List<byte>>();

            foreach (byte n in numLists[oKey])
            {
                foreach (byte i in numLists[oKey])
                {
                    List<byte> combo = new List<byte>() { n };
                    if (i != n)
                    {
                        //Add the first to the second then sort it, so we don't treat 1 + 9 and 9 + 1 as different combos
                        combo.Add(i);
                        combo.Sort();
                        //Use Linq to check if it's already there, if not then add it.
                        if (!combinations.Any(o => o.SequenceEqual(combo)))
                            combinations.Add(combo);
                    }
                }
            }

            //With the list of combinations created, we go through the groups of pencilmarks in the 
            //column, row, or box and look for intersections
            foreach (List<byte> combo in combinations)
            {
                List<Intersection> intersections = new List<Intersection>();

                foreach (string key in numLists.Keys)
                {
                    //Make sure we don't include the box we're checking against for intersections.
                    if (oKey != key)
                    {
                        //Use exclusive or (XOR) to check if either combo number exists alone in a square,
                        //if it does, then this whole combo is invalid and we break out to move to the next.
                        if (numLists[key].Contains(combo[0]) ^ numLists[key].Contains(combo[1]))
                        {
                            intersections.Clear();
                            break;
                        }

                        //Check to see if both numbers of the combo exist in the square's pencilmarks
                        //If so, add them to the intersections list
                        if (numLists[key].Contains(combo[0]) && numLists[key].Contains(combo[1]))
                        {
                            intersections.Add(new Intersection(oKey, key, combo));
                        }
                    }
                }

                //Go through our intersections and figure out which ones are unique
                if (intersections.Count() == 1)
                    return intersections[0];
            }

            //If we got here, then we didn't find anything.  Return a default layout to act as null.
            return new Intersection();
        }

        /// <summary>
        /// Checks pencilmarks list for any singles, if it finds one then it marks it and notes that the puzzle was changed
        /// </summary>
        /// <param name="puzzle">A reference to the full formatted puzzle</param>
        /// <param name="pencilMarks">A dictionary of pencilmarks</param>
        /// <returns>Whether or not the puzzle has been changed</returns>
        private bool FindSingles(ref byte[,] puzzle, Dictionary<string, List<byte>> pencilMarks)
        {
            //Then take that dictionary and go through each key...
            bool changed = false;
            foreach (string key in pencilMarks.Keys)
            {
                //..if there's only one pencil mark, then that's gotta be the answer.  Pen it in then mark that changes have been made.
                if (pencilMarks[key].Count == 1)
                {
                    puzzle[int.Parse(key[0].ToString()), int.Parse(key[1].ToString())] = pencilMarks[key][0];
                    changed = true;
                }
            }
            return changed;
        }

        /// <summary>
        /// Cross references columns, rows, and boxes, to find numbers that can only exist in one square, 
        /// even if other numbers are pencilmarked there as well.
        /// </summary>
        /// <param name="puzzle">A reference to the full formatted puzzle</param>
        /// <param name="pencilMarks">A dictionary of pencilmarks</param>
        /// <returns>Whether or not the puzzle has been changed</returns>
        private bool FindNegativeSingles(ref byte[,] puzzle, Dictionary<string, List<byte>> pencilMarks)
        {
            bool changed = false;
            //Go through every quare on the grid...
            foreach (string key in pencilMarks.Keys)
            {
                //...then check their pencilmarks to see if any of them are unique in their boxes, columns, or rows.
                // If they aren't, it returns 0, so I use Math.Max to find the biggest number, which will either be 0 if
                // all 3 checks return negative, or the unique number if one of them is positive.
                byte uniqueNumber = Math.Max(Math.Max(
                    UniqueNumberCheck(pencilMarks[key], GetRow(int.Parse(key[0].ToString()), pencilMarks)),
                    UniqueNumberCheck(pencilMarks[key], GetColumn(int.Parse(key[1].ToString()), pencilMarks))),
                    UniqueNumberCheck(pencilMarks[key], GetBox(int.Parse(key[0].ToString()), int.Parse(key[1].ToString()), pencilMarks))
                        );

                //If the result isn't zero...
                if (uniqueNumber != 0)
                {
                    //...then pen in the number we got and note that it's been changed.
                    puzzle[int.Parse(key[0].ToString()), int.Parse(key[1].ToString())] = uniqueNumber;
                    changed = true;
                }
            }
            return changed;
        }

        /// <summary>
        /// Uses a list of pencilmarks and checks to see if any of them are unique in a formatted
        /// list of columns, rows, and boxes.
        /// </summary>
        /// <param name="pencilMarks">Single list of pencilmarks</param>
        /// <param name="numLists">List of Lists of pencilmarks</param>
        /// <returns>Returns the unique number or 0 if no unique numbers found</returns>
        private byte UniqueNumberCheck(List<byte> pencilMarks, Dictionary<string, List<byte>> numLists)
        {
            Dictionary<byte, byte> numCount = new Dictionary<byte, byte>();
            foreach (List<byte> numList in numLists.Values)
            {
                foreach (byte num in pencilMarks)
                {
                    if (numList.Contains(num))
                    {
                        if (numCount.ContainsKey(num))
                            numCount[num]++;
                        else
                            numCount.Add(num, 1);
                    }
                }
            }

            byte uniqueNumber = 0;
            foreach (byte n in numCount.Keys)
            {
                if (numCount[n] == 1)
                    uniqueNumber = n;
            }

            return uniqueNumber;
        }

        /// <summary>
        /// Check for duplicates in a 1D array
        /// </summary>
        /// <param name="numList">A one-dimensional chunk of the puzzle, i.e.: column, row, or box</param>
        /// <returns>True if duplicate found, false if no duplicates found</returns>
        private bool DuplicateCheck(byte[] numList)
        {
            //Lists have better functionality for this, so we'll use that.
            List<byte> nums = new List<byte>();
            foreach (byte n in numList)
            {
                //Ignore 0s, those are placeholders for boxes that haven't been filled out yet.
                if (n != 0)
                {
                    //If the element is found, then we return true: the list has duplicates, otherwise we add the element to the list
                    if (nums.Contains(n))
                        return true;
                    nums.Add(n);
                }
            }
            //If we managed to get through the loop without returning true, then we return false: no duplicates found.
            return false;
        }

        /// <summary>
        /// Take a row number and a full puzzle, and return a 1-D array of row numbers
        /// </summary>
        /// <param name="x">Number of the row to return from 0-8</param>
        /// <param name="puzzle">Full formatted puzzle</param>
        /// <returns>1-D array with all numbers in selected row</returns>
        private byte[] GetRow(int x, byte[,] puzzle)
        {
            byte[] n = new byte[9];
            for (int i = 0; i < 9; i++)
            {
                n[i] = puzzle[x, i];
            }
            return n;
        }

        /// <summary>
        /// Take a column number and a dictionary of pencilmarks, and return the column's worth of pencilmarks
        /// </summary>
        /// <param name="y">Number of the column from 0-8</param>
        /// <param name="pencilMarks">Dictionary of pencilmarks</param>
        /// <returns>Column of pencilmarks</returns>
        private Dictionary<string, List<byte>> GetRow(int x, Dictionary<string, List<byte>> pencilMarks)
        {
            Dictionary<string, List<byte>> box = new Dictionary<string, List<byte>>();

            for (byte i = 0; i < 9; i++)
            {
                if (pencilMarks.ContainsKey(x.ToString() + i.ToString()))
                    box.Add(x.ToString() + i.ToString(), pencilMarks[x.ToString() + i.ToString()]);
            }

            return box;
        }

        /// <summary>
        /// Take a column number and a full puzzle, and return a 1-D array of column numbers
        /// </summary>
        /// <param name="y">Number of the column from 0-8</param>
        /// <param name="puzzle">Full formatted puzzle</param>
        /// <returns>1-D array with all numbers in selected column</returns>
        private byte[] GetColumn(int y, byte[,] puzzle)
        {
            byte[] n = new byte[9];
            for (int i = 0; i < 9; i++)
            {
                n[i] = puzzle[i, y];
            }
            return n;
        }

        /// <summary>
        /// Take a column number and a dictionary of pencilmarks, and return the column's worth of pencilmarks
        /// </summary>
        /// <param name="y">Number of the column from 0-8</param>
        /// <param name="pencilMarks">Dictionary of pencilmarks</param>
        /// <returns>Column of pencilmarks</returns>
        private Dictionary<string, List<byte>> GetColumn(int y, Dictionary<string, List<byte>> pencilMarks)
        {
            Dictionary<string, List<byte>> box = new Dictionary<string, List<byte>>();

            for (byte i = 0; i < 9; i++)
            {
                if (pencilMarks.ContainsKey(i.ToString() + y.ToString()))
                    box.Add(i.ToString() + y.ToString(), pencilMarks[i.ToString() + y.ToString()]);
            }

            return box;
        }

        /// <summary>
        /// Take a x,y grid number and a full puzzle, and return a 1-D array with 
        /// all the numbers from that grid quadrant.
        /// </summary>
        /// <param name="x">Row number from 0-8</param>
        /// <param name="y">Column number from 0-8</param>
        /// <param name="puzzle">Full formatted puzzle</param>
        /// <returns>1-D array with all numbers in selected grid quadrant</returns>
        private byte[] GetBox(int x, int y, byte[,] puzzle)
        {
            List<byte> n = new List<byte>();

            //Get the max range so we can define the box size.
            int xQuadMax = GetMaxRange(x);
            int yQuadMax = GetMaxRange(y);

            //Subtract Max range by 3 to get Min Range, now that we know the location
            //of the box on the puzzle grid, recurse through all it's numbers and add them
            for (int i = xQuadMax - 3; i < xQuadMax; i++)
            {
                for (int j = yQuadMax - 3; j < yQuadMax; j++)
                {
                    n.Add(puzzle[i, j]);
                }
            }

            return n.ToArray();
        }

        /// <summary>
        /// Take a x,y grid number and a dictionary of pencilmarks, and return a box
        /// of pencilmarks from that grid quadrant.
        /// </summary>
        /// <param name="x">Row number from 0-8</param>
        /// <param name="y">Column number from 0-8</param>
        /// <param name="pencilMarks">Full Dictionary of Pencilmarks</param>
        /// <returns>List of Lists with just a box quadrant of pencilmarks</returns>
        private Dictionary<string, List<byte>> GetBox(int x, int y, Dictionary<string, List<byte>> pencilMarks)
        {
            Dictionary<string, List<byte>> box = new Dictionary<string, List<byte>>();

            //Get the max range so we can define the box size.
            int xQuadMax = GetMaxRange(x);
            int yQuadMax = GetMaxRange(y);

            //Subtract Max range by 3 to get Min Range, now that we know the location
            //of the box on the puzzle grid, recurse through all it's numbers and add them
            for (int i = xQuadMax - 3; i < xQuadMax; i++)
            {
                for (int j = yQuadMax - 3; j < yQuadMax; j++)
                {
                    if (pencilMarks.ContainsKey(i.ToString() + j.ToString()))
                        box.Add(i.ToString() + j.ToString(), pencilMarks[i.ToString() + j.ToString()]);
                }
            }

            return box;
        }

        /// <summary>
        /// Takes a row or column number and rounds up to the highest grid quadrant range.
        /// Either 3, 6, or 9.  For example, giving 2 will return 3.  Giving 4 will return 6, etc.
        /// </summary>
        /// <param name="n">Row or column number</param>
        /// <returns>Row or column number that represents the max range of a quadrant</returns>
        private int GetMaxRange(int n)
        {
            return (int)((n + 1) / 3M + 0.7M) * 3;
        }
    }

    struct Intersection
    {
        public string key1;
        public string key2;
        public List<byte> combo;

        public Intersection(string k1, string k2, List<byte> c)
        {
            key1 = k1;
            key2 = k2;
            combo = c;
        }
    }
}
