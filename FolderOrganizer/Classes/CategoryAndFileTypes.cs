using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FolderOrganizer
{
    /// <summary>
    /// Main class for storing a file category and its associated file types
    /// </summary>
    internal class CategoryAndFileTypes
    {
        public string Category { get; set; }
        public List<string> FileTypesList { get; set; }

        public string FileTypesFormatted
        {
            get { return FormatFileTypeString(FileTypesList); }
        }

        public CategoryAndFileTypes(string category, List<string> fileTypesList)
        {
            Category = category;
            FileTypesList = fileTypesList;
        }

        private string FormatFileTypeString(List<string> fileTypesList)
        {

            string fileTypesFormatted = fileTypesList[0];

            for (int i = 1; i < fileTypesList.Count; i++)
            {
                fileTypesFormatted += ", " + fileTypesList[i];
            }

            return fileTypesFormatted;
        }
    }
}
