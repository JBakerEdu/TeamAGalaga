using System;
using System.Linq;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;

namespace Galaga.View.Sprites
{
    internal class ExplosionAnimationManager
    {
        private static readonly TimeSpan FrameDuration = TimeSpan.FromMilliseconds(100);

        public static async Task Play(Canvas canvas, double x, double y)
        {
            var frames = new UserControl[]
            {
                new ShipExplosionFrame1(),
                new ShipExplosionFrame2(),
                new ShipExplosionFrame3(),
            };

            foreach (var frame in frames)
            {
                canvas.Children.Add(frame);
                Canvas.SetLeft(frame, x);
                Canvas.SetTop(frame, y);
                await Task.Delay(FrameDuration);
                canvas.Children.Remove(frame);
            }
        }
    }
}