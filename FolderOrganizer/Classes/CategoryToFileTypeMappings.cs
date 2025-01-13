using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Media.AppBroadcasting;

namespace FolderOrganizer
{
    internal static class CategoryToFileTypeMappings
    {
        // Return the map sorted by keys on get
        public static Dictionary<string,List<string>> Map
        {
            get { return map.OrderBy(keyValuePair => keyValuePair.Key).ToDictionary(pair => pair.Key, pair => pair.Value); }
        }

        // Dictionary mapping file category to its common file types
        public static Dictionary<string, List<string>> map { get; private set; } = new Dictionary<string, List<string>>
        {
            {
                "Document",
                new List<string>
                {
                    ".txt",
                    ".doc",
                    ".docx",
                    ".rtf",
                    ".odt",
                    ".pdf",
                    ".md"
                }
            },
            {
                "Image",
                new List<string>
                {
                    ".jpg",
                    ".jpeg",
                    ".png",
                    ".gif",
                    ".bmp",
                    ".tiff",
                    ".webp",
                    ".svg",
                    ".eps",
                    ".ai",
                    ".cdr"
                }
            },
            {
                "Audio",
                new List<string>
                {
                    ".mp3",
                    ".wav",
                    ".aac",
                    ".ogg",
                    ".flac",
                    ".m4a",
                    ".wma"
                }
            },
            {
                "Video",
                new List<string>
                {
                    ".mp4",
                    ".avi",
                    ".mov",
                    ".mkv",
                    ".wmv",
                    ".flv",
                    ".webm"
                }
            },
            {
                "Archive",
                new List<string>
                {
                    ".zip",
                    ".rar",
                    ".7z",
                    ".tar",
                    ".gz",
                    ".iso"
                }
            },
            {
                "Executable",
                new List<string>
                {
                    ".exe",
                    ".dll",
                    ".bat",
                    ".cmd",
                    ".sh",
                    ".msi",
                    ".app"
                }
            },
            {
                "Data",
                new List<string>
                {
                    ".csv",
                    ".json",
                    ".xml",
                    ".db",
                    ".sqlite",
                    ".mdb",
                    ".accdb",
                    ".sql"
                }
            },
            {
                "Code",
                new List<string>
                {
                    ".py",
                    ".java",
                    ".cs",
                    ".cpp",
                    ".js",
                    ".html",
                    ".css"
                }
            },
            {
                "Presentation",
                new List<string>
                {
                    ".ppt",
                    ".pptx",
                    ".key",
                    ".odp"
                }
            },
            {
                "Spreadsheet",
                new List<string>
                {
                    ".xls",
                    ".xlsx",
                    ".ods"
                }
            },
            {
                "System",
                new List<string>
                {
                    ".sys",
                    ".ini",
                    ".log",
                    ".cfg",
                    ".tmp",
                    ".dat"
                }
            },
            {
                "Font",
                new List<string>
                {
                    ".ttf",
                    ".otf",
                    ".woff",
                    ".woff2",
                    ".eot"
                }
            },
            {
                "3DModel",
                new List<string>
                {
                    ".obj",
                    ".fbx",
                    ".stl",
                    ".dae",
                    ".blend",
                    ".dwg"
                }
            }
        };

        public static List<CategoryAndFileTypes> CategoryAndFileTypesList = new List<CategoryAndFileTypes>
        {
            new CategoryAndFileTypes
            (
               "3D Model",
                new List<string>
                {
                    ".obj",
                    ".fbx",
                    ".stl",
                    ".dae",
                    ".blend",
                    ".dwg"
                }
            ),
            new CategoryAndFileTypes
            (
                "Archive",
                new List<string>
                {
                    ".zip",
                    ".rar",
                    ".7z",
                    ".tar",
                    ".gz",
                    ".iso"
                }
            ),
            new CategoryAndFileTypes
            (
                "Audio",
                new List<string>
                {
                    ".mp3",
                    ".wav",
                    ".aac",
                    ".ogg",
                    ".flac",
                    ".m4a",
                    ".wma"
                }
            ),
            new CategoryAndFileTypes
            (
               "Code",
                new List<string>
                {
                    ".py",
                    ".java",
                    ".cs",
                    ".cpp",
                    ".js",
                    ".html",
                    ".css"
                }
            ),
            new CategoryAndFileTypes
            (
                "Data",
                new List<string>
                {
                    ".csv",
                    ".json",
                    ".xml",
                    ".db",
                    ".sqlite",
                    ".mdb",
                    ".accdb",
                    ".sql"
                }
            ),
            new CategoryAndFileTypes
            (
                "Document",
                new List<string>
                {
                    ".txt",
                    ".doc",
                    ".docx",
                    ".rtf",
                    ".odt",
                    ".pdf",
                    ".md"
                }
            ),
            new CategoryAndFileTypes
            (
                "Executable",
                new List<string>
                {
                    ".exe",
                    ".dll",
                    ".bat",
                    ".cmd",
                    ".sh",
                    ".msi",
                    ".app"
                }
            ),
            new CategoryAndFileTypes
            (
               "Font",
                new List<string>
                {
                    ".ttf",
                    ".otf",
                    ".woff",
                    ".woff2",
                    ".eot"
                }
            ),
            new CategoryAndFileTypes
            (
                "Image",
                new List<string>
                {
                    ".jpg",
                    ".jpeg",
                    ".png",
                    ".gif",
                    ".bmp",
                    ".tiff",
                    ".webp",
                    ".svg",
                    ".eps",
                    ".ai",
                    ".cdr"
                }
            ),
            new CategoryAndFileTypes
            (
               "Presentation",
                new List<string>
                {
                    ".ppt",
                    ".pptx",
                    ".key",
                    ".odp"
                }
            ),
            new CategoryAndFileTypes
            (
               "Spreadsheet",
                new List<string>
                {
                    ".xls",
                    ".xlsx",
                    ".ods"
                }
            ),
            new CategoryAndFileTypes
            (
               "System",
                new List<string>
                {
                    ".sys",
                    ".ini",
                    ".log",
                    ".cfg",
                    ".tmp",
                    ".dat"
                }
            ),
            new CategoryAndFileTypes
            (
                "Video",
                new List<string>
                {
                    ".mp4",
                    ".avi",
                    ".mov",
                    ".mkv",
                    ".wmv",
                    ".flv",
                    ".webm"
                }
            )
        };

        public static CategoryAndFileTypes? GetCategoryAndFileTypes(string categoryName)
        {
            foreach (CategoryAndFileTypes categoryAndFileTypes in CategoryAndFileTypesList)
            {
                if (categoryAndFileTypes.Category ==  categoryName)
                {
                    return categoryAndFileTypes;
                }
            }
            return null;
        }

    }
        
    
}

