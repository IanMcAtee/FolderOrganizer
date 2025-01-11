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
    public sealed partial class FileTypeSettingsPage : Page
    {
        public FileTypeSettingsPage()
        {
            this.InitializeComponent();

            PopulateFileTypeListView();
        }

        private void PopulateFileTypeListView()
        {
            ListView listView = fileTypeListView;
            List<FileTypeListItem> fileTypeList = new List<FileTypeListItem>();
            
            FileTypeListItem? fileTypeListItem = null;
            string fileTypesFormatted = "";

            foreach (KeyValuePair<string,List<string>> categoryFileTypeMap in CategoryToFileTypeMappings.Map)
            {   
                // Populate the first file type without the "," separator
                fileTypesFormatted += categoryFileTypeMap.Value[0];

                // Iterate through remaining file types separating each with ","
                for (int i = 1; i < categoryFileTypeMap.Value.Count; i++) 
                {
                    fileTypesFormatted += ", " + categoryFileTypeMap.Value[i];
                }
                
                // Add a FileTypeList item with matching category and file types to the list
                fileTypeListItem = new FileTypeListItem(categoryFileTypeMap.Key, fileTypesFormatted);
                fileTypeList.Add(fileTypeListItem);

                // Reset the file types string
                fileTypesFormatted = "";
            }

            listView.ItemsSource = fileTypeList;    
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

        private void OnFileTypesTextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            string category = (string)textBox.Tag;
            List<string> customFileTypes = new List<string>();
            string customFileType = "";
            for (int i = 0; i < textBox.Text.Length; i++)
            {
                if (textBox.Text[i] == '.')
                {
                    customFileType += textBox.Text[i];
                }
                else if (textBox.Text[i] == ',')
                {
                    customFileTypes.Add(customFileType);
                    customFileType = "";
                }
                else if (textBox.Text[i] == ' ')
                {
                    continue;
                }
                else
                {
                    customFileType += textBox.Text[i];
                }

                // Handle end of string case where there is no "," separator
                if (i == textBox.Text.Length - 1)
                {
                    customFileTypes.Add(customFileType);
                }
            }
            SettingsManager.Instance.Settings.CategoryToFileTypeMap[category] = customFileTypes;
            Debug.WriteLine($"Custom File Type Added In Catefory {SettingsManager.Instance.Settings.CategoryToFileTypeMap[category]}");
            foreach (string s in SettingsManager.Instance.Settings.CategoryToFileTypeMap[category])
            {
                Debug.WriteLine(s);
            }
        }
    }


}

internal class FileTypeListItem
{
    public string Category { get; private set; }
    public string AssociatedFileTypes { get; private set; }

    public FileTypeListItem(string category, string associatedFileTypes)
    {
        Category = category;
        AssociatedFileTypes = associatedFileTypes;
    }
}
