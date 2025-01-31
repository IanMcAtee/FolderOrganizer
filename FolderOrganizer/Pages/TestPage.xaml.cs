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
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Windows.Media.AppBroadcasting;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace FolderOrganizer.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class TestPage : Page
    {
        internal ObservableCollection<DefaultCategoryViewModel> CategoryListViewCollection = new ObservableCollection<DefaultCategoryViewModel>();
        public TestPage()
        {
            this.InitializeComponent();
            PopulateListViews();
        }

        private void PopulateListViews()
        {
            foreach (CategoryAndFileTypes caft in SettingsManager.Instance.Settings.DefaultCaftList)
            {
                DefaultCategoryViewModel viewModel = new DefaultCategoryViewModel(caft);
                
                CategoryListViewCollection.Add(viewModel);
            }
            defaultCategoriesListView.ItemsSource = CategoryListViewCollection;

            CustomCategoryViewModel customCategoryViewModel = new CustomCategoryViewModel(new CategoryAndFileTypes("Materials", new List<string> { ".mat", ".pbr" }));
            customCategoryViewModel.Caft = new CategoryAndFileTypes("Materials", new List<string> { ".mat", ".pbr" });

            CategoryListViewCollection.Add(customCategoryViewModel);
        }

        private void SelectAllCategories_OnClick(object sender, RoutedEventArgs e)
        {
            foreach (DefaultCategoryViewModel viewModel in CategoryListViewCollection)
            {
                viewModel.IsCategorySelected = true;
            }
        }

        private void RemoveAllCategories_OnClick(object sender, RoutedEventArgs e)
        {
            foreach (DefaultCategoryViewModel viewModel in CategoryListViewCollection)
            {
                viewModel.IsCategorySelected = false;
            }
        }

        private void ExpandAllCategories_OnClick(object sender, RoutedEventArgs e)
        {
            foreach (DefaultCategoryViewModel viewModel in CategoryListViewCollection)
            {
                viewModel.SetPanelVisibility(true);
            }
        }

        private void CollapseAllCategories_OnClick(object sender, RoutedEventArgs e)
        {
            foreach (DefaultCategoryViewModel viewModel in CategoryListViewCollection)
            {
                viewModel.SetPanelVisibility(false);
            }
        }


    }



    internal class DefaultCategoryViewModel : INotifyPropertyChanged
    {
        protected const string EXPANDGLYPH = "\xE700";
        protected const string COLLAPSEGLYPH = "\xE70E";
        protected bool _allowCustomFileTypes = false;
        protected bool _isCategorySelected = false;
        protected bool _isPanelVisible = false;
        protected string _buttonGlyph = EXPANDGLYPH;
        protected bool _readyToApplyFileTypes = false;

        public event PropertyChangedEventHandler? PropertyChanged = delegate { };

        public CategoryAndFileTypes Caft { get; set; }

        // Constructor
        public DefaultCategoryViewModel(CategoryAndFileTypes caft)
        {
            Caft = caft;
        }

        public bool IsCategorySelected
        {
            get { return _isCategorySelected; }
            set
            {
                _isCategorySelected = value;
                SettingsManager.Instance.AddToSelectedCategories(Caft);
                OnPropertyChanged();
            }
        }

        public bool AllowCustomFileTypes
        {
            get { return _allowCustomFileTypes; }
            set
            {
                _allowCustomFileTypes = value;
                OnPropertyChanged();
            }
        }
        

        public bool IsPanelVisible
        {
            get { return _isPanelVisible; }
            set
            {
                _isPanelVisible = value;
                OnPropertyChanged();
            }
        }

        public string ButtonGlyph
        {
            get { return _buttonGlyph; }
            set
            {
                _buttonGlyph = value;
                OnPropertyChanged();
            }
        }

        public bool ReadyToApplyFileTypes
        {
            get { return _readyToApplyFileTypes; }
            set
            {
                _readyToApplyFileTypes = value;
                OnPropertyChanged();
            }
        }

        public void SetPanelVisibility(bool visible)
        {
            IsPanelVisible = visible;

            if (visible)
            {
                ButtonGlyph = COLLAPSEGLYPH;
            }
            else
            {
                ButtonGlyph = EXPANDGLYPH;
            }
        }

        public void TogglePanelVisibility()
        {
            SetPanelVisibility(!IsPanelVisible);
        }

        public void OnFileTypeTextChanged()
        {
            ReadyToApplyFileTypes = true;
        }


        public void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            // Raise the PropertyChanged event, passing the name of the property whose value has changed.
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        
    }

    internal class CustomCategoryViewModel : DefaultCategoryViewModel
    {
        //private string _customCategoryName;

        // Constructor
        public CustomCategoryViewModel(CategoryAndFileTypes caft) : base(caft)
        {
            CustomCategoryName = Caft.Category;
        }

        

        // Expose a reference to base class in order to bind base methods in xaml
        public DefaultCategoryViewModel BaseInstance { get { return this; } }


        public string CustomCategoryName
        {
            get { return Caft.Category; }
            set
            {
                Caft.Category = value;
                OnPropertyChanged();
            }
        }


        
    }

    internal class CategoryDataTemplateSelector : DataTemplateSelector
    {
        public DataTemplate? DefaultCategoryTemplate { get; set; }
        public DataTemplate? CustomCategoryTemplate { get; set; }

        protected override DataTemplate? SelectTemplateCore(object item)
        {
            if (item.GetType() == typeof(CustomCategoryViewModel))
            {
                return CustomCategoryTemplate;
            }
            else
            {
                return DefaultCategoryTemplate;
            }
        }
    }

    internal class BooleanToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is bool boolValue)
            {
                return boolValue ? Visibility.Visible : Visibility.Collapsed;
            }
            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            if (value is Visibility visibility)
            {
                return visibility == Visibility.Visible;
            }
            return false;
        }
    }

}
