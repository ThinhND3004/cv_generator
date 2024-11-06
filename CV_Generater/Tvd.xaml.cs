using Microsoft.Win32;
using PdfSharpCore.Drawing;
using PdfSharpCore.Pdf;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace CV_Generater
{
    public partial class Tvd : Window
    {
        public Tvd()
        {
            InitializeComponent();
        }

        private void UploadPhoto_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Image Files (*.jpg; *.jpeg; *.png; *.bmp)|*.jpg;*.jpeg;*.png;*.bmp"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                BitmapImage bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.UriSource = new Uri(openFileDialog.FileName);
                bitmap.EndInit();

                ProfileImage.Source = bitmap;
            }
        }

        private void PersonalInfoLabel_MouseDown(object sender, MouseButtonEventArgs e)
        {
            PersonalInfoLabel.Visibility = Visibility.Collapsed;
            PersonalInfoTextBox.Text = PersonalInfoLabel.Text;
            PersonalInfoTextBox.Visibility = Visibility.Visible;
            PersonalInfoTextBox.Focus();
        }

        private void PersonalInfoTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            PersonalInfoLabel.Text = PersonalInfoTextBox.Text;
            PersonalInfoTextBox.Visibility = Visibility.Collapsed;
            PersonalInfoLabel.Visibility = Visibility.Visible;
        }

        private void PositionLabel_MouseDown(object sender, MouseButtonEventArgs e)
        {
            PositionLabel.Visibility = Visibility.Collapsed;
            PositionTextBox.Text = PositionLabel.Text;
            PositionTextBox.Visibility = Visibility.Visible;
            PositionTextBox.Focus();
        }

        private void PositionTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            PositionLabel.Text = PositionTextBox.Text;
            PositionTextBox.Visibility = Visibility.Collapsed;
            PositionLabel.Visibility = Visibility.Visible;
        }

        private void EmailLabel_MouseDown(object sender, MouseButtonEventArgs e)
        {
            EmailLabel.Visibility = Visibility.Collapsed;
            EmailTextBox.Text = EmailLabel.Text;
            EmailTextBox.Visibility = Visibility.Visible;
            EmailTextBox.Focus();
        }

        private void EmailTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            EmailLabel.Text = EmailTextBox.Text;
            EmailTextBox.Visibility = Visibility.Collapsed;
            EmailLabel.Visibility = Visibility.Visible;
        }

        private void PhoneLabel_MouseDown(object sender, MouseButtonEventArgs e)
        {
            PhoneLabel.Visibility = Visibility.Collapsed;
            PhoneTextBox.Text = PhoneLabel.Text;
            PhoneTextBox.Visibility = Visibility.Visible;
            PhoneTextBox.Focus();
        }

        private void PhoneTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            PhoneLabel.Text = PhoneTextBox.Text;
            PhoneTextBox.Visibility = Visibility.Collapsed;
            PhoneLabel.Visibility = Visibility.Visible;
        }

        private void AddressLabel_MouseDown(object sender, MouseButtonEventArgs e)
        {
            AddressLabel.Visibility = Visibility.Collapsed;
            AddressTextBox.Text = AddressLabel.Text;
            AddressTextBox.Visibility = Visibility.Visible;
            AddressTextBox.Focus();
        }

        private void AddressTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            AddressLabel.Text = AddressTextBox.Text;
            AddressTextBox.Visibility = Visibility.Collapsed;
            AddressLabel.Visibility = Visibility.Visible;
        }

        private void CompanyNameLabel_MouseDown(object sender, MouseButtonEventArgs e)
        {
            CompanyNameLabel.Visibility = Visibility.Collapsed;
            CompanyNameTextBox.Text = CompanyNameLabel.Text;
            CompanyNameTextBox.Visibility = Visibility.Visible;
            CompanyNameTextBox.Focus();
        }

        private void CompanyNameTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            CompanyNameLabel.Text = CompanyNameTextBox.Text;
            CompanyNameTextBox.Visibility = Visibility.Collapsed;
            CompanyNameLabel.Visibility = Visibility.Visible;
        }



        private void YearsLabel_MouseDown(object sender, MouseButtonEventArgs e)
        {
            YearsLabel.Visibility = Visibility.Collapsed;
            YearsTextBox.Text = YearsLabel.Text;
            YearsTextBox.Visibility = Visibility.Visible;
            YearsTextBox.Focus();
        }

        private void YearsTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            YearsLabel.Text = YearsTextBox.Text;
            YearsTextBox.Visibility = Visibility.Collapsed;
            YearsLabel.Visibility = Visibility.Visible;
        }


        private void DescriptionLabel_MouseDown(object sender, MouseButtonEventArgs e)
        {
            DescriptionLabel.Visibility = Visibility.Collapsed;
            DescriptionTextBox.Text = DescriptionLabel.Text;
            DescriptionTextBox.Visibility = Visibility.Visible;
            DescriptionTextBox.Focus();
        }

        private void DescriptionTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            DescriptionLabel.Text = DescriptionTextBox.Text;
            DescriptionTextBox.Visibility = Visibility.Collapsed;
            DescriptionLabel.Visibility = Visibility.Visible;
        }


        private void DegreeLabel_MouseDown(object sender, MouseButtonEventArgs e)
        {
            DegreeLabel.Visibility = Visibility.Collapsed;
            DegreeTextBox.Text = DegreeLabel.Text;
            DegreeTextBox.Visibility = Visibility.Visible;
            DegreeTextBox.Focus();
        }

        private void DegreeTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            DegreeLabel.Text = DegreeTextBox.Text;
            DegreeTextBox.Visibility = Visibility.Collapsed;
            DegreeLabel.Visibility = Visibility.Visible;
        }


        private void DegreeYearsLabel_MouseDown(object sender, MouseButtonEventArgs e)
        {
            DegreeYearsLabel.Visibility = Visibility.Collapsed;
            DegreeYearsTextBox.Text = DegreeYearsLabel.Text;
            DegreeYearsTextBox.Visibility = Visibility.Visible;
            DegreeYearsTextBox.Focus();
        }

        private void DegreeYearsTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            DegreeYearsLabel.Text = DegreeYearsTextBox.Text;
            DegreeYearsTextBox.Visibility = Visibility.Collapsed;
            DegreeYearsLabel.Visibility = Visibility.Visible;
        }

        private void GenerateToPdf_Click(object sender, RoutedEventArgs e)
        {


            PdfDocument document = new PdfDocument();
            document.Info.Title = "CV Document";
            PdfPage page = document.AddPage();
            XGraphics gfx = XGraphics.FromPdfPage(page);
            XFont titleFont = new XFont("Arial", 20, XFontStyle.Bold);
            XFont subtitleFont = new XFont("Arial", 16, XFontStyle.Bold);
            XFont normalFont = new XFont("Arial", 12, XFontStyle.Regular);
            XBrush brush = XBrushes.White;


            

            // Set background color
            gfx.DrawRectangle(new XSolidBrush(XColor.FromArgb(0x2D, 0x3E, 0x50)), 0, 0, page.Width / 3, page.Height);
            int x = 200;
            int y = 20;
            if (ProfileImage.Source is BitmapImage bitmapImage)
            {
                MemoryStream ms = new MemoryStream();
                BitmapEncoder encoder = new PngBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(bitmapImage));
                encoder.Save(ms);
                ms.Position = 0;

                // Load the image to PdfSharp XImage
                XImage xImage = XImage.FromStream(() => ms);

                // Draw Profile Image in the PDF
                gfx.DrawImage(xImage, 25, 40, 150, 150); // Position and size (x, y, width, height)
            }
            else
            {
                MessageBox.Show("KHONG CO HINH ANH", "LOI ROI THANG NGU", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            // Profile Section
            gfx.DrawString("Personal Details", titleFont, brush, new XPoint(y, 30 + x));
            gfx.DrawString(PersonalInfoLabel.Text, subtitleFont, brush, new XPoint(y, 70 + x));
            gfx.DrawString(PositionLabel.Text, subtitleFont, brush, new XPoint(y, 100 + x));
            gfx.DrawString(EmailLabel.Text, normalFont, brush, new XPoint(y, 130 + x));
            gfx.DrawString(PhoneLabel.Text, normalFont, brush, new XPoint(y, 160 + x));
            gfx.DrawString(AddressLabel.Text, normalFont, brush, new XPoint(y, 190 + x));

            // Experience Section
            gfx.DrawString("Professional Experience", titleFont, XBrushes.Black, new XPoint(200, 40));
            gfx.DrawString(CompanyNameLabel.Text, subtitleFont, XBrushes.Black, new XPoint(200, 70));
            gfx.DrawString(YearsLabel.Text, normalFont, XBrushes.Black, new XPoint(200, 100));
            gfx.DrawString(DescriptionLabel.Text, normalFont, XBrushes.Black, new XPoint(200, 130));

            // Education Section
            gfx.DrawString("Education", titleFont, XBrushes.Black, new XPoint(200, 180));
            gfx.DrawString(DegreeLabel.Text, subtitleFont, XBrushes.Black, new XPoint(200, 210));
            gfx.DrawString(DegreeYearsLabel.Text, normalFont, XBrushes.Black, new XPoint(200, 240));



            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                Filter = "PDF file (*.pdf)|*.pdf",
                FileName = "Generated_CV.pdf"
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                string filename = saveFileDialog.FileName;
                using (FileStream stream = new FileStream(filename, FileMode.Create, FileAccess.Write))
                {
                    document.Save(stream);
                }
            }


            document.Close();
            MessageBox.Show("PDF generated successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}
