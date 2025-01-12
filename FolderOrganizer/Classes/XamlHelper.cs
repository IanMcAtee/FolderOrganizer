using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using System.Collections.Generic;

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


        public static List<T> GetAllChildrenInObjectOfType<T>(DependencyObject root)
        {
            List<T> childrenOfType = new List<T>();

            int childCount = VisualTreeHelper.GetChildrenCount(root);

            for (int i = 0; i < childCount; i++)
            {
                var child = VisualTreeHelper.GetChild(root, i);

                if (child is T typedChild)
                {
                    childrenOfType.Add(typedChild);
                }

                childrenOfType.AddRange(GetAllChildrenInObjectOfType<T>(child));
            }

            return childrenOfType;
        }
    }
}
