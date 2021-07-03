using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace crossword
{
    public partial class Form1 : Form
    {
        Form2 clue_window = new Form2();
        List<id_cells>idc = new List<id_cells>();
        public String puzzle_file = Application.StartupPath + "\\Puzzles\\puzzle_1.txt";
        public int mr1, mr2, time;
        public string name;
        public bool flag = true;

        public Form1(string Name)
        {
            InitializeComponent();
            buildWordList();
            name = Name;
        }

        public Form1()
        {
            buildWordList();
            InitializeComponent();
        }

        //реализация выхода из программы
        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        //Загружаем информацию на вторую форму из файла
        private void buildWordList()
        {
            String line = "";
            using (StreamReader s = new StreamReader(puzzle_file))
            {
                line = s.ReadLine();
                while((line = s.ReadLine()) != null)
                {
                    String[] l = line.Split('|');
                    idc.Add(new id_cells(Int32.Parse(l[0]), Int32.Parse(l[1]), l[2], l[3], l[4], l[5]));
                    clue_window.clue_table.Rows.Add(new String[] { l[3], l[2], l[5] });
                }
            }
        }

        //Информационное окно
        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Если слово введено верно, то оно будет выделено зеленым цветом. Иначе красным.", "Сделано Галактионовым Никитой Олеговичем");
        }

        //Событие загрузки первой формы
        private void Form1_Load(object sender, EventArgs e)
        {
            InitializeBoard();
            clue_window.SetDesktopLocation(this.Location.X + this.Width + 1, this.Location.Y);
            clue_window.StartPosition = FormStartPosition.Manual;

            clue_window.Show();
            clue_window.clue_table.AutoResizeColumns();
        }

        //Реализация интерактивного пространства. Вывод пользователского интерфейса программы.
        private void InitializeBoard()
        {
            board.BackgroundColor = Color.Black;
            board.DefaultCellStyle.BackColor = Color.Black;

            for (int i = 0; i < 21; i++)
                board.Rows.Add();

            foreach (DataGridViewColumn c in board.Columns)
                c.Width = board.Width / board.Columns.Count;

            foreach (DataGridViewRow r in board.Rows)
                r.Height = board.Height / board.Rows.Count;

            for (int row = 0; row < board.Rows.Count; row++)
            {
                for (int col = 0; col < board.Columns.Count; col++)
                    board[col, row].ReadOnly = true;
            }

            foreach(id_cells i in idc)
            {
                int start_col = i.X;
                int start_row = i.Y;
                char[] word = i.word.ToCharArray();
                

                for (int j = 0; j < word.Length; j++)
                {
                    if (i.direction.ToUpper() == "ПО ГОРИЗОНТАЛИ")
                        formatCell(start_row, start_col + j, word[j].ToString());
                    if (i.direction.ToUpper() == "ПО ВЕРТИКАЛИ")
                        formatCell(start_row + j, start_col, word[j].ToString());                   
                }
            }

            for (int i = 0; i < board.Rows.Count; i++)
            {
                for (int j = 0; j < board.Columns.Count; j++)
                    if (board[j, i].Style.BackColor == Color.White)
                    {
                        mr2++;
                    }
            }

        }

        //Стилизация клеток поля(dataGridView)
        private void formatCell(int row, int col, String letter)
        {
            DataGridViewCell c = board[col, row];
            c.Style.BackColor = Color.White;
            c.ReadOnly = false;
            c.Style.SelectionBackColor = Color.Cyan;
            c.Tag = letter;
        }

        //Склеиваем две формы
        private void Form1_LocationChanged(object sender, EventArgs e)
        {
            clue_window.SetDesktopLocation(this.Location.X + this.Width + 1, this.Location.Y);

        }

        //Приводим вводимый текст в нужный для корректной работы программы формат. 
        //Добавляем раскраску верных и не верных слов.
        private void board_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                board[e.ColumnIndex, e.RowIndex].Value = board[e.ColumnIndex, e.RowIndex].Value.ToString().ToUpper();
            }
            catch
            { }

            try
            {
                if(board[e.ColumnIndex, e.RowIndex].Value.ToString().Length >1)
                    board[e.ColumnIndex, e.RowIndex].Value = board[e.ColumnIndex, e.RowIndex].Value.ToString().Substring(0, 1);
            }
            catch
            {}
            try
            {
                if (board[e.ColumnIndex, e.RowIndex].Value.ToString().ToUpper().Equals(board[e.ColumnIndex, e.RowIndex].Tag.ToString().ToUpper()))
                {
                    mr1++;
                }

                if (board[e.ColumnIndex, e.RowIndex].Value.ToString().ToUpper().Equals(board[e.ColumnIndex, e.RowIndex].Tag.ToString().ToUpper()))
                {
                    board[e.ColumnIndex, e.RowIndex].Style.ForeColor = Color.Green;
                    if ((mr1+1)/2 == mr2)
                    {
                        timer1.Enabled = false;
                        MessageBox.Show("Вы закончили со временем " + time + " секунд " + " ," + name, "Этап пройден!" );                        
                    }
                }
                else
                    board[e.ColumnIndex, e.RowIndex].Style.ForeColor = Color.Red;
            }
            catch
            { }
        }

        //Реализация возможности выбрать нужный файл с кроссвордом
        private void fileChoiseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            if (ofd.ShowDialog().Equals(DialogResult.OK))
            {
                ofd.InitialDirectory = "C:\\Users\\Никита\\OneDrive\\Рабочий стол\\crossword1\\crossword\\bin\\Debug\\puzzles";
                puzzle_file = ofd.FileName;
                board.Rows.Clear();
                clue_window.clue_table.Rows.Clear();
                idc.Clear();

                buildWordList();
                InitializeBoard();
                time = 0;
                mr2 = 0;

                for (int i = 0; i < board.Rows.Count; i++)
                {
                    for (int j = 0; j < board.Columns.Count; j++)
                        if (board[j, i].Style.BackColor == Color.White)
                        {
                            mr2++;
                        }
                }
            }
            }

        private void button1_Click(object sender, EventArgs e)
        {
            string str = name + " - " + time;
            using (StreamWriter sw = new StreamWriter("Рейтинг.txt", true, System.Text.Encoding.Default))
            {
                sw.WriteLine(str);
            }
            Application.Exit();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (flag == true)
            {
                clue_window.Hide();
                flag = !flag;
            }
            else
            {
                clue_window.Show();
                flag = !flag;
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            time++;
            label1.Text = time.ToString();
        }

        //Добавление номера строки в угол на рабочей доске.
        private void board_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            String number = "";
            if(idc.Any(c =>(number = c.number)!="" && c.X == e.ColumnIndex && c.Y == e.RowIndex))
            {
                Rectangle r = new Rectangle(e.CellBounds.X, e.CellBounds.Y, e.CellBounds.Width, e.CellBounds.Height);
                e.Graphics.FillRectangle(Brushes.White, r);
                Font f = new Font(e.CellStyle.Font.FontFamily, 7);
                e.Graphics.DrawString(number, f, Brushes.Black, r);
                e.PaintContent(e.ClipBounds);
                e.Handled = true;
            }
        }
    }

    public class id_cells
    {
        public int X;
        public int Y;
        public String direction;
        public String number;
        public String word;
        public String clue;

        public id_cells(int x , int y, String d, String n, String w, String c)
        {
            this.X = x;
            this.Y = y;
            this.direction = d;
            this.number = n;
            this.word = w;
            this.clue = c;
        }
    }
}
