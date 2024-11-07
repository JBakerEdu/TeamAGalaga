using Windows.UI.Xaml.Controls;

namespace Galaga.View.Sprites
{
    /// <summary>
    /// Defines BaseSprite from which all sprites inherit.
    /// </summary>
    /// <seealso cref="Windows.UI.Xaml.Controls.UserControl" />
    /// <seealso cref="Windows.UI.Xaml.Markup.IComponentConnector" />
    /// <seealso cref="Windows.UI.Xaml.Markup.IComponentConnector2" />
    /// <seealso cref="Galaga.View.Sprites.ISpriteRenderer" />
    public abstract partial class BaseSprite : ISpriteRenderer
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BaseSprite"/> class.
        /// </summary>
        protected BaseSprite()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Renders sprite at the specified (x,y) location in relation
        /// to the top, left part of the canvas.
        /// </summary>
        /// <param name="x">x location</param>
        /// <param name="y">y location</param>
        public void RenderAt(double x, double y)
        {
            Canvas.SetLeft(this, x);
            Canvas.SetTop(this, y);
        }
    }


}
