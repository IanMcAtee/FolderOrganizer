using System;
using System.Collections.Generic;
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

        internal OrganizationSettings Settings = new OrganizationSettings();

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
        /// Adds a category from the CategoryToFileTypeMappings to the settings if not present
        /// </summary>
        /// <param name="categoryName"></param>
        internal void AddCommonFileCategory(string categoryName)
        {
            // Check if category already in settings
            if (IsCategoryInSettings(categoryName))
            {
                return;
            }

            // Retrieve the category and file type from the common mappings
            CategoryAndFileTypes? caft = CommonCategoryToFileTypeMappings.GetCategoryAndFileTypes(categoryName);

            // Add category if found
            if (caft != null)
            {
                Settings.SelectedCategoryFileTypesList.Add(caft);
            }
        }

        /// <summary>
        /// Removes a category from the settings if present
        /// </summary>
        /// <param name="categoryName"></param>
        internal void RemoveFileCategory(string categoryName)
        {
            if (!IsCategoryInSettings(categoryName))
            {
                return;
            }
            foreach (CategoryAndFileTypes caft in Settings.SelectedCategoryFileTypesList)
            {
                if (caft.Category == categoryName)
                {
                    Settings.SelectedCategoryFileTypesList.Remove(caft);
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
            foreach (CategoryAndFileTypes caft in Settings.SelectedCategoryFileTypesList)
            {
                if (caft.Category == categoryName)
                {
                    return caft;
                }
            }
            return null;
        }


        /// <summary>
        /// Determines if a category is already present in setting's category list
        /// </summary>
        /// <param name="category"></param>
        /// <returns></returns>
        internal bool IsCategoryInSettings(string category)
        {
            foreach (CategoryAndFileTypes caft in Settings.SelectedCategoryFileTypesList)
            {
                if (caft.Category == category)
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

            foreach (CategoryAndFileTypes caft in Settings.SelectedCategoryFileTypesList)
            {
                if (caft.Category == categoryName)
                {
                    caft.FileTypesList = customFileTypesList;
                }
            }
        }

        internal SettingsResponse AddCustomCategoryAndFileTypes(string categoryName, List<string> customFileTypes)
        {
            // Check if category already in settings or if it is already a common category
            if (IsCategoryInSettings(categoryName) || CommonCategoryToFileTypeMappings.IsCategoryInCommonCategories(categoryName))
            {
                return new SettingsResponse(false, $" The category, {categoryName}, is already defined");
            }

            // Check if there are custom file types associated with the category
            if (customFileTypes.Count == 0)
            {
                return new SettingsResponse(false, $"No valid file types provided for the category, {categoryName}");
            }

            CategoryAndFileTypes customCaft = new CategoryAndFileTypes(categoryName, customFileTypes);

            Settings.SelectedCategoryFileTypesList.Add(customCaft);

            return new SettingsResponse(true, $"The category, {categoryName}, and its filetypes were successfully added");
            
        }

    }

    internal class OrganizationSettings
    {
        //DEPRECIATED: USE SELECTEDCATEGORYFILETYPESLIST INSTEAD
        public Dictionary<string, List<string>> CategoryToFileTypeMap { get; private set; } = new Dictionary<string, List<string>>();
        
        public List<CategoryAndFileTypes> SelectedCategoryFileTypesList { get; private set; } = new List<CategoryAndFileTypes>();
        public bool UnpackSubfolders = false;

    }

    internal class SettingsResponse
    {
        public bool Success { get; private set; } = false;
        public string? Response { get; private set; } = null;

        public SettingsResponse(bool success, string response)
        {
            Success = success;
            Response = response;
        }
    }
}




