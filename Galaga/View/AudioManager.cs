using System;
using System.Diagnostics;
using Windows.Media.Core;
using Windows.Media.Playback;

namespace Galaga.View
{
    internal static class AudioManager
    {
        private const string AudioFilesFolder = "View/AudioFiles";

        /// <summary>
        /// Plays the sound for enemy blowing up.
        /// </summary>
        public static void PlayEnemyBlowUp()
        {
            playSound("enemyBlowUp.wav");
        }

        /// <summary>
        /// Plays the sound for enemy shooting.
        /// </summary>
        public static void PlayEnemyShoot()
        {
            playSound("enemyShoot.wav");
        }

        /// <summary>
        /// Plays the sound for the ending of a game.
        /// </summary>
        public static void PlayGameOver()
        {
            playSound("gameOver.wav");
        }

        /// <summary>
        /// Plays the sound for player blowing up.
        /// </summary>
        public static void PlayPlayerBlowUp()
        {
            playSound("playerBlowUp.wav");
        }

        /// <summary>
        /// Plays the sound for player shooting.
        /// </summary>
        public static void PlayPlayerShoot()
        {
            playSound("playerShoot.wav");
        }

        /// <summary>
        /// General method to play a sound file.
        /// </summary>
        /// <param name="fileName">The name of the .wav file to play.</param>
        private static async void playSound(string fileName)
        {
            Debug.Print("Called playSound");

            try
            {
                var uri = new Uri($"ms-appx:///{AudioFilesFolder}/{fileName}");

                var mediaPlayer = new MediaPlayer();
                mediaPlayer.Source = MediaSource.CreateFromUri(uri);
                mediaPlayer.Volume = 1.0;
                mediaPlayer.Play();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error playing sound: {ex.Message}");
            }
        }
    }
}