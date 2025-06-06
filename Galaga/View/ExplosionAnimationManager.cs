﻿using System;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using Galaga.Model;
using Galaga.View.Sprites.HolidayGame;
using Galaga.View.Sprites.OriginalGame;

namespace Galaga.View
{
    internal class ExplosionAnimationManager
    {
        private static readonly TimeSpan FrameDuration = TimeSpan.FromMilliseconds(100);

        public static async Task Play(Canvas canvas, double x, double y, GameType gameType)
        {
            UserControl[] frames;
            if (gameType == GameType.HolidayGame)
            {
                frames = new UserControl[]
                {
                    new HolidayShipExplosionFrame1(),
                    new HolidayShipExplosionFrame2(),
                    new HolidayShipExplosionFrame3()
                };
            }
            else
            {
                frames = new UserControl[]
                {
                    new ShipExplosionFrame1(),
                    new ShipExplosionFrame2(),
                    new ShipExplosionFrame3()
                };
            }

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