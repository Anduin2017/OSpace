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
    /// Interaction logic for AnFW.xaml
    /// </summary>
    public partial class AnFW : UserControl
    {
        public AnFW()
        {
            InitializeComponent();
        }
        public string fwidenvalue
        {
            get
            {
                return this.Identifiervalve.Content as string;
            }
            set
            {
                this.Identifiervalve.Content = value as string;
            }
        }
        public string fwnamevalue
        {
            get
            {
                return Name.Content as string;
            }
            set
            {
                Name.Content = value as string;
            }
        }
        public string fwproterites
        {
            get; set;
        }
    }
}
