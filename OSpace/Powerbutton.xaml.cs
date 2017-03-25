using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.IO;
using System.Threading;

namespace OSpace
{
    /// <summary>
    /// Interaction logic for Power.xaml
    /// </summary>
    public partial class Powerbutton : Window
    {
        public Powerbutton()
        {
            InitializeComponent();
        }
        static string LangXMLPaht;
        public void setpoint(Point apoint, string langpath, double heeight)
        {
            WindowStartupLocation = WindowStartupLocation.Manual;
            Left = apoint.Y + 528;
            Top = heeight + apoint.X - 125;
            LangXMLPaht = langpath;
            loadlanguagepace();
        }
        private void loadlanguagepace()
        {
            StreamReader LangReader = null;
            try
            {
                LangReader = new StreamReader(LangXMLPaht);
                Language Lang = Xml.XmlHelper.DeSerialize<Language>(LangReader);
                Dispatcher.Invoke(new Action(() =>
                {

                }));
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            LangReader.Close();
        }
        void textboxclean()
        {
            Dispatcher.Invoke(new Action(() =>
            {
                Thread.Sleep(10);
                textBox.Clear();
            }));
        }
        private void textBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                Convert.ToDecimal(textBox.Text);
            }
            catch
            {
                Thread clean = new Thread(textboxclean);
                clean.Start();
            }
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            if (textBox.Text == "")
            {
                return;
            }
            try
            {
                CoreEdit.direct_command("shutdown /r /t " + Convert.ToInt32( Convert.ToDouble(textBox.Text) * 60));
                textBox.Visibility = Visibility.Hidden;
                label_Copy.Visibility = Visibility.Hidden;
                button1.Visibility = Visibility.Visible;
            }
            catch
            {
                CoreEdit.direct_command("shutdown /a");
            }
        }

        private void button_Copy_Click(object sender, RoutedEventArgs e)
        {
            if (textBox.Text == "")
            {
                return;
            }
            try
            {
                CoreEdit.direct_command("shutdown /s /t " + Convert.ToInt32(Convert.ToDouble(textBox.Text) * 60));
                textBox.Visibility = Visibility.Hidden;
                label_Copy.Visibility = Visibility.Hidden;
                button1.Visibility = Visibility.Visible;
            }
            catch
            {
                CoreEdit.direct_command("shutdown /a");
            }

        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            CoreEdit.direct_command("shutdown /a");
            textBox.Visibility = Visibility.Visible;
            label_Copy.Visibility = Visibility.Visible;
            button1.Visibility = Visibility.Hidden;
        }

        private void Window_Deactivated(object sender, EventArgs e)
        {
            Hide();
        }
    }
}
