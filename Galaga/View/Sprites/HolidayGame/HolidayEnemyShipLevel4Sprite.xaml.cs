using Windows.UI.Xaml.Controls;

namespace Galaga.View.Sprites
{
    /// <summary>
    /// Defines HolidayEnemyShipLevel4Sprite from which all sprites inherit.
    /// </summary>
    public partial class HolidayEnemyShipLevel4Sprite
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HolidayEnemyShipLevel4Sprite"/> class.
        /// </summary>
        public HolidayEnemyShipLevel4Sprite()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Renders sprite at the specified (x,y) location in relation
        /// to the top, left part of the canvas.
        /// </summary>
        /// <param name="x">x location</param>
        /// <param name="y">y location</param>
        public new void RenderAt(double x, double y)
        {
            Canvas.SetLeft(this, x);
            Canvas.SetTop(this, y);
        }
    }
}
