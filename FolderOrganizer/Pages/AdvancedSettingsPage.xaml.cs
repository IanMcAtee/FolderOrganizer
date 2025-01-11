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

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace FolderOrganizer
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class AdvancedSettingsPage : Page
    {
        

        public AdvancedSettingsPage()
        {
            this.InitializeComponent();
            this.NavigationCacheMode = NavigationCacheMode.Enabled;
            
            PopulateCategoryListView();
            PopulateFileTypeListView();
        }

        
        private void PopulateCategoryListView()
        {
            categoriesListView.ItemsSource = CategoryToFileTypeMappings.CategoryAndFileTypesList;
        }

        private void PopulateFileTypeListView()
        {
            ListView listView = fileTypeListView;
            List<CategoryAndFileTypesItem> fileTypeList = new List<CategoryAndFileTypesItem>();

            CategoryAndFileTypesItem? fileTypeListItem = null;
            string fileTypesFormatted = "";

            foreach (KeyValuePair<string, List<string>> categoryFileTypeMap in CategoryToFileTypeMappings.Map)
            {
                // Populate the first file type without the "," separator
                fileTypesFormatted += categoryFileTypeMap.Value[0];

                // Iterate through remaining file types separating each with ","
                for (int i = 1; i < categoryFileTypeMap.Value.Count; i++)
                {
                    fileTypesFormatted += ", " + categoryFileTypeMap.Value[i];
                }

                // Add a FileTypeList item with matching category and file types to the list
                fileTypeListItem = new CategoryAndFileTypesItem(categoryFileTypeMap.Key, fileTypesFormatted);
                fileTypeList.Add(fileTypeListItem);

                // Reset the file types string
                fileTypesFormatted = "";
            }

            listView.ItemsSource = fileTypeList;
        }

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
                
                // Check if the select all toggle is on, if so, toggle it off
                if (selectAllCategoriesToggleSwitch.IsOn)
                {
                    selectAllCategoriesToggleSwitch.IsOn = false;
                }
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

            // Get all the children toggle switches in the categories list view
            List<ToggleSwitch> categoryToggleSwitches = XamlHelper.GetAllChildrenInObject<ToggleSwitch>(categoriesListView);
            
            // Iterate through category toggles and match the toggle state to select all toggle
            if (selectAllToggleSwitch.IsOn)
            {
                foreach (ToggleSwitch categoryToggle in categoryToggleSwitches)
                {
                    categoryToggle.IsOn = selectAllToggleSwitch.IsOn;
                }
            }
            
        }

        private void AllowFileTypeEditing_OnToggle(object sender, RoutedEventArgs e)
        {
            ToggleSwitch toggleSwitch = (ToggleSwitch)sender;
            TextBox textBox = new TextBox();
            StackPanel parentPanel = new StackPanel();

            try
            {
                parentPanel = (StackPanel)toggleSwitch.Parent;
            }
            catch
            {
                return;
            }

            foreach (object childObject in parentPanel.Children)
            {
                if (childObject is TextBox)
                {
                    textBox = (TextBox)childObject;
                }
            }

            textBox.IsEnabled = toggleSwitch.IsOn;
        }

      

        private void ApplyCustomFileTypes_OnClick(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            TextBox? textBox = XamlHelper.GetFirstSiblingInPanelOfType<TextBox>(button);

            if (textBox == null)
            {
                return;
            }

            string category = (string)textBox.Tag;
            List<string> customFileTypes = new List<string>();
            string? customFileType = null;
            bool inValidSequence = false;
            
            for (int i = 0; i < textBox.Text.Length; i++)
            {
                if (textBox.Text[i] == '.')
                {
                    inValidSequence = true;
                    customFileType += textBox.Text[i];
                }
                else if (textBox.Text[i] == ',')
                {
                    if (customFileType != null)
                    {
                        customFileTypes.Add(customFileType);
                    }
                    inValidSequence = false;
                    customFileType = null;
                }
                else if (textBox.Text[i] == ' ')
                {
                    continue;
                }
                else
                {
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
            SettingsManager.Instance.Settings.CategoryToFileTypeMap[category] = customFileTypes;

            Debug.WriteLine($"Custom File Type Added In Catefory {SettingsManager.Instance.Settings.CategoryToFileTypeMap.ContainsKey(category)}");
            Debug.WriteLine($"Number of Extensions: {SettingsManager.Instance.Settings.CategoryToFileTypeMap[category].Count}");
            foreach (string s in SettingsManager.Instance.Settings.CategoryToFileTypeMap[category])
            {
                Debug.WriteLine(s);
            }

        }
    }


}

internal class CategoryAndFileTypesItem
{
    public string Category { get; private set; }
    public string AssociatedFileTypes { get; private set; }

    public CategoryAndFileTypesItem(string category, string associatedFileTypes)
    {
        Category = category;
        AssociatedFileTypes = associatedFileTypes;
    }
}
