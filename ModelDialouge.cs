using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

// namespace for custom dialouge box
namespace RookardMelissa_GameOfLife_Attempt2_
{
    public partial class ModelDialouge : Form
    {
        public ModelDialouge()
        {
            InitializeComponent();
        }
        public int GetSeed()
        {
            return (int)numericUpDown1.Value;
        }
        public void SetSeed(int seeder)
        {
            numericUpDown1.Value = seeder;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Random seedRandom = new Random();
            int seed = seedRandom.Next(int.MinValue, int.MaxValue);
            SetSeed(seed);
        }
    }
}
