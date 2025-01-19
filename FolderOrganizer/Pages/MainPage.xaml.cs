using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Microsoft.UI.Xaml.Documents;
using System.Diagnostics;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace FolderOrganizer
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
            SetDefaultSettings();
        }

        private void SetDefaultSettings()
        {
            foreach (var child in commonFileTypeCheckBoxPanel.Children)
            {
                if (child is CheckBox)
                {
                    CheckBox checkBox = (CheckBox)child;
                    checkBox.IsChecked = true;
                }
            }
        }

        private async void FolderSelect_OnClck(object sender, RoutedEventArgs e)
        {
           
            await FolderOrganizerHelper.PickRootFolder(App.MainWindow);
            if (FolderOrganizerHelper.SelectedFolder != null)
            {
                folderTextBlock.Text = FolderOrganizerHelper.SelectedFolder.Name;
            }
            else
            {
                folderTextBlock.Text = "Select Folder";
            }

            FormatFolderDetailsText();
            folderDetailsTextBlock.Visibility = Visibility.Visible;
            organizeButton.Visibility = Visibility.Visible;
        }

        private async void FormatFolderDetailsText()
        {
            if (FolderOrganizerHelper.SelectedFolder != null)
            {
                Dictionary<string, int> categoryToNumberMap = await FolderOrganizerHelper.GetFolderFileDetails();

                int totalNumFiles = 0;

                // Optimization point
                foreach (int amount in categoryToNumberMap.Values)
                {
                    totalNumFiles += amount;
                }

                Bold numFilesTextBold = new Bold();
                numFilesTextBold.Inlines.Add(new Run { Text = "Total Number of Files: " });
                folderDetailsTextBlock.Inlines.Add(numFilesTextBold);
                folderDetailsTextBlock.Inlines.Add(new Run { Text = totalNumFiles.ToString() });
                folderDetailsTextBlock.Inlines.Add(new LineBreak());

                foreach (KeyValuePair<string, int> categoryAmountPair in categoryToNumberMap.OrderBy(key => key.Key))
                {
                    Bold categoryTextBold = new Bold();
                    categoryTextBold.Inlines.Add(new Run { Text = categoryAmountPair.Key + ": " });
                    folderDetailsTextBlock.Inlines.Add(categoryTextBold);
                    folderDetailsTextBlock.Inlines.Add(new Run { Text = categoryAmountPair.Value.ToString() });
                    folderDetailsTextBlock.Inlines.Add(new LineBreak());
                }

            }
        }

        private async void OrganizeFolder_OnClick(object sender, RoutedEventArgs e)
        {
            await FolderOrganizerHelper.OrganizeFolder();
        }

        private void SplitViewButton_OnClick(object sender, RoutedEventArgs e)
        {
            splitView.IsPaneOpen = !splitView.IsPaneOpen;
        }


        private void SetFileCategory_OnCheck(object sender, RoutedEventArgs e)
        {
            CheckBox categoryCheckBox = (CheckBox)sender;
            string? category = categoryCheckBox.Tag.ToString();
            if (category != null)
            {
                //SettingsManager.Instance.SetFileCategory(category);
            }
            foreach (string cat in SettingsManager.Instance.Settings.CategoryToFileTypeMap.Keys)
            {
                Debug.WriteLine(cat);
            }
        }

        private void UnsetFileCategory_OffCheck(object sender, RoutedEventArgs e)
        {
            CheckBox categoryCheckBox = (CheckBox)sender;
            string? category = categoryCheckBox.Tag.ToString();
            if (category != null)
            {
                SettingsManager.Instance.RemoveCategoryFromSelectedCategories(category);
            }
            foreach (string cat in SettingsManager.Instance.Settings.CategoryToFileTypeMap.Keys)
            {
                Debug.WriteLine(cat);
            }
        }


        private void UnpackSubfolders_OnToggle(object sender, RoutedEventArgs e)
        {
            ToggleSwitch unpackSubFoldersToggle = (ToggleSwitch)sender;
            SettingsManager.Instance.SetUnpackSubFolders(unpackSubFoldersToggle.IsOn);
            Debug.WriteLine(SettingsManager.Instance.Settings.UnpackSubfolders);
        }

        

        private void AdvancedSettings_OnClick(object sender, RoutedEventArgs e)
        {
            
            if (Frame.CanGoForward)
            {
                Frame.GoForward();
            }
            else
            {
                Frame.Navigate(typeof(AdvancedSettingsPage));
            }

        }
    }


}
