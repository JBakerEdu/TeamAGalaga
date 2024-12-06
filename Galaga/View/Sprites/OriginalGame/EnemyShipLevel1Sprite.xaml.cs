using Windows.UI.Xaml.Controls;

namespace Galaga.View.Sprites.OriginalGame
{
    /// <summary>
    /// Defines EnemyShipLevel1Sprite from which all sprites inherit.
    /// </summary>
    public partial class EnemyShipLevel1Sprite
    { 
        /// <summary>
        /// Initializes a new instance of the <see cref="EnemyShipLevel1Sprite"/> class.
        /// </summary>
        public EnemyShipLevel1Sprite()
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