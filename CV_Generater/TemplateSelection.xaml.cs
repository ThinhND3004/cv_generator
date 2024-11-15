using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace CV_Generator
{
    /// <summary>
    /// Interaction logic for TemplateSelection.xaml
    /// </summary>
    public partial class TemplateSelection : Window
    {
        public int SelectedTemplate { get; private set; } = 0;
        public TemplateSelection()
        {
            InitializeComponent();
        }

        private void Template1_Click(object sender, RoutedEventArgs e)
        {
            SelectedTemplate = 1;
            DialogResult = true;
            this.Close();
        }

        private void Template2_Click(object sender, RoutedEventArgs e)
        {
            SelectedTemplate = 2;
            DialogResult = true;
            this.Close();
        }
    }
}
