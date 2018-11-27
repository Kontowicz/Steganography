using Microsoft.Win32;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace stenography
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private bool rectangle = false;
        private BitmapImage test;
        private System.Drawing.Bitmap work;
        public MainWindow()
        {
            InitializeComponent();
        }

        void checkRectangle(object sender, RoutedEventArgs e)
        {
            rectangle = true;
            border.BorderBrush = new SolidColorBrush(Colors.Blue);
        }

        private void save(object sender, RoutedEventArgs e)
        {
            SaveFileDialog dialog = new SaveFileDialog();
            dialog.Title = "Save a picture";
            dialog.Filter = "Portable Network Graphic (*.png)|*.png|" + "Bmp files (*.bmp)|*.bmp";


            if (dialog.ShowDialog() == true)
            {
                //var encoder = new JpegBitmapEncoder();
                //encoder.Frames.Add(BitmapFrame.Create(test));
                //encoder.QualityLevel = 100;

                //using (var stream = new FileStream(dialog.FileName, FileMode.Create))
                //{
                //    encoder.Save(stream);
                //}
                work.Save(dialog.FileName);
            }
        }

        private void read(object sender, RoutedEventArgs e)
        {
            if (rectangle)
            {
                try
                {
                    OpenFileDialog op = new OpenFileDialog();
                    op.Title = "Select a picture";
                    op.Filter = "Portable Network Graphic (*.png)|*.png|" + "Bmp files (*.bmp)|*.bmp";
                    if (op.ShowDialog() == true)
                    {
                        img.Source = new BitmapImage(new Uri(op.FileName));
                        work = new Bitmap(op.FileName);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Coś poszło nie tak.\n Orginal message:" + ex.Message, "Błąd");
                }
                border.BorderBrush = new SolidColorBrush(Colors.Black);
                rectangle = false;

            }
            else
            {
                try
                {
                    OpenFileDialog op = new OpenFileDialog();
                    if (op.ShowDialog() == true)
                        text.Text = File.ReadAllText(op.FileName);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Coś poszło nie tak.\n Orginal message:" + ex.Message, "Błąd");
                }
            }
        }

        private bool checkText(string text)
        {
            string s = Regex.Replace(text, "^[0-9a-zA-Z]", "");
            if (s == text)
                return true;
            return false;
        }

        private void hide(BitArray message)
        {
            int cnt = 0;
            for (int i = 0; i < work.Height && cnt < message.Length; ++i)
            {
                for (int j = 0; j < work.Width && cnt < message.Length; ++j)
                {
                    System.Drawing.Color tmp = work.GetPixel(j, i);
                    int[] val = new int[4];
                    val[0] = tmp.A - tmp.A % 2 + (message[cnt] ? 1 : 0);
                    ++cnt;
                   // System.Diagnostics.Debug.Write(val[0]);
                    val[1] = tmp.R - tmp.R % 2 + (message[cnt] ? 1 : 0);
                    ++cnt;
                   // System.Diagnostics.Debug.Write(val[1]);
                    val[2] = tmp.G - tmp.G % 2 + (message[cnt] ? 1 : 0);
                    ++cnt;
                   // System.Diagnostics.Debug.Write(val[2]);
                    val[3] = tmp.B - tmp.B % 2 + (message[cnt] ? 1 : 0);
                    ++cnt;
                   // System.Diagnostics.Debug.Write(val[3]);

                    work.SetPixel(j, i, System.Drawing.Color.FromArgb(val[0], val[1], val[2], val[3]));
                    //System.Diagnostics.Debug.Write(work.GetPixel(j, i));
                }
            }
        }

        private void get()
        {
            long cnt = 0;
            bool[] arr = new bool[1000];
            for (int i = 0; i < work.Height && cnt < 10; ++i)
            {
                for (int j = 0; j < work.Width && cnt < 10; ++j)
                {
                    System.Drawing.Color tmp = work.GetPixel(j, i);

                    arr[cnt+0] = tmp.A % 2 == 1;
                    System.Diagnostics.Debug.Write(arr[cnt + 0] + " ");
                    arr[cnt+1] = tmp.R % 2 == 1;
                    System.Diagnostics.Debug.Write(arr[cnt + 1] + " ");
                    arr[cnt+2] = tmp.G % 2 == 1;
                    System.Diagnostics.Debug.Write(arr[cnt + 2] + " ");
                    arr[cnt+3] = tmp.B % 2 == 1;
                    System.Diagnostics.Debug.Write(arr[cnt + 3] + " ");
                    cnt += 4;

                }
            }
            //reverse
            for (int i = 7; i >= 0; i--)
            {
                System.Diagnostics.Debug.Write(arr[i] + " ");
            }
        }

        private void hideData(object sender, RoutedEventArgs e)
        {
            byte[] data = Encoding.ASCII.GetBytes(text.Text);
            
            BitArray bit = new BitArray(data);
            hide(bit);
            get();
        }
    }
}
