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
    /// Interaction logic for Login.xaml
    /// </summary>
    public partial class Login : Window
    {
        MainWindow me;
        public bool readytodo = true;
        static string LangXMLPaht = null;
        public bool draggable = false;
        public Login()
        {
            InitializeComponent();
        }
        public void setpoint(Point apoint, MainWindow main, string langpath)
        {
            WindowStartupLocation = WindowStartupLocation.Manual;
            Left = apoint.Y + 580;
            Top = apoint.X + 74;
            me = main;
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
                    Sign_in.Text = Xml.DESHelper.Decrypt(Lang.Sign_in);
                    Lab_Account.Content = Xml.DESHelper.Decrypt(Lang.Account);
                    Lab_Password.Content = Xml.DESHelper.Decrypt(Lang.Password);
                    But_Sign_in.Content = Xml.DESHelper.Decrypt(Lang.But_Sing_in);
                    Label_sign_up.Content = Xml.DESHelper.Decrypt(Lang.register);
                    Label_sign_up_Copy.Content = Xml.DESHelper.Decrypt(Lang.Privacy_statement);
                }));
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            LangReader.Close();
        }

        private void Window_Deactivated(object sender, EventArgs e)
        {
            if (readytodo)
            {
                readytodo = false;
                Hide();
                me.But_Signin.IsChecked = true;
            }
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            readytodo = false;
            me.But_Signin.Content = "UserName";
            me.Caution("Successfully signed in");
            me.But_Signin.IsChecked = true;
            Hide();
            // Close();
        }

        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed &&draggable)
                DragMove();
        }
    }
}
