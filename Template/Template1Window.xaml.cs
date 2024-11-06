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
        private void Section_MouseEnter(object sender, MouseEventArgs e)
        {
            // Hiển thị nút "Add Section" khi di chuột vào vùng Grid
            if (sender is Grid grid)
            {
                var addButton = FindChildByUid<Button>(grid, "AddSectionButton");
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
                var addButton = FindChildByUid<Button>(grid, "AddSectionButton");
                if (addButton != null && !addButton.IsMouseOver)
                {
                    addButton.Visibility = Visibility.Collapsed;
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
                            System.Windows.MessageBox.Show("Add new section here");
                            
                                int index = BodyCVStackPanel.Children.IndexOf(parentStackPanel);
                            BodyCVStackPanel.Children.Insert(index + 1, newSection);
                                System.Windows.MessageBox.Show("Index: " + index);
                        }
                        else
                        {
                            System.Windows.MessageBox.Show("New Section create fail");
                        }
                    }
                    else
                    {
                        System.Windows.MessageBox.Show("StackPanel is not a Exist");
                    }
                }
                else
                {
                    System.Windows.MessageBox.Show("Gird is not a Exist");
                }
            }
            else
            {
                System.Windows.MessageBox.Show("Sender is not a button");
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
