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
    /// Interaction logic for AnOS.xaml
    /// </summary>
    /// 

    public partial class AnOS : UserControl
    {
        public AnOS()
        {
            InitializeComponent();
        }
        public AnOS(string identifier)
        {
            InitializeComponent();
            Name_Label = identifier as string;// identifier;
        }
        public string all_Properties
        {
            get; set;
        }

        public string path_string
        {
            get; set;
        }
        public string inherit_string
        {
            get; set;
        }
        public string osdevice_string
        {
            get; set;
        }
        public string systemroot_string
        {
            get; set;
        }
        public string Header_string
        {
            get; set;
        }
        public string Description_string
        {
            get; set;
        }
        public string Name_Label
        {
            get
            {
                return Name.Content as string;
            }
            set
            {
                Name.Content = value;
            }
        }
        public string Partition_String
        {
            get
            {
                return Partition.Content as string;
            }
            set
            {
                Partition.Content = value;
            }
        }
        public string Identifier_String
        {
            get
            {
                return Identifier.Content as string;
            }
            set
            {
                Identifier.Content = value;
            }
        }
        public string Local_String
        {
            get
            {
                return Local.Content as string;
            }
            set
            {
                Local.Content = value;
            }
        }
        public string Partitionvalve_String
        {
            get
            {
                return Partitionvalve.Content as string;
            }
            set
            {
                Partitionvalve.Content = value;
            }
        }
        public string Identifiervalve_String
        {
            get
            {
                return Identifiervalve.Content as string;
            }
            set
            {
                Identifiervalve.Content = value;
            }
        }
        public string Localvalve_String
        {
            get
            {
                return Localvalve.Content as string;
            }
            set
            {
                Localvalve.Content = value;
            }
        }

        private void Grid_MouseEnter(object sender, MouseEventArgs e)
        {
            back.Background = new SolidColorBrush(Color.FromArgb(255, 237, 240, 244));
        }

        private void Grid_MouseLeave(object sender, MouseEventArgs e)
        {
            back.Background = new SolidColorBrush(Color.FromArgb(255, 246, 247, 249));
        }

    }
}
