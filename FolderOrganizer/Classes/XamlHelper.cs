using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FolderOrganizer
{
    internal static class XamlHelper
    {
        public static T? GetFirstSiblingInPanelOfType<T>(Control xamlControl)
        {
            try
            {
                Panel parentPanel = (Panel)xamlControl.Parent;

                foreach(object child in parentPanel.Children)
                {
                    if (child.GetType() == typeof(T))
                    {
                        return (T)child;
                    }
                }
                return default;
            }
            catch { return default;}
        }

        public static List<T> GetAllChildrenInPanelOfType<T>(Panel rootPanel)
        {
            List<T> childrenOfType = new List<T>();

            foreach (object child in rootPanel.Children)
            {
                if (child.GetType() == typeof(Panel))
                {
                    childrenOfType = childrenOfType.Concat(GetAllChildrenInPanelOfType<T>((Panel)child)).ToList();
                }
                else if (child.GetType() == typeof(T))
                {
                    childrenOfType.Add((T)child);   
                }
            }

            return childrenOfType;
        } 
    }
}
