using Microsoft.Win32;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
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
            if (end < 0)
                end = source.Length + end;

            int len = end - start;

            T[] res = new T[len];
            for (int i = 0; i < len; i++)
                res[i] = source[i + start];

            return res;
        }
    }

    public partial class MainWindow : Window
    {
        private bool rectangle = false;
        private System.Drawing.Bitmap work;
        public MainWindow()
        {
            InitializeComponent();
        }

        private bool check(string text)
        {
            if (checkText(text) && (text.Length*8 < (work.Height * work.Width)))
            {
                return true;
            }
            throw new Exception("Za długi tekst.");
        }

        void checkRectangle(object sender, RoutedEventArgs e)
        {
            rectangle = true;
            border.BorderBrush = new SolidColorBrush(Colors.Blue);
        }

        private void save(object sender, RoutedEventArgs e)
        {
            try
            {
                SaveFileDialog dialog = new SaveFileDialog();
                dialog.Title = "Save a picture";
                dialog.Filter = "Portable Network Graphic (*.png)|*.png|" + "Bmp files (*.bmp)|*.bmp";

                if (dialog.ShowDialog() == true)
                    work.Save(dialog.FileName);
            }catch(Exception ex)
            {
                MessageBox.Show("Orginal message:" + ex.Message, "Błąd");
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
                    MessageBox.Show("Orginal message:" + ex.Message, "Błąd");
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
                    MessageBox.Show("Orginal message:" + ex.Message, "Błąd");
                }
            }
        }

        private bool checkText(string text)
        {
            string s = Regex.Replace(text, @"[^a-zA-Z\p{P}\d\s]", "");
            if (s == text)
                return true;
            throw new Exception("Niedozwolone symbole.");
        }

        private void hide_horizontal(BitArray message)
        {
            int cnt = 0;
            for (int i = 0; i < work.Height && cnt < message.Length; ++i)
            {
                for (int j = 0; j < work.Width && cnt < message.Length; ++j)
                {
                    System.Drawing.Color tmp = work.GetPixel(j, i);
                    int[] val = new int[] { 0, 0, 0, 0 };

                    val[0] = tmp.A - tmp.A % 2 + (message[cnt++] ? 1 : 0);
                    val[1] = tmp.R - tmp.R % 2 + (message[cnt++] ? 1 : 0);
                    val[2] = tmp.G - tmp.G % 2 + (message[cnt++] ? 1 : 0);
                    val[3] = tmp.B - tmp.B % 2 + (message[cnt++] ? 1 : 0);
                    work.SetPixel(j, i, System.Drawing.Color.FromArgb(val[0], val[1], val[2], val[3]));
                }
            }
        }

        private void hide_vertical(BitArray message)
        {
            int cnt = 0;
            for (int i = 0; i < work.Width && cnt < message.Length; ++i)
            {
                for (int j = 0; j < work.Height && cnt < message.Length; ++j)
                {
                    System.Drawing.Color tmp = work.GetPixel(i, j);
                    int[] val = new int[] { 0, 0, 0, 0 };
                    val[0] = tmp.A - tmp.A % 2 + (message[cnt++] ? 1 : 0);
                    val[1] = tmp.R - tmp.R % 2 + (message[cnt++] ? 1 : 0);
                    val[2] = tmp.G - tmp.G % 2 + (message[cnt++] ? 1 : 0);
                    val[3] = tmp.B - tmp.B % 2 + (message[cnt++] ? 1 : 0);
                    work.SetPixel(i, j, System.Drawing.Color.FromArgb(val[0], val[1], val[2], val[3]));
                }
            }
        }

        private byte ConvertBoolArrayToByte(bool[] source)
        {
            byte result = 0;
            int index = 8 - source.Length;
            foreach (bool b in source)
            {
                if (b)
                    result |= (byte)(1 << (7 - index));

                index++;
            }

            return result;
        }

        private bool[] reverse(bool[] arr)
        {
            for (int i = 0; i < arr.Length / 2; i++)
            {
                bool tmp = arr[i];
                arr[i] = arr[arr.Length - i - 1];
                arr[arr.Length - i - 1] = tmp;
            }

            return arr;
        }

        private char[] reverse(char[] arr)
        {
            for (int i = 0; i < arr.Length / 2; i++)
            {
                char tmp = arr[i];
                arr[i] = arr[arr.Length - i - 1];
                arr[arr.Length - i - 1] = tmp;
            }

            return arr;
        }

        private byte[] convertBoolArrToByteArr(bool[] arr)
        {
            arr = reverse(arr);
            byte[] toReturn = new byte[arr.Length / 8];
            int cnt = 0;
            for(int i = 0; i < arr.Length; i+=8)
            {
                toReturn[cnt++] = ConvertBoolArrayToByte(arr.Slice(i, i+8));
            }
            return toReturn;
        }

        private byte[] get_horizontal()
        {
            List<bool> l = new List<bool>();
            bool[] arr = new bool[4];
            for (int i = 0; i < work.Height; ++i)
            {
                for (int j = 0; j < work.Width; ++j)
                {
                    System.Drawing.Color tmp = work.GetPixel(j, i);
                    arr[0] = tmp.A % 2 == 1;
                    arr[1] = tmp.R % 2 == 1;
                    arr[2] = tmp.G % 2 == 1;
                    arr[3] = tmp.B % 2 == 1;

                    l.Add(arr[0]);
                    l.Add(arr[1]);
                    l.Add(arr[2]);
                    l.Add(arr[3]);
                    if(l.Count > 8)
                    {
                        if (
                        l[l.Count - 1] == false &&
                        l[l.Count - 2] == true &&
                        l[l.Count - 3] == true &&
                        l[l.Count - 4] == true &&

                        l[l.Count - 5] == true &&
                        l[l.Count - 6] == true &&
                        l[l.Count - 7] == true &&
                        l[l.Count - 8] == false)
                        {
                            bool[] toReturn = l.ToArray();
                            return convertBoolArrToByteArr(toReturn.Slice(0, toReturn.Length - 8));
                        }
                    }
                }
            }
            return new byte[] { 1 };
        }

        private byte[] get_vertical()
        {
            List<bool> l = new List<bool>();
            bool[] arr = new bool[4];
            for (int i = 0; i < work.Width; ++i)
            {
                for (int j = 0; j < work.Height; ++j)
                {
                    System.Drawing.Color tmp = work.GetPixel(i, j);
                    arr[0] = tmp.A % 2 == 1;
                    arr[1] = tmp.R % 2 == 1;
                    arr[2] = tmp.G % 2 == 1;
                    arr[3] = tmp.B % 2 == 1;

                    l.Add(arr[0]);
                    l.Add(arr[1]);
                    l.Add(arr[2]);
                    l.Add(arr[3]);
                   if (l.Count > 8)
                    {
                        if (
                        l[l.Count - 1] == false &&
                        l[l.Count - 2] == true &&
                        l[l.Count - 3] == true &&
                        l[l.Count - 4] == true &&

                        l[l.Count - 5] == true &&
                        l[l.Count - 6] == true &&
                        l[l.Count - 7] == true &&
                        l[l.Count - 8] == false)
                        {
                            bool[] toReturn = l.ToArray();
                            return convertBoolArrToByteArr(toReturn.Slice(0, toReturn.Length - 8));
                        }
                    }
                }
            }
            return new byte[] { 1 };
        }

        private void hideData(object sender, RoutedEventArgs e)
        {
            try
            {
                check(text.Text);      
                byte[] data = Encoding.ASCII.GetBytes(text.Text + "~");
                BitArray bit = new BitArray(data);

                if (horizontal.IsChecked == true)
                    hide_horizontal(bit);
                else if (vertical.IsChecked == true)
                    hide_vertical(bit);
            }catch(Exception ex)
            {
                MessageBox.Show(ex.Message, "Błąd");
            }
        }

        private void getData(object sender, RoutedEventArgs e)
        {
            if (horizontal.IsChecked == true)
            {
                var da = get_horizontal();
                da.Reverse();
                string decoded = Encoding.ASCII.GetString(da);
                char[] arr = decoded.ToCharArray();
                arr = reverse(arr);
                text.Text = new string(arr);
            }
            else if (vertical.IsChecked == true)
            {
                var da = get_vertical();
                string decoded = Encoding.ASCII.GetString(da);
                char[] arr = decoded.ToCharArray();
                arr = reverse(arr);
                text.Text = new string(arr);
            }
        }

        private void about(object sender, RoutedEventArgs e)
        {
            try
            {
                System.Diagnostics.Process.Start(@"..\..\about.html");
            }catch(Exception ex)
            {
                MessageBox.Show("Orginal message:" + ex.Message, "Błąd");
            }
            
        }
    }
}
