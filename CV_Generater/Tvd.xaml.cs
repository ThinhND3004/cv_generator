using Microsoft.Win32;
using PdfSharpCore.Drawing;
using PdfSharpCore.Pdf;
using System.IO;
using System.Reflection.Metadata;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using static System.Net.Mime.MediaTypeNames;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using Document = DocumentFormat.OpenXml.Wordprocessing.Document;
using Paragraph = DocumentFormat.OpenXml.Wordprocessing.Paragraph;
using Run = DocumentFormat.OpenXml.Wordprocessing.Run;
using Text = DocumentFormat.OpenXml.Wordprocessing.Text;
using Bold = DocumentFormat.OpenXml.Wordprocessing.Bold;

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

        private void GenerateToDocs_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                Filter = "Word Document (*.docx)|*.docx",
                FileName = "Generated_CV.docx"
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                string filePath = saveFileDialog.FileName;

                // Create a new Word document
                using (WordprocessingDocument wordDocument = WordprocessingDocument.Create(filePath, DocumentFormat.OpenXml.WordprocessingDocumentType.Document))
                {
                    // Add a main document part
                    MainDocumentPart mainPart = wordDocument.AddMainDocumentPart();
                    mainPart.Document = new Document();
                    Body body = new Body();

                    // Add Title
                    Paragraph titleParagraph = new Paragraph(new Run(new Text("CV")));
                    titleParagraph.ParagraphProperties = new ParagraphProperties(new Justification() { Val = JustificationValues.Center });
                    body.Append(titleParagraph);

                    // Personal Information Section
                    body.Append(CreateSectionHeader("Personal Information"));
                    body.Append(CreateInfoParagraph("Name:", PersonalInfoLabel.Text));
                    body.Append(CreateInfoParagraph("Position:", PositionLabel.Text));
                    body.Append(CreateInfoParagraph("Email:", EmailLabel.Text));
                    body.Append(CreateInfoParagraph("Phone:", PhoneLabel.Text));
                    body.Append(CreateInfoParagraph("Address:", AddressLabel.Text));

                    // Professional Experience Section
                    body.Append(CreateSectionHeader("Professional Experience"));
                    body.Append(CreateInfoParagraph("Company:", CompanyNameLabel.Text));
                    body.Append(CreateInfoParagraph("Position:", PositionLabel.Text));
                    body.Append(CreateInfoParagraph("Years:", YearsLabel.Text));
                    body.Append(CreateInfoParagraph("Description:", DescriptionLabel.Text));

                    // Education Section
                    body.Append(CreateSectionHeader("Education"));
                    body.Append(CreateInfoParagraph("Degree:", DegreeLabel.Text));
                    body.Append(CreateInfoParagraph("Years:", DegreeYearsLabel.Text));

                    // Save the document
                    mainPart.Document.Append(body);
                    mainPart.Document.Save();
                }

                MessageBox.Show("DOCX generated successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);

            }


        }
        // Helper method to create section headers
        private Paragraph CreateSectionHeader(string headerText)
        {
            Paragraph headerParagraph = new Paragraph(new Run(new Text(headerText)));
            headerParagraph.ParagraphProperties = new ParagraphProperties(new Justification() { Val = JustificationValues.Left });
            headerParagraph.ParagraphProperties.Append(new Bold());
            return headerParagraph;
        }

        // Helper method to create info paragraphs
        private Paragraph CreateInfoParagraph(string label, string info)
        {
            Run labelRun = new Run(new Text(label + " "));
            Run infoRun = new Run(new Text(info));
            Paragraph paragraph = new Paragraph();
            paragraph.Append(labelRun);
            paragraph.Append(infoRun);
            return paragraph;
        }
    }
}
