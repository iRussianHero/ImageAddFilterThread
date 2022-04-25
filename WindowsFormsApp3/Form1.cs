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
            Task task = new Task(new Action(() =>
            {
                ConvertImageAsync();
            }));
            task.Start();
        }


        public void ConvertImageAsync()
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
            Image img0 = CutImage(src, rect0);
            Image img1 = CutImage(src, rect2);
            Image img2 = CutImage(src, rect1);
            Image img3 = CutImage(src, rect3);
            //////////////////////////////////////////////////////////////////////////////////////

            // Нанести фильтр на разрезанные части в потоках
            ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            Task task0 = new Task(new Action(() =>
             {
                 Bitmap src0 = new Bitmap(img0, pictureBox1.Width, pictureBox1.Height);
                 img0 = Filter(src0);
             }));
            task0.Start();

            Task task1 = new Task(new Action(() =>
            {
                Bitmap src1 = new Bitmap(img1, pictureBox1.Width, pictureBox1.Height);
                img1 = Filter(src1);
            }));
            task1.Start();

            Task task2 = new Task(new Action(() =>
            {
                Bitmap src2 = new Bitmap(img2, pictureBox1.Width, pictureBox1.Height);
                img2 = Filter(src2);
            }));
            task2.Start();

            Task task3 = new Task(new Action(() =>
            {
                Bitmap src3 = new Bitmap(img3, pictureBox1.Width, pictureBox1.Height);
                img3 = Filter(src3);
            }));
            task3.Start();

            task0.Wait();
            task1.Wait();
            task2.Wait();
            task3.Wait();
            ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            Task.Run(() => GlueImage(img0, img1, img2, img3, stopeWatch));
        }

        public Bitmap CutImage(Bitmap src, Rectangle rect)
        {
            Bitmap bmp = new Bitmap(src.Width, src.Height); //создаем битмап
            Graphics g = Graphics.FromImage(bmp);
            g.DrawImage(src, 0, 0, rect, GraphicsUnit.Pixel); //перерисовываем с источника по координатам
            return bmp;
        }
        public void GlueImage(Image img0, Image img1, Image img2, Image img3, Stopwatch stopeWatch)
        {
            // Склейка битмапов в картинку
            ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            Bitmap resultImg = new Bitmap(pictureBox1.Width, pictureBox1.Height);

            Graphics g = Graphics.FromImage(resultImg);

            g.DrawImage(img0, 0, 0);
            g.DrawImage(img1, 0, img0.Height / 2);
            g.DrawImage(img2, img2.Width / 2, 0);
            g.DrawImage(img3, img3.Width / 2, img0.Height / 2);
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



        public Image Filter(Bitmap img)
        {
            for (int i = 0; i < img.Height; ++i)
            {
                for (int j = 0; j < img.Width; ++j)
                {
                    Color color = img.GetPixel(j, i);
                    color = Color.FromArgb(color.G, color.B, color.R);
                    img.SetPixel(j, i, color);
                }
            }
            Rectangle rect = new Rectangle(0, 0, img.Width, img.Height);
            return CutImage(img, rect);
        }
    }
}
