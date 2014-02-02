using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;

namespace Soheil
{
    public static class VisualTreeHelper
    {
        /// <summary>
        /// Get visual tree children of a type
        /// </summary>
        /// <typeparam name="T">Visual tree children type</typeparam>
        /// <param name="current"></param>
        /// <param name="children">A collection of one visual tree children type</param>
        private static void GetVisualChildren<T>(DependencyObject current, Collection<T> children)
            where T : DependencyObject
        {
            if (current != null)
            {
                if (current.GetType() == typeof (T))
                {
                    children.Add((T) current);
                }

                for (int i = 0; i < System.Windows.Media.VisualTreeHelper.GetChildrenCount(current); i++)
                {
                    GetVisualChildren(System.Windows.Media.VisualTreeHelper.GetChild(current, i), children);
                }
            }
        }

        /// <summary>
        /// Get visual tree children of a type
        /// </summary>
        /// <typeparam name="T">Visaul tree children type</typeparam>
        /// <param name="current"></param>
        /// <returns>Returns a collection of one visual tree children type</returns>
        public static Collection<T> GetVisualChildren<T>(DependencyObject current) where T : DependencyObject
        {
            if (current == null)
            {
                return null;
            }

            var children = new Collection<T>();

            GetVisualChildren(current, children);

            return children;
        }

        /// <summary>
        /// Get the first visual child from a FrameworkElement Template
        /// </summary>
        /// <typeparam name="TP">FrameworkElement type</typeparam>
        /// <typeparam name="T">FrameworkElement type</typeparam>
        /// <param name="templatedParent"></param>
        /// <returns>Returns a FrameworkElement visual child typeof T if found one; returns null otherwise</returns>
        public static T GetVisualChild<T, TP>(TP templatedParent)
            where T : FrameworkElement
            where TP : FrameworkElement
        {
            Collection<T> children = GetVisualChildren<T>(templatedParent);

            return children.FirstOrDefault(child => Equals(child.TemplatedParent, templatedParent));
        }


		public static FrameworkElement GetVisualParent(this FrameworkElement target, string parentName)
		{
			FrameworkElement parent = target;
			while (parent != null)
			{
				if (parent.Name == parentName) return parent;
				parent = System.Windows.Media.VisualTreeHelper.GetParent(parent) as FrameworkElement;
			}
			return null;
		}

		public static FrameworkElement GetNextVisual(this FrameworkElement target)
		{
			if (target != null)
			{
				var parent = target.Parent as System.Windows.Controls.Panel;
				if (parent != null)
				{
					var idx = parent.Children.IndexOf(target) + 1;
					if (idx < parent.Children.Count)
						return parent.Children[idx] as FrameworkElement;
				}
			}
			return null;
		}
    }
}