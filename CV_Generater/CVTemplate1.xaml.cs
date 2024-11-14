using CV_Generator.BLL.Services;
using CV_Generator.DAL.Entities;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.Win32;
using PdfSharpCore.Drawing;
using PdfSharpCore.Pdf;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Path = System.IO.Path;

namespace CV_Generater
{
    public partial class CVTemplate1 : Window
    {
        public Account UserCreateCV { get; set; }
        private readonly CurViService _cvService = new();
        private const double A4_WIDTH_MM = 210;
        private const double A4_HEIGHT_MM = 297;
        private const double MM_TO_POINT = 2.83465;

        public CVTemplate1()
        {
            InitializeComponent();
        }

        // Xử lý sự kiện click để upload ảnh
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

        // Xử lý sự kiện click để chỉnh sửa text
        private void TextBlock_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed && sender is TextBlock textBlock)
            {
                if (textBlock.Tag is TextBox textBox)
                {
                    textBlock.Visibility = Visibility.Collapsed;
                    textBox.Text = textBlock.Text;
                    textBox.Visibility = Visibility.Visible;
                    textBox.Focus();
                    e.Handled = true;
                }
            }
        }

        private void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (sender is TextBox textBox && textBox.Tag is TextBlock textBlock)
            {
                textBlock.Text = textBox.Text;
                textBox.Visibility = Visibility.Collapsed;
                textBlock.Visibility = Visibility.Visible;
            }
        }

        // Classes hỗ trợ cho PDF
        private class PDFFonts
        {
            public XFont Name { get; } = new XFont("Arial", 24, XFontStyle.Bold);
            public XFont SectionTitle { get; } = new XFont("Arial", 18, XFontStyle.Bold);
            public XFont SubTitle { get; } = new XFont("Arial", 14, XFontStyle.Bold);
            public XFont Normal { get; } = new XFont("Arial", 12, XFontStyle.Regular);
            public XFont Small { get; } = new XFont("Arial", 11, XFontStyle.Regular);
        }

        private class PDFLayout
        {
            public double PageWidth { get; }
            public double PageHeight { get; }
            public double LeftPanelWidth { get; }
            public double RightPanelX { get; }
            public double Margin { get; } = 20 * 2.83465; // 20mm margin
            public double SectionSpacing { get; } = 15 * 2.83465; // 15mm spacing

            public PDFLayout(double pageWidth, double pageHeight)
            {
                PageWidth = pageWidth;
                PageHeight = pageHeight;
                LeftPanelWidth = pageWidth / 3;
                RightPanelX = LeftPanelWidth + Margin;
            }
        }

        // Generate PDF methods
        private void GeneratePDFButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                PdfDocument document = new PdfDocument();
                document.Info.Title = "CV Document";

                var pageWidth = A4_WIDTH_MM * MM_TO_POINT;
                var pageHeight = A4_HEIGHT_MM * MM_TO_POINT;
                var fonts = new PDFFonts();
                var layout = new PDFLayout(pageWidth, pageHeight);

                // Khởi tạo trang đầu tiên
                PdfPage currentPage = document.AddPage();
                currentPage.Width = pageWidth;
                currentPage.Height = pageHeight;
                XGraphics gfx = XGraphics.FromPdfPage(currentPage);

                // Vẽ left panel (chỉ ở trang đầu)
                gfx.DrawRectangle(new XSolidBrush(XColor.FromArgb(0x2D, 0x3E, 0x50)),
                    0, 0, layout.LeftPanelWidth, pageHeight);
                DrawProfilePhoto(gfx, layout);
                DrawLeftPanel(gfx, fonts, layout);

                // Bắt đầu vẽ right panel
                double currentY = layout.Margin;
                bool isFirstPage = true;
                bool needNewPage = false;

                // Vẽ từng section và xử lý việc chuyển trang
                currentY = DrawSection(gfx, "Overview", OverViewTextBlock.Text, fonts, layout, currentY, ref needNewPage, isFirstPage);
                
                if (needNewPage)
                {
                    currentPage = CreateNewPage(document, pageWidth, pageHeight, ref gfx);
                    currentY = layout.Margin;
                    isFirstPage = false;
                    needNewPage = false;
                }

                currentY = DrawWorkExperience(gfx, fonts, layout, currentY, ref needNewPage, isFirstPage);
                
                if (needNewPage)
                {
                    currentPage = CreateNewPage(document, pageWidth, pageHeight, ref gfx);
                    currentY = layout.Margin;
                    isFirstPage = false;
                    needNewPage = false;
                }

                currentY = DrawEducation(gfx, fonts, layout, currentY, ref needNewPage, isFirstPage);

                if (needNewPage)
                {
                    currentPage = CreateNewPage(document, pageWidth, pageHeight, ref gfx);
                    currentY = layout.Margin;
                    isFirstPage = false;
                    needNewPage = false;
                }

                currentY = DrawSkills(gfx, fonts, layout, currentY, ref needNewPage, isFirstPage);

                SavePDFDocument(document);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error generating PDF: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private double DrawWorkExperience(XGraphics gfx, PDFFonts fonts, PDFLayout layout,
    double currentY, ref bool needNewPage, bool isFirstPage)
        {
            double x = isFirstPage ? layout.RightPanelX : layout.Margin;
            double contentWidth = isFirstPage ?
                layout.PageWidth - layout.RightPanelX - layout.Margin * 2 :
                layout.PageWidth - layout.Margin * 2;

            // Vẽ tiêu đề Work Experience
            if (currentY + fonts.SectionTitle.Height > layout.PageHeight - layout.Margin)
            {
                needNewPage = true;
                return currentY;
            }

            gfx.DrawString("Work Experience", fonts.SectionTitle, XBrushes.Black, new XPoint(x, currentY));
            currentY += fonts.SectionTitle.Height * 1.2;

            // Vẽ ngày tháng
            if (currentY + fonts.Normal.Height > layout.PageHeight - layout.Margin)
            {
                needNewPage = true;
                return currentY;
            }
            gfx.DrawString(WorkExpDateTextBlock.Text, fonts.Normal, XBrushes.DarkGreen, new XPoint(x, currentY));
            currentY += fonts.Normal.Height * 1.2;

            // Vẽ tên công ty
            if (currentY + fonts.SubTitle.Height > layout.PageHeight - layout.Margin)
            {
                needNewPage = true;
                return currentY;
            }
            gfx.DrawString(CompanyNameTextBlock.Text, fonts.SubTitle, XBrushes.Black, new XPoint(x, currentY));
            currentY += fonts.SubTitle.Height * 1.2;

            // Vẽ mô tả công việc
            string[] jobDescLines = JobDescriptionTextBlock.Text.Split(new[] { "\r\n", "\r", "\n" },
                StringSplitOptions.None);
            foreach (string line in jobDescLines)
            {
                if (!string.IsNullOrWhiteSpace(line))
                {
                    currentY = DrawMultilineText(gfx, line.Trim(), fonts.Normal, XBrushes.Black,
                        x, currentY, contentWidth, layout.PageHeight, ref needNewPage);
                    if (needNewPage) return currentY;
                    currentY += fonts.Normal.Height * 0.3;
                }
            }

            return currentY + layout.SectionSpacing;
        }

        private double DrawEducation(XGraphics gfx, PDFFonts fonts, PDFLayout layout,
            double currentY, ref bool needNewPage, bool isFirstPage)
        {
            double x = isFirstPage ? layout.RightPanelX : layout.Margin;
            double contentWidth = isFirstPage ?
                layout.PageWidth - layout.RightPanelX - layout.Margin * 2 :
                layout.PageWidth - layout.Margin * 2;

            // Vẽ tiêu đề Education
            if (currentY + fonts.SectionTitle.Height > layout.PageHeight - layout.Margin)
            {
                needNewPage = true;
                return currentY;
            }

            gfx.DrawString("Education", fonts.SectionTitle, XBrushes.Black, new XPoint(x, currentY));
            currentY += fonts.SectionTitle.Height * 1.2;

            // Vẽ ngày tháng
            if (currentY + fonts.Normal.Height > layout.PageHeight - layout.Margin)
            {
                needNewPage = true;
                return currentY;
            }
            gfx.DrawString(EducationExpDateTextBlock.Text, fonts.Normal, XBrushes.DarkGreen, new XPoint(x, currentY));
            currentY += fonts.Normal.Height * 1.2;

            // Vẽ tên trường
            if (currentY + fonts.SubTitle.Height > layout.PageHeight - layout.Margin)
            {
                needNewPage = true;
                return currentY;
            }
            gfx.DrawString(SchoolNameTextBlock.Text, fonts.SubTitle, XBrushes.Black, new XPoint(x, currentY));
            currentY += fonts.SubTitle.Height * 1.2;

            // Vẽ mô tả học vấn
            string[] eduDescLines = EduDescriptionTextBlock.Text.Split(new[] { "\r\n", "\r", "\n" },
                StringSplitOptions.None);
            foreach (string line in eduDescLines)
            {
                if (!string.IsNullOrWhiteSpace(line))
                {
                    currentY = DrawMultilineText(gfx, line.Trim(), fonts.Normal, XBrushes.Black,
                        x, currentY, contentWidth, layout.PageHeight, ref needNewPage);
                    if (needNewPage) return currentY;
                    currentY += fonts.Normal.Height * 0.3;
                }
            }

            return currentY + layout.SectionSpacing;
        }

        private double DrawSkills(XGraphics gfx, PDFFonts fonts, PDFLayout layout,
            double currentY, ref bool needNewPage, bool isFirstPage)
        {
            double x = isFirstPage ? layout.RightPanelX : layout.Margin;
            double contentWidth = isFirstPage ?
                layout.PageWidth - layout.RightPanelX - layout.Margin * 2 :
                layout.PageWidth - layout.Margin * 2;

            // Vẽ tiêu đề Skills
            if (currentY + fonts.SectionTitle.Height > layout.PageHeight - layout.Margin)
            {
                needNewPage = true;
                return currentY;
            }

            gfx.DrawString("Skills", fonts.SectionTitle, XBrushes.Black, new XPoint(x, currentY));
            currentY += fonts.SectionTitle.Height * 1.2;

            // Technical Skills
            if (currentY + fonts.SubTitle.Height > layout.PageHeight - layout.Margin)
            {
                needNewPage = true;
                return currentY;
            }
            gfx.DrawString("Technical Skills", fonts.SubTitle, XBrushes.Black, new XPoint(x, currentY));
            currentY += fonts.SubTitle.Height * 1.2;

            // Vẽ chi tiết technical skills
            string[] techSkillsLines = TechnicalSkillsDetailTextBlock.Text.Split(new[] { "\r\n", "\r", "\n" },
                StringSplitOptions.None);
            foreach (string line in techSkillsLines)
            {
                if (!string.IsNullOrWhiteSpace(line))
                {
                    currentY = DrawMultilineText(gfx, line.Trim(), fonts.Normal, XBrushes.Black,
                        x, currentY, contentWidth, layout.PageHeight, ref needNewPage);
                    if (needNewPage) return currentY;
                    currentY += fonts.Normal.Height * 0.3;
                }
            }

            // Soft Skills
            currentY += layout.SectionSpacing;
            if (currentY + fonts.SubTitle.Height > layout.PageHeight - layout.Margin)
            {
                needNewPage = true;
                return currentY;
            }
            gfx.DrawString("Soft Skills", fonts.SubTitle, XBrushes.Black, new XPoint(x, currentY));
            currentY += fonts.SubTitle.Height * 1.2;

            // Vẽ chi tiết soft skills
            string[] softSkillsLines = SoftSkillsDetailTextBlock.Text.Split(new[] { "\r\n", "\r", "\n" },
                StringSplitOptions.None);
            foreach (string line in softSkillsLines)
            {
                if (!string.IsNullOrWhiteSpace(line))
                {
                    currentY = DrawMultilineText(gfx, line.Trim(), fonts.Normal, XBrushes.Black,
                        x, currentY, contentWidth, layout.PageHeight, ref needNewPage);
                    if (needNewPage) return currentY;
                    currentY += fonts.Normal.Height * 0.3;
                }
            }

            return currentY + layout.SectionSpacing;
        }

        private PdfPage CreateNewPage(PdfDocument document, double pageWidth, double pageHeight, ref XGraphics gfx)
        {
            var newPage = document.AddPage();
            newPage.Width = pageWidth;
            newPage.Height = pageHeight;
            gfx = XGraphics.FromPdfPage(newPage);
            return newPage;
        }

        private double DrawSection(XGraphics gfx, string title, string content, PDFFonts fonts, 
            PDFLayout layout, double currentY, ref bool needNewPage, bool isFirstPage)
        {
            double x = isFirstPage ? layout.RightPanelX : layout.Margin;
            double contentWidth = isFirstPage ? 
                layout.PageWidth - layout.RightPanelX - layout.Margin * 2 :
                layout.PageWidth - layout.Margin * 2;

            // Vẽ tiêu đề section
            if (currentY + fonts.SectionTitle.Height > layout.PageHeight - layout.Margin)
            {
                needNewPage = true;
                return currentY;
            }

            gfx.DrawString(title, fonts.SectionTitle, XBrushes.Black, new XPoint(x, currentY));
            currentY += fonts.SectionTitle.Height * 1.2;

            // Xử lý nội dung với xuống dòng
            string[] lines = content.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
            foreach (string line in lines)
            {
                if (!string.IsNullOrWhiteSpace(line))
                {
                    currentY = DrawMultilineText(gfx, line.Trim(), fonts.Normal, XBrushes.Black,
                        x, currentY, contentWidth, layout.PageHeight, ref needNewPage);
                    
                    if (needNewPage)
                        return currentY;

                    currentY += fonts.Normal.Height * 0.3;
                }
            }

            return currentY + layout.SectionSpacing;
        }

        private double DrawMultilineText(XGraphics gfx, string text, XFont font, XBrush brush,
            double x, double y, double maxWidth, double pageHeight, ref bool needNewPage)
        {
            if (string.IsNullOrEmpty(text)) return y;

            double lineHeight = font.Height * 1.2;
            string[] words = text.Split(' ');
            string currentLine = "";

            foreach (string word in words)
            {
                string testLine = currentLine + (currentLine.Length > 0 ? " " : "") + word;
                XSize size = gfx.MeasureString(testLine, font);

                if (size.Width > maxWidth && currentLine.Length > 0)
                {
                    if (y + lineHeight > pageHeight - 20 * MM_TO_POINT)
                    {
                        needNewPage = true;
                        return y;
                    }

                    gfx.DrawString(currentLine, font, brush, new XPoint(x, y));
                    currentLine = word;
                    y += lineHeight;
                }
                else
                {
                    currentLine = testLine;
                }
            }

            if (currentLine.Length > 0)
            {
                if (y + lineHeight > pageHeight - 20 * MM_TO_POINT)
                {
                    needNewPage = true;
                    return y;
                }
                gfx.DrawString(currentLine, font, brush, new XPoint(x, y));
                y += lineHeight;
            }

            return y;
        }

        private double DrawLeftPanel(XGraphics gfx, PDFFonts fonts, PDFLayout layout)
        {
            double y = 200; // Start below photo

            // Personal Info
            gfx.DrawString(FullNameTextBlock.Text, fonts.Name, XBrushes.White,
                new XPoint(layout.Margin, y));
            y += 40;

            gfx.DrawString(PositionTextBlock.Text, fonts.SubTitle, XBrushes.White,
                new XPoint(layout.Margin, y));
            y += 40;

            // Contact Details
            var contactInfo = new[]
            {
            EmailTextBlock.Text,
            PhoneTextBlock.Text,
            AddressTextBlock.Text
        };

            foreach (var info in contactInfo)
            {
                gfx.DrawString(info, fonts.Small, XBrushes.White,
                    new XPoint(layout.Margin, y));
                y += 25;
            }

            return y;
        }

        private double DrawProfilePhoto(XGraphics gfx, PDFLayout layout)
        {
            if (ProfileImage.Source is BitmapImage bitmapImage)
            {
                try
                {
                    using (MemoryStream ms = new MemoryStream())
                    {
                        BitmapEncoder encoder = new PngBitmapEncoder();
                        encoder.Frames.Add(BitmapFrame.Create(bitmapImage));
                        encoder.Save(ms);
                        ms.Position = 0;

                        XImage xImage = XImage.FromStream(() => ms);

                        // Điều chỉnh kích thước ảnh để phù hợp
                        double photoSize = Math.Min(layout.LeftPanelWidth - layout.Margin * 2, 100 * 2.83465); // Max 100mm
                        double x = layout.Margin;
                        double y = layout.Margin;

                        // Vẽ ảnh với tỷ lệ khung hình đúng
                        double aspectRatio = xImage.PixelWidth / (double)xImage.PixelHeight;
                        double width = photoSize;
                        double height = photoSize / aspectRatio;

                        if (height > photoSize)
                        {
                            height = photoSize;
                            width = height * aspectRatio;
                        }

                        gfx.DrawImage(xImage, x, y, width, height);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error processing image: {ex.Message}");
                }
            }

            return layout.Margin;
        }
        private void SavePDFDocument(PdfDocument document)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                Filter = "PDF file (*.pdf)|*.pdf",
                FileName = "Generated_CV.pdf"
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                string filename = saveFileDialog.FileName;
                document.Save(filename);

                // Lưu tên file CV vào database
                CurriculumVitae newCV = new()
                {
                    Name = Path.GetFileName(filename)
                };
                _cvService.CreateCV(newCV);

                MessageBox.Show("PDF generated successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }

            document.Close();
        }
    }
}
