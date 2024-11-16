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
using System.Windows.Media.Animation;
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


        private async void LoginButton_Click(object sender, RoutedEventArgs e)
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


            // Hiển thị vòng xoay
            Spinner.Visibility = Visibility.Visible;

            // Tạo hoạt ảnh xoay
            var rotationAnimation = new DoubleAnimation(0, 360, new Duration(TimeSpan.FromSeconds(1)))
            {
                RepeatBehavior = RepeatBehavior.Forever // Lặp vô hạn
            };

            // Gắn hoạt ảnh với SpinnerTransform
            SpinnerTransform.BeginAnimation(RotateTransform.AngleProperty, rotationAnimation);

            // Giả lập xử lý đăng nhập
            await Task.Delay(3000); // Thay bằng logic xử lý thực tế.

            // Dừng hoạt ảnh và ẩn vòng xoay
            SpinnerTransform.BeginAnimation(RotateTransform.AngleProperty, null);
            Spinner.Visibility = Visibility.Collapsed;








            HomeWindow home = new();
            home.LoginAcc = loginAcc;
            this.Hide();
            home.ShowDialog();
            this.Show();
            this.Activate();
        }

        
    }
}
