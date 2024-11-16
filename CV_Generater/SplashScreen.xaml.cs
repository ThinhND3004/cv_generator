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
using System.Windows.Threading;

namespace CV_Generator
{
    /// <summary>
    /// Interaction logic for SplashScreen.xaml
    /// </summary>
    public partial class SplashScreen : Window
    {
        private DispatcherTimer _timer;
        public SplashScreen()
        {
            InitializeComponent();
            // Khởi tạo Timer
            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromSeconds(10); // Đặt thời gian là 3 giây
            _timer.Tick += Timer_Tick; // Sự kiện khi hết thời gian
            _timer.Start();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            // Dừng timer và đóng cửa sổ
            _timer.Stop();
            this.Close();
        }
    }
}
