using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace crossword
{
    public partial class Form3 : Form
    {
        string name ;

        public Form3()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            name = textBox1.Text;
            Form ifrm = new Form1(name);
            ifrm.Show(); // отображаем Form2
            Hide(); // скрываем Form3 
            
        }

        private void Form3_Load(object sender, EventArgs e)
        {
            string path = "Рейтинг.txt";
            StreamReader read = new StreamReader(path, Encoding.Default);
            string str;
            while ((str = read.ReadLine()) != null)
            {
                string[] data = str.Split('-');
                dataGridView1.Rows.Add(data);
            }
            read.Close();
            dataGridView1.AutoSize = true;
        }
    }
}
