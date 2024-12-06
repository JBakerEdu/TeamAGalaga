using System;
using Windows.UI.Xaml.Controls;

namespace Galaga.Model
{
    /// <summary>
    /// an interface of how to navigate through the system
    /// </summary>
    public interface INavigationService
    {
        /// <summary>
        /// Navigates to the specified page type with an optional parameter.
        /// </summary>
        /// <param name="pageType">The type of the page to navigate to.</param>
        /// <param name="parameter">An optional parameter to pass to the target page.</param>
        public void Navigate(Type pageType, object parameter = null);

        /// <summary>
        /// Retrieves the type of the page that was most recently navigated away from.
        /// </summary>
        /// <returns>The <see cref="Type"/> of the previous page, or <c>null</c> if no previous page exists.</returns>
        public Type GetPreviousPageType();

        /// <summary>
        /// Gets the number of entries in the back navigation stack.
        /// </summary>
        public int BackStackDepth { get; }

    }

    /// <summary>
    /// Provides navigation functionality within the application, including navigating to pages, 
    /// retrieving the previous page type, and managing the back stack depth.
    /// Implements the <see cref="INavigationService"/> interface.
    /// </summary>
    public class NavigationService : INavigationService
    {
        private Frame Frame => (Frame)Windows.UI.Xaml.Window.Current.Content;

        /// <summary>
        /// Navigates to the specified page type with an optional parameter.
        /// </summary>
        /// <param name="pageType">The type of the page to navigate to.</param>
        /// <param name="parameter">An optional parameter to pass to the target page.</param>
        public void Navigate(Type pageType, object parameter = null)
        {
            this.Frame.Navigate(pageType, parameter);
        }

        /// <summary>
        /// Retrieves the type of the page that was navigated to before the current page.
        /// </summary>
        /// <returns>
        /// The <see cref="Type"/> of the previous page if the back stack is not empty; otherwise, <c>null</c>.
        /// </returns>
        public Type GetPreviousPageType()
        {
            if (this.Frame.BackStackDepth > 0)
            {
                return this.Frame.BackStack[this.Frame.BackStack.Count - 1].SourcePageType;
            }
            return null;
        }

        /// <summary>
        /// Gets the number of entries in the navigation back stack.
        /// </summary>
        public int BackStackDepth => this.Frame.BackStackDepth;
    }
}
