using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Media.AppBroadcasting;
using Windows.UI.Input.Spatial;

namespace FolderOrganizer
{
    internal class SettingsManager
    {
        // IMPLEMENT THREAD-SAFE SINGLETON PATTERN
        private SettingsManager() { }
        private static SettingsManager? _instance = null;

        // Lock object used to synchronize threads during first access to singleton
        private static readonly object _lock = new object();

        // Check with double locking
        public static SettingsManager Instance
        {
            get
            {
                // All threads will get here
                if (_instance == null)
                {
                    // Only first thread will enter lock
                    lock (_lock)
                    {
                        // First thread will enter and set the singleton reference
                        if (_instance == null)
                        {
                            _instance = new SettingsManager();
                        }
                    }
                }
                return _instance;
            }
        }

        // Internal variables exposed to the rest of the namespace
        internal OrganizationSettings Settings = new OrganizationSettings();
        
        // Callbacks 
        internal static event Action? OnCategorySelect = null;
        internal static event Action? OnCustomCategoryAdded = null;

        internal void SetUnpackSubFolders(bool set)
        {
            Settings.UnpackSubfolders = set;
        }


        internal void AddFileType(string category, string extension)
        {
            if (Settings.CategoryToFileTypeMap.Keys.Contains(category))
            {
                Settings.CategoryToFileTypeMap[category].Add(extension);
            }
        }

        /// <summary>
        /// Adds a category and its associated file types to the selected category and file types list
        /// </summary>
        /// <param name="categoryName"></param>
        /// <returns></returns>
        internal void AddCategoryToSelectedCategories(string categoryName)
        {
            // Check if the category is already selected, if so, return 
            if (IsCategorySelected(categoryName))
            {
                return;
            }

            // Check if category is a common category, if so, add to selected categories and invoke callback
            CategoryAndFileTypes? caft = CommonCategoryToFileTypeMappings.GetCategoryAndFileTypes(categoryName);
            if (caft != null)
            {
                Settings.SelectedCategoryAndFileTypesList.Add(caft);
                OnCategorySelect?.Invoke();
                return;
            }

            // Check if category is a custom category, if so, add to selected categories and invoke callback
            caft = GetCustomCategoryAndFileTypes(categoryName);
            if (caft != null)
            {
                Settings.SelectedCategoryAndFileTypesList.Add(caft);
                OnCategorySelect?.Invoke();
            }
        }


        /// <summary>
        /// Removes a category from the settings if present
        /// </summary>
        /// <param name="categoryName"></param>
        internal void RemoveCategoryFromSelectedCategories(string categoryName)
        {
            foreach (CategoryAndFileTypes caft in Settings.SelectedCategoryAndFileTypesList)
            {
                if (caft.Category == categoryName)
                {
                    Settings.SelectedCategoryAndFileTypesList.Remove(caft);
                    break;
                }
            }
        }

        /// <summary>
        /// Gets a category and file types from settings if it exists
        /// </summary>
        /// <param name="categoryName"></param>
        /// <returns></returns>
        internal CategoryAndFileTypes? GetSelectedCategoryAndFileTypes(string categoryName)
        {
            foreach (CategoryAndFileTypes caft in Settings.SelectedCategoryAndFileTypesList)
            {
                if (caft.Category == categoryName)
                {
                    return caft;
                }
            }
            return null;
        }

        internal CategoryAndFileTypes? GetCustomCategoryAndFileTypes(string categoryName)
        {
            foreach (CategoryAndFileTypes caft in Settings.CustomCategoryAndFileTypesList)
            {
                if (caft.Category == categoryName)
                {
                    return caft;
                }
            }
            return null;
        }


        /// <summary>
        /// Determines if a category is already present in setting's slected category list
        /// </summary>
        /// <param name="categoryName"></param>
        /// <returns></returns>
        internal bool IsCategorySelected(string categoryName)
        {
            foreach (CategoryAndFileTypes caft in Settings.SelectedCategoryAndFileTypesList)
            {
                if (caft.Category == categoryName)
                {
                    return true;
                }
            }

            return false;
        }

        internal bool IsCustomCategory(string categoryName)
        {
            foreach (CategoryAndFileTypes caft in Settings.CustomCategoryAndFileTypesList)
            {
                if (caft.Category == categoryName)
                {
                    return true;
                }
            }

            return false;
        }


        /// <summary>
        /// Adds custom file types to a category if present in settings
        /// </summary>
        /// <param name="categoryName"></param>
        /// <param name="customFileTypesList"></param>
        internal void AddCustomFileTypesToCategory(string categoryName, List<string> customFileTypesList)
        {
            if (customFileTypesList.Count == 0 || customFileTypesList == null)
            {
                return;
            }

            foreach (CategoryAndFileTypes caft in Settings.SelectedCategoryAndFileTypesList)
            {
                if (caft.Category == categoryName)
                {
                    caft.FileTypesList = customFileTypesList;
                }
            }
        }

        /// <summary>
        /// Adds a custom file category to the settings manager custom category and file types list
        /// </summary>
        /// <param name="categoryName"></param>
        /// <param name="customFileTypes"></param>
        /// <returns></returns>
        internal SettingsResponse AddCustomCategoryAndFileTypes(string categoryName, List<string> customFileTypes)
        {
            // Check if category name is not empty or null
            if (categoryName == null || categoryName.Length == 0)
            {
                return new SettingsResponse(false, "No category name provided");
            }

            // Check if category already a custom category or if it is already a common category
            if (IsCustomCategory(categoryName) || CommonCategoryToFileTypeMappings.IsCategoryInCommonCategories(categoryName))
            {
                return new SettingsResponse(false, $" The category, {categoryName}, is already defined");
            }

            // Check if there are custom file types associated with the category
            if (customFileTypes.Count == 0)
            {
                return new SettingsResponse(false, $"No valid file types provided for the {categoryName} category");
            }

            // Check if a file type is already present in another category
            foreach (string customFileType in customFileTypes)
            {
                // Search the common categories
                foreach (CategoryAndFileTypes caft in CommonCategoryToFileTypeMappings.CategoryAndFileTypesList)
                {
                    if (caft.FileTypesList.Contains(customFileType))
                    {
                        return new SettingsResponse(false, $"File type, {customFileType}, already defined in {caft.Category} category");
                    }
                }
                // Search the custom category
                foreach (CategoryAndFileTypes caft in Settings.CustomCategoryAndFileTypesList)
                {
                    if (caft.FileTypesList.Contains(customFileType))
                    {
                        return new SettingsResponse(false, $"File type, {customFileType}, already defined in {caft.Category} custom category");
                    }
                }
            }

            // Instantiate a new custom category and file type
            CategoryAndFileTypes customCaft = new CategoryAndFileTypes(categoryName, customFileTypes);

            // Add it to the custom categories and file types list
            Settings.CustomCategoryAndFileTypesList.Add(customCaft);

            // Invoke any callbacks
            OnCustomCategoryAdded?.Invoke();

            return new SettingsResponse(true, $"The category, {categoryName}, and its filetypes were successfully added");
            
        }

    }

    internal class OrganizationSettings
    {
        //DEPRECIATED: USE SELECTEDCATEGORYFILETYPESLIST INSTEAD
        public Dictionary<string, List<string>> CategoryToFileTypeMap { get; private set; } = new Dictionary<string, List<string>>();
        
        public List<CategoryAndFileTypes> SelectedCategoryAndFileTypesList { get; private set; } = new List<CategoryAndFileTypes>();
        public List<CategoryAndFileTypes> CustomCategoryAndFileTypesList { get; private set; } = new List<CategoryAndFileTypes>();
        public bool UnpackSubfolders = false;

    }

    internal class SettingsResponse
    {
        public bool Success { get; private set; } = false;
        public string? Response { get; private set; } = null;

        public SettingsResponse(bool success, string response = "")
        {
            Success = success;
            Response = response;
        }
    }
}




