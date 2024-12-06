using Windows.UI.Xaml.Controls;

namespace Galaga.View.Sprites.HolidayGame
{
    /// <summary>
    /// Defines HolidayShipExplosionFrame1 from which all sprites inherit.
    /// </summary>
    public partial class HolidayShipExplosionFrame1
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HolidayShipExplosionFrame1"/> class.
        /// </summary>
        public HolidayShipExplosionFrame1()
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