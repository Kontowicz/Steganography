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
    /// 
    public static class Extensions
    {
        /// <summary>
        /// Get the array slice between the two indexes.
        /// ... Inclusive for start index, exclusive for end index.
        /// </summary>
        public static T[] Slice<T>(this T[] source, int start, int end)
        {
            // Handles negative ends.
            if (end < 0)
            {
                end = source.Length + end;
            }
            int len = end - start;

            // Return new array.
            T[] res = new T[len];
            for (int i = 0; i < len; i++)
            {
                res[i] = source[i + start];
            }
            return res;
        }
    }
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
                    val[1] = tmp.R - tmp.R % 2 + (message[cnt] ? 1 : 0);
                    ++cnt;
                    val[2] = tmp.G - tmp.G % 2 + (message[cnt] ? 1 : 0);
                    ++cnt;
                    val[3] = tmp.B - tmp.B % 2 + (message[cnt] ? 1 : 0);
                    ++cnt;

                    work.SetPixel(j, i, System.Drawing.Color.FromArgb(val[0], val[1], val[2], val[3]));
                }
            }
        }
        private byte ConvertBoolArrayToByte(bool[] source)
        {
            byte result = 0;
            // This assumes the array never contains more than 8 elements!
            int index = 8 - source.Length;

            // Loop through the array
            foreach (bool b in source)
            {
                // if the element is 'true' set the bit at that position
                if (b)
                    result |= (byte)(1 << (7 - index));

                index++;
            }

            return result;
        }

        private byte[] convertBoolArrToByteArr(bool[] arr)
        {

            for (int i = 0; i < arr.Length / 2; i++)
            {
                bool tmp = arr[i];
                arr[i] = arr[arr.Length - i - 1];
                arr[arr.Length - i - 1] = tmp;
            }

            byte[] toReturn = new byte[arr.Length / 8];
            int cnt = 0;
            for(int i = 0; i < arr.Length; i+=8)
            {
                toReturn[cnt] = ConvertBoolArrayToByte(arr.Slice(i, i+8));
            }
            return toReturn;
        }


        private byte[] get()
        {
            bool[] stop = { false, true, true, true, true, false, false, false };
            int bre = 0;
            long cnt = 0;
            List<bool> l = new List<bool>();
            bool[] arr = new bool[4];
            for (int i = 0; i < work.Height; ++i)
            {
                for (int j = 0; j < work.Width; ++j)
                {
                    System.Drawing.Color tmp = work.GetPixel(j, i);
                    cnt = 0;
                    arr[cnt+0] = tmp.A % 2 == 1;
                    arr[cnt+1] = tmp.R % 2 == 1;
                    arr[cnt+2] = tmp.G % 2 == 1;
                    arr[cnt+3] = tmp.B % 2 == 1;

                    if(arr[0] == false &&
                        arr[1] == false &&
                        arr[2] == false &&
                        arr[3] == true)
                    {
                        if( i < work.Height)
                        {
                            tmp = work.GetPixel(j, i);
                            cnt = 0;
                            arr[cnt + 0] = tmp.A % 2 == 1;
                            arr[cnt + 1] = tmp.R % 2 == 1;
                            arr[cnt + 2] = tmp.G % 2 == 1;
                            arr[cnt + 3] = tmp.B % 2 == 1;

                            if(arr[0] == !true &&
                                arr[1] == !true &&
                                arr[2] == !true &&
                                arr[3] == !false)
                            {
                                bool[] toReturn = l.ToArray();
                                return convertBoolArrToByteArr(toReturn);
                            }
                        }
                    }
                    bre = 0;
                    l.Add(arr[0]);
                    l.Add(arr[1]);
                    l.Add(arr[2]);
                    l.Add(arr[3]);
                    cnt += 4;

                }
            }
            return new byte[] { 1 };
        }

        private void hideData(object sender, RoutedEventArgs e)
        {
            byte[] data = Encoding.ASCII.GetBytes(text.Text+"x");
            BitArray bit = new BitArray(data);
            hide(bit);
            var da = get();

        }
    }
}
