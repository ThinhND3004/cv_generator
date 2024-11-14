using CV_Generator.BLL.Services;
using CV_Generator.DAL.Entities;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.Win32;
using PdfSharpCore.Drawing;
using PdfSharpCore.Pdf;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp;
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
                if (string.IsNullOrEmpty(textBox.Text))
                {
                    textBlock.Text = string.Empty;
                    textBox.Visibility = Visibility.Collapsed;
                    textBlock.Visibility = Visibility.Collapsed;
                    textBlock.Tag = null;
                }
                else
                {
                    textBlock.Text = textBox.Text;
                    textBox.Visibility = Visibility.Collapsed;
                    textBlock.Visibility = Visibility.Visible;
                }
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

        private TextBlock CreateTextBlock(string name, string text, double fontSize, FontWeight fontWeight, Brush foreground, Thickness margin, MouseButtonEventHandler mouseDownEvent, out TextBox associatedTextBox)
        {
            TextBlock textBlock = new TextBlock
            {
                Name = name,
                Text = text,
                FontSize = fontSize,
                FontWeight = fontWeight,
                Margin = margin,
                Foreground = foreground
            };
            textBlock.MouseDown += mouseDownEvent;

            associatedTextBox = new TextBox
            {
                Name = name.Replace("TextBlock", "TextBox"),
                Visibility = Visibility.Collapsed,
                FontSize = fontSize,
                Margin = new Thickness(0, 5, 0, 5),
                AcceptsReturn = true
            };
            associatedTextBox.LostFocus += TextBox_LostFocus;

            textBlock.Tag = associatedTextBox;
            associatedTextBox.Tag = textBlock;

            return textBlock;
        }

        private void AddNewRow(object sender, RoutedEventArgs e)
        {
            RowDefinition newRow = new RowDefinition();
            newRow.Height = GridLength.Auto;
            WorkExperienceGrid.RowDefinitions.Add(newRow);

            int currentIndex = WorkExperienceGrid.RowDefinitions.Count;


            StackPanel newStackPanel1 = new StackPanel();
            newStackPanel1.VerticalAlignment = VerticalAlignment.Top;
            newStackPanel1.SetValue(Grid.RowProperty, currentIndex);
            newStackPanel1.SetValue(Grid.ColumnProperty, 0);

            TextBox workExpDateTextBox;
            TextBlock workExpDateTextBlock = CreateTextBlock(
                $"WorkExpDateTextBlock{currentIndex}",
                "Jan 2020 - Present",
                10,
                FontWeights.Bold,
                Foreground = new SolidColorBrush(Colors.Green),
                new Thickness(0, 8, 5, 5),
                TextBlock_MouseDown,
                out workExpDateTextBox
            );

            newStackPanel1.Children.Add(workExpDateTextBlock);
            newStackPanel1.Children.Add(workExpDateTextBox);
            WorkExperienceGrid.Children.Add(newStackPanel1);


            StackPanel newStackPanel2 = new StackPanel();
            newStackPanel2.SetValue(Grid.RowProperty, currentIndex);
            newStackPanel2.SetValue(Grid.ColumnProperty, 1);

            TextBox companyNameTextBox;
            TextBlock companyNameTextBlock = CreateTextBlock(
                $"CompanyNameTextBlock{currentIndex}",
                "Full Group Inc.",
                14,
                FontWeights.Bold,
                Foreground = new SolidColorBrush(Colors.Black),
                new Thickness(0, 5, 0, 5),
                TextBlock_MouseDown,
                out companyNameTextBox
            );

            TextBox jobDescriptionTextBox;
            TextBlock jobDescriptionTextBlock = CreateTextBlock(
                $"JobDescriptionTextBlock{currentIndex}",
                "Responsibilities: Worked on backend development using PHP, MySQL, and Node.js.\nAchievements: Improved system performance by 20%.",
                12,
                FontWeights.Normal,
                Foreground = new SolidColorBrush(Colors.Black),
                new Thickness(0, 5, 0, 5),
                TextBlock_MouseDown,
                out jobDescriptionTextBox
            );

            newStackPanel2.Children.Add(companyNameTextBlock);
            newStackPanel2.Children.Add(companyNameTextBox);
            newStackPanel2.Children.Add(jobDescriptionTextBlock);
            newStackPanel2.Children.Add(jobDescriptionTextBox);

            WorkExperienceGrid.Children.Add(newStackPanel2);
        }




        private void AddNewEduRow(object sender, RoutedEventArgs e)
        {
            int currentIndex = EducationGrid.RowDefinitions.Count;


            TextBox educationExpDateTextBox;
            TextBlock educationExpDateTextBlock = CreateTextBlock(
                $"EducationExpDateTextBlock{currentIndex}",
                "Jan 2020 - Present",
                10,
                FontWeights.Bold,
                new SolidColorBrush(Colors.Green),
                new Thickness(0, 8, 5, 5),
                TextBlock_MouseDown,
                out educationExpDateTextBox
            );


            TextBox schoolNameTextBox;
            TextBlock schoolNameTextBlock = CreateTextBlock(
                $"SchoolNameTextBlock{currentIndex}",
                "FPT University",
                14,
                FontWeights.Bold,
                new SolidColorBrush(Colors.Black),
                new Thickness(0, 5, 0, 0),
                TextBlock_MouseDown,
                out schoolNameTextBox
            );


            TextBox eduDescriptionTextBox;
            TextBlock eduDescriptionTextBlock = CreateTextBlock(
                $"EduDescriptionTextBlock{currentIndex}",
                "Degree: Bachelor\nMajor: Website, Mobile Programming",
                12,
                FontWeights.Normal,
                new SolidColorBrush(Colors.Black),
                new Thickness(0, 5, 0, 0),
                TextBlock_MouseDown,
                out eduDescriptionTextBox
            );

            StackPanel dateStackPanel = new StackPanel
            {
                VerticalAlignment = VerticalAlignment.Top
            };
            dateStackPanel.Children.Add(educationExpDateTextBlock);
            dateStackPanel.Children.Add(educationExpDateTextBox);

            StackPanel detailsStackPanel = new StackPanel();
            detailsStackPanel.Children.Add(schoolNameTextBlock);
            detailsStackPanel.Children.Add(schoolNameTextBox);
            detailsStackPanel.Children.Add(eduDescriptionTextBlock);
            detailsStackPanel.Children.Add(eduDescriptionTextBox);

            EducationGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });


            Grid.SetRow(dateStackPanel, currentIndex);
            Grid.SetColumn(dateStackPanel, 0);

            Grid.SetRow(detailsStackPanel, currentIndex);
            Grid.SetColumn(detailsStackPanel, 1);


            EducationGrid.Children.Add(dateStackPanel);
            EducationGrid.Children.Add(detailsStackPanel);
        }



        private void AddNewProject(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < 4; i++)
            {
                RowDefinition newRow = new RowDefinition();
                newRow.Height = GridLength.Auto;
                ProjectsGrid.RowDefinitions.Add(newRow);
            }

            int currentIndex = ProjectsGrid.RowDefinitions.Count - 4;

            AddTextBlockAndTextBox("ProjectName", "Project Name\n(2017 - Present)", currentIndex, fontSize: 14, fontWeight: FontWeights.Bold);
            AddTextBlockAndTextBox("Descriptions", "Descriptions:", currentIndex + 1);
            AddTextBlockAndTextBox("DescriptionsDetail", "Detailed description of the project...", currentIndex + 1, 1);
            AddTextBlockAndTextBox("PositionInProject", "Position:", currentIndex + 2);
            AddTextBlockAndTextBox("PositionInProjectDetail", "Backend Developer", currentIndex + 2, 1);
            AddTextBlockAndTextBox("TechnologyInUse", "Technology in Use:", currentIndex + 3);
            AddTextBlockAndTextBox("TechnologyInUseDetail", "C#, .NET, SQL Server, Azure", currentIndex + 3, 1);
        }

        private void AddTextBlockAndTextBox(string name, string text, int row, int column = 0, double fontSize = 12, FontWeight fontWeight = default)
        {
            StackPanel stackPanel = new StackPanel();
            stackPanel.SetValue(Grid.RowProperty, row);
            stackPanel.SetValue(Grid.ColumnProperty, column);


            fontWeight = fontWeight == default ? FontWeights.Normal : fontWeight;

            TextBlock textBlock = new TextBlock
            {
                Name = $"{name}TextBlock",
                Text = text,
                FontSize = fontSize,
                FontWeight = fontWeight,
                Margin = new Thickness(0, 5, 0, 5),
                TextWrapping = TextWrapping.Wrap
            };

            TextBox textBox = new TextBox
            {
                Name = $"{name}TextBox",
                FontSize = fontSize,
                Text = text,
                Margin = new Thickness(0, 5, 0, 5),
                Visibility = Visibility.Collapsed,
                AcceptsReturn = true,
                TextWrapping = TextWrapping.Wrap
            };

            textBlock.Tag = textBox;
            textBox.Tag = textBlock;

            stackPanel.Children.Add(textBlock);
            stackPanel.Children.Add(textBox);

            ProjectsGrid.Children.Add(stackPanel);

            textBlock.MouseDown += TextBlock_MouseDown;
            textBox.LostFocus += TextBox_LostFocus;
        }






        public void GeneratePdfFromWpfContent(UIElement content, string filePath)
        {
            try
            {
                // Create a RenderTargetBitmap to render the UI content into an image
                RenderTargetBitmap rtb = new RenderTargetBitmap(
                    (int)content.RenderSize.Width,
                    (int)content.RenderSize.Height,
                    96, 96, // DPI
                    PixelFormats.Pbgra32);

                // Render the UI element content into the bitmap
                content.Measure(new System.Windows.Size(double.PositiveInfinity, double.PositiveInfinity));
                content.Arrange(new Rect(content.DesiredSize));
                rtb.Render(content);

                // Create a Pdf document
                PdfDocument pdfDoc = new PdfDocument();
                PdfPage page = pdfDoc.AddPage();
                XGraphics gfx = XGraphics.FromPdfPage(page);

                // Convert the RenderTargetBitmap to a byte array (PNG format)
                using (MemoryStream ms = new MemoryStream())
                {
                    // Save the RenderTargetBitmap to a PNG in memory
                    PngBitmapEncoder encoder = new PngBitmapEncoder();
                    encoder.Frames.Add(BitmapFrame.Create(rtb));
                    encoder.Save(ms);

                    // Ensure the MemoryStream is at the beginning before reading it
                    ms.Seek(0, SeekOrigin.Begin);

                    // Create a Func<Stream> to return the MemoryStream
                    Func<Stream> streamFunc = () => ms;

                    // Create an XImage from the stream using the Func
                    XImage xImage = XImage.FromStream(streamFunc);

                    // Draw the image onto the PDF
                    gfx.DrawImage(xImage, 0, 0, page.Width, page.Height);
                }

                // Save the PDF to the specified file path
                pdfDoc.Save(filePath);
                Console.WriteLine("PDF generated successfully!");
            }
            catch (Exception ex)
            {
                // Handle any errors
                Console.WriteLine("Error generating PDF: " + ex.Message);
            }
        }

        private void GenerateButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Ensure the method is being called
                Console.WriteLine("GenerateButton_Click triggered");

                // Assuming your content is inside a Grid named "MainGrid"
                GeneratePdfFromWpfContent(MainGrid, "output.pdf");
            }
            catch (Exception ex)
            {
                // Handle errors during the button click
                Console.WriteLine("Error during button click: " + ex.Message);
            }
        }



        


        //private void AddNewLine(StackPanel targetStackPanel, Button clickedButton)
        //{
        //    StackPanel textWithButtonPanel = new StackPanel
        //    {
        //        Orientation = Orientation.Horizontal,
        //        Margin = new Thickness(0, 5, 0, 5)
        //    };

        //    TextBlock newTextBlock = new TextBlock
        //    {
        //        Text = "Click to edit",
        //        FontSize = 12,
        //        VerticalAlignment = VerticalAlignment.Center
        //    };

        //    Button removeButton = new Button
        //    {
        //        Content = "X",
        //        Width = 20,
        //        Height = 20,
        //        Margin = new Thickness(5, 0, 0, 0),
        //        HorizontalAlignment = HorizontalAlignment.Left
        //    };

        //    removeButton.Click += (s, e) =>
        //    {
        //        targetStackPanel.Children.Remove(textWithButtonPanel);
        //    };

        //    TextBox newTextBox = new TextBox
        //    {
        //        Visibility = Visibility.Collapsed,
        //        Margin = new Thickness(0, 5, 0, 5)
        //    };

        //    newTextBlock.MouseDown += (s, e) =>
        //    {
        //        newTextBlock.Visibility = Visibility.Collapsed;
        //        newTextBox.Text = newTextBlock.Text;
        //        newTextBox.Visibility = Visibility.Visible;
        //        newTextBox.Focus();
        //    };

        //    newTextBox.LostFocus += (s, e) =>
        //    {
        //        newTextBlock.Text = newTextBox.Text;
        //        newTextBox.Visibility = Visibility.Collapsed;
        //        newTextBlock.Visibility = Visibility.Visible;
        //    };

        //    textWithButtonPanel.Children.Add(newTextBlock);
        //    textWithButtonPanel.Children.Add(removeButton);
        //    textWithButtonPanel.Children.Add(newTextBox);

        //    targetStackPanel.Children.Add(textWithButtonPanel);
        //}


        //private void AddNewLineButton_Click(object sender, RoutedEventArgs e)
        //{
        //    //Button clickedButton = (Button)sender;
        //    //StackPanel parentStackPanel = (StackPanel)((FrameworkElement)sender).Parent;
        //    //AddNewLine(parentStackPanel, clickedButton);
        //    ShowInputTextBox();
        //}
        //private void ShowInputTextBox()
        //{

        //    inputTextBox = new TextBox
        //    {
        //        Width = 200,
        //        Margin = new Thickness(0, 5, 0, 5)
        //    };

        //    // Tạo nút Confirm
        //    confirmButton = new Button
        //    {
        //        Content = "Confirm",
        //        Margin = new Thickness(5)
        //    };
        //    // Nút Cancel
        //    cancelButton = new Button
        //    {
        //        Content = "Cancel",
        //        Margin = new Thickness(5)
        //    };
        //    confirmButton.Click += ConfirmButton_Click;
        //    //Event cho Cancel
        //    cancelButton.Click += (s, args) =>
        //    {
        //        // Xóa TextBox và nút Confirm và Cancel
        //        ExperienceList.Children.Remove(inputTextBox);
        //        ExperienceList.Children.Remove(confirmButton);
        //        ExperienceList.Children.Remove(cancelButton);
        //    };

        //    ExperienceList.Children.Add(inputTextBox);
        //    ExperienceList.Children.Add(confirmButton);
        //    ExperienceList.Children.Add(cancelButton);
        //    inputTextBox.Focus();
        //}
        //private void ConfirmButton_Click(object sender, RoutedEventArgs e)
        //{
        //    string entry = inputTextBox.Text.Trim();

        //    if (!string.IsNullOrEmpty(entry))
        //    {

        //        //StackPanel để xóa
        //        StackPanel entryPanel = new StackPanel { Orientation = Orientation.Horizontal, Margin = new Thickness(0, 5, 0, 5) };
        //        //TextBlock để hiển thị thông tin
        //        TextBlock entryTextBlock = new TextBlock
        //        {
        //            Text = entry,
        //            FontSize = 16,
        //            Margin = new Thickness(0, 5, 0, 5)
        //        };
        //        var normalEntry = new FormattedEntry
        //        {
        //            Text = entry,
        //            Font = new XFont("Arial", 16, XFontStyle.Regular),
        //            Brush = XBrushes.Black
        //        };
        //        entries.Add(normalEntry);
        //        //Nút xóa
        //        Button deleteButton = new Button
        //        {
        //            Content = "X",
        //            Width = 20,
        //            Height = 20,
        //            Margin = new Thickness(5, 0, 0, 0)
        //        };
        //        // Đăng ký sự kiện cho nút xóa
        //        deleteButton.Click += (s, args) =>
        //        {
        //            ExperienceList.Children.Remove(entryPanel);
        //        };

        //        // Thêm TextBlock và nút xóa vào StackPanel
        //        entryPanel.Children.Add(entryTextBlock);
        //        entryPanel.Children.Add(deleteButton);

        //        // Thêm StackPanel vào danh sách hiển thị
        //        ExperienceList.Children.Add(entryPanel);

        //        // Xóa TextBox và nút Confirm
        //        ExperienceList.Children.Remove(inputTextBox);
        //        ExperienceList.Children.Remove(confirmButton);
        //        ExperienceList.Children.Remove(cancelButton);
        //    }
        //    else
        //    {
        //        MessageBox.Show("Please enter a valid entry.");
        //    }
        //}
        //private void AddNewBoldLine(StackPanel targetStackPanel, Button clickedButton)
        //{


        //    StackPanel textWithButtonPanel = new StackPanel
        //    {
        //        Orientation = Orientation.Horizontal,
        //        Margin = new Thickness(0, 5, 0, 5)
        //    };

        //    TextBlock newTextBlock = new TextBlock
        //    {
        //        Text = "Click to edit",
        //        FontWeight = FontWeights.Bold,
        //        FontSize = 16,
        //        Margin = new Thickness(0, 20, 0, 5)
        //    };

        //    Button removeButton = new Button
        //    {
        //        Content = "X",
        //        Width = 20,
        //        Height = 20,
        //        Margin = new Thickness(5, 0, 0, 0),
        //        HorizontalAlignment = HorizontalAlignment.Left
        //    };

        //    removeButton.Click += (s, e) =>
        //    {
        //        targetStackPanel.Children.Remove(textWithButtonPanel);
        //    };

        //    TextBox newTextBox = new TextBox
        //    {
        //        Visibility = Visibility.Collapsed,
        //        Margin = new Thickness(0, 5, 0, 5)
        //    };

        //    newTextBlock.MouseDown += (s, e) =>
        //    {
        //        newTextBlock.Visibility = Visibility.Collapsed;
        //        newTextBox.Text = newTextBlock.Text;
        //        newTextBox.Visibility = Visibility.Visible;
        //        newTextBox.Focus();
        //    };

        //    newTextBox.LostFocus += (s, e) =>
        //    {
        //        newTextBlock.Text = newTextBox.Text;
        //        newTextBox.Visibility = Visibility.Collapsed;
        //        newTextBlock.Visibility = Visibility.Visible;
        //    };

        //    textWithButtonPanel.Children.Add(newTextBlock);
        //    textWithButtonPanel.Children.Add(removeButton);
        //    textWithButtonPanel.Children.Add(newTextBox);

        //    targetStackPanel.Children.Add(textWithButtonPanel);
        //}

        //private void AddNewBoldLineButton_Click(object sender, RoutedEventArgs e)
        //{
        //    //Button clickedButton = (Button)sender;
        //    //StackPanel parentStackPanel = (StackPanel)((FrameworkElement)sender).Parent;
        //    //AddNewBoldLine(parentStackPanel, clickedButton);
        //    isBold = true; // Đặt kiểu chữ là đậm
        //    ShowInputBoldTextBox();
        //}

        //private void ShowInputBoldTextBox()
        //{
        //    // Tạo một TextBox mới để nhập dữ liệu
        //    inputTextBox = new TextBox
        //    {
        //        Width = 200,
        //        Margin = new Thickness(0, 5, 0, 5)
        //    };

        //    // Tạo nút Confirm
        //    confirmButton = new Button
        //    {
        //        Content = "Confirm",
        //        Margin = new Thickness(5)
        //    };

        //    // Tạo nút Cancel
        //    cancelButton = new Button
        //    {
        //        Content = "Cancel",
        //        Margin = new Thickness(5)
        //    };

        //    // Đăng ký sự kiện cho nút Confirm
        //    confirmButton.Click += ConfirmBoldButton_Click;

        //    // Đăng ký sự kiện cho nút Cancel
        //    cancelButton.Click += (s, args) =>
        //    {
        //        // Xóa TextBox và nút Confirm và Cancel
        //        ExperienceList.Children.Remove(inputTextBox);
        //        ExperienceList.Children.Remove(confirmButton);
        //        ExperienceList.Children.Remove(cancelButton);
        //    };

        //    // Thêm TextBox và nút vào StackPanel
        //    ExperienceList.Children.Add(inputTextBox);
        //    ExperienceList.Children.Add(confirmButton);
        //    ExperienceList.Children.Add(cancelButton);

        //    inputTextBox.Focus(); // Tập trung vào TextBox
        //}

        //private void ConfirmBoldButton_Click(object sender, RoutedEventArgs e)
        //{
        //    string entry = inputTextBox.Text.Trim();

        //    if (!string.IsNullOrEmpty(entry))
        //    {

        //        // Tạo một StackPanel để chứa TextBlock và nút xóa
        //        StackPanel entryPanel = new StackPanel { Orientation = Orientation.Horizontal, Margin = new Thickness(0, 5, 0, 5) };

        //        // Tạo một TextBlock để hiển thị thông tin
        //        TextBlock entryTextBlock = new TextBlock
        //        {
        //            Text = entry,
        //            FontSize = 16,
        //            FontWeight = isBold ? FontWeights.Bold : FontWeights.Normal, // Đặt kiểu chữ dựa trên biến isBold
        //            Margin = new Thickness(0, 0, 5, 0)
        //        };
        //        var normalEntry = new FormattedEntry
        //        {
        //            Text = entry,
        //            Font = new XFont("Arial", 16, XFontStyle.Bold),
        //            Brush = XBrushes.Black
        //        };
        //        entries.Add(normalEntry);

        //        // Tạo nút xóa
        //        Button deleteButton = new Button
        //        {
        //            Content = "X",
        //            Width = 20,
        //            Height = 20,
        //            Margin = new Thickness(5, 0, 0, 0)
        //        };

        //        // Đăng ký sự kiện cho nút xóa
        //        deleteButton.Click += (s, args) =>
        //        {
        //            ExperienceList.Children.Remove(entryPanel);
        //        };

        //        // Thêm TextBlock và nút xóa vào StackPanel
        //        entryPanel.Children.Add(entryTextBlock);
        //        entryPanel.Children.Add(deleteButton);

        //        // Thêm StackPanel vào danh sách hiển thị
        //        ExperienceList.Children.Add(entryPanel);

        //        // Xóa TextBox và nút Confirm và Cancel
        //        ExperienceList.Children.Remove(inputTextBox);
        //        ExperienceList.Children.Remove(confirmButton);
        //        ExperienceList.Children.Remove(cancelButton);
        //    }
        //    else
        //    {
        //        MessageBox.Show("Please enter a valid entry.");
        //    }
        //}

        //private void AddNewStackPanelWithButtons(StackPanel parentStackPanel, Button clickedButton)
        //{
        //    StackPanel newStackPanel = new StackPanel
        //    {
        //        Orientation = Orientation.Vertical,
        //        Margin = new Thickness(0, 10, 0, 10)
        //    };

        //    TextBlock newTextBlock = new TextBlock
        //    {
        //        Text = "Click to edit",
        //        FontWeight = FontWeights.Bold,
        //        FontSize = 20,
        //        Margin = new Thickness(0, 20, 0, 5)
        //    };

        //    TextBox newTextBox = new TextBox
        //    {
        //        Text = newTextBlock.Text,
        //        Visibility = Visibility.Collapsed,
        //        FontWeight = FontWeights.Bold,
        //        FontSize = 20,
        //        Margin = new Thickness(0, 20, 0, 5)
        //    };

        //    newTextBlock.MouseDown += (s, e) =>
        //    {
        //        newTextBlock.Visibility = Visibility.Collapsed;
        //        newTextBox.Visibility = Visibility.Visible;
        //        newTextBox.Focus();
        //    };

        //    newTextBox.LostFocus += (s, e) =>
        //    {
        //        newTextBlock.Text = newTextBox.Text;
        //        newTextBox.Visibility = Visibility.Collapsed;
        //        newTextBlock.Visibility = Visibility.Visible;
        //    };

        //    newStackPanel.Children.Add(newTextBlock);
        //    newStackPanel.Children.Add(newTextBox);

        //    Button addNewLineButton = new Button
        //    {
        //        Content = "Add New Line",
        //        Margin = new Thickness(0, 5, 0, 5),
        //        HorizontalAlignment = HorizontalAlignment.Left
        //    };
        //    addNewLineButton.Click += (sender, e) => AddNewLineWithSlackPanel(newStackPanel, addNewLineButton);

        //    Button addNewBoldLineButton = new Button
        //    {
        //        Content = "Add New Bold Line",
        //        Margin = new Thickness(0, 5, 0, 5),
        //        HorizontalAlignment = HorizontalAlignment.Left
        //    };
        //    addNewBoldLineButton.Click += (sender, e) => AddNewBoldLineWithSlackPanel(newStackPanel, addNewBoldLineButton);

        //    newStackPanel.Children.Add(addNewLineButton);
        //    newStackPanel.Children.Add(addNewBoldLineButton);

        //    Button removeButton = new Button
        //    {
        //        Content = "Remove StackPanel",
        //        HorizontalAlignment = HorizontalAlignment.Left
        //    };
        //    removeButton.Click += (sender, e) => RemoveStackPanel(newStackPanel);

        //    newStackPanel.Children.Add(removeButton);

        //    int insertIndex = parentStackPanel.Children.IndexOf(clickedButton);
        //    parentStackPanel.Children.Insert(insertIndex, newStackPanel);
        //}

        //private void AddNewLineWithSlackPanel(StackPanel targetStackPanel, Button clickedButton)
        //{
        //    StackPanel textWithButtonPanel = new StackPanel
        //    {
        //        Orientation = Orientation.Horizontal,
        //        Margin = new Thickness(0, 5, 0, 5)
        //    };

        //    TextBlock newTextBlock = new TextBlock
        //    {
        //        Text = "Click to edit",
        //        FontSize = 12,
        //        VerticalAlignment = VerticalAlignment.Center
        //    };

        //    Button removeButton = new Button
        //    {
        //        Content = "X",
        //        Width = 20,
        //        Height = 20,
        //        Margin = new Thickness(5, 0, 0, 0),
        //        HorizontalAlignment = HorizontalAlignment.Left
        //    };

        //    removeButton.Click += (s, e) =>
        //    {
        //        targetStackPanel.Children.Remove(textWithButtonPanel);
        //    };

        //    TextBox newTextBox = new TextBox
        //    {
        //        Visibility = Visibility.Collapsed,
        //        Margin = new Thickness(0, 5, 0, 5)
        //    };

        //    newTextBlock.MouseDown += (s, e) =>
        //    {
        //        newTextBlock.Visibility = Visibility.Collapsed;
        //        newTextBox.Text = newTextBlock.Text;
        //        newTextBox.Visibility = Visibility.Visible;
        //        newTextBox.Focus();
        //    };

        //    newTextBox.LostFocus += (s, e) =>
        //    {
        //        newTextBlock.Text = newTextBox.Text;
        //        newTextBox.Visibility = Visibility.Collapsed;
        //        newTextBlock.Visibility = Visibility.Visible;
        //    };

        //    textWithButtonPanel.Children.Add(newTextBlock);
        //    textWithButtonPanel.Children.Add(removeButton);
        //    textWithButtonPanel.Children.Add(newTextBox);

        //    targetStackPanel.Children.Add(textWithButtonPanel);
        //}

        //private void AddNewBoldLineWithSlackPanel(StackPanel targetStackPanel, Button clickedButton)
        //{
        //    StackPanel textWithButtonPanel = new StackPanel
        //    {
        //        Orientation = Orientation.Horizontal,
        //        Margin = new Thickness(0, 20, 0, 5)
        //    };

        //    TextBlock newTextBlock = new TextBlock
        //    {
        //        Text = "Click to edit",
        //        FontWeight = FontWeights.Bold,
        //        FontSize = 16,
        //        VerticalAlignment = VerticalAlignment.Center
        //    };

        //    Button removeButton = new Button
        //    {
        //        Content = "X",
        //        Width = 20,
        //        Height = 20,
        //        Margin = new Thickness(5, 0, 0, 0),
        //        HorizontalAlignment = HorizontalAlignment.Left
        //    };

        //    removeButton.Click += (s, e) =>
        //    {
        //        targetStackPanel.Children.Remove(textWithButtonPanel);
        //    };

        //    TextBox newTextBox = new TextBox
        //    {
        //        Visibility = Visibility.Collapsed,
        //        Margin = new Thickness(0, 20, 0, 5)
        //    };

        //    newTextBlock.MouseDown += (s, e) =>
        //    {
        //        newTextBlock.Visibility = Visibility.Collapsed;
        //        newTextBox.Text = newTextBlock.Text;
        //        newTextBox.Visibility = Visibility.Visible;
        //        newTextBox.Focus();
        //    };

        //    newTextBox.LostFocus += (s, e) =>
        //    {
        //        newTextBlock.Text = newTextBox.Text;
        //        newTextBox.Visibility = Visibility.Collapsed;
        //        newTextBlock.Visibility = Visibility.Visible;
        //    };

        //    textWithButtonPanel.Children.Add(newTextBlock);
        //    textWithButtonPanel.Children.Add(removeButton);
        //    textWithButtonPanel.Children.Add(newTextBox);

        //    targetStackPanel.Children.Add(textWithButtonPanel);
        //}

        //private void RemoveStackPanel(StackPanel stackPanel)
        //{
        //    StackPanel parentStackPanel = (StackPanel)stackPanel.Parent;
        //    parentStackPanel.Children.Remove(stackPanel);
        //}

        //private void AddNewStackPanelWithButtonsButton_Click(object sender, RoutedEventArgs e)
        //{
        //    Button clickedButton = (Button)sender;
        //    StackPanel parentStackPanel = (StackPanel)((FrameworkElement)sender).Parent;
        //    AddNewStackPanelWithButtons(parentStackPanel, clickedButton);
        //}






        //private void GenerateToPdf_Click(object sender, RoutedEventArgs e)
        //{


        //    PdfDocument document = new PdfDocument();
        //    document.Info.Title = "CV Document";
        //    PdfPage page = document.AddPage();
        //    XGraphics gfx = XGraphics.FromPdfPage(page);
        //    XFont titleFont = new XFont("Arial", 20, XFontStyle.Bold);
        //    XFont subtitleFont = new XFont("Arial", 16, XFontStyle.Bold);
        //    XFont normalFont = new XFont("Arial", 12, XFontStyle.Regular);
        //    XBrush brush = XBrushes.White;




        //    // Set background color
        //    gfx.DrawRectangle(new XSolidBrush(XColor.FromArgb(0x2D, 0x3E, 0x50)), 0, 0, page.Width / 3, page.Height);
        //    int x = 200;
        //    int y = 20;
        //    if (ProfileImage.Source is BitmapImage bitmapImage)
        //    {
        //        MemoryStream ms = new MemoryStream();
        //        BitmapEncoder encoder = new PngBitmapEncoder();
        //        encoder.Frames.Add(BitmapFrame.Create(bitmapImage));
        //        encoder.Save(ms);
        //        ms.Position = 0;

        //        // Load the image to PdfSharp XImage
        //        XImage xImage = XImage.FromStream(() => ms);

        //        // Draw Profile Image in the PDF
        //        gfx.DrawImage(xImage, 25, 40, 150, 150); // Position and size (x, y, width, height)
        //    }
        //    else
        //    {
        //        MessageBox.Show("KHONG CO HINH ANH", "LOI ROI THANG NGU", MessageBoxButton.OK, MessageBoxImage.Error);
        //    }
        //    // Profile Section
        //    //gfx.DrawString("Personal Details", titleFont, brush, new XPoint(y, 30 + x));
        //    //gfx.DrawString(PersonalInfoTextBlock.Text, subtitleFont, brush, new XPoint(y, 70 + x));
        //    //gfx.DrawString(PositionTextBlock.Text, subtitleFont, brush, new XPoint(y, 100 + x));
        //    //gfx.DrawString(EmailTextBlock.Text, normalFont, brush, new XPoint(y, 130 + x));
        //    //gfx.DrawString(PhoneTextBlock.Text, normalFont, brush, new XPoint(y, 160 + x));
        //    //gfx.DrawString(AddressTextBlock.Text, normalFont, brush, new XPoint(y, 190 + x));

        //    // Experience Section
        //    //gfx.DrawString("Professional Experience", titleFont, XBrushes.Black, new XPoint(200, 40));
        //    //gfx.DrawString(CompanyNameLabel.Text, subtitleFont, XBrushes.Black, new XPoint(200, 70));
        //    //gfx.DrawString(YearsLabel.Text, normalFont, XBrushes.Black, new XPoint(200, 100));
        //    //gfx.DrawString(DescriptionLabel.Text, normalFont, XBrushes.Black, new XPoint(200, 130));
        //    int yPosition = 70; // Vị trí bắt đầu cho từng mục kinh nghiệm
        //                        //foreach (var experience in entries)
        //                        //{
        //                        //    gfx.DrawString(experience, subtitleFont, XBrushes.Black, new XPoint(200, yPosition));
        //                        //    yPosition += 30; // Cập nhật khoảng cách cho mục tiếp theo
        //                        //    //gfx.DrawString(experience, normalFont, XBrushes.Black, new XPoint(200, yPosition));
        //                        //    //yPosition += 20; // Cập nhật khoảng cách cho mục tiếp theo
        //                        //    //gfx.DrawString(experience, normalFont, XBrushes.Black, new XPoint(200, yPosition));
        //                        //    //yPosition += 40; // Thêm khoảng cách giữa các mục
        //                        //}
        //    foreach (var entry in entries)
        //    {
        //        gfx.DrawString(entry.Text, entry.Font, entry.Brush, new XPoint(200, yPosition));
        //        yPosition += 30; // Điều chỉnh khoảng cách giữa các mục
        //    }
        //    // Education Section

        //    SaveFileDialog saveFileDialog = new SaveFileDialog
        //    {
        //        Filter = "PDF file (*.pdf)|*.pdf",
        //        FileName = "Generated_CV.pdf"
        //    };

        //    if (saveFileDialog.ShowDialog() == true)
        //    {
        //        string filename = saveFileDialog.FileName;
        //        using (FileStream stream = new FileStream(filename, FileMode.Create, FileAccess.Write))
        //        {
        //            document.Save(stream);
        //            CurriculumVitae newCV = new();
        //            newCV.Name = System.IO.Path.GetFileName(saveFileDialog.FileName);
        //            newCV.CreateAt = DateTime.Now;
        //            newCV.CreateBy = UserCreateCV.Id;
        //            _cvService.CreateCV(newCV);
        //        }
        //    }







        //    document.Close();
        //    MessageBox.Show("PDF generated successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
        //}

        //private void AddTitleButton_Click(object sender, RoutedEventArgs e)
        //{
        //    isBold = true; // Đặt kiểu chữ là đậm
        //    ShowInputTitleTextBox(); // Hiển thị ô nhập liệu cho tiêu đề
        //}

        //private void ShowInputTitleTextBox()
        //{
        //    // Tạo một TextBox mới để nhập dữ liệu
        //    inputTextBox = new TextBox
        //    {
        //        Width = 200,
        //        Margin = new Thickness(0, 5, 0, 5)
        //    };

        //    // Tạo nút Confirm
        //    confirmButton = new Button
        //    {
        //        Content = "Confirm",
        //        Margin = new Thickness(5)
        //    };

        //    // Tạo nút Cancel
        //    cancelButton = new Button
        //    {
        //        Content = "Cancel",
        //        Margin = new Thickness(5)
        //    };

        //    // Đăng ký sự kiện cho nút Confirm
        //    confirmButton.Click += ConfirmTitleButton_Click;

        //    // Đăng ký sự kiện cho nút Cancel
        //    cancelButton.Click += (s, args) =>
        //    {
        //        // Xóa TextBox và nút Confirm và Cancel
        //        ExperienceList.Children.Remove(inputTextBox);
        //        ExperienceList.Children.Remove(confirmButton);
        //        ExperienceList.Children.Remove(cancelButton);
        //    };

        //    // Thêm TextBox và nút vào StackPanel của giáo dục
        //    ExperienceList.Children.Add(inputTextBox);
        //    ExperienceList.Children.Add(confirmButton);
        //    ExperienceList.Children.Add(cancelButton);

        //    inputTextBox.Focus(); // Tập trung vào TextBox
        //}

        //private void ConfirmTitleButton_Click(object sender, RoutedEventArgs e)
        //{
        //    string entry = inputTextBox.Text.Trim();

        //    if (!string.IsNullOrEmpty(entry))
        //    {

        //        // Tạo một StackPanel để chứa TextBlock và nút xóa
        //        StackPanel entryPanel = new StackPanel { Orientation = Orientation.Horizontal, Margin = new Thickness(0, 5, 0, 5) };

        //        // Tạo một TextBlock để hiển thị thông tin
        //        TextBlock entryTextBlock = new TextBlock
        //        {
        //            Text = entry,
        //            FontSize = 24, // Kích thước chữ lớn hơn
        //            FontWeight = FontWeights.Bold, // Chữ in đậm
        //            Foreground = Brushes.Red, // Màu chữ đỏ
        //            Margin = new Thickness(0, 0, 5, 0)
        //        };
        //        var normalEntry = new FormattedEntry
        //        {
        //            Text = entry,
        //            Font = new XFont("Arial", 24, XFontStyle.Bold),
        //            Brush = XBrushes.Red
        //        };
        //        entries.Add(normalEntry);

        //        // Tạo nút xóa
        //        Button deleteButton = new Button
        //        {
        //            Content = "X",
        //            Width = 20,
        //            Height = 20,
        //            Margin = new Thickness(5, 0, 0, 0)
        //        };

        //        // Đăng ký sự kiện cho nút xóa
        //        deleteButton.Click += (s, args) =>
        //        {
        //            ExperienceList.Children.Remove(entryPanel);
        //        };

        //        // Thêm TextBlock và nút xóa vào StackPanel
        //        entryPanel.Children.Add(entryTextBlock);
        //        entryPanel.Children.Add(deleteButton);

        //        // Thêm StackPanel vào danh sách hiển thị
        //        ExperienceList.Children.Add(entryPanel);

        //        // Xóa TextBox và nút Confirm và Cancel
        //        ExperienceList.Children.Remove(inputTextBox);
        //        ExperienceList.Children.Remove(confirmButton);
        //        ExperienceList.Children.Remove(cancelButton);
        //    }
        //    else
        //    {
        //        MessageBox.Show("Please enter a valid entry.");
        //    }
        //}

        //private void GenerateToDocx_Click(object sender, RoutedEventArgs e)
        //{
        //    SaveFileDialog saveFileDialog = new SaveFileDialog
        //    {
        //        Filter = "Word Document (*.docx)|*.docx",
        //        FileName = "Generated_CV.docx"
        //    };

        //    if (saveFileDialog.ShowDialog() == true)
        //    {
        //        string filePath = saveFileDialog.FileName;

        //        // Create a new Word document
        //        using (WordprocessingDocument wordDocument = WordprocessingDocument.Create(filePath, DocumentFormat.OpenXml.WordprocessingDocumentType.Document))
        //        {
        //            // Add a main document part
        //            MainDocumentPart mainPart = wordDocument.AddMainDocumentPart();
        //            mainPart.Document = new Document();
        //            Body body = new Body();

        //            // Add Title
        //            Paragraph titleParagraph = new Paragraph(new Run(new Text("CV")));
        //            titleParagraph.ParagraphProperties = new ParagraphProperties(new Justification() { Val = JustificationValues.Center });
        //            body.Append(titleParagraph);

        //            // Personal Information Section
        //            //body.Append(CreateSectionHeader("Personal Information"));
        //            //body.Append(CreateInfoParagraph("Name:", PersonalInfoTextBlock.Text));
        //            //body.Append(CreateInfoParagraph("Position:", PositionTextBlock.Text));
        //            //body.Append(CreateInfoParagraph("Email:", EmailTextBlock.Text));
        //            //body.Append(CreateInfoParagraph("Phone:", PhoneTextBlock.Text));
        //            //body.Append(CreateInfoParagraph("Address:", AddressTextBlock.Text));

        //            //// Professional Experience Section
        //            //body.Append(CreateSectionHeader("Professional Experience"));
        //            //body.Append(CreateInfoParagraph("Company:", CompanyNameTextBlock.Text));
        //            //body.Append(CreateInfoParagraph("Position:", PositionTextBlock.Text));
        //            //body.Append(CreateInfoParagraph("Years:", YearsTextBlock.Text));
        //            //body.Append(CreateInfoParagraph("Description:", DescriptionTextBlock.Text));

        //            //// Education Section
        //            //body.Append(CreateSectionHeader("Education"));
        //            //body.Append(CreateInfoParagraph("Degree:", DegreeLabel.Text));
        //            //body.Append(CreateInfoParagraph("Years:", DegreeYearsLabel.Text));

        //            // Save the document
        //            mainPart.Document.Append(body);
        //            mainPart.Document.Save();
        //        }

        //        MessageBox.Show("DOCX generated successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);

        //    }

        //}

        //// Helper method to create section headers
        //private Paragraph CreateSectionHeader(string headerText)
        //{
        //    Paragraph headerParagraph = new Paragraph(new Run(new Text(headerText)));
        //    headerParagraph.ParagraphProperties = new ParagraphProperties(new Justification() { Val = JustificationValues.Left });
        //    headerParagraph.ParagraphProperties.Append(new Bold());
        //    return headerParagraph;
        //}

        //// Helper method to create info paragraphs
        //private Paragraph CreateInfoParagraph(string label, string info)
        //{
        //    Run labelRun = new Run(new Text(label + " "));
        //    Run infoRun = new Run(new Text(info));
        //    Paragraph paragraph = new Paragraph();
        //    paragraph.Append(labelRun);
        //    paragraph.Append(infoRun);
        //    return paragraph;
        //}


    }
}
