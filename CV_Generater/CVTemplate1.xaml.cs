using CV_Generator.BLL.Services;
using CV_Generator.DAL.Entities;
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

namespace CV_Generater
{
    public partial class CVTemplate1 : Window
    {
        public Account UserCreateCV { get; set; }
        private CurViService _cvService = new();
        private TextBox inputTextBox;
        private Button confirmButton;
        private Button cancelButton;
        private bool isBold;
        private List<FormattedEntry> entries = new List<FormattedEntry>();
        public class FormattedEntry
        {
            public string Text { get; set; }
            public XFont Font { get; set; }
            public XBrush Brush { get; set; }
        }
        public CVTemplate1()
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


        //private void DegreeLabel_MouseDown(object sender, MouseButtonEventArgs e)
        //{
        //    DegreeLabel.Visibility = Visibility.Collapsed;
        //    DegreeTextBox.Text = DegreeLabel.Text;
        //    DegreeTextBox.Visibility = Visibility.Visible;
        //    DegreeTextBox.Focus();
        //}

        //private void DegreeTextBox_LostFocus(object sender, RoutedEventArgs e)
        //{
        //    DegreeLabel.Text = DegreeTextBox.Text;
        //    DegreeTextBox.Visibility = Visibility.Collapsed;
        //    DegreeLabel.Visibility = Visibility.Visible;
        //}


        //private void DegreeYearsLabel_MouseDown(object sender, MouseButtonEventArgs e)
        //{
        //    DegreeYearsLabel.Visibility = Visibility.Collapsed;
        //    DegreeYearsTextBox.Text = DegreeYearsLabel.Text;
        //    DegreeYearsTextBox.Visibility = Visibility.Visible;
        //    DegreeYearsTextBox.Focus();
        //}

        //private void DegreeYearsTextBox_LostFocus(object sender, RoutedEventArgs e)
        //{
        //    DegreeYearsLabel.Text = DegreeYearsTextBox.Text;
        //    DegreeYearsTextBox.Visibility = Visibility.Collapsed;
        //    DegreeYearsLabel.Visibility = Visibility.Visible;
        //}
        //private void SkillLabel_MouseDown(object sender, MouseButtonEventArgs e)
        //{
        //    SkillLabel.Visibility = Visibility.Collapsed;
        //    SkillTextBox.Text = SkillLabel.Text;
        //    SkillTextBox.Visibility = Visibility.Visible;
        //    SkillTextBox.Focus();
        //}


        //private void SkillTextBox_LostFocus(object sender, RoutedEventArgs e)
        //{
        //    SkillLabel.Text = SkillTextBox.Text;
        //    SkillTextBox.Visibility = Visibility.Collapsed;
        //    SkillLabel.Visibility = Visibility.Visible;
        //}


        private void AddNewLine(StackPanel targetStackPanel, Button clickedButton)
        {
            StackPanel textWithButtonPanel = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                Margin = new Thickness(0, 5, 0, 5)
            };

            TextBlock newTextBlock = new TextBlock
            {
                Text = "Click to edit",
                FontSize = 12,
                VerticalAlignment = VerticalAlignment.Center
            };

            Button removeButton = new Button
            {
                Content = "X",
                Width = 20,
                Height = 20,
                Margin = new Thickness(5, 0, 0, 0),
                HorizontalAlignment = HorizontalAlignment.Left
            };

            removeButton.Click += (s, e) =>
            {
                targetStackPanel.Children.Remove(textWithButtonPanel);
            };

            TextBox newTextBox = new TextBox
            {
                Visibility = Visibility.Collapsed,
                Margin = new Thickness(0, 5, 0, 5)
            };

            newTextBlock.MouseDown += (s, e) =>
            {
                newTextBlock.Visibility = Visibility.Collapsed;
                newTextBox.Text = newTextBlock.Text;
                newTextBox.Visibility = Visibility.Visible;
                newTextBox.Focus();
            };

            newTextBox.LostFocus += (s, e) =>
            {
                newTextBlock.Text = newTextBox.Text;
                newTextBox.Visibility = Visibility.Collapsed;
                newTextBlock.Visibility = Visibility.Visible;
            };

            textWithButtonPanel.Children.Add(newTextBlock);
            textWithButtonPanel.Children.Add(removeButton);
            textWithButtonPanel.Children.Add(newTextBox);

            targetStackPanel.Children.Add(textWithButtonPanel);
        }


        private void AddNewLineButton_Click(object sender, RoutedEventArgs e)
        {
            //Button clickedButton = (Button)sender;
            //StackPanel parentStackPanel = (StackPanel)((FrameworkElement)sender).Parent;
            //AddNewLine(parentStackPanel, clickedButton);
            ShowInputTextBox();
        }
        private void ShowInputTextBox()
        {

            inputTextBox = new TextBox
            {
                Width = 200,
                Margin = new Thickness(0, 5, 0, 5)
            };

            // Tạo nút Confirm
            confirmButton = new Button
            {
                Content = "Confirm",
                Margin = new Thickness(5)
            };
            // Nút Cancel
            cancelButton = new Button
            {
                Content = "Cancel",
                Margin = new Thickness(5)
            };
            confirmButton.Click += ConfirmButton_Click;
            //Event cho Cancel
            cancelButton.Click += (s, args) =>
            {
                // Xóa TextBox và nút Confirm và Cancel
                ExperienceList.Children.Remove(inputTextBox);
                ExperienceList.Children.Remove(confirmButton);
                ExperienceList.Children.Remove(cancelButton);
            };

            ExperienceList.Children.Add(inputTextBox);
            ExperienceList.Children.Add(confirmButton);
            ExperienceList.Children.Add(cancelButton);
            inputTextBox.Focus();
        }
        private void ConfirmButton_Click(object sender, RoutedEventArgs e)
        {
            string entry = inputTextBox.Text.Trim();

            if (!string.IsNullOrEmpty(entry))
            {

                //StackPanel để xóa
                StackPanel entryPanel = new StackPanel { Orientation = Orientation.Horizontal, Margin = new Thickness(0, 5, 0, 5) };
                //TextBlock để hiển thị thông tin
                TextBlock entryTextBlock = new TextBlock
                {
                    Text = entry,
                    FontSize = 16,
                    Margin = new Thickness(0, 5, 0, 5)
                };
                var normalEntry = new FormattedEntry
                {
                    Text = entry,
                    Font = new XFont("Arial", 16, XFontStyle.Regular),
                    Brush = XBrushes.Black
                };
                entries.Add(normalEntry);
                //Nút xóa
                Button deleteButton = new Button
                {
                    Content = "X",
                    Width = 20,
                    Height = 20,
                    Margin = new Thickness(5, 0, 0, 0)
                };
                // Đăng ký sự kiện cho nút xóa
                deleteButton.Click += (s, args) =>
                {
                    ExperienceList.Children.Remove(entryPanel);
                };

                // Thêm TextBlock và nút xóa vào StackPanel
                entryPanel.Children.Add(entryTextBlock);
                entryPanel.Children.Add(deleteButton);

                // Thêm StackPanel vào danh sách hiển thị
                ExperienceList.Children.Add(entryPanel);

                // Xóa TextBox và nút Confirm
                ExperienceList.Children.Remove(inputTextBox);
                ExperienceList.Children.Remove(confirmButton);
                ExperienceList.Children.Remove(cancelButton);
            }
            else
            {
                MessageBox.Show("Please enter a valid entry.");
            }
        }
        private void AddNewBoldLine(StackPanel targetStackPanel, Button clickedButton)
        {


            StackPanel textWithButtonPanel = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                Margin = new Thickness(0, 5, 0, 5)
            };

            TextBlock newTextBlock = new TextBlock
            {
                Text = "Click to edit",
                FontWeight = FontWeights.Bold,
                FontSize = 16,
                Margin = new Thickness(0, 20, 0, 5)
            };

            Button removeButton = new Button
            {
                Content = "X",
                Width = 20,
                Height = 20,
                Margin = new Thickness(5, 0, 0, 0),
                HorizontalAlignment = HorizontalAlignment.Left
            };

            removeButton.Click += (s, e) =>
            {
                targetStackPanel.Children.Remove(textWithButtonPanel);
            };

            TextBox newTextBox = new TextBox
            {
                Visibility = Visibility.Collapsed,
                Margin = new Thickness(0, 5, 0, 5)
            };

            newTextBlock.MouseDown += (s, e) =>
            {
                newTextBlock.Visibility = Visibility.Collapsed;
                newTextBox.Text = newTextBlock.Text;
                newTextBox.Visibility = Visibility.Visible;
                newTextBox.Focus();
            };

            newTextBox.LostFocus += (s, e) =>
            {
                newTextBlock.Text = newTextBox.Text;
                newTextBox.Visibility = Visibility.Collapsed;
                newTextBlock.Visibility = Visibility.Visible;
            };

            textWithButtonPanel.Children.Add(newTextBlock);
            textWithButtonPanel.Children.Add(removeButton);
            textWithButtonPanel.Children.Add(newTextBox);

            targetStackPanel.Children.Add(textWithButtonPanel);
        }

        private void AddNewBoldLineButton_Click(object sender, RoutedEventArgs e)
        {
            //Button clickedButton = (Button)sender;
            //StackPanel parentStackPanel = (StackPanel)((FrameworkElement)sender).Parent;
            //AddNewBoldLine(parentStackPanel, clickedButton);
            isBold = true; // Đặt kiểu chữ là đậm
            ShowInputBoldTextBox();
        }

        private void ShowInputBoldTextBox()
        {
            // Tạo một TextBox mới để nhập dữ liệu
            inputTextBox = new TextBox
            {
                Width = 200,
                Margin = new Thickness(0, 5, 0, 5)
            };

            // Tạo nút Confirm
            confirmButton = new Button
            {
                Content = "Confirm",
                Margin = new Thickness(5)
            };

            // Tạo nút Cancel
            cancelButton = new Button
            {
                Content = "Cancel",
                Margin = new Thickness(5)
            };

            // Đăng ký sự kiện cho nút Confirm
            confirmButton.Click += ConfirmBoldButton_Click;

            // Đăng ký sự kiện cho nút Cancel
            cancelButton.Click += (s, args) =>
            {
                // Xóa TextBox và nút Confirm và Cancel
                ExperienceList.Children.Remove(inputTextBox);
                ExperienceList.Children.Remove(confirmButton);
                ExperienceList.Children.Remove(cancelButton);
            };

            // Thêm TextBox và nút vào StackPanel
            ExperienceList.Children.Add(inputTextBox);
            ExperienceList.Children.Add(confirmButton);
            ExperienceList.Children.Add(cancelButton);

            inputTextBox.Focus(); // Tập trung vào TextBox
        }

        private void ConfirmBoldButton_Click(object sender, RoutedEventArgs e)
        {
            string entry = inputTextBox.Text.Trim();

            if (!string.IsNullOrEmpty(entry))
            {
              
                // Tạo một StackPanel để chứa TextBlock và nút xóa
                StackPanel entryPanel = new StackPanel { Orientation = Orientation.Horizontal, Margin = new Thickness(0, 5, 0, 5) };

                // Tạo một TextBlock để hiển thị thông tin
                TextBlock entryTextBlock = new TextBlock
                {
                    Text = entry,
                    FontSize = 16,
                    FontWeight = isBold ? FontWeights.Bold : FontWeights.Normal, // Đặt kiểu chữ dựa trên biến isBold
                    Margin = new Thickness(0, 0, 5, 0)
                };
                var normalEntry = new FormattedEntry
                {
                    Text = entry,
                    Font = new XFont("Arial", 16, XFontStyle.Bold),
                    Brush = XBrushes.Black
                };
                entries.Add(normalEntry);

                // Tạo nút xóa
                Button deleteButton = new Button
                {
                    Content = "X",
                    Width = 20,
                    Height = 20,
                    Margin = new Thickness(5, 0, 0, 0)
                };

                // Đăng ký sự kiện cho nút xóa
                deleteButton.Click += (s, args) =>
                {
                    ExperienceList.Children.Remove(entryPanel);
                };

                // Thêm TextBlock và nút xóa vào StackPanel
                entryPanel.Children.Add(entryTextBlock);
                entryPanel.Children.Add(deleteButton);

                // Thêm StackPanel vào danh sách hiển thị
                ExperienceList.Children.Add(entryPanel);

                // Xóa TextBox và nút Confirm và Cancel
                ExperienceList.Children.Remove(inputTextBox);
                ExperienceList.Children.Remove(confirmButton);
                ExperienceList.Children.Remove(cancelButton);
            }
            else
            {
                MessageBox.Show("Please enter a valid entry.");
            }
        }

        private void AddNewStackPanelWithButtons(StackPanel parentStackPanel, Button clickedButton)
        {
            StackPanel newStackPanel = new StackPanel
            {
                Orientation = Orientation.Vertical,
                Margin = new Thickness(0, 10, 0, 10)
            };

            TextBlock newTextBlock = new TextBlock
            {
                Text = "Click to edit",
                FontWeight = FontWeights.Bold,
                FontSize = 20,
                Margin = new Thickness(0, 20, 0, 5)
            };

            TextBox newTextBox = new TextBox
            {
                Text = newTextBlock.Text,
                Visibility = Visibility.Collapsed,
                FontWeight = FontWeights.Bold,
                FontSize = 20,
                Margin = new Thickness(0, 20, 0, 5)
            };

            newTextBlock.MouseDown += (s, e) =>
            {
                newTextBlock.Visibility = Visibility.Collapsed;
                newTextBox.Visibility = Visibility.Visible;
                newTextBox.Focus();
            };

            newTextBox.LostFocus += (s, e) =>
            {
                newTextBlock.Text = newTextBox.Text;
                newTextBox.Visibility = Visibility.Collapsed;
                newTextBlock.Visibility = Visibility.Visible;
            };

            newStackPanel.Children.Add(newTextBlock);
            newStackPanel.Children.Add(newTextBox);

            Button addNewLineButton = new Button
            {
                Content = "Add New Line",
                Margin = new Thickness(0, 5, 0, 5),
                HorizontalAlignment = HorizontalAlignment.Left
            };
            addNewLineButton.Click += (sender, e) => AddNewLineWithSlackPanel(newStackPanel, addNewLineButton);

            Button addNewBoldLineButton = new Button
            {
                Content = "Add New Bold Line",
                Margin = new Thickness(0, 5, 0, 5),
                HorizontalAlignment = HorizontalAlignment.Left
            };
            addNewBoldLineButton.Click += (sender, e) => AddNewBoldLineWithSlackPanel(newStackPanel, addNewBoldLineButton);

            newStackPanel.Children.Add(addNewLineButton);
            newStackPanel.Children.Add(addNewBoldLineButton);

            Button removeButton = new Button
            {
                Content = "Remove StackPanel",
                HorizontalAlignment = HorizontalAlignment.Left
            };
            removeButton.Click += (sender, e) => RemoveStackPanel(newStackPanel);

            newStackPanel.Children.Add(removeButton);

            int insertIndex = parentStackPanel.Children.IndexOf(clickedButton);
            parentStackPanel.Children.Insert(insertIndex, newStackPanel);
        }

        private void AddNewLineWithSlackPanel(StackPanel targetStackPanel, Button clickedButton)
        {
            StackPanel textWithButtonPanel = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                Margin = new Thickness(0, 5, 0, 5)
            };

            TextBlock newTextBlock = new TextBlock
            {
                Text = "Click to edit",
                FontSize = 12,
                VerticalAlignment = VerticalAlignment.Center
            };

            Button removeButton = new Button
            {
                Content = "X",
                Width = 20,
                Height = 20,
                Margin = new Thickness(5, 0, 0, 0),
                HorizontalAlignment = HorizontalAlignment.Left
            };

            removeButton.Click += (s, e) =>
            {
                targetStackPanel.Children.Remove(textWithButtonPanel);
            };

            TextBox newTextBox = new TextBox
            {
                Visibility = Visibility.Collapsed,
                Margin = new Thickness(0, 5, 0, 5)
            };

            newTextBlock.MouseDown += (s, e) =>
            {
                newTextBlock.Visibility = Visibility.Collapsed;
                newTextBox.Text = newTextBlock.Text;
                newTextBox.Visibility = Visibility.Visible;
                newTextBox.Focus();
            };

            newTextBox.LostFocus += (s, e) =>
            {
                newTextBlock.Text = newTextBox.Text;
                newTextBox.Visibility = Visibility.Collapsed;
                newTextBlock.Visibility = Visibility.Visible;
            };

            textWithButtonPanel.Children.Add(newTextBlock);
            textWithButtonPanel.Children.Add(removeButton);
            textWithButtonPanel.Children.Add(newTextBox);

            targetStackPanel.Children.Add(textWithButtonPanel);
        }

        private void AddNewBoldLineWithSlackPanel(StackPanel targetStackPanel, Button clickedButton)
        {
            StackPanel textWithButtonPanel = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                Margin = new Thickness(0, 20, 0, 5)
            };

            TextBlock newTextBlock = new TextBlock
            {
                Text = "Click to edit",
                FontWeight = FontWeights.Bold,
                FontSize = 16,
                VerticalAlignment = VerticalAlignment.Center
            };

            Button removeButton = new Button
            {
                Content = "X",
                Width = 20,
                Height = 20,
                Margin = new Thickness(5, 0, 0, 0),
                HorizontalAlignment = HorizontalAlignment.Left
            };

            removeButton.Click += (s, e) =>
            {
                targetStackPanel.Children.Remove(textWithButtonPanel);
            };

            TextBox newTextBox = new TextBox
            {
                Visibility = Visibility.Collapsed,
                Margin = new Thickness(0, 20, 0, 5)
            };

            newTextBlock.MouseDown += (s, e) =>
            {
                newTextBlock.Visibility = Visibility.Collapsed;
                newTextBox.Text = newTextBlock.Text;
                newTextBox.Visibility = Visibility.Visible;
                newTextBox.Focus();
            };

            newTextBox.LostFocus += (s, e) =>
            {
                newTextBlock.Text = newTextBox.Text;
                newTextBox.Visibility = Visibility.Collapsed;
                newTextBlock.Visibility = Visibility.Visible;
            };

            textWithButtonPanel.Children.Add(newTextBlock);
            textWithButtonPanel.Children.Add(removeButton);
            textWithButtonPanel.Children.Add(newTextBox);

            targetStackPanel.Children.Add(textWithButtonPanel);
        }

        private void RemoveStackPanel(StackPanel stackPanel)
        {
            StackPanel parentStackPanel = (StackPanel)stackPanel.Parent;
            parentStackPanel.Children.Remove(stackPanel);
        }

        private void AddNewStackPanelWithButtonsButton_Click(object sender, RoutedEventArgs e)
        {
            Button clickedButton = (Button)sender;
            StackPanel parentStackPanel = (StackPanel)((FrameworkElement)sender).Parent;
            AddNewStackPanelWithButtons(parentStackPanel, clickedButton);
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
            //gfx.DrawString("Professional Experience", titleFont, XBrushes.Black, new XPoint(200, 40));
            //gfx.DrawString(CompanyNameLabel.Text, subtitleFont, XBrushes.Black, new XPoint(200, 70));
            //gfx.DrawString(YearsLabel.Text, normalFont, XBrushes.Black, new XPoint(200, 100));
            //gfx.DrawString(DescriptionLabel.Text, normalFont, XBrushes.Black, new XPoint(200, 130));
            int yPosition = 70; // Vị trí bắt đầu cho từng mục kinh nghiệm
                                //foreach (var experience in entries)
                                //{
                                //    gfx.DrawString(experience, subtitleFont, XBrushes.Black, new XPoint(200, yPosition));
                                //    yPosition += 30; // Cập nhật khoảng cách cho mục tiếp theo
                                //    //gfx.DrawString(experience, normalFont, XBrushes.Black, new XPoint(200, yPosition));
                                //    //yPosition += 20; // Cập nhật khoảng cách cho mục tiếp theo
                                //    //gfx.DrawString(experience, normalFont, XBrushes.Black, new XPoint(200, yPosition));
                                //    //yPosition += 40; // Thêm khoảng cách giữa các mục
                                //}
            foreach (var entry in entries)
            {
                gfx.DrawString(entry.Text, entry.Font, entry.Brush, new XPoint(200, yPosition));
                yPosition += 30; // Điều chỉnh khoảng cách giữa các mục
            }
            // Education Section

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
                    CurriculumVitae newCV = new();
                    newCV.Name = System.IO.Path.GetFileName(saveFileDialog.FileName);
                    newCV.CreateAt = DateTime.Now;
                    newCV.CreateBy = UserCreateCV.Id;
                    _cvService.CreateCV(newCV); 
                }
            }


            




            document.Close();
            MessageBox.Show("PDF generated successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void AddTitleButton_Click(object sender, RoutedEventArgs e)
        {
            isBold = true; // Đặt kiểu chữ là đậm
            ShowInputTitleTextBox(); // Hiển thị ô nhập liệu cho tiêu đề
        }

        private void ShowInputTitleTextBox()
        {
            // Tạo một TextBox mới để nhập dữ liệu
            inputTextBox = new TextBox
            {
                Width = 200,
                Margin = new Thickness(0, 5, 0, 5)
            };

            // Tạo nút Confirm
            confirmButton = new Button
            {
                Content = "Confirm",
                Margin = new Thickness(5)
            };

            // Tạo nút Cancel
            cancelButton = new Button
            {
                Content = "Cancel",
                Margin = new Thickness(5)
            };

            // Đăng ký sự kiện cho nút Confirm
            confirmButton.Click += ConfirmTitleButton_Click;

            // Đăng ký sự kiện cho nút Cancel
            cancelButton.Click += (s, args) =>
            {
                // Xóa TextBox và nút Confirm và Cancel
                ExperienceList.Children.Remove(inputTextBox);
                ExperienceList.Children.Remove(confirmButton);
                ExperienceList.Children.Remove(cancelButton);
            };

            // Thêm TextBox và nút vào StackPanel của giáo dục
            ExperienceList.Children.Add(inputTextBox);
            ExperienceList.Children.Add(confirmButton);
            ExperienceList.Children.Add(cancelButton);

            inputTextBox.Focus(); // Tập trung vào TextBox
        }

        private void ConfirmTitleButton_Click(object sender, RoutedEventArgs e)
        {
            string entry = inputTextBox.Text.Trim();

            if (!string.IsNullOrEmpty(entry))
            {
                
                // Tạo một StackPanel để chứa TextBlock và nút xóa
                StackPanel entryPanel = new StackPanel { Orientation = Orientation.Horizontal, Margin = new Thickness(0, 5, 0, 5) };

                // Tạo một TextBlock để hiển thị thông tin
                TextBlock entryTextBlock = new TextBlock
                {
                    Text = entry,
                    FontSize = 24, // Kích thước chữ lớn hơn
                    FontWeight = FontWeights.Bold, // Chữ in đậm
                    Foreground = Brushes.Red, // Màu chữ đỏ
                    Margin = new Thickness(0, 0, 5, 0)
                };
                var normalEntry = new FormattedEntry
                {
                    Text = entry,
                    Font = new XFont("Arial", 24, XFontStyle.Bold),
                    Brush = XBrushes.Red
                };
                entries.Add(normalEntry);

                // Tạo nút xóa
                Button deleteButton = new Button
                {
                    Content = "X",
                    Width = 20,
                    Height = 20,
                    Margin = new Thickness(5, 0, 0, 0)
                };

                // Đăng ký sự kiện cho nút xóa
                deleteButton.Click += (s, args) =>
                {
                    ExperienceList.Children.Remove(entryPanel);
                };

                // Thêm TextBlock và nút xóa vào StackPanel
                entryPanel.Children.Add(entryTextBlock);
                entryPanel.Children.Add(deleteButton);

                // Thêm StackPanel vào danh sách hiển thị
                ExperienceList.Children.Add(entryPanel);

                // Xóa TextBox và nút Confirm và Cancel
                ExperienceList.Children.Remove(inputTextBox);
                ExperienceList.Children.Remove(confirmButton);
                ExperienceList.Children.Remove(cancelButton);
            }
            else
            {
                MessageBox.Show("Please enter a valid entry.");
            }
        }
    }
}
