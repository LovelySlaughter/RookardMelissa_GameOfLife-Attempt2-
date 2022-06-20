using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace RookardMelissa_GameOfLife_Attempt2_
{
    public partial class Form1 : Form
    {
        // HUD on and off bool
        bool hudOnOrOff = true;

        // Boundry type bool
        bool boundry = false;

        // Width
        static int wide = 50;

        // Height
        static int tall = 50;
        // The universe array
        bool[,] universe = new bool[wide, tall];

        //Scratchpad
        bool[,] scratch = new bool[wide, tall];

        // My Seed
        int seeder = 25;

        // Drawing colors
        Color gridColor = Color.Green;
        Color cellColor = Color.MediumPurple;
        Color hudcolor = Color.Black;

        // Font
        Font font = new Font("Arial", 12f);

        // The Timer class
        Timer timer = new Timer();

        // Generation count
        int generations = 0;

        // Neighbor count
        int numofneighbors = 0;

        public Form1()
        {
            InitializeComponent();

            // Setup the timer
            timer.Interval = 100; // milliseconds
            timer.Tick += Timer_Tick;
            timer.Enabled = false; // start timer running
        }
        private void BoundryCheck(int x, int y, out int counter)
        {
            if (boundry == false)
            {
                counter = CountNeighborsFinite(x, y);
            }
            else
            {
                counter = CountNeighborsToroidal(x, y);
            }
        }

        // Calculate the next generation of cells
        private void NextGeneration()
        {
            int living = 0;
            for (int y = 0; y < universe.GetLength(1); y++)
            {
                for (int x = 0; x < universe.GetLength(0); x++)
                {
                    if (universe[x, y] == true)
                    {
                        living++;
                    }
                    BoundryCheck(x, y, out int neighborcount);
                    if (universe[x, y] == true)
                    {
                        if (neighborcount < 2)
                        {
                            scratch[x, y] = false;
                        }
                        else if (neighborcount > 3)
                        {
                            scratch[x, y] = false;
                        }
                        else if (neighborcount == 2 || neighborcount == 3)
                        {
                            scratch[x, y] = true;
                        }
                    }
                    else
                    {
                        if (neighborcount == 3)
                        {
                            scratch[x, y] = true;
                        }
                    }
                }
            }
            bool[,] swapper = universe;
            universe = scratch;
            scratch = swapper;
            bool[,] emptyscratch = new bool[wide, tall];
            scratch = emptyscratch;
            // Increment generation count
            generations++;

            // Update status strip generations
            toolStripStatusLabelGenerations.Text = "Generations = " + generations.ToString();
            LivingCells.Text = "Living Cells = " + living.ToString();
            seedStatusLabel1.Text = "Seed = " + seeder.ToString();

            graphicsPanel1.Invalidate();
        }

        // The event called by the timer every Interval milliseconds.
        private void Timer_Tick(object sender, EventArgs e)
        {
            NextGeneration();
        }

        private void graphicsPanel1_Paint(object sender, PaintEventArgs e)
        {
            // Calculate the width and height of each cell in pixels
            // CELL WIDTH = WINDOW WIDTH / NUMBER OF CELLS IN X
            int cellWidth = graphicsPanel1.ClientSize.Width / universe.GetLength(0);
            // CELL HEIGHT = WINDOW HEIGHT / NUMBER OF CELLS IN Y
            int cellHeight = graphicsPanel1.ClientSize.Height / universe.GetLength(1);

            // A Pen for drawing the grid lines (color, width)
            Pen gridPen = new Pen(gridColor, 1);

            // A Brush for filling living cells interiors (color)
            Brush cellBrush = new SolidBrush(cellColor);
            // A Brush for the hud (color)
            Brush hbrush = new SolidBrush(hudcolor);

            // Iterate through the universe in the y, top to bottom
            for (int y = 0; y < universe.GetLength(1); y++)
            {
                // Iterate through the universe in the x, left to right
                for (int x = 0; x < universe.GetLength(0); x++)
                {
                    // A rectangle to represent each cell in pixels
                    Rectangle cellRect = Rectangle.Empty;
                    cellRect.X = x * cellWidth;
                    cellRect.Y = y * cellHeight;
                    cellRect.Width = cellWidth;
                    cellRect.Height = cellHeight;
                    // Fill the cell with a brush if alive
                    if (universe[x, y] == true)
                    {
                        e.Graphics.FillRectangle(cellBrush, cellRect);
                    }

                    // Outline the cell with a pen
                    e.Graphics.DrawRectangle(gridPen, cellRect.X, cellRect.Y, cellRect.Width, cellRect.Height);


                }
            }

            // HUD
            int living = 0;
            for (int y = 0; y < universe.GetLength(1); y++)
            {
                for (int x = 0; x < universe.GetLength(0); x++)
                {
                    if (universe[x, y] == true)
                    {
                        living++;
                    }
                }
            }
            if (hudOnOrOff == true)
            {
                RectangleF hud = RectangleF.Empty;
                string display = "Generation = " + generations + " Living Cells = " + living + " Universe Size = " + wide + ", " + tall;
                hud.Width = graphicsPanel1.Width;
                hud.Height = graphicsPanel1.Height;
                hud.X = graphicsPanel1.Width / 2;
                hud.Y = graphicsPanel1.Height - 20;

                e.Graphics.DrawString(display.ToString(), font, hbrush, hud);
            }

            // Cleaning up pens and brushes
            gridPen.Dispose();
            cellBrush.Dispose();
        }

        private void graphicsPanel1_MouseClick(object sender, MouseEventArgs e)
        {
            int living = 0;
            // If the left mouse button was clicked
            if (e.Button == MouseButtons.Left)
            {
                // Calculate the width and height of each cell in pixels
                int cellWidth = graphicsPanel1.ClientSize.Width / universe.GetLength(0);
                int cellHeight = graphicsPanel1.ClientSize.Height / universe.GetLength(1);

                // Calculate the cell that was clicked in
                // CELL X = MOUSE X / CELL WIDTH
                int x = e.X / cellWidth;
                // CELL Y = MOUSE Y / CELL HEIGHT
                int y = e.Y / cellHeight;

                // Toggle the cell's state
                universe[x, y] = !universe[x, y];

                toolStripStatusLabelGenerations.Text = "Generations = " + generations.ToString();
                LivingCells.Text = "Living Cells = " + living.ToString();
                seedStatusLabel1.Text = "Seed = " + seeder.ToString();
                // Tell Windows you need to repaint
                graphicsPanel1.Invalidate();
            }
        }
        public bool IsAlive(int x, int y)
        {
            bool status = universe[x, y];
            return status;
        }

        private void playToolStripMenuItem_Click(object sender, EventArgs e)
        {
            timer.Enabled = true;
        }

        private void pauseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            timer.Enabled = false;
        }

        private void resetToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int living = 0;
            timer.Enabled = false;
            for (int y = 0; y < universe.GetLength(1); y++)
            {
                for (int x = 0; x < universe.GetLength(0); x++)
                {
                    universe[x, y] = false;
                }
            }
            generations = 0;
            toolStripStatusLabelGenerations.Text = "Generations = " + generations.ToString();
            LivingCells.Text = "Living Cells = " + living.ToString();
            seedStatusLabel1.Text = "Seed = " + seeder.ToString();
            graphicsPanel1.Invalidate();
        }
        private int CountNeighborsFinite(int x, int y)
        {
            int count = 0;
            int xLen = universe.GetLength(0);
            int yLen = universe.GetLength(1);
            for (int yOffset = -1; yOffset <= 1; yOffset++)
            {
                for (int xOffset = -1; xOffset <= 1; xOffset++)
                {
                    int xCheck = x + xOffset;
                    int yCheck = y + yOffset;
                    // if xOffset and yOffset are both equal to 0 then continue
                    if (xOffset == 0 && yOffset == 0)
                    {
                        continue;
                    }
                    // if xCheck is less than 0 then continue
                    if (xCheck < 0)
                    {
                        continue;
                    }
                    // if yCheck is less than 0 then continue
                    if (yCheck < 0)
                    {
                        continue;
                    }
                    // if xCheck is greater than or equal too xLen then continue
                    if (xCheck >= xLen)
                    {
                        continue;
                    }
                    // if yCheck is greater than or equal too yLen then continue
                    if (yCheck >= yLen)
                    {
                        continue;
                    }

                    if (universe[xCheck, yCheck] == true) count++;
                }
            }
            return count;
        }
        private int CountNeighborsToroidal(int x, int y)
        {
            int count = 0;
            int xLen = universe.GetLength(0);
            int yLen = universe.GetLength(1);
            for (int yOffset = -1; yOffset <= 1; yOffset++)
            {
                for (int xOffset = -1; xOffset <= 1; xOffset++)
                {
                    int xCheck = x + xOffset;
                    int yCheck = y + yOffset;
                    // if xOffset and yOffset are both equal to 0 then continue
                    if (xOffset == 0 && yOffset == 0)
                    {
                        continue;
                    }
                    // if xCheck is less than 0 then set to xLen - 1
                    if (xCheck < 0)
                    {
                        xCheck = xLen - 1;
                    }
                    // if yCheck is less than 0 then set to yLen - 1
                    if (yCheck < 0)
                    {
                        yCheck = yLen - 1;
                    }
                    // if xCheck is greater than or equal too xLen then set to 0
                    if (xCheck >= xLen)
                    {
                        xCheck = 0;
                    }
                    // if yCheck is greater than or equal too yLen then set to 0
                    if (yCheck >= yLen)
                    {
                        yCheck = 0;
                    }
                    if (universe[xCheck, yCheck] == true) count++;
                }
            }
            return count;
        }

        private void pToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }
        // Button to move foreward one generation
        private void nextGenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            NextGeneration();
        }
        // Button to randomize
        private void randomToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Random gen = new Random();
            for (int y = 0; y < universe.GetLength(1); y++)
            {
                for (int x = 0; x < universe.GetLength(0); x++)
                {
                    universe[x, y] = gen.Next(100) <= 34 ? true : false;
                }
            }

            // Invalidate Graphics Panel
            graphicsPanel1.Invalidate();
        }

        private void stillLifeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            universe[3, 2] = true;
            universe[4, 2] = true;
            universe[17, 2] = true;
            universe[18, 2] = true;
            universe[3, 3] = true;
            universe[4, 3] = true;
            universe[16, 3] = true;
            universe[19, 3] = true;
            universe[16, 4] = true;
            universe[19, 4] = true;
            universe[17, 5] = true;
            universe[18, 5] = true;
            universe[30, 24] = true;
            universe[29, 24] = true;
            universe[28, 25] = true;
            universe[31, 25] = true;
            universe[29, 26] = true;
            universe[30, 26] = true;
            universe[45, 40] = true;
            universe[46, 40] = true;
            universe[44, 41] = true;
            universe[47, 41] = true;
            universe[44, 42] = true;
            universe[47, 42] = true;
            universe[45, 43] = true;
            universe[46, 43] = true;
            universe[13, 32] = true;
            universe[14, 32] = true;
            universe[13, 33] = true;
            universe[16, 32] = true;
            universe[16, 33] = true;
            universe[15, 33] = true;
            universe[3, 6] = true;
            universe[4, 6] = true;
            universe[3, 7] = true;
            universe[4, 7] = true;
            universe[41, 10] = true;
            universe[42, 10] = true;
            universe[41, 11] = true;
            universe[44, 10] = true;
            universe[44, 11] = true;
            universe[43, 11] = true;
            // Invalidate Graphics Panel
            graphicsPanel1.Invalidate();
        }

        private void gliderRunToolStripMenuItem_Click(object sender, EventArgs e)
        {
            universe[4, 14] = true;
            universe[5, 14] = true;
            universe[4, 15] = true;
            universe[5, 15] = true;
            universe[14, 14] = true;
            universe[14, 15] = true;
            universe[14, 16] = true;
            universe[15, 13] = true;
            universe[15, 17] = true;
            universe[16, 12] = true;
            universe[17, 12] = true;
            universe[16, 18] = true;
            universe[16, 18] = true;
            universe[18, 15] = true;
            universe[19, 13] = true;
            universe[19, 17] = true;
            universe[20, 14] = true;
            universe[20, 15] = true;
            universe[20, 16] = true;
            universe[21, 15] = true;
            universe[24, 14] = true;
            universe[24, 13] = true;
            universe[24, 12] = true;
            universe[25, 14] = true;
            universe[25, 13] = true;
            universe[25, 12] = true;
            universe[26, 11] = true;
            universe[26, 15] = true;
            universe[28, 10] = true;
            universe[28, 11] = true;
            universe[28, 15] = true;
            universe[28, 16] = true;
            universe[38, 12] = true;
            universe[38, 13] = true;
            universe[39, 12] = true;
            universe[39, 13] = true;
            graphicsPanel1.Invalidate();
        }
        // Button to randomize with a random seed
        private void randomSeedToolStripMenuItem1_Click(object sender, EventArgs e)
        {

            Random randseed = new Random();

            for (int y = 0; y < universe.GetLength(1); y++)
            {
                for (int x = 0; x < universe.GetLength(0); x++)
                {
                    seeder = randseed.Next(0, 2);
                    if (seeder == 0)
                    {
                        universe[x, y] = true;
                    }
                    else if (seeder == 1 || seeder == 2)
                    {
                        universe[x, y] = false;
                    }
                }
            }
            graphicsPanel1.Invalidate();
        }
        // tool that allows user to input a custom seed for custom randomization
        private void customSeedToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }
        // tool to save your setting and universe to a file
        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog dlg = new SaveFileDialog();
            dlg.Filter = "All Files|*.*|Cells|*.cells";
            dlg.FilterIndex = 2; dlg.DefaultExt = "cells";
            if (DialogResult.OK == dlg.ShowDialog())
            {
                // Use of the using opens and closes the file for me
                using (StreamWriter writer = new StreamWriter(dlg.FileName))
                {


                    writer.WriteLine(dlg.FileName);
                    // Iterate through the universe one row at a time.
                    for (int y = 0; y < universe.GetLength(1); y++)
                    {
                        // Create a string to represent the current row.
                        String currentRow = "";

                        // Iterate through the current row one cell at a time.
                        for (int x = 0; x < universe.GetLength(0); x++)
                        {
                            // If the universe[x,y] is alive then append 'O' (capital O) to the row string.
                            if (universe[x, y] == true)
                            {
                                currentRow += "O";
                            }
                            // Else if the universe[x,y] is dead then append '.' (period)
                            else if (universe[x, y] == false)
                            {
                                currentRow += ".";
                            }
                            // to the row string.    
                        }
                        writer.WriteLine(currentRow);

                        // Once the current row has been read through and the 
                        // string constructed then write it to the file using WriteLine.
                    }
                }
                // After all rows and columns have been written then close the file.
            }

        }
        // tool to start a new universe
        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int living = 0;
            timer.Enabled = false;
            for (int y = 0; y < universe.GetLength(1); y++)
            {
                for (int x = 0; x < universe.GetLength(0); x++)
                {
                    universe[x, y] = false;
                }
            }
            generations = 0;
            toolStripStatusLabelGenerations.Text = "Generations = " + generations.ToString();
            LivingCells.Text = "Living Cells = " + living.ToString();
            seedStatusLabel1.Text = "Seed = " + seeder.ToString();
            graphicsPanel1.Invalidate();
        }
        // tool to open a saved file
        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "All Files|*.*|Cells|*.cells";
            dlg.FilterIndex = 2;

            if (DialogResult.OK == dlg.ShowDialog())
            {
                // Use of the using opens and closes the file for me
                using (StreamReader reader = new StreamReader(dlg.FileName))
                {


                    // Create a couple variables to calculate the width and height
                    // of the data in the file.
                    int maxWidth = 0;
                    int maxHeight = 0;

                    // Iterate through the file once to get its size.
                    while (!reader.EndOfStream)
                    {
                        // Read one row at a time.
                        string row = reader.ReadLine();

                        // If the row begins with '!' then it is a comment
                        // and should be ignored.
                        if (row.StartsWith("!"))
                        {
                            continue;
                        }

                        // If the row is not a comment then it is a row of cells.
                        // Increment the maxHeight variable for each row read.
                        if (row.Contains("O") || row.Contains("."))
                        {
                            maxHeight++;
                        }
                        // Get the length of the current row string
                        // and adjust the maxWidth variable if necessary.
                        if (row.Length > maxWidth)
                        {
                            maxWidth = row.Length;
                        }
                    }

                    // Resize the current universe and scratchPad
                    // to the width and height of the file calculated above.
                    wide = maxWidth;
                    tall = maxHeight;
                    universe = new bool[wide, tall];
                    scratch = new bool[wide, tall];
                    // Reset the file pointer back to the beginning of the file.
                    reader.BaseStream.Seek(0, SeekOrigin.Begin);
                    int y = 0;
                    // Iterate through the file again, this time reading in the cells.
                    while (!reader.EndOfStream)
                    {
                        // Read one row at a time.
                        string row = reader.ReadLine();

                        // If the row begins with '!' then
                        // it is a comment and should be ignored.
                        if (row.StartsWith("!"))
                        {
                            continue;
                        }
                        // If the row is not a comment then 
                        // it is a row of cells and needs to be iterated through.
                        if (row.Contains("O") || row.Contains("."))
                        {


                            for (int xPos = 0; xPos < row.Length; xPos++)
                            {
                                // If row[xPos] is a 'O' (capital O) then
                                // set the corresponding cell in the universe to alive.
                                if (row[xPos] == 'O')
                                {
                                    universe[xPos, y] = true;
                                }
                                // If row[xPos] is a '.' (period) then
                                // set the corresponding cell in the universe to dead.
                                else if (row[xPos] == '.')
                                {
                                    universe[xPos, y] = false;
                                }
                            }
                            y++;
                        }
                    }
                }
            }
            graphicsPanel1.Invalidate();
        }
        // tool to open a saved file
        private void openToolStripButton_Click(object sender, EventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "All Files|*.*|Cells|*.cells";
            dlg.FilterIndex = 2;

            if (DialogResult.OK == dlg.ShowDialog())
            {
                // Use of the using opens and closes the file for me
                using (StreamReader reader = new StreamReader(dlg.FileName))
                {


                    // Create a couple variables to calculate the width and height
                    // of the data in the file.
                    int maxWidth = 0;
                    int maxHeight = 0;

                    // Iterate through the file once to get its size.
                    while (!reader.EndOfStream)
                    {
                        // Read one row at a time.
                        string row = reader.ReadLine();

                        // If the row begins with '!' then it is a comment
                        // and should be ignored.
                        if (row.StartsWith("!"))
                        {
                            continue;
                        }

                        // If the row is not a comment then it is a row of cells.
                        // Increment the maxHeight variable for each row read.
                        if (row.Contains("O") || row.Contains("."))
                        {
                            maxHeight++;
                        }
                        // Get the length of the current row string
                        // and adjust the maxWidth variable if necessary.
                        if (row.Length > maxWidth)
                        {
                            maxWidth = row.Length;
                        }
                    }

                    // Resize the current universe and scratchPad
                    // to the width and height of the file calculated above.
                    wide = maxWidth;
                    tall = maxHeight;
                    universe = new bool[wide, tall];
                    scratch = new bool[wide, tall];
                    // Reset the file pointer back to the beginning of the file.
                    reader.BaseStream.Seek(0, SeekOrigin.Begin);
                    int y = 0;
                    // Iterate through the file again, this time reading in the cells.
                    while (!reader.EndOfStream)
                    {
                        // Read one row at a time.
                        string row = reader.ReadLine();

                        // If the row begins with '!' then
                        // it is a comment and should be ignored.
                        if (row.StartsWith("!"))
                        {
                            continue;
                        }
                        // If the row is not a comment then 
                        // it is a row of cells and needs to be iterated through.
                        if (row.Contains("O") || row.Contains("."))
                        {


                            for (int xPos = 0; xPos < row.Length; xPos++)
                            {
                                // If row[xPos] is a 'O' (capital O) then
                                // set the corresponding cell in the universe to alive.
                                if (row[xPos] == 'O')
                                {
                                    universe[xPos, y] = true;
                                }
                                // If row[xPos] is a '.' (period) then
                                // set the corresponding cell in the universe to dead.
                                else if (row[xPos] == '.')
                                {
                                    universe[xPos, y] = false;
                                }
                            }
                            y++;
                        }
                    }
                }
            }
            graphicsPanel1.Invalidate();
        }

        private void cutToolStripButton_Click(object sender, EventArgs e)
        {

        }
        // to turn hud on
        private void hUDONToolStripMenuItem_Click(object sender, EventArgs e)
        {
            hUDONToolStripMenuItem.Checked = true;
            hUDOFFToolStripMenuItem.Checked = false;
            hudOnOrOff = true;
            graphicsPanel1.Invalidate();
        }
        // to turn hud off
        private void hUDOFFToolStripMenuItem_Click(object sender, EventArgs e)
        {
            hUDONToolStripMenuItem.Checked = false;
            hUDOFFToolStripMenuItem.Checked = true;
            hudOnOrOff = false;
            graphicsPanel1.Invalidate();
        }
        // to turn hud on
        private void hUDONToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            hUDOFFToolStripMenuItem1.Checked = false;
            hUDONToolStripMenuItem1.Checked = true;
            hudOnOrOff = true;
        }
        // to turn hud off
        private void hUDOFFToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            hUDOFFToolStripMenuItem1.Checked = true;
            hUDONToolStripMenuItem1.Checked = false;
            hudOnOrOff = false;
        }
        // Grid Color Options
        private void blackToolStripMenuItem_Click(object sender, EventArgs e)
        {
            gridColor = Color.Black;
            graphicsPanel1.Invalidate();
        }

        private void greenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            gridColor = Color.Green;
            graphicsPanel1.Invalidate();
        }

        private void blueToolStripMenuItem_Click(object sender, EventArgs e)
        {
            gridColor = Color.Blue;
            graphicsPanel1.Invalidate();
        }

        private void redToolStripMenuItem_Click(object sender, EventArgs e)
        {
            gridColor = Color.Red;
            graphicsPanel1.Invalidate();
        }

        private void purpleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            gridColor = Color.Purple;
            graphicsPanel1.Invalidate();
        }

        private void orangeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            gridColor = Color.Orange;
            graphicsPanel1.Invalidate();
        }

        private void yellowToolStripMenuItem_Click(object sender, EventArgs e)
        {
            gridColor = Color.Yellow;
            graphicsPanel1.Invalidate();
        }
        // End of Grid color options
        //Start of Cell Color Options
        private void blackToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            cellColor = Color.Black;
            graphicsPanel1.Invalidate();
        }

        private void greenToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            cellColor = Color.Green;
            graphicsPanel1.Invalidate();
        }

        private void blueToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            cellColor = Color.Blue;
            graphicsPanel1.Invalidate();
        }

        private void redToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            cellColor = Color.Red;
            graphicsPanel1.Invalidate();
        }

        private void purpleToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            cellColor = Color.Purple;
            graphicsPanel1.Invalidate();
        }

        private void orangeToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            cellColor = Color.Orange;
            graphicsPanel1.Invalidate();
        }

        private void yellowToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            cellColor = Color.Yellow;
            graphicsPanel1.Invalidate();
        }
        // End of Cell color options
        //Start of BacGround Color Options
        private void whiteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            graphicsPanel1.BackColor = Color.White;
            graphicsPanel1.Invalidate();
        }

        private void blackToolStripMenuItem2_Click(object sender, EventArgs e)
        {
            graphicsPanel1.BackColor = Color.Black;
            graphicsPanel1.Invalidate();
        }

        private void greenToolStripMenuItem2_Click(object sender, EventArgs e)
        {
            graphicsPanel1.BackColor = Color.Green;
            graphicsPanel1.Invalidate();
        }

        private void blueToolStripMenuItem2_Click(object sender, EventArgs e)
        {
            graphicsPanel1.BackColor = Color.Blue;
            graphicsPanel1.Invalidate();
        }

        private void redToolStripMenuItem2_Click(object sender, EventArgs e)
        {
            graphicsPanel1.BackColor = Color.Red;
            graphicsPanel1.Invalidate();
        }

        private void purpleToolStripMenuItem2_Click(object sender, EventArgs e)
        {
            graphicsPanel1.BackColor = Color.Purple;
            graphicsPanel1.Invalidate();
        }

        private void orangeToolStripMenuItem2_Click(object sender, EventArgs e)
        {
            graphicsPanel1.BackColor = Color.Orange;
            graphicsPanel1.Invalidate();
        }

        private void yellowToolStripMenuItem2_Click(object sender, EventArgs e)
        {
            graphicsPanel1.BackColor = Color.Yellow;
            graphicsPanel1.Invalidate();
        }
        // end of background color options
        private void customSeedToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            ModelDialouge randomseedgen = new ModelDialouge();
            randomseedgen.SetSeed(seeder);
            if (DialogResult.OK == randomseedgen.ShowDialog())
            {
                seeder = randomseedgen.GetSeed();
                Random randseed = new Random();
                int saveSeed;
                for (int y = 0; y < universe.GetLength(1); y++)
                {
                    for (int x = 0; x < universe.GetLength(0); x++)
                    {
                        saveSeed = randseed.Next(0, 2);
                        if (saveSeed == 0)
                        {
                            universe[x, y] = true;
                        }
                        else
                        {
                            universe[x, y] = false;
                        }
                    }
                }
            }
            graphicsPanel1.Invalidate();
        }

        private void finiteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            finiteToolStripMenuItem.CheckState = CheckState.Checked;
            toroidalToolStripMenuItem.CheckState = CheckState.Unchecked;
            boundry = false;
        }

        private void toroidalToolStripMenuItem_Click(object sender, EventArgs e)
        {
            toroidalToolStripMenuItem.CheckState = CheckState.Checked;
            finiteToolStripMenuItem.CheckState = CheckState.Unchecked;
            boundry = true;
        }

        private void gameSpeedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ModelDialougeTimer timerMenu = new ModelDialougeTimer();
            timerMenu.SetTimer(timer.Interval);
            if (DialogResult.OK == timerMenu.ShowDialog())
            {
                timer.Interval = timerMenu.GetTimer();
            }
            graphicsPanel1.Invalidate();
        }

        private void universeSizeToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            ModelDialougeUniResizer uniResizer = new ModelDialougeUniResizer();
            uniResizer.SetX(wide);
            uniResizer.SetY(tall);
            if (DialogResult.OK == uniResizer.ShowDialog())
            {
                wide = uniResizer.GetX();
                tall = uniResizer.GetY();
                universe = new bool[wide, tall];
                scratch = new bool[wide, tall];
            }
            graphicsPanel1.Invalidate();
        }
    }
}
