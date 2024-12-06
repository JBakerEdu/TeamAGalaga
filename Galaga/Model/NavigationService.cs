using System;
using Windows.UI.Xaml.Controls;

namespace Galaga.Model
{
    public interface INavigationService
    {
        void Navigate(Type pageType, object parameter = null);
        Type GetPreviousPageType();
        int BackStackDepth { get; }
    }

    public class NavigationService : INavigationService
    {
        private Frame Frame => (Frame)Windows.UI.Xaml.Window.Current.Content;

        public void Navigate(Type pageType, object parameter = null)
        {
            this.Frame.Navigate(pageType, parameter);
        }

        public Type GetPreviousPageType()
        {
            if (this.Frame.BackStackDepth > 0)
            {
                return this.Frame.BackStack[this.Frame.BackStack.Count - 1].SourcePageType;
            }
            return null;
        }

        public int BackStackDepth => this.Frame.BackStackDepth;
    }
}
