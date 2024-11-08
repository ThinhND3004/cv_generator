using CV_Generator;
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

namespace CV_Generater
{
    /// <summary>
    /// Interaction logic for LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        private AccountService _service = new(); 
        public LoginWindow()
        {
            InitializeComponent();
        }

        private void CloseClick(object sender, MouseButtonEventArgs e)
        {
            this.Close();
        }

        private void MinimizeClick(object sender, MouseButtonEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }


        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            string email = EmailTextBox.Text.Trim();
            string password = PasswordTextBox.Text;
            Account loginAcc = null;

            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password)) 
            {
                MessageBox.Show("Email or Password is Requires!", "Login Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (!string.IsNullOrWhiteSpace(email) && !string.IsNullOrWhiteSpace(password))
            {
                loginAcc = _service.GetAccountByEmail(email);
                if (loginAcc == null)
                {
                    MessageBox.Show("Account not found!", "Login Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                if (!_service.CheckLogin(loginAcc, password))
                {
                    MessageBox.Show("Password is incorrect!", "Login Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }   
            }

            HomeWindow home = new();
            home.LoginAcc = loginAcc;
            this.Hide();
            home.ShowDialog();
            this.Show();
            this.Activate();
        }
    }
}
