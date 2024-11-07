using System;
using Windows.UI.Xaml.Controls;
using Galaga.View.Sprites;

namespace Galaga.Model
{
    /// <summary>
    /// Represents an enemy ship with customizable properties.
    /// </summary>
    public class EnemyShip : BaseSprite
    {
        /// <summary>
        /// The Sprite Image Assigned
        /// </summary>
        public BaseSprite Sprite { get; private set; }
        /// <summary>
        /// The Speed Assigned
        /// </summary>
        public double Speed { get; private set; }
        private readonly Random random = new Random();

        /// <summary>
        /// The X values of the Script
        /// </summary>
        public double X
        {
            get => Canvas.GetLeft(this);
            set => Canvas.SetLeft(this, value);
        }

        /// <summary>
        /// The Y values of the Script
        /// </summary>
        public double Y
        {
            get => Canvas.GetTop(this);
            set => Canvas.SetTop(this, value);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EnemyShip"/> class.
        /// </summary>
        /// <param name="sprite">The sprite instance to be used for the enemy ship.</param>
        /// <param name="speed">The speed of the enemy ship.</param>
        public EnemyShip(BaseSprite sprite, double speed)
        {
            this.Sprite = sprite;
            Width = 50;
            Height = 50;
            this.Speed = speed;

            InitializeComponent();
            Content = this.Sprite;
        }

        /// <summary>
        /// Renders the enemy ship at the specified (x,y) location.
        /// </summary>
        public new void RenderAt(double x, double y)
        {
            this.X = x; 
            this.Y = y;
        }

        /// <summary>
        /// Moves the enemy ship in a specified direction based on its speed.
        /// </summary>
        public void Move(double deltaX)
        {
            this.X += deltaX * this.Speed;
        }
    }
}
