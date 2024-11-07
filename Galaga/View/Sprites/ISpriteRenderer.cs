namespace Galaga.View.Sprites
{
    /// <summary>
    ///     Interface to render a sprite. This interface is needed within every sprite
    ///     that is associated with a game object.
    ///     The interface is to be implemented in the sprite class.
    /// </summary>
    public interface ISpriteRenderer
    {
        #region Methods

        /// <summary>
        /// Renders sprite at the specified (x,y) location in relation
        /// to the top, left part of the canvas.
        /// </summary>
        /// <param name="x">x location</param>
        /// <param name="y">y location</param>
        void RenderAt(double x, double y);

        #endregion
    }
}
