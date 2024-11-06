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
            document.Info.Title = "Generated CV";


            PdfPage page = document.AddPage();
            XGraphics gfx = XGraphics.FromPdfPage(page);


            XFont titleFont = new XFont("Verdana", 20, XFontStyle.Bold);
            XFont headerFont = new XFont("Verdana", 14, XFontStyle.Bold);
            XFont bodyFont = new XFont("Verdana", 12, XFontStyle.Regular);


            gfx.DrawString("CV", titleFont, XBrushes.Black, new XRect(0, 0, page.Width, 0), XStringFormats.TopCenter);

            // Personal Information
            gfx.DrawString("Personal Information", headerFont, XBrushes.Black, new XRect(40, 80, page.Width, 0), XStringFormats.TopLeft);
            gfx.DrawString($"Name: {PersonalInfoLabel.Text}", bodyFont, XBrushes.Black, new XRect(40, 110, page.Width, 0), XStringFormats.TopLeft);
            gfx.DrawString($"Position: {PositionLabel.Text}", bodyFont, XBrushes.Black, new XRect(40, 130, page.Width, 0), XStringFormats.TopLeft);
            gfx.DrawString($"Email: {EmailLabel.Text}", bodyFont, XBrushes.Black, new XRect(40, 150, page.Width, 0), XStringFormats.TopLeft);
            gfx.DrawString($"Phone: {PhoneLabel.Text}", bodyFont, XBrushes.Black, new XRect(40, 170, page.Width, 0), XStringFormats.TopLeft);
            gfx.DrawString($"Address: {AddressLabel.Text}", bodyFont, XBrushes.Black, new XRect(40, 190, page.Width, 0), XStringFormats.TopLeft);

            // Professional Experience
            gfx.DrawString("Professional Experience", headerFont, XBrushes.Black, new XRect(40, 230, page.Width, 0), XStringFormats.TopLeft);
            gfx.DrawString($"Company: {CompanyNameLabel.Text}", bodyFont, XBrushes.Black, new XRect(40, 260, page.Width, 0), XStringFormats.TopLeft);
            gfx.DrawString($"Position: {PositionLabel.Text}", bodyFont, XBrushes.Black, new XRect(40, 280, page.Width, 0), XStringFormats.TopLeft);
            gfx.DrawString($"Years: {YearsLabel.Text}", bodyFont, XBrushes.Black, new XRect(40, 300, page.Width, 0), XStringFormats.TopLeft);
            gfx.DrawString($"Description: {DescriptionLabel.Text}", bodyFont, XBrushes.Black, new XRect(40, 320, page.Width, 0), XStringFormats.TopLeft);

            // Education
            gfx.DrawString("Education", headerFont, XBrushes.Black, new XRect(40, 360, page.Width, 0), XStringFormats.TopLeft);
            gfx.DrawString($"Degree: {DegreeLabel.Text}", bodyFont, XBrushes.Black, new XRect(40, 390, page.Width, 0), XStringFormats.TopLeft);
            gfx.DrawString($"Years: {DegreeYearsLabel.Text}", bodyFont, XBrushes.Black, new XRect(40, 410, page.Width, 0), XStringFormats.TopLeft);


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
