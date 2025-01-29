using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace FolderOrganizer.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class TestPage : Page
    {
        internal List<DefaultCategoryViewModel> DefaultCategoryListViewItems = new List<DefaultCategoryViewModel>();
        public TestPage()
        {
            
            
            this.InitializeComponent();

            foreach (CategoryAndFileTypes caft in CommonCategoryToFileTypeMappings.CategoryAndFileTypesList)
            {
                DefaultCategoryViewModel viewModel = new DefaultCategoryViewModel();
                viewModel.Caft = caft;
                DefaultCategoryListViewItems.Add(viewModel);
            }
            defaultCategoriesListView.ItemsSource = DefaultCategoryListViewItems;
        }

        private void DefaultCategoryExpand_OnClick(object sender, RoutedEventArgs e)
        {
            Button expandButton = (Button)sender;
            Panel? collapsiblePanel = XamlHelper.GetFirstSiblingOfType<Panel>(expandButton);
            if (collapsiblePanel != null)
            {
                collapsiblePanel.Visibility = Visibility.Visible;
                expandButton.Visibility = Visibility.Collapsed;
            }
        }

        private void DefaultCategoryCollapse_OnClick(object sender, RoutedEventArgs e)
        {
            Button collapseButton = (Button)sender;
            Panel parentPanel = (Panel)collapseButton.Parent;
            parentPanel.Visibility = Visibility.Collapsed;
            Button? expandButton = XamlHelper.GetFirstSiblingOfType<Button>(parentPanel);
            if (expandButton != null)
            {
                expandButton.Visibility = Visibility.Visible;
                
            }
        }
    }

    

    internal class DefaultCategoryViewModel
    {
        public CategoryAndFileTypes? Caft { get; set; }
        public ToggleSwitch? CategorySelectToggleSwitch { get; set; }
        public TextBox? FileTypesTextBox { get; set; }
        public Button? ApplyFileTypesButton { get; set; }
        public ToggleSwitch? UseCustomFileTypesToggleSwitch { get; set; }

        public void Expand_OnClick(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine("Expand Clicked");
            
        }

        public void UseCustomFileTypes_Toggle(object sender, RoutedEventArgs e)
        {
            ToggleSwitch toggleSwitch = (ToggleSwitch)sender;
        
        }

        /////I THINK AN ONLOADED METHOD TO POPULATE PROPERTIES IS THE WAY TO GO
    }
}
