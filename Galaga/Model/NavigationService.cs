using System;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

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
        Frame.Navigate(pageType, parameter);
    }

    public Type GetPreviousPageType()
    {
        if (Frame.BackStackDepth > 0)
        {
            return Frame.BackStack[Frame.BackStack.Count - 1].SourcePageType;
        }
        return null;
    }

    public int BackStackDepth => Frame.BackStackDepth;
}