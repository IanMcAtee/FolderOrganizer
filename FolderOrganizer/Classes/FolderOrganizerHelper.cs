using Microsoft.UI.Xaml;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Pickers;

namespace FolderOrganizer
{
    internal static class FolderOrganizerHelper
    {
        public static StorageFolder? SelectedFolder { get; private set; } = null;

        /// <summary>
        /// Method to allow a user to pick the root folder to organize
        /// </summary>
        /// <param name="mainWindow"></param>
        /// <returns></returns>
        internal static async Task PickRootFolder(Window mainWindow)
        {
            // Tutorial on using File/Folder Picker with desktop apps: https://github.com/microsoft/WindowsAppSDK/issues/1188
            FolderPicker folderPicker = new FolderPicker();

            // Get the current window's HWND by passing in the Window object
            var hwnd = WinRT.Interop.WindowNative.GetWindowHandle(mainWindow);

            // Associate the HWND with the file picker
            WinRT.Interop.InitializeWithWindow.Initialize(folderPicker, hwnd);

            // Use file picker like normal
            folderPicker.SuggestedStartLocation = PickerLocationId.Desktop;
            folderPicker.FileTypeFilter.Add("*");

            // Let the user select the folder
            SelectedFolder = await folderPicker.PickSingleFolderAsync();

            if (SelectedFolder != null)
            {
                // Application now has read/write access to all contents in the picked folder
                // (including other sub-folder contents)
                Windows.Storage.AccessCache.StorageApplicationPermissions.
                FutureAccessList.AddOrReplace("PickedFolderToken", SelectedFolder);
      
            }
            else
            {
                Debug.WriteLine("Pick Folder Operation Failed");
            }
        }

        internal static async Task OrganizeFolder()
        {
            // Check if the selected folder is not null
            if (SelectedFolder == null)
            {
                return;
            }

            // Create subfolders for the desired file categories
            foreach (string category in SettingsManager.Instance.Settings.CategoryToFileTypeMap.Keys)
            {
                await SelectedFolder.CreateFolderAsync(category, CreationCollisionOption.GenerateUniqueName);
            }

            // Get a list of all files in the selected folder
            Windows.Storage.Search.StorageFileQueryResult queryResult = SelectedFolder.CreateFileQuery();
            IReadOnlyList<StorageFile> fileList = await queryResult.GetFilesAsync();

            // Organize folder by sorting files into newly created subfolder based on file type
            foreach (StorageFile file in fileList)
            {
                foreach (KeyValuePair<string, List<string>> categoryTypePair in SettingsManager.Instance.Settings.CategoryToFileTypeMap)
                {
                    if (categoryTypePair.Value.Contains(file.FileType))
                    {
                        Debug.WriteLine($"File {file.Name} belongs in folder {categoryTypePair.Key}");
                        string destPath = SelectedFolder.Path + $"\\{categoryTypePair.Key}";
                        StorageFolder destFolder = await StorageFolder.GetFolderFromPathAsync(destPath);
                        await file.MoveAsync(destFolder);
                    }
                }
            }
        }


        internal static async Task<Dictionary<string,int>> GetFolderFileDetails()
        {
            Dictionary<string, int> categoryToNumberMap = new Dictionary<string, int>();

            if (SelectedFolder == null)
            {
                return categoryToNumberMap;
            }

            Windows.Storage.Search.StorageFileQueryResult queryResult = SelectedFolder.CreateFileQuery();
            IReadOnlyList<StorageFile> fileList = await queryResult.GetFilesAsync();

            //string fileDetails = $"Total Number of Files: {fileList.Count}";
            
            
            
            foreach (StorageFile file in fileList)
            {
                foreach (KeyValuePair<string, List<string>> categoryTypePair in SettingsManager.Instance.Settings.CategoryToFileTypeMap)
                {
                    if (categoryTypePair.Value.Contains(file.FileType))
                    {
                        if (categoryToNumberMap.ContainsKey(categoryTypePair.Key))
                        {
                            categoryToNumberMap[categoryTypePair.Key] += 1;
                        }
                        else
                        {
                            categoryToNumberMap.Add(categoryTypePair.Key, 1);
                        }
                    }
                }
            }

            return categoryToNumberMap;



            //foreach (KeyValuePair<string,int> categoryAmountPair in  categoryToNumberMap.OrderBy(key => key.Key))
            //{
            //    fileDetails += $"\n{categoryAmountPair.Key}: {categoryAmountPair.Value}";
            //}

            //return fileDetails;
        }
    }
}
