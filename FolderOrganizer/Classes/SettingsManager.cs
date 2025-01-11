using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Media.AppBroadcasting;

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

        internal void RemoveFileCategory(string categoryToRemove)
        {
            if (Settings.CategoryToFileTypeMap.Count == 0)
            {
                return;
            }
            foreach (string category in Settings.CategoryToFileTypeMap.Keys)
            {
                if (category == categoryToRemove)
                {
                    Settings.CategoryToFileTypeMap.Remove(category);
                }
            }
        }

        internal void AddFileType(string category, string extension)
        {
            if (Settings.CategoryToFileTypeMap.Keys.Contains(category))
            {
                Settings.CategoryToFileTypeMap[category].Add(extension);
            }
        }

    }

    internal class OrganizationSettings
    {
        public Dictionary<string, List<string>> CategoryToFileTypeMap { get; private set; } = new Dictionary<string, List<string>>();
        public bool UnpackSubfolders = false;

    }
}
