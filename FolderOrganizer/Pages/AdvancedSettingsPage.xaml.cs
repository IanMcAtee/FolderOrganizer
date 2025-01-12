using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Diagnostics;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;

namespace FolderOrganizer
{
    /// <summary>
    /// Advanced Settings Page
    /// </summary>
    public sealed partial class AdvancedSettingsPage : Page
    {
        private List<ToggleSwitch> _categoryToggleSwitches = new List<ToggleSwitch>();

        public AdvancedSettingsPage()
        {
            this.InitializeComponent();
            this.NavigationCacheMode = NavigationCacheMode.Enabled;

            // Populate the category and file types list views
            PopulateListViews();
        }

        /// <summary>
        /// Initializes the list views on first launch
        /// </summary>
        private void PopulateListViews()
        {
            categoriesListView.ItemsSource = CategoryToFileTypeMappings.CategoryAndFileTypesList;
            fileTypeListView.ItemsSource = CategoryToFileTypeMappings.CategoryAndFileTypesList;
        }

        /// <summary>
        /// Runs after page initialization and is fully loaded
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnPageLoaded(object sender, RoutedEventArgs e)
        {
            // Get the category toggles and cache
            _categoryToggleSwitches = XamlHelper.GetAllChildrenInObjectOfType<ToggleSwitch>(categoriesListView);
        }

        /// <summary>
        /// On click method to go back to the main page
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BackButton_OnClick(object sender, RoutedEventArgs e)
        {
            Frame.GoBack();
        }

        private void CategoryToggleSwitch_OnLoaded(object sender, RoutedEventArgs e)
        {
            ToggleSwitch toggleSwitch = (ToggleSwitch)sender; 

            foreach (CategoryAndFileTypes categoryAndFileTypes in SettingsManager.Instance.Settings.SelectedCategoryFileTypesList)
            {
                if ((string)toggleSwitch.Tag == categoryAndFileTypes.Category)
                {
                    toggleSwitch.IsOn = true;
                }
            }
        }

        /// <summary>
        /// Toggle event for category toggle switch. 
        /// Adds or removes associated category in settings based on toggle state
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CategoryToggleSwitch_OnToggle(object sender, RoutedEventArgs e)
        {
            ToggleSwitch toggleSwitch = (ToggleSwitch)sender;

            // Add or remove category in settings based on toggle state
            if (toggleSwitch.IsOn)
            {
                SettingsManager.Instance.AddCommonFileCategory((string)toggleSwitch.Tag);
            }
            else
            {
                SettingsManager.Instance.RemoveFileCategory((string)toggleSwitch.Tag);
            }

            // DEBUG
            Debug.WriteLine("CATEGORY TOGGLE TOGGLED");
            Debug.WriteLine("Categories in Settings:");
            foreach (CategoryAndFileTypes c in SettingsManager.Instance.Settings.SelectedCategoryFileTypesList)
            {
                Debug.WriteLine(c.Category);
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

        /// <summary>
        /// Toggle event for the allow file type editing toggle.
        /// Sets the associated textbox to enabled to allow editing
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AllowFileTypeEditing_OnToggle(object sender, RoutedEventArgs e)
        {
            ToggleSwitch toggleSwitch = (ToggleSwitch)sender;

            // If toggled on, also set the associated category toggle on
            if (toggleSwitch.IsOn)
            {
                foreach (ToggleSwitch categoryToggleSwitch in _categoryToggleSwitches)
                {
                    if (toggleSwitch.Tag == categoryToggleSwitch.Tag)
                    {
                        categoryToggleSwitch.IsOn = true;
                    }
                }
            }

            

            // Get the associated text box of the panel 
            TextBox? textBox = XamlHelper.GetFirstSiblingInPanelOfType<TextBox>(toggleSwitch);

            if (textBox == null)
            {
                return;
            }

            // Toggle the enabled status of the text box
            textBox.IsEnabled = toggleSwitch.IsOn;
        }

      
        /// <summary>
        /// Sets the associated file types of a category in settings based on user textbox input
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ApplyCustomFileTypes_OnClick(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;

            // Get the associated text box of the panel
            TextBox? textBox = XamlHelper.GetFirstSiblingInPanelOfType<TextBox>(button);

            if (textBox == null)
            {
                return;
            }

            // The associated category is stored in the xaml tag of the textbox
            string category = (string)textBox.Tag;

            // Create a new list to hold the new custom file types
            List<string> customFileTypes = new List<string>();
            
            // Set iteration variables
            string? customFileType = null;
            bool inValidSequence = false;
            
            // Iterate through the user input one character at a time
            // - If (.) we know that we are at the start of a file type
            // - If (,) we know that we are at the end of a file type
            // - White space is ignored
            for (int i = 0; i < textBox.Text.Length; i++)
            {
                if (textBox.Text[i] == '.')
                {
                    // In valid sequence, begin adding characters
                    inValidSequence = true;
                    customFileType += textBox.Text[i];
                }
                else if (textBox.Text[i] == ',')
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
                else if (textBox.Text[i] == ' ')
                {
                    // Ignore white space
                    continue;
                }
                else
                {
                    // Add characters to file type if following (.)
                    if (inValidSequence)
                    {
                        customFileType += textBox.Text[i];
                    }
                }

                // Handle end of string case where there is no "," separator
                if (i == textBox.Text.Length - 1)
                {
                    if (customFileType != null)
                    {
                        customFileTypes.Add(customFileType);
                    }
                    
                }
            }

            // Update the category's associated file types in settings
            SettingsManager.Instance.AddCustomFileTypesToCategory(category, customFileTypes);

            // DEBUG
            Debug.WriteLine("CUSTOM FILE TYPES ADDED");
            Debug.WriteLine($"Category: {SettingsManager.Instance.GetSelectedCategoryAndFileTypes(category).Category}");
            Debug.WriteLine($"Customs File Types: {SettingsManager.Instance.GetSelectedCategoryAndFileTypes(category).FileTypesFormatted}");
            

        }
    }
}

