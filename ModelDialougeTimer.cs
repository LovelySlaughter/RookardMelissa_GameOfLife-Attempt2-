using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RookardMelissa_GameOfLife_Attempt2_
{
    public partial class ModelDialougeTimer : Form
    {
        public ModelDialougeTimer()
        {
            InitializeComponent();
        }
        public int GetTimer()
        {
            return (int)numericUpDown1.Value;
        }
        public void SetTimer(int time)
        {
            numericUpDown1.Value = time;
        }
    }
}
