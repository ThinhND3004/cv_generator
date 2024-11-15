using CV_Generater;
using CV_Generator.BLL.Services;
using CV_Generator.DAL.Entities;
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
using Template;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Window;

namespace CV_Generator
{
    /// <summary>
    /// Interaction logic for HomeWindow.xaml
    /// </summary>
    public partial class HomeWindow : Window
    {
        public Account LoginAcc { get; set; }
        private CurViService _cvService = new();
        public HomeWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Log Out?", "Logout Warning", MessageBoxButton.YesNo, MessageBoxImage.Warning);

            if (result == MessageBoxResult.No)
            {
                // Code to execute if user clicks "Yes"
                return;
            }

            this.Close();

        }

        private void AddCVButton_Click(object sender, RoutedEventArgs e)
        {
            if (LoginAcc == null) 
            {
                MessageBox.Show("Login before create CV!", "Create CV Error!", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            TemplateSelection templateSelect = new TemplateSelection();
            bool? result = templateSelect.ShowDialog();
            if (result == true)
            {
                if (templateSelect.SelectedTemplate == 1)
                {
                    CVTemplate1 cVTemplate1 = new CVTemplate1();
                    cVTemplate1.UserCreateCV = LoginAcc;
                    cVTemplate1.ShowDialog();
                }
                else if (templateSelect.SelectedTemplate == 2)
                {
                    Template1Window Template1Window = new Template1Window();
                    Template1Window.ShowDialog();
                }
            }
            //CVTemplate1 cVTemplate1 = new CVTemplate1();
            //cVTemplate1.UserCreateCV = LoginAcc;
            //cVTemplate1.ShowDialog();

        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (LoginAcc.Role == 1)
            {
                AddCVButton.IsEnabled = false;
            }    


            HelloAccountLabel.Content = $"Hello, {LoginAcc.FullName}";
            CVDataGrid.ItemsSource = _cvService.GetCVsByLoginUserId(LoginAcc.Id);

            int numberOfCreateCv = _cvService.GetCVsByLoginUserId(LoginAcc.Id).Count;

            if (numberOfCreateCv > 0)
            {
                string cvText = numberOfCreateCv > 1 ? "CVs" : "CV";
                CountCreatedCVLabel.Content = $"You have created {numberOfCreateCv} {cvText}!!";
            }


        }
    }
}
