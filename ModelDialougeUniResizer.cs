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
    public partial class ModelDialougeUniResizer : Form
    {
        public ModelDialougeUniResizer()
        {
            InitializeComponent();
        }
        public int GetX()
        {
            return (int)numericUpDown1.Value;
        }
        public void SetX(int wide)
        {
            numericUpDown1.Value = wide;
        }
        public int GetY()
        {
            return (int)numericUpDown2.Value;
        }
        public void SetY(int tall)
        {
            numericUpDown2.Value = tall;
        }
    }
}
