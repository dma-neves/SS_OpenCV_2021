using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SS_OpenCV
{
    public partial class FilterForm : Form
    {
        public float[,] matrix = new float[3, 3];
        public float weight, offset;

        public FilterForm()
        {
            InitializeComponent();
        }


        private void button2_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox1.Text.Equals("Edge Highlighting"))
            {
                textBox1.Text = "-1";
                textBox2.Text = "-1";
                textBox3.Text = "-1";
                textBox4.Text = "-1";
                textBox5.Text = "9";
                textBox6.Text = "-1";
                textBox7.Text = "-1";
                textBox8.Text = "-1";
                textBox9.Text = "-1";

                textBox10.Text = "1";
                textBox11.Text = "0";
            }

            if (comboBox1.Text.Equals("Gaussian"))
            {
                textBox1.Text = "1";
                textBox2.Text = "2";
                textBox3.Text = "1";
                textBox4.Text = "2";
                textBox5.Text = "4";
                textBox6.Text = "2";
                textBox7.Text = "1";
                textBox8.Text = "2";
                textBox9.Text = "1";

                textBox10.Text = "16";
                textBox11.Text = "0";
            }

            if (comboBox1.Text.Equals("Laplacian Hard"))
            {
                textBox1.Text = "1";
                textBox2.Text = "-2";
                textBox3.Text = "1";
                textBox4.Text = "-2";
                textBox5.Text = "4";
                textBox6.Text = "-2";
                textBox7.Text = "1";
                textBox8.Text = "-2";
                textBox9.Text = "1";

                textBox10.Text = "16";
                textBox11.Text = "0";
            }

            if (comboBox1.Text.Equals("Vertical Lines"))
            {
                textBox1.Text = "0";
                textBox2.Text = "0";
                textBox3.Text = "0";
                textBox4.Text = "-1";
                textBox5.Text = "2";
                textBox6.Text = "-1";
                textBox7.Text = "0";
                textBox8.Text = "0";
                textBox9.Text = "0";

                textBox10.Text = "16";
                textBox11.Text = "128";
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // TODO verify return of tryparse
            float.TryParse(textBox1.Text, out matrix[0, 0]);
            float.TryParse(textBox2.Text, out matrix[0, 1]);
            float.TryParse(textBox3.Text, out matrix[0, 2]);
            float.TryParse(textBox4.Text, out matrix[1, 0]);
            float.TryParse(textBox5.Text, out matrix[1, 1]);
            float.TryParse(textBox6.Text, out matrix[1, 2]);
            float.TryParse(textBox7.Text, out matrix[2, 0]);
            float.TryParse(textBox8.Text, out matrix[2, 1]);
            float.TryParse(textBox9.Text, out matrix[2, 2]);

            float.TryParse(textBox10.Text, out weight);
            float.TryParse(textBox11.Text, out offset);

            if (weight < 0)
                weight = 0;
            if (weight > 255)
                weight = 255;
            if (offset < 0)
                offset = 0;
            if (offset > 255)
                offset = 255;

            DialogResult = DialogResult.OK;
        }
    }
}
