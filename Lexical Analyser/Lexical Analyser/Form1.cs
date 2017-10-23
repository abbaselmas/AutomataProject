using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Lexical_Analyser
{
    public partial class Form1 : Form
    {
        lexgram abbas= new lexgram();
        public Form1()
        {
            InitializeComponent();

        }

        private void button1_Click(object sender, EventArgs e)
        {
            
            if (richTextBox1.Text.Length > 1)
            {
                abbas.kod = richTextBox1.Text;
                abbas.trimmer();
                richTextBox1.Text = abbas.kod;
            }
            
        }

        private void button2_Click(object sender, EventArgs e)
        {
            abbas.BeginWhileEndKontrol();

            if (richTextBox1.Text.Length > 1)
            {
                abbas.check();
                richTextBox2.Text = abbas.output;
                if (abbas.acceptStatement)
                {
                    textBox1.Text = "acceptable";
                    textBox1.BackColor = Color.Green;
                }
                else
                {
                    textBox1.Text = "unacceptable";
                    textBox1.BackColor = Color.Red;
                    //richTextBox2.Text = null; // Bunu sonradan düzelteceğim
                }
            }
        }

    }
}