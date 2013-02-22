using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;

namespace SudokuCheater
{
    public partial class SudokuCheater : Form
    {
        public SudokuCheater()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Button to solve puzzle based on input given.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button_solve_Click(object sender, EventArgs e)
        {
            Solve s = Solve.GetInstance();
            byte[,] finishedPuzzle = s.Run(Build());
            Fill(finishedPuzzle);

            foreach (byte b in finishedPuzzle)
            {
                if (b == 0)
                {
                    MessageBox.Show("Sorry! The program was unable to solve the puzzle.");
                    break;
                }
            }
        }

        /// <summary>
        /// Fills all the inputs with new puzzle data
        /// </summary>
        /// <param name="puzzle">New puzzle data to fill with</param>
        private void Fill(byte[,] puzzle)
        {
            for (int x = 0; x < 9; x++)
            {
                for (int y = 0; y < 9; y++)
                {
                    string str = string.Empty;
                    if (puzzle[x, y] != 0)
                    {
                        str = puzzle[x, y].ToString();
                    }
                    else
                    {
                        str = "_";
                    }
                    input.Controls.Find("input" + x + y, false)[0].Text = str;
                }
            }
        }

        /// <summary>
        /// Builds a formatted puzzle array from the GUI inputs
        /// </summary>
        /// <returns>Formatted puzzle array</returns>
        private byte[,] Build()
        {
            //Create a 2D array of bytes with input text from all the Text boxes 
            byte[,] puzzle = new byte[9, 9];
            foreach (MaskedTextBox box in input.Controls)
            {
                //These are based under the assumption that the MaskedTextBox is 
                //in the format "inputxy" where xy is the grid number, i.e.: input34
                int x = int.Parse(box.Name[5].ToString());
                int y = int.Parse(box.Name[6].ToString());

                //Check if the input is empty...
                if (!string.IsNullOrEmpty(box.Text))
                {
                    //...if not, then set the input.
                    puzzle[x, y] = byte.Parse(box.Text);
                }
                else
                {
                    //...if so, then we use 0 as a placeholder.
                    puzzle[x, y] = 0;
                }

            }

            return puzzle;
        }

        /// <summary>
        /// Fills the inputs with some default puzzles, chosen randomly.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button_fill_Click(object sender, EventArgs e)
        {
            List<byte[,]> puzzleList = new List<byte[,]>();

            //Easy
            puzzleList.Add(new byte[9, 9]{
                {0, 0, 0, 9, 6, 7, 0, 0, 4},
                {3, 1, 0, 0, 0, 2, 8, 0, 0},
                {7, 0, 4, 0, 8, 0, 2, 5, 0},
                {9, 6, 3, 0, 0, 8, 0, 4, 0},
                {2, 0, 0, 7, 0, 9, 0, 0, 3},
                {0, 8, 0, 3, 0, 0, 1, 9, 5},
                {0, 4, 8, 0, 3, 0, 9, 0, 7},
                {0, 0, 2, 0, 0, 0, 0, 3, 1},
                {0, 0, 0, 2, 7, 5, 0, 0, 0},
            });

            //Easy
            puzzleList.Add(new byte[9, 9]{
                {0, 3, 0, 8, 0, 0, 0, 0, 0},
                {9, 0, 5, 6, 0, 0, 7, 0, 0},
                {0, 0, 1, 0, 9, 3, 2, 0, 0},
                {8, 0, 6, 5, 0, 0, 0, 0, 0},
                {0, 4, 0, 0, 3, 0, 0, 0, 0},
                {0, 0, 0, 0, 0, 0, 0, 0, 0},
                {4, 7, 2, 3, 0, 6, 9, 5, 0},
                {0, 1, 9, 4, 8, 7, 0, 6, 0},
                {3, 6, 8, 2, 5, 9, 0, 1, 0},
            });

            //Easy
            puzzleList.Add(new byte[9, 9]{
                {1, 5, 0, 0, 0, 0, 9, 2, 4},
                {0, 0, 4, 0, 0, 0, 7, 0, 6},
                {0, 0, 0, 0, 0, 0, 3, 8, 5},
                {0, 0, 0, 0, 0, 0, 1, 0, 0},
                {0, 2, 0, 3, 0, 0, 0, 6, 0},
                {0, 0, 6, 7, 0, 0, 4, 0, 3},
                {0, 0, 2, 4, 0, 0, 5, 3, 1},
                {0, 0, 7, 2, 0, 3, 6, 9, 8},
                {0, 8, 3, 0, 0, 1, 2, 4, 0},
            });

            //Medium
            puzzleList.Add(new byte[9, 9]{
                {0, 0, 5, 0, 0, 8, 0, 0, 0},
                {0, 0, 0, 0, 6, 0, 0, 0, 4},
                {0, 8, 1, 7, 0, 3, 0, 0, 5},
                {3, 9, 0, 0, 0, 0, 7, 0, 0},
                {0, 2, 0, 0, 0, 0, 0, 4, 0},
                {0, 0, 6, 0, 0, 0, 0, 9, 1},
                {9, 0, 0, 5, 0, 7, 4, 2, 0},
                {6, 0, 0, 0, 3, 0, 0, 0, 0},
                {0, 0, 0, 4, 0, 0, 8, 0, 0},
            });

            //Medium
            puzzleList.Add(new byte[9, 9]{
                {0, 0, 6, 0, 0, 0, 0, 0, 0},
                {0, 0, 0, 0, 5, 9, 4, 0, 0},
                {0, 2, 0, 0, 0, 6, 0, 7, 8},
                {0, 0, 4, 0, 0, 0, 1, 9, 0},
                {5, 1, 0, 0, 0, 0, 0, 3, 0},
                {0, 3, 9, 0, 7, 0, 0, 0, 2},
                {3, 8, 1, 0, 0, 0, 0, 0, 9},
                {0, 0, 0, 0, 0, 0, 0, 0, 0},
                {0, 9, 0, 0, 2, 0, 0, 6, 0},
            });

            //Medium
            puzzleList.Add(new byte[9, 9]{
                {0, 0, 0, 0, 3, 4, 7, 0, 0},
                {0, 0, 3, 2, 7, 9, 0, 8, 0},
                {0, 0, 0, 5, 0, 0, 0, 0, 0},
                {0, 0, 0, 3, 5, 0, 0, 7, 9},
                {0, 0, 0, 0, 0, 0, 0, 0, 0},
                {0, 0, 0, 9, 0, 8, 1, 0, 0},
                {7, 0, 0, 0, 8, 0, 6, 9, 0},
                {8, 0, 2, 1, 0, 0, 0, 0, 0},
                {0, 0, 5, 0, 0, 0, 0, 4, 0},
            });

            //Hard
            puzzleList.Add(new byte[9, 9]{
                {4, 7, 0, 0, 2, 0, 0, 0, 8},
                {0, 0, 6, 0, 5, 3, 0, 0, 0},
                {0, 0, 0, 0, 7, 0, 0, 0, 6},
                {0, 0, 5, 0, 0, 0, 6, 1, 0},
                {0, 9, 0, 7, 0, 5, 0, 0, 0},
                {1, 0, 0, 3, 0, 0, 0, 0, 2},
                {0, 6, 0, 9, 0, 0, 0, 2, 0},
                {0, 0, 4, 0, 0, 7, 0, 6, 0},
                {5, 0, 0, 0, 8, 0, 4, 0, 0},
            });

            //Very Hard
            puzzleList.Add(new byte[9, 9]{
                {5, 0, 0, 3, 6, 0, 0, 0, 0},
                {0, 4, 0, 0, 0, 0, 8, 0, 0},
                {0, 0, 0, 0, 7, 0, 5, 0, 0},
                {0, 2, 0, 0, 5, 0, 0, 0, 7},
                {0, 3, 8, 0, 1, 0, 0, 0, 0},
                {0, 0, 0, 2, 0, 4, 0, 0, 3},
                {7, 0, 0, 0, 0, 0, 3, 9, 0},
                {6, 0, 0, 1, 0, 0, 0, 5, 0},
                {0, 0, 0, 0, 0, 6, 0, 1, 0},
            });

            //Very Hard
            puzzleList.Add(new byte[9, 9]{
                {0, 0, 0, 0, 0, 0, 0, 3, 0},
                {0, 8, 0, 0, 0, 9, 6, 0, 0},
                {3, 0, 0, 0, 7, 0, 0, 5, 0},
                {0, 0, 0, 4, 9, 0, 0, 0, 0},
                {0, 5, 3, 0, 0, 0, 0, 8, 1},
                {2, 0, 0, 1, 0, 0, 3, 0, 9},
                {8, 0, 0, 0, 0, 0, 0, 0, 7},
                {0, 9, 0, 0, 0, 0, 0, 1, 0},
                {0, 0, 7, 6, 0, 2, 0, 0, 0},
            });

            //Impossible
            //puzzleList.Add(new byte[9, 9]{
            //    {0, 9, 8, 0, 0, 0, 0, 6, 0},
            //    {0, 0, 0, 0, 8, 0, 0, 0, 9},
            //    {7, 0, 0, 0, 0, 9, 5, 0, 0},
            //    {0, 1, 0, 4, 0, 0, 3, 0, 6},
            //    {0, 0, 0, 3, 7, 5, 0, 0, 0},
            //    {5, 0, 3, 0, 0, 6, 0, 4, 0},
            //    {0, 0, 4, 9, 0, 0, 0, 0, 5},
            //    {6, 0, 0, 0, 2, 0, 0, 0, 0},
            //    {0, 7, 0, 0, 0, 0, 4, 8, 0},
            //});

            ////Blank
            //puzzleList.Add(new byte[9, 9]{
            //    {0, 0, 0, 0, 0, 0, 0, 0, 0},
            //    {0, 0, 0, 0, 0, 0, 0, 0, 0},
            //    {0, 0, 0, 0, 0, 0, 0, 0, 0},
            //    {0, 0, 0, 0, 0, 0, 0, 0, 0},
            //    {0, 0, 0, 0, 0, 0, 0, 0, 0},
            //    {0, 0, 0, 0, 0, 0, 0, 0, 0},
            //    {0, 0, 0, 0, 0, 0, 0, 0, 0},
            //    {0, 0, 0, 0, 0, 0, 0, 0, 0},
            //    {0, 0, 0, 0, 0, 0, 0, 0, 0},
            //});

            Random r = new Random();
            int randNum = r.Next(0, puzzleList.Count);
            Fill(puzzleList[randNum]);
        }

        private void button_clear_Click(object sender, EventArgs e)
        {
            foreach (MaskedTextBox mtb in input.Controls)
            {
                mtb.Text = "_";
            }
        }
    }
}
