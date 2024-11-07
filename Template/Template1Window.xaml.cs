using Microsoft.Win32;
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

        private void ExportToPdf_Click(object sender, RoutedEventArgs e)
        {
            // Implement the logic to export to PDF
            System.Windows.MessageBox.Show("Exporting to PDF...");
            ExportPopup.IsOpen = false;
        }

        private void ExportToWord_Click(object sender, RoutedEventArgs e)
        {
            // Implement the logic to export to Word
            System.Windows.MessageBox.Show("Exporting to Word...");
            ExportPopup.IsOpen = false;
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

        //---------------------------------------------------------------        
    }
}
