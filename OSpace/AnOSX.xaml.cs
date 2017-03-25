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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace OSpace
{
    /// <summary>
    /// Interaction logic for AnOSX.xaml
    /// </summary>
    public partial class AnOSX : UserControl
    {
        MainWindow main;
        //被主类告知当前被选中编号和我的编号
        public static int Bootmenuselectedindex___
        {
            get;set;
        }
        public void testcolor(int currentselindex)
        {
            if (currentselindex != meindex)
            {
                back.Background = new SolidColorBrush(Color.FromArgb(255, 246, 247, 249));
            }
        }
        public int meindex
        {
            get; set;
        }
        public AnOSX(MainWindow m)
        {
            InitializeComponent();
            main = m;
        }
        public string name
        {
            get
            {
                return Name.Content as string;
            }
            set
            {
                this.Name.Content = value as string;
            }
        }
        public string identifierlabel
        {
            get
            {
                return Identifier.Content as string;
            }
            set
            {
                this.Identifier.Content = value as string;
            }
        }
        public string identi
        {
            get
            {
                return Identifiervalve.Content as string;
            }
            set
            {
                this.Identifiervalve.Content = value as string;

            }
        }


        private void back_MouseEnter(object sender, MouseEventArgs e)
        {
            if (Bootmenuselectedindex___ != meindex)//如果它是被选中的那个
                back.Background = new SolidColorBrush(Color.FromArgb(255, 237, 240, 244));
        }

        private void back_MouseLeave(object sender, MouseEventArgs e)
        {
            if (Bootmenuselectedindex___ == meindex)//如果它是被选中的那个
            {
                back.Background = new SolidColorBrush(Color.FromArgb(255, 211, 225, 239));
            }
            else
            {
                back.Background = new SolidColorBrush(Color.FromArgb(255, 246, 247, 249));
            }
        }

        private void back_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            MainWindow.Bootmenuselectedindex = meindex;
            AnOSX.Bootmenuselectedindex___ = meindex;
            foreach (AnOSX a in main.Bootmenu.Children)
            {
                a.testcolor(meindex);
            }
        }
    }
}
