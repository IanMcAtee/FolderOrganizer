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

        internal void SetFileCategory(string categoryToAdd)
        { 
            // BUG: Check if key already exists prior to adding
            foreach (KeyValuePair<string, List<string>> mapping in CategoryToFileTypeMappings.Map)
            {
                if (mapping.Key == categoryToAdd)
                {
                    Settings.CategoryToFileTypeMap.Add(mapping.Key, mapping.Value);
                }
            }
        }

        //internal void RemoveFileCategory(string categoryToRemove)
        //{
        //    if (Settings.CategoryToFileTypeMap.Count == 0)
        //    {
        //        return;
        //    }
        //    foreach (string category in Settings.CategoryToFileTypeMap.Keys)
        //    {
        //        if (category == categoryToRemove)
        //        {
        //            Settings.CategoryToFileTypeMap.Remove(category);
        //        }
        //    }
        //}

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
            CategoryAndFileTypes? categoryAndFileTypes = CategoryToFileTypeMappings.GetCategoryAndFileTypes(categoryName);
            
            // Add category if found
            if (categoryAndFileTypes != null)
            {
                Settings.SelectedCategoryFileTypesList.Add(categoryAndFileTypes);   
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
        /// Determines if a category is already present in setting's category list
        /// </summary>
        /// <param name="category"></param>
        /// <returns></returns>
        internal bool IsCategoryInSettings(string category)
        {
            foreach (CategoryAndFileTypes categoryAndFileTypes in Settings.SelectedCategoryFileTypesList)
            {
                if (categoryAndFileTypes.Category == category)
                {
                    return true;
                }
            }

            return false;
        }

    }

    internal class OrganizationSettings
    {
        public Dictionary<string, List<string>> CategoryToFileTypeMap { get; private set; } = new Dictionary<string, List<string>>();
        public List<CategoryAndFileTypes> SelectedCategoryFileTypesList { get; private set; } = new List<CategoryAndFileTypes>();
        public bool UnpackSubfolders = false;

    }
}
