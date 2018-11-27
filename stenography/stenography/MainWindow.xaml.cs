using Microsoft.Win32;
using System;
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
                var encoder = new JpegBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(test));
                encoder.QualityLevel = 100;

                using (var stream = new FileStream(dialog.FileName, FileMode.Create))
                {
                    encoder.Save(stream);
                }
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

        private void hideData(object sender, RoutedEventArgs e)
        {

            for (int i = 0; i < test.PixelHeight; ++i)
            {

                for (int j = 0; j < test.PixelWidth; ++j)
                {
                }
            }
        }
    }
}
