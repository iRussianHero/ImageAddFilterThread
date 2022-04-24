using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp3
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        Bitmap bitmap;
        private void button1_Click(object sender, EventArgs e)
        {
            pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
            using (OpenFileDialog fileDialog = new OpenFileDialog())
            {
                fileDialog.Filter = "JPG|*.jpg|PNG|*.pmg|BMP|*.bmp";
                if (fileDialog.ShowDialog() == DialogResult.OK)
                {
                    bitmap = new Bitmap(fileDialog.FileName);
                    pictureBox1.Image = bitmap;
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Filter();
        }


        void Filter()
        {
            DateTime first = DateTime.Now;
            int h = bitmap.Height;
            int w = bitmap.Width;
            Bitmap bitmap2 = new Bitmap(w, h);

            for (int i = 0; i < h; i++)
            {
                for (int j = 0; j < w; j++)
                {
                    Color c = bitmap.GetPixel(j, i);
                    c = Color.FromArgb(c.G, c.B, c.R);
                    bitmap2.SetPixel(j, i, c);
                }
            }
            pictureBox2.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBox2.Image = bitmap2;
            DateTime second = DateTime.Now;
            MessageBox.Show((second - first).ToString());
        }

        private void button3_Click(object sender, EventArgs e)
        {

            Task task = new Task(new Action(() =>
            {
                Filter();
            }));

            task.Start();

        }

        private void button4_Click(object sender, EventArgs e)
        {
            // TODO :: разделить картинку 

            FilterAsync(2);
        }


        public void FilterAsync(int x)
        {
            // Таймер
            // DateTime first = DateTime.Now;
            Stopwatch stopeWatch = Stopwatch.StartNew();

            // Размеры картинки
            /////////////////////////////////////////////////////////////////////////////////////            
            Point fullBoxPoint = new Point(pictureBox1.Width, pictureBox1.Height);
            Point pictureBox0Point = new Point(0, 0);
            Point pictureBox1Point = new Point(pictureBox1.Width / 2, 0);
            Point pictureBox2Point = new Point(0, pictureBox1.Height / 2);
            Point pictureBox3Point = new Point(pictureBox1.Width / 2, pictureBox1.Height / 2);
            Size half = new Size(pictureBox1.Width / 2, pictureBox1.Height / 2);
            /////////////////////////////////////////////////////////////////////////////////////
            
            // Задать координаты по которым будет обрезаться исходная картинка
            /////////////////////////////////////////////////////////////////////////////////////
            Image img = pictureBox1.Image;
            Bitmap src = new Bitmap(img, pictureBox1.Width, pictureBox1.Height);
            Rectangle rect0 = new Rectangle(pictureBox0Point, half);
            Rectangle rect1 = new Rectangle(pictureBox1Point, half);
            Rectangle rect2 = new Rectangle(pictureBox2Point, half);
            Rectangle rect3 = new Rectangle(pictureBox3Point, half);
            //////////////////////////////////////////////////////////////////////////////////////

            // Разрезать картинку на 4 части
            //////////////////////////////////////////////////////////////////////////////////////
            Image img1 = CutImage(src, rect0);
            Image img2 = CutImage(src, rect2);
            Image img3 = CutImage(src, rect1);
            Image img4 = CutImage(src, rect3);
            //////////////////////////////////////////////////////////////////////////////////////

            // Нанести фильтр на разрезанные части в потоках
            ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////

            //Task task1 = new Task(new Action(() =>
            //{
            //    for (int i = 0; i < h /*/ x*/; i++)
            //    {
            //        for (int j = 0; j < w; ++j)
            //        {
            //            Color c = bitmap.GetPixel(j, i);
            //            c = Color.FromArgb(c.G, c.B, c.R);
            //            bitmap2.SetPixel(j, i, c);
            //        }
            //    }
            //}));
            //task1.Start();
            //task1.Wait();

            //Task task2 = new Task(new Action(() =>
            //{
            //    for (int i = h / 2; i < h; ++i)
            //    {
            //        for (int j = w / 2; j < w; ++j)
            //        {
            //            Color c = bitmap.GetPixel(j, i);
            //            c = Color.FromArgb(c.G, c.B, c.R);
            //            bitmap2.SetPixel(j, i, c);
            //        }
            //    }
            //}));
            //task2.Start();
            //task2.Wait();


            ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////


            // Склейка битмапов в картинку
            ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////

            Bitmap resultImg = new Bitmap(pictureBox1.Width, pictureBox1.Height);

            Graphics g = Graphics.FromImage(resultImg);

            g.DrawImage(img1, 0, 0);
            g.DrawImage(img2, 0, img1.Height / 2);
            g.DrawImage(img3, img3.Width / 2, 0);
            g.DrawImage(img4, img4.Width / 2, img1.Height / 2);
            ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////


            // Вывод картики в окно
            //////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            pictureBox2.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBox2.Image = resultImg;
            // Таймер
            //////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            //DateTime second = DateTime.Now;
            //MessageBox.Show((second - first).ToString());
            MessageBox.Show(stopeWatch.Elapsed.ToString());
            //////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        }


        public Bitmap CutImage(Bitmap src, Rectangle rect)
        {
            Bitmap bmp = new Bitmap(src.Width, src.Height); //создаем битмап
            Graphics g = Graphics.FromImage(bmp);
            g.DrawImage(src, 0, 0, rect, GraphicsUnit.Pixel); //перерисовываем с источника по координатам
            return bmp;
        }
    }
}
