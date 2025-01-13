using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using System.Collections.Generic;

namespace FolderOrganizer
{
    internal static class XamlHelper
    {
        public static T? GetFirstSiblingOfType<T>(DependencyObject childObject)
        {
            DependencyObject parent = VisualTreeHelper.GetParent(childObject);

            int childCount = VisualTreeHelper.GetChildrenCount(parent);

            for (int i = 0; i < childCount; i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(parent, i);
                if (child is T typedChild)
                {
                    return typedChild;
                }
            }

            return default;
        }


        public static List<T> GetAllChildrenOfType<T>(DependencyObject root)
        {
            List<T> childrenOfType = new List<T>();

            int childCount = VisualTreeHelper.GetChildrenCount(root);

            for (int i = 0; i < childCount; i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(root, i);

                if (child is T typedChild)
                {
                    childrenOfType.Add(typedChild);
                }

                childrenOfType.AddRange(GetAllChildrenOfType<T>(child));
            }

            return childrenOfType;
        }
    }
}
