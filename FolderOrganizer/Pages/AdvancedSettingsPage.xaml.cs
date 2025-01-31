using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Diagnostics;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Windows.UI.Core;
using System.Threading.Tasks;
using Windows.Security.Cryptography.Core;
using System.Collections.ObjectModel;

namespace FolderOrganizer
{
    /// <summary>
    /// Advanced Settings Page
    /// </summary>
    public sealed partial class AdvancedSettingsPage : Page
    {
        // List of all the category toggles in category pane
        private List<ToggleSwitch> _categoryToggleSwitches = new List<ToggleSwitch>();

        internal ObservableCollection<CategoryAndFileTypes> CategoriesCollection { get; private set; }
        internal ObservableCollection<CategoryAndFileTypes> FileTypesCollection { get; private set; }


        // Lists of all elements in a list view item in the file types pane
        private List<TextBlock> _fileTypesHeaderTextBlocks = new List<TextBlock>();
        private List<ToggleSwitch> _allowCustomFileTypesToggleSwitches = new List<ToggleSwitch>();
        private List<TextBox> _customFileTypesTextBoxes = new List<TextBox>();
        private List<Button> _applyCustomFileTypesButtons = new List<Button>();

        // Brush colors used on the page
        private static readonly SolidColorBrush SteelBlueBrush = new SolidColorBrush(Color.FromArgb(255, 70, 130, 180));
        private static readonly SolidColorBrush RedBrush = new SolidColorBrush(Color.FromArgb(255, 255, 0, 0));
        private static readonly SolidColorBrush GreenBrush = new SolidColorBrush(Color.FromArgb(255, 0, 255, 0));
        private static readonly SolidColorBrush WhiteBrush = new SolidColorBrush(Color.FromArgb(255, 255, 255, 255));

        #region Page Initialization
        public AdvancedSettingsPage()
        {
            DataContext = this;
            CategoriesCollection = new ObservableCollection<CategoryAndFileTypes>();
            FileTypesCollection = new ObservableCollection<CategoryAndFileTypes>();

            this.InitializeComponent();
            this.NavigationCacheMode = NavigationCacheMode.Enabled;

            // Register callbacks from the SettingsManager
            SettingsManager.OnCategorySelect += UpdateUIOnCategorySelect;
            SettingsManager.OnCustomCategoryAdded += UpdateUIOnCustomCategoryAdd;

            // Populate the category and file types list views
            PopulateListViews();
        }

        
        /// <summary>
        /// Initializes the list views on first launch
        /// </summary>
        private void PopulateListViews()
        {
            foreach(CategoryAndFileTypes caft in CommonCategoryToFileTypeMappings.CategoryAndFileTypesList)
            {
                CategoriesCollection.Add(caft);
                FileTypesCollection.Add(caft);
            }
            categoriesListView.ItemsSource = CategoriesCollection;
            fileTypeListView.ItemsSource = FileTypesCollection;
            
        }

        /// <summary>
        /// On Load event for the category toggle switches of the category pane
        /// Caches the category toggle switch in the _categoriesToggleSwitches list
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CategoryToggleSwitch_OnLoaded(object sender, RoutedEventArgs e)
        {
            ToggleSwitch categoryToggleSwitch = (ToggleSwitch)sender;
            _categoryToggleSwitches.Add(categoryToggleSwitch);
        }

        /// <summary>
        /// On Load event for the xaml controls in the file types list view item.
        /// Caches the control in member variable list for 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FileTypesListViewItem_OnLoaded(object sender, RoutedEventArgs e)
        {
            switch (sender)
            {
                case TextBlock textBlock:
                    _fileTypesHeaderTextBlocks.Add(textBlock);
                    break;
                case ToggleSwitch toggleSwitch:
                    _allowCustomFileTypesToggleSwitches.Add(toggleSwitch);
                    break;
                case TextBox textBox:
                    _customFileTypesTextBoxes.Add(textBox);
                    break;
                case Button button:
                    _applyCustomFileTypesButtons.Add(button);
                    break;
                default:
                    return;
                    
            }
        }
        #endregion


        /// <summary>
        /// On click method to go back to the main page
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BackButton_OnClick(object sender, RoutedEventArgs e)
        {
            Frame.GoBack();
        }

        #region Category Toggles Behaviours 

        /// <summary>
        /// Toggle event for category toggle switch. 
        /// Adds or removes associated category in settings based on toggle state
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CategoryToggleSwitch_OnToggle(object sender, RoutedEventArgs e)
        {
            ToggleSwitch toggleSwitch = (ToggleSwitch)sender;

            // Find the associated Header in the file type associations list
            TextBlock headerTextBlock = new TextBlock();
            foreach (TextBlock textBlock in _fileTypesHeaderTextBlocks)
            {
                if (toggleSwitch.Tag == textBlock.Tag)
                {
                    headerTextBlock = textBlock;
                }
            }

            // Add or remove category in settings based on toggle state and set color
            if (toggleSwitch.IsOn)
            {
                SettingsManager.Instance.AddCategoryToSelectedCategories((string)toggleSwitch.Tag);
                toggleSwitch.Foreground = SteelBlueBrush;
                headerTextBlock.Foreground = SteelBlueBrush;

                ///// DEBUG /////
                Debug.WriteLine("CATEGORY ADDED TO SELECTED CATEGORIES");
                Debug.WriteLine($"Category Added: {(string)toggleSwitch.Tag}");
                Debug.WriteLine("Categories in Settings:");
                foreach (CategoryAndFileTypes c in SettingsManager.Instance.Settings.SelectedCaftList)
                {
                    Debug.WriteLine(c.Category);
                }
                Debug.WriteLine("--------------------");
                /////////////////
            }
            else
            {
                SettingsManager.Instance.RemoveCategoryFromSelectedCategories((string)toggleSwitch.Tag);
                toggleSwitch.Foreground = WhiteBrush;
                headerTextBlock.Foreground = WhiteBrush;
                
                ///// DEBUG /////
                Debug.WriteLine("CATEGORY REMOVED FROM SELECTED CATEGORIES");
                Debug.WriteLine($"Category Removed: {(string)toggleSwitch.Tag}");
                Debug.WriteLine("Categories in Settings:");
                foreach (CategoryAndFileTypes c in SettingsManager.Instance.Settings.SelectedCaftList)
                {
                    Debug.WriteLine(c.Category);
                }
                Debug.WriteLine("--------------------");
                /////////////////
            }




        }

        /// <summary>
        /// Toggle event for the select all categories toggle switch.
        /// Toggles all categories and adds/removes associated categories in settings
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SelectAllCategories_OnToggle(object sender, RoutedEventArgs e)
        {
            ToggleSwitch selectAllToggleSwitch = (ToggleSwitch)sender;

            // Iterate through category toggles and match the toggle state to select all toggle
            foreach (ToggleSwitch categoryToggle in _categoryToggleSwitches)
            {
                categoryToggle.IsOn = selectAllToggleSwitch.IsOn;
            }

        }

        #endregion

        #region Associated File Types Behaviours

        /// <summary>
        /// Toggle event for the allow file type editing toggle.
        /// Sets the associated textbox to enabled to allow editing
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AllowFileTypeEditing_OnToggle(object sender, RoutedEventArgs e)
        {
            ToggleSwitch toggleSwitch = (ToggleSwitch)sender;

            // Get the associated text box of the panel 
            TextBox? textBox = XamlHelper.GetFirstSiblingOfType<TextBox>(toggleSwitch);

            if (textBox != null)
            {
                // Toggle the enabled status of the text box
                textBox.IsEnabled = toggleSwitch.IsOn;
            }
        }

        /// <summary>
        /// On enable event for the the custom file type text box.
        /// Sets the apply button associated with that file category to visible
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CustomFileTextBox_OnEnableChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            TextBox textBox = (TextBox)sender;

            // Find the "apply" button associated with this category
            Button? applyButton = XamlHelper.GetFirstSiblingOfType<Button>(textBox);

            // Set the enable status of the button
            if (applyButton != null)
            {
                if (textBox.IsEnabled)
                {
                    applyButton.Visibility = Visibility.Visible;
                }
                else
                {
                    applyButton.Visibility = Visibility.Collapsed;
                }
            }
        }

      
        /// <summary>
        /// Sets the associated file types of a category in settings based on user textbox input
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ApplyCustomFileTypes_OnClick(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;

            // Toggle the corresponding category on so that the category is in the selected categories

            // Get the associated text box of the panel
            TextBox? textBox = XamlHelper.GetFirstSiblingOfType<TextBox>(button);

            if (textBox == null)
            {
                return;
            }

            // The associated category is stored in the xaml tag of the textbox
            string category = (string)textBox.Tag;

            List<string> customFileTypes = ParseCustomFileTypes(textBox.Text);

            // Update the category's associated file types in settings
            if (customFileTypes.Count != 0)
            {
                
                foreach (ToggleSwitch categoryToggleSwitch in _categoryToggleSwitches)
                {
                    if ((string)categoryToggleSwitch.Tag == category)
                    {
                        if (!categoryToggleSwitch.IsOn)
                        {
                            categoryToggleSwitch.IsOn = true;
                        }
                        categoryToggleSwitch.Foreground = GreenBrush;
                    }
                }

                SettingsManager.Instance.AddCustomFileTypesToCategory(category, customFileTypes);
            }
            

            // DEBUG
            if (customFileTypes.Count != 0)
            {
                Debug.WriteLine("CUSTOM FILE TYPES ADDED");
                Debug.WriteLine($"Category: {SettingsManager.Instance.GetSelectedCategoryAndFileTypes(category).Category}");
                Debug.WriteLine($"Customs File Types: {SettingsManager.Instance.GetSelectedCategoryAndFileTypes(category).FileTypesFormatted}");
            }

        }

        #endregion

        #region Add Custom Category Behaviours

        /// <summary>
        /// On Click event to open the content dialog to add a custom file category
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void AddCustomCategoryAndFileTypes_OnClick(object sender, RoutedEventArgs e)
        {
            // Clear any previous input
            customCategoryNameTextBox.Text = "";
            customCategoryFileTypesTextBox.Text = "";
            customCategoryMessageTextBox.Text = "";
            customCategoryMessageTextBox.Visibility = Visibility.Collapsed;

            // Open the dialog
            await addCustomCategoryContentDialog.ShowAsync();
        }

        /// <summary>
        /// On click event to save a custom category and file types from the content dialog
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SaveCustomCategoryAndFileTypes_OnClick(object sender, RoutedEventArgs e)
        {
            // Get the name of the custom category
            string customCategoryName = customCategoryNameTextBox.Text;
            
            // Get the custom file types by parsing the formatted string
            List<string> customFileTypes = ParseCustomFileTypes(customCategoryFileTypesTextBox.Text);

            // Try to add the new category and file types to the settings
            SettingsResponse addCustomCategoryResponse = SettingsManager.Instance.AddCustomCategoryAndFileTypes(customCategoryName, customFileTypes);

            // If error, set the error message of the content dialog to visible and set the text to the message
            if (addCustomCategoryResponse.Success == false)
            {
                customCategoryMessageTextBox.Text = addCustomCategoryResponse.Response;
                customCategoryMessageTextBox.Visibility = Visibility.Visible;
            }
            // If success, close the content dialog
            else
            {
                // Get the newly added category and files type from the settings 
                CategoryAndFileTypes? customCaft = SettingsManager.Instance.GetCustomCategoryAndFileTypes(customCategoryName);

                if (customCaft != null)
                {
                    CategoriesCollection.Add(customCaft);
                    FileTypesCollection.Add(customCaft);
                }
                
                addCustomCategoryContentDialog.Hide();

                ///// DEBUG /////
                Debug.WriteLine("CUSTOM CATEGORY ADDED");
                Debug.WriteLine($"Category: {SettingsManager.Instance.Settings.CustomCategoryAndFileTypesList[SettingsManager.Instance.Settings.CustomCategoryAndFileTypesList.Count - 1].Category}");
                Debug.WriteLine("File Types:");
                foreach (string fileType in SettingsManager.Instance.Settings.CustomCategoryAndFileTypesList[SettingsManager.Instance.Settings.CustomCategoryAndFileTypesList.Count - 1].FileTypesList)
                {
                    Debug.WriteLine($"{fileType}");
                }
                Debug.WriteLine("--------------------");
                ///////////////
            }



        }

        /// <summary>
        /// On Click event to close the add custom category content dialog
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CloseAddCustomCategoryContentDialog_OnClick(object sender, RoutedEventArgs e)
        {
            addCustomCategoryContentDialog.Hide();
        }

        #endregion




        #region SettingsManager Callbacks

        private void UpdateUIOnCategorySelect()
        {

        }

        private void UpdateUIOnCustomCategoryAdd()
        {

        }
        #endregion

        #region Helper Functions 

        private List<string> ParseCustomFileTypes(string formattedFileTypesString)
        {
            // The output list of parsed file types
            List<string> customFileTypes = new List<string>();

            // Set iteration variables
            string? customFileType = null;
            bool inValidSequence = false;

            // Iterate through the user input one character at a time
            // - If (.) we know that we are at the start of a file type
            // - If (,) we know that we are at the end of a file type
            // - White space is ignored
            for (int i = 0; i < formattedFileTypesString.Length; i++)
            {
                if (formattedFileTypesString[i] == '.')
                {
                    // In valid sequence, begin adding characters
                    inValidSequence = true;
                    customFileType += formattedFileTypesString[i];
                }
                else if (formattedFileTypesString[i] == ',')
                {
                    // End of valid sequence
                    // Add file type to list and clear variables
                    if (customFileType != null)
                    {
                        customFileTypes.Add(customFileType);
                    }
                    inValidSequence = false;
                    customFileType = null;
                }
                else if (formattedFileTypesString[i] == ' ')
                {
                    // Ignore white space
                    continue;
                }
                else
                {
                    // Add characters to file type if following (.)
                    if (inValidSequence)
                    {
                        customFileType += formattedFileTypesString[i];
                    }
                }

                // Handle end of string case where there is no "," separator
                if (i == formattedFileTypesString.Length - 1)
                {
                    if (customFileType != null)
                    {
                        customFileTypes.Add(customFileType);
                    }

                }
            }

            return customFileTypes;
        }

        #endregion
    }


}

