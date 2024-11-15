using Microsoft.Win32;
using PdfSharpCore.Drawing;
using PdfSharpCore.Pdf;
using System;
using System.Collections.Generic;
using System.IO;
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
using Xceed.Wpf.Toolkit;

namespace Template
{
    /// <summary>
    /// Interaction logic for Template1Window.xaml
    /// </summary>
    public partial class Template1Window : Window
    {
        private const double A4_WIDTH_MM = 210;
        private const double A4_HEIGHT_MM = 297;
        private const double MM_TO_POINT = 2.83465;

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
            public double Margin { get; } = 20 * MM_TO_POINT; // 20mm margin
            public double SectionSpacing { get; } = 15 * MM_TO_POINT; // 15mm spacing

            public PDFLayout(double pageWidth, double pageHeight)
            {
                PageWidth = pageWidth;
                PageHeight = pageHeight;
                LeftPanelWidth = pageWidth / 3;
                RightPanelX = LeftPanelWidth + Margin;
            }
        }

        public Template1Window()
        {
            InitializeComponent();

            PopulateFontSelector();

            // Set default font to Arial if it's available in the list
            SetDefaultFont("Arial");

            LoadAvaterImage(null);

            // Get the path to the Background folder within the output directory
            backgroundFolderPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Background");

            // Load images into the selection panel
            LoadBackgroundImages();
        }

        private string backgroundFolderPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Background");



        //---------------------------------------------------------------
        // Font Selection
        private void FontsComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedFont = FontsComboBox.SelectedItem as string;
            if (!string.IsNullOrEmpty(selectedFont))
            {
                // Apply the selected font to a target element, e.g., ExampleTextElement
                ContentScrollViewer.FontFamily = new FontFamily(selectedFont);
            }
            else
            {
                ContentScrollViewer.FontFamily = new FontFamily("Arial");

            }
        }

        private void PopulateFontSelector()
        {
            foreach (var fontFamily in Fonts.SystemFontFamilies)
            {
                FontsComboBox.Items.Add(fontFamily.Source);
            }
        }

        private void SetDefaultFont(string defaultFont)
        {
            // Check if the ComboBox contains the default font
            if (FontsComboBox.Items.Contains(defaultFont))
            {
                // Set the ComboBox selection to the default font
                FontsComboBox.SelectedItem = defaultFont;
            }
        }



        //---------------------------------------------------------------
        // Color Selection
        private void ThemeColorButton_Click(object sender, RoutedEventArgs e)
        {

            ColorCanvasPopup.IsOpen = true;
            ColorCanvasPopup.StaysOpen = true;
            Overlay.Visibility = Visibility.Visible;
        }


        private void PopupColorCanvas_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {
            // Update the circle color when a new color is selected
            if (e.NewValue.HasValue)
            {
                // Clone the existing brush to modify it
                var originalBrush = (SolidColorBrush)this.Resources["ThemeColorBrush"];
                var clonedBrush = originalBrush.Clone();
                clonedBrush.Color = e.NewValue.Value;

                // Replace the frozen brush in the resources with the cloned brush
                this.Resources["ThemeColorBrush"] = clonedBrush;
            }

        }

        //---------------------------------------------------------------
        // Background Image Selection

        private void BackgroundButton_Click(object sender, RoutedEventArgs e)
        {
            BackgroundPopup.IsOpen = !BackgroundPopup.IsOpen;
            BackgroundPopup.StaysOpen = !BackgroundPopup.StaysOpen;
            Overlay.Visibility = BackgroundPopup.IsOpen ? Visibility.Visible : Visibility.Collapsed;
        }

        private void LoadBackgroundImages()
        {
            // Clear previous images
            ImageSelectionPanel.Children.Clear();

            // Load images from the Background folder
            if (Directory.Exists(backgroundFolderPath))
            {
                var imageFiles = Directory.GetFiles(backgroundFolderPath, "*.png"); // Adjust for other formats if needed
                foreach (var filePath in imageFiles)
                {
                    var image = new Image
                    {
                        Source = new BitmapImage(new Uri(filePath, UriKind.Absolute)),
                        Width = 80,
                        Height = 80,
                        Margin = new Thickness(5),
                        Stretch = Stretch.UniformToFill
                    };
                    image.MouseDown += BackgroundImage_MouseDown;
                    ImageSelectionPanel.Children.Add(image);
                }
            }
        }

        private void BackgroundImage_MouseDown(object sender, MouseButtonEventArgs e)
        {
            // Handle the selection of an image
            Image? selectedImage = sender as Image;
            if (selectedImage != null)
            {
                // Set the background of your CV area to the selected image
                ContentScrollViewer.Background = new ImageBrush((BitmapImage)selectedImage.Source);
            }
        }

        private void ExportButton_Click(object sender, RoutedEventArgs e)
        {
            ExportPopup.IsOpen = true;
        }

        private void GeneratePDFButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                PdfDocument document = new PdfDocument();
                document.Info.Title = "Template1 Document";

                var pageWidth = A4_WIDTH_MM * MM_TO_POINT;
                var pageHeight = A4_HEIGHT_MM * MM_TO_POINT;
                var fonts = new PDFFonts();
                var layout = new PDFLayout(pageWidth, pageHeight);

                // Initialize the first page
                PdfPage currentPage = document.AddPage();
                currentPage.Width = pageWidth;
                currentPage.Height = pageHeight;
                XGraphics gfx = XGraphics.FromPdfPage(currentPage);

                // Draw left panel (only on the first page)
                gfx.DrawRectangle(new XSolidBrush(XColor.FromArgb(0x2D, 0x3E, 0x50)),
                    0, 0, layout.LeftPanelWidth, pageHeight);
                DrawProfilePhoto(gfx, layout);
                DrawLeftPanel(gfx, fonts, layout);

                // Start drawing right panel
                double currentY = layout.Margin;
                bool isFirstPage = true;
                bool needNewPage = false;

                // Draw each section and handle page transitions
                currentY = DrawSection(gfx, "Overview", "Overview content here...", fonts, layout, currentY, ref needNewPage, isFirstPage);

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

        private PdfPage CreateNewPage(PdfDocument document, double pageWidth, double pageHeight, ref XGraphics gfx)
        {
            var newPage = document.AddPage();
            newPage.Width = pageWidth;
            newPage.Height = pageHeight;
            gfx = XGraphics.FromPdfPage(newPage);
            return newPage;
        }

        private double DrawSection(XGraphics gfx, string title, string content, PDFFonts fonts, PDFLayout layout, double currentY, ref bool needNewPage, bool isFirstPage)
        {
            double x = isFirstPage ? layout.RightPanelX : layout.Margin;
            double contentWidth = isFirstPage ? layout.PageWidth - layout.RightPanelX - layout.Margin * 2 : layout.PageWidth - layout.Margin * 2;

            // Draw section title
            if (currentY + fonts.SectionTitle.Height > layout.PageHeight - layout.Margin)
            {
                needNewPage = true;
                return currentY;
            }

            gfx.DrawString(title, fonts.SectionTitle, XBrushes.Black, new XPoint(x, currentY));
            currentY += fonts.SectionTitle.Height * 1.2;

            // Handle content with line breaks
            string[] lines = content.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
            foreach (string line in lines)
            {
                if (!string.IsNullOrWhiteSpace(line))
                {
                    currentY = DrawMultilineText(gfx, line.Trim(), fonts.Normal, XBrushes.Black, x, currentY, contentWidth, layout.PageHeight, ref needNewPage);
                    if (needNewPage) return currentY;
                    currentY += fonts.Normal.Height * 0.3;
                }
            }

            return currentY + layout.SectionSpacing;
        }

        private double DrawMultilineText(XGraphics gfx, string text, XFont font, XBrush brush, double x, double y, double maxWidth, double pageHeight, ref bool needNewPage)
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

                        // Adjust image size to fit
                        double photoSize = Math.Min(layout.LeftPanelWidth - layout.Margin * 2, 100 * MM_TO_POINT); // Max 100mm
                        double x = layout.Margin;
                        double y = layout.Margin;

                        // Draw image with correct aspect ratio
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

        private double DrawLeftPanel(XGraphics gfx, PDFFonts fonts, PDFLayout layout)
        {
            double y = 200; // Start below photo

            // Personal Info
            gfx.DrawString("Full Name", fonts.Name, XBrushes.White, new XPoint(layout.Margin, y));
            y += 40;

            gfx.DrawString("Position", fonts.SubTitle, XBrushes.White, new XPoint(layout.Margin, y));
            y += 40;

            // Contact Details
            var contactInfo = new[]
            {
                "Email",
                "Phone",
                "Address"
            };

            foreach (var info in contactInfo)
            {
                gfx.DrawString(info, fonts.Small, XBrushes.White, new XPoint(layout.Margin, y));
                y += 25;
            }

            return y;
        }

        private double DrawWorkExperience(XGraphics gfx, PDFFonts fonts, PDFLayout layout, double currentY, ref bool needNewPage, bool isFirstPage)
        {
            return DrawSection(gfx, "Work Experience", "Work experience details here...", fonts, layout, currentY, ref needNewPage, isFirstPage);
        }

        private double DrawEducation(XGraphics gfx, PDFFonts fonts, PDFLayout layout, double currentY, ref bool needNewPage, bool isFirstPage)
        {
            return DrawSection(gfx, "Education", "Education details here...", fonts, layout, currentY, ref needNewPage, isFirstPage);
        }

        private double DrawSkills(XGraphics gfx, PDFFonts fonts, PDFLayout layout, double currentY, ref bool needNewPage, bool isFirstPage)
        {
            return DrawSection(gfx, "Skills", "Skills details here...", fonts, layout, currentY, ref needNewPage, isFirstPage);
        }

        private void SavePDFDocument(PdfDocument document)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                Filter = "PDF file (*.pdf)|*.pdf",
                FileName = "Template1_Document.pdf"
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                string filename = saveFileDialog.FileName;
                document.Save(filename);
                MessageBox.Show("PDF generated successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }

            document.Close();
        }

        //---------------------------------------------------------------
        // Overlay for capturing outside clicks
        private void Overlay_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            // Close the popup and hide the overlay when clicking outside
            ColorCanvasPopup.IsOpen = false;
            ColorCanvasPopup.StaysOpen = false;

            BackgroundPopup.IsOpen = false;
            BackgroundPopup.StaysOpen = false;

            Overlay.Visibility = Visibility.Collapsed;
        }

        private void LoadAvaterImage(string avatarPath)
        {
            string defaultAvatarPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Avatar", "default-avatar.png");

            ImageBrush avatarBrush = new ImageBrush();

            if (!string.IsNullOrEmpty(avatarPath) && File.Exists(avatarPath))
            {
                avatarBrush.ImageSource = new BitmapImage(new Uri(avatarPath, UriKind.Absolute));
            }
            else
            {
                avatarBrush.ImageSource = new BitmapImage(new Uri(defaultAvatarPath, UriKind.Absolute));
            }

            avatarBrush.Stretch = Stretch.UniformToFill;

            AvatarEllipse.Fill = avatarBrush;
        }

        private void AvatarBorder_MouseEnter(object sender, MouseEventArgs e)
        {
            // Show the edit button and set the border color
            UploadAvatarButton.Visibility = Visibility.Visible;
            AvatarBorder.BorderBrush = ColorEllipse.Fill;
        }

        private void AvatarBorder_MouseLeave(object sender, MouseEventArgs e)
        {
            // Hide the edit button and reset the border color
            UploadAvatarButton.Visibility = Visibility.Collapsed;
            AvatarBorder.BorderBrush = Brushes.Transparent;
        }

        private void UploadAvatarButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Title = "Select an Avatar Image",
                Filter = "Image Files|*.jpg;*.jpeg;*.png;*.gif;*.bmp",
            };

            if (openFileDialog.ShowDialog() == true)
            {
                LoadAvaterImage(openFileDialog.FileName);
            }
        }
        //////////////////////////////////////////////

        private T FindChildByUid<T>(DependencyObject parent, string uid) where T : FrameworkElement
        {
            if (parent == null) return null;

            int childCount = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < childCount; i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                if (child is T typedChild && typedChild.Uid == uid)
                {
                    return typedChild;
                }

                var result = FindChildByUid<T>(child, uid);
                if (result != null)
                {
                    return result;
                }
            }

            return null;
        }
        private T FindChild<T>(DependencyObject parent) where T : FrameworkElement
        {
            if (parent == null) return null;

            int childCount = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < childCount; i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                if (child is T typedChild)
                {
                    return typedChild;
                }

                var result = FindChild<T>(child);
                if (result != null)
                {
                    return result;
                }
            }

            return null;
        }
        private void Section_MouseEnter(object sender, MouseEventArgs e)
        {
            // Hiển thị nút "Add Section" khi di chuột vào vùng Grid
            if (sender is Grid grid)
            {
                var addButton = FindChildByUid<Button>(grid, "AddSectionButton");
                var subInfoAddButton = FindChildByUid<Button>(grid, "SubInformationAddSectionButton");
                if (addButton != null)
                {
                    addButton.Visibility = Visibility.Visible;
                }
                if (subInfoAddButton != null)
                {
                    subInfoAddButton.Visibility = Visibility.Visible;
                }
            }
        }

        private void Section_MouseLeave(object sender, MouseEventArgs e)
        {
            // Ẩn nút "Add Section" khi chuột rời khỏi vùng Grid hoàn toàn
            if (sender is Grid grid)
            {
                var addButton = FindChildByUid<Button>(grid, "AddSectionButton");
                var subInfoAddButton = FindChildByUid<Button>(grid, "SubInformationAddSectionButton");
                if (addButton != null && !addButton.IsMouseOver)
                {
                    addButton.Visibility = Visibility.Collapsed;
                }
                if (subInfoAddButton != null && !subInfoAddButton.IsMouseOver)
                {
                    subInfoAddButton.Visibility = Visibility.Collapsed;
                }
            }
        }

        private void AddSectionButton_Click(object sender, RoutedEventArgs e)
        {
            // Add a new section below the current section
            if (sender is Button addButton)
            {
                var grid = addButton.Parent as Grid;
                if (grid != null)
                {
                    var parentStackPanel = grid.Parent as StackPanel;
                    if (parentStackPanel != null)
                    {
                        var newSection = CreateNewSection();
                        if (newSection != null)
                        {

                            int index = BodyCVStackPanel.Children.IndexOf(parentStackPanel);
                            BodyCVStackPanel.Children.Insert(index + 1, newSection);
                        }
                        else
                        {
                            System.Windows.MessageBox.Show("New Section create fail");
                        }
                    }
                }
            }
        }

        private void AddLeftSectionButton_Click(object sender, RoutedEventArgs e)
        {
            if(sender is Button addButton)
            {
                var grid = addButton.Parent as Grid;
                if(grid != null)
                {
                    var parentStackPanel = grid.Parent as StackPanel;
                    if (parentStackPanel != null)
                    {
                        var newSection = CreateNewSection();
                        if (newSection != null)
                        {
                            int index = LeftSectionStackPanel.Children.IndexOf(parentStackPanel);
                            LeftSectionStackPanel.Children.Insert(index + 1, newSection);
                        }
                        else
                        {
                            System.Windows.MessageBox.Show("New Section create fail");
                        }
                    }
                }
            }
        }

        private void AddRightSectionButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button addButton)
            {
                var grid = addButton.Parent as Grid;
                if (grid != null)
                {
                    var parentStackPanel = grid.Parent as StackPanel;
                    if (parentStackPanel != null)
                    {
                        var newSection = CreateNewSection();
                        if (newSection != null)
                        {
                            System.Windows.MessageBox.Show("Add new section here");

                            int index = RightSectionStackPanel.Children.IndexOf(parentStackPanel);
                            RightSectionStackPanel.Children.Insert(index + 1, newSection);
                            System.Windows.MessageBox.Show("Index: " + index);
                        }
                        else
                        {
                            System.Windows.MessageBox.Show("New Section create fail");
                        }
                    }
                }
            }
        }

        private void AddWithColumnRightSectionButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button addButton)
            {
                var grid = addButton.Parent as Grid;
                if (grid != null)
                {
                    var parentStackPanel = grid.Parent as StackPanel;
                    if (parentStackPanel != null)
                    {
                        var newSection = CreateSubInfoSectionWithoutSubTitle();
                        if (newSection != null)
                        {
                            System.Windows.MessageBox.Show("Add new section here");

                            int index = RightSectionStackPanel.Children.IndexOf(parentStackPanel);
                            RightSectionStackPanel.Children.Insert(index + 1, newSection);
                            System.Windows.MessageBox.Show("Index: " + index);
                        }
                        else
                        {
                            System.Windows.MessageBox.Show("New Section create fail");
                        }
                    }
                }
            }
        }


        private StackPanel CreateNewSection()
        {
            // Create the outer StackPanel
            var sectionPanel = new StackPanel { Margin = new Thickness(0, 10, 0, 10) };

            // Create the Grid
            var grid = new Grid();
            grid.MouseEnter += Section_MouseEnter;
            grid.MouseLeave += Section_MouseLeave;

            // Define the RowDefinitions for the Grid
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

            // Create and configure the first TextBox (for title)
            var titleTextBox = new TextBox
            {
                Text = "ADDITIONAL INFORMATION",
                Style = (Style)FindResource("TitleTextBox")
            };
            Grid.SetRow(titleTextBox, 0);
            grid.Children.Add(titleTextBox);

            // Create and configure the second TextBox (for additional information)
            var infoTextBox = new TextBox
            {
                Text = "Fill in additional information",
                Margin = new Thickness(0, 5, 0, 0)
            };
            Grid.SetRow(infoTextBox, 1);
            grid.Children.Add(infoTextBox);

            // Create and configure the Button
            var addButton = new Button
            {
                Content = "+ Add Section",
                Style = (Style)FindResource("AddSectionButtonStyle"),
                Uid = "AddSectionButton"
            };
            addButton.Click += AddSectionButton_Click;
            Grid.SetRow(addButton, 2);
            grid.Children.Add(addButton);

            // Add the Grid to the StackPanel
            sectionPanel.Children.Add(grid);

            return sectionPanel;
        }

        private void SubInformationAddSectionButton_Click(object sender, RoutedEventArgs e)
        {
            // Add a new section below the current section
            if (sender is Button addButton)
            {
                var grid = addButton.Parent as Grid;
                if (grid != null)
                {
                    var parentStackPanel = FindChild<StackPanel>(grid);
                    if (parentStackPanel != null)
                    {
                        var newSection = CreateSubInfoSection();
                        if (newSection != null)
                        {
                            System.Windows.MessageBox.Show("Add new section here");

                            parentStackPanel.Children.Insert(parentStackPanel.Children.Count, newSection);
                            System.Windows.MessageBox.Show("Index: " + parentStackPanel.Children.Count);
                        }
                        else
                        {
                            System.Windows.MessageBox.Show("New Section create fail");
                        }
                    }
                }
            }
        }


        private StackPanel CreateSubInfoSection()
        {
            var subInfoPanel = new StackPanel { Margin = new Thickness(0, 10, 0, 0) };

            var grid = new Grid();

            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(200) });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });

            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

            var leftStackPanel = new StackPanel { Margin = new Thickness(0, 1, 0, 0) };
            var rightStackPanel = new StackPanel { Margin = new Thickness(0, 1, 0, 0) };

            var positionTextBox = new TextBox
            {
                Text = "Position",
                FontWeight = FontWeights.Bold,
                Foreground = (Brush)FindResource("ThemeColorBrush")
            };
            var fromToTextBox = new TextBox { Text = "From - To" };

            var organizationTextBox = new TextBox
            {
                Text = "Organization Name",
                FontWeight = FontWeights.Bold,
                Foreground = (Brush)FindResource("ThemeColorBrush")
            };
            var activityTextBox = new TextBox { Text = "Activity description" };

            leftStackPanel.Children.Add(positionTextBox);
            leftStackPanel.Children.Add(fromToTextBox);

            rightStackPanel.Children.Add(organizationTextBox);
            rightStackPanel.Children.Add(activityTextBox);

            Grid.SetColumn(leftStackPanel, 0);
            Grid.SetRow(leftStackPanel, 0);
            Grid.SetColumn(rightStackPanel, 1);
            Grid.SetRow(rightStackPanel, 0);

            grid.Children.Add(leftStackPanel);
            grid.Children.Add(rightStackPanel);

            subInfoPanel.Children.Add(grid);

            return subInfoPanel;
        }

        private StackPanel CreateSubInfoSectionWithoutSubTitle()
        {
            //var subInfoPanel = new StackPanel { Margin = new Thickness(0, 10, 0, 0) };

            //var grid = new Grid();

            //grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(200) });
            //grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });

            //var leftStackPanel = new StackPanel { Margin = new Thickness(0, 1, 0, 0) };
            //var rightStackPanel = new StackPanel { Margin = new Thickness(0, 1, 0, 0) };

            //var fromToTextBox = new TextBox { Text = "From - To" };

            //var activityTextBox = new TextBox { Text = "Activity description" };

            //leftStackPanel.Children.Add(fromToTextBox);

            //rightStackPanel.Children.Add(activityTextBox);

            //Grid.SetColumn(leftStackPanel, 0);
            //Grid.SetRow(leftStackPanel, 0);
            //Grid.SetColumn(rightStackPanel, 1);
            //Grid.SetRow(rightStackPanel, 0);

            //grid.Children.Add(leftStackPanel);
            //grid.Children.Add(rightStackPanel);

            //subInfoPanel.Children.Add(grid);

            //return subInfoPanel;

            var subInfoPanel = new StackPanel { Margin = new Thickness(0, 10, 0, 0) };

            var grid = new Grid();

            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(200) });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });

            var leftStackPanel = new StackPanel { Margin = new Thickness(0, 1, 0, 0) };
            var rightStackPanel = new StackPanel { Margin = new Thickness(0, 1, 0, 0) };

            var fromToTextBox = new TextBox { Text = "From - To" };

            var activityTextBox = new TextBox { Text = "Activity description" };

            leftStackPanel.Children.Add(fromToTextBox);

            rightStackPanel.Children.Add(activityTextBox);

            Grid.SetColumn(leftStackPanel, 0);
            Grid.SetRow(leftStackPanel, 0);
            Grid.SetColumn(rightStackPanel, 1);
            Grid.SetRow(rightStackPanel, 0);

            grid.Children.Add(leftStackPanel);
            grid.Children.Add(rightStackPanel);

            subInfoPanel.Children.Add(grid);

            return subInfoPanel;
        }



        //---------------------------------------------------------------        
    }
}
