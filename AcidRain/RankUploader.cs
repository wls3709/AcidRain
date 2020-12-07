using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AcidRain
{
    public partial class RankUploader : Form
    {
        public Form1 parent;
        public RankUploader()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string name = textBox1.Text;
            parent.InsertRanking(name);
            this.Dispose();
        }

        private void RankUploader_FormClosed(object sender, FormClosedEventArgs e)
        {
            parent.OpenRankList();
        }
    }
}
