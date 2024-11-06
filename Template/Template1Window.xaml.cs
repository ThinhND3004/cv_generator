﻿using Microsoft.Win32;
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

        private void LoadAvaterImage (string avatarPath)
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
            AvatarBorder.BorderBrush = Brushes.Green;
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

        private void Section_MouseEnter(object sender, MouseEventArgs e)
        {
            // Hiển thị nút "Add Section" khi di chuột vào vùng Grid
            if (sender is Grid grid)
            {
                var addButton = grid.FindName("AddSectionButton") as Button;
                if (addButton != null)
                {
                    addButton.Visibility = Visibility.Visible;
                }
            }
        }

        private void Section_MouseLeave(object sender, MouseEventArgs e)
        {
            // Ẩn nút "Add Section" khi chuột rời khỏi vùng Grid hoàn toàn
            if (sender is Grid grid)
            {
                var addButton = grid.FindName("AddSectionButton") as Button;
                if (addButton != null && !addButton.IsMouseOver)
                {
                    addButton.Visibility = Visibility.Collapsed;
                }
            }
        }

        private void AddSectionButton_Click(object sender, RoutedEventArgs e)
        {
            // Thêm một section mới dưới section hiện tại
            if (sender is Button addButton)
            {
                var parentStackPanel = addButton.Parent as StackPanel;
                if (parentStackPanel != null)
                {
                    var newSection = CreateNewSection();
                    System.Windows.MessageBox.Show("Add new section here");
                    var mainContainer = ContentScrollViewer.Content as StackPanel;
                    if (mainContainer != null)
                    {
                        int index = mainContainer.Children.IndexOf(parentStackPanel);
                        mainContainer.Children.Insert(index + 1, newSection);
                    }
                }
            }
        }

        private StackPanel CreateNewSection()
        {
            var newSection = new StackPanel
            {
                Margin = new Thickness(15, 20, 15, 10)
            };

            var border = new Border
            {
                BorderBrush = System.Windows.Media.Brushes.Green,
                BorderThickness = new Thickness(0, 0, 0, 1),
                Margin = new Thickness(0, 0, 0, 5)
            };

            var textBlock = new TextBlock
            {
                Text = "NEW SECTION",
                FontSize = 16,
                FontWeight = FontWeights.Bold,
                Foreground = System.Windows.Media.Brushes.Green,
                Margin = new Thickness(0, 20, 0, 0)
            };

            border.Child = textBlock;
            newSection.Children.Add(border);

            // Tạo nút "Add Section" cho section mới
            var addSectionButton = new Button
            {
                Content = "+ Add Section",
                Width = 120,
                Height = 35,
                HorizontalAlignment = HorizontalAlignment.Center,
                Margin = new Thickness(0, 10, 0, 0),
                Background = System.Windows.Media.Brushes.Green,
                Foreground = System.Windows.Media.Brushes.White,
                FontSize = 14,
                FontWeight = FontWeights.Bold,
                Visibility = Visibility.Collapsed
            };
            addSectionButton.Click += AddSectionButton_Click;
            newSection.Children.Add(addSectionButton);

            return newSection;
        }

        //---------------------------------------------------------------
        //Edit text block
        private void DisplayTextBlock_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2)
            {
                var textBlock = sender as TextBlock;
                var parent = textBlock?.TemplatedParent as ContentControl;
                var editTextBox = (TextBox)parent.Template.FindName("EditTextBox", parent);

                if (editTextBox != null)
                {
                    editTextBox.Visibility = Visibility.Visible;
                    textBlock.Visibility = Visibility.Collapsed;
                    editTextBox.Focus();
                    editTextBox.SelectAll();
                }
            }
        }

        private void EditTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            var textBox = sender as TextBox;
            var parent = textBox?.TemplatedParent as ContentControl;
            var displayTextBlock = (TextBlock)parent.Template.FindName("DisplayTextBlock", parent);

            if (displayTextBlock != null)
            {
                displayTextBlock.Visibility = Visibility.Visible;
                textBox.Visibility = Visibility.Collapsed;
            }
        }

        private void EditTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                var textBox = sender as TextBox;
                var parent = textBox?.TemplatedParent as ContentControl;
                var displayTextBlock = (TextBlock)parent.Template.FindName("DisplayTextBlock", parent);

                if (displayTextBlock != null)
                {
                    displayTextBlock.Visibility = Visibility.Visible;
                    textBox.Visibility = Visibility.Collapsed;
                }
            }
        }


    }
}
