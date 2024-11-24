using System;
using Windows.UI.Xaml.Controls;

namespace Galaga.View
{
    internal static class AudioManager
    {
        // Path to the folder containing audio files
        private const string AudioFilesFolder = "AudioFiles";

        /// <summary>
        /// Plays the sound for enemy blowing up.
        /// </summary>
        public static void PlayEnemyBlowUp()
        {
            PlaySound("enemyBlowUp.wav");
        }

        /// <summary>
        /// Plays the sound for player blowing up.
        /// </summary>
        public static void PlayPlayerBlowUp()
        {
            PlaySound("playerBlowUp.wav");
        }

        /// <summary>
        /// General method to play a sound file.
        /// </summary>
        /// <param name="fileName">The name of the .wav file to play.</param>
        private static async void PlaySound(string fileName)
        {
            try
            {
                var uri = new Uri($"ms-appx:///{AudioFilesFolder}/{fileName}");
                MediaElement mediaElement = new MediaElement
                {
                    AutoPlay = true
                };
                mediaElement.Source = uri;
                mediaElement.Play();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error playing sound: {ex.Message}");
            }
        }
    }
}