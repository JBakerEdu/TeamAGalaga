using Windows.Foundation;
using Windows.UI.ViewManagement;

namespace Galaga.Model
{
    public static class WindowResizer
    {
        public static void ResizeCurrentView(double width, double height)
        {
            ApplicationView.GetForCurrentView().TryResizeView(new Size(width, height));
        }
    }
}