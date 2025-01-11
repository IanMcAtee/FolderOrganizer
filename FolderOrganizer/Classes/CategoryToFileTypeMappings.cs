using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FolderOrganizer
{
    internal static class CategoryToFileTypeMappings
    {
        public static Dictionary<string, List<string>> Map { get; private set; } = new Dictionary<string, List<string>>
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

        
    }

    
}

