using System;
using Windows.Media.Core;
using Windows.Media.Playback;
using Galaga.View.Sprites;

namespace Galaga.View
{
    internal static class AudioManager
    {
        private const string OriginalAudioFilesFolder = "View/AudioFiles/OriginalGame";
        private const string HolidayAudioFilesFolder = "View/AudioFiles/HolidayGame";

        /// <summary>
        /// Plays the sound for enemy blowing up.
        /// </summary>
        public static void PlayEnemyBlowUp(GameType gameType)
        {
            playSound("enemyBlowUp.wav", gameType);
        }

        /// <summary>
        /// Plays the sound for enemy shooting.
        /// </summary>
        public static void PlayEnemyShoot(GameType gameType)
        {
            playSound("enemyShoot.wav", gameType);
        }

        /// <summary>
        /// Plays the sound for the ending of a game.
        /// </summary>
        public static void PlayGameOver(GameType gameType)
        {
            playSound("gameOver.wav", gameType);
        }

        /// <summary>
        /// Plays the sound for player blowing up.
        /// </summary>
        public static void PlayPlayerBlowUp(GameType gameType)
        {
            playSound("playerBlowUp.wav", gameType);
        }

        /// <summary>
        /// Plays the sound for player shooting.
        /// </summary>
        public static void PlayPlayerShoot(GameType gameType)
        {
            playSound("playerShoot.wav", gameType);
        }

        /// <summary>
        /// Plays the sound for player has the power up active.
        /// </summary>
        public static void PlayActivePowerUp(GameType gameType)
        {
            playSound("playerPowerUp.wav", gameType);
        }

        /// <summary>
        /// Plays the sound for when a bonus ship appears and is on canvas.
        /// </summary>
        public static void PlayActiveBonusShip(GameType gameType)
        {
            playSound("bonusShipActive.wav", gameType);
        }

        /// <summary>
        /// General method to play a sound file.
        /// </summary>
        /// <param name="fileName">The name of the .wav file to play.</param>
        private static async void playSound(string fileName, GameType gameType)
        {
            try
            {
                string folderPath = gameType switch
                {
                    GameType.HolidayGame => HolidayAudioFilesFolder,
                    GameType.OriginalGame => OriginalAudioFilesFolder,
                    _ => throw new ArgumentException("Unsupported game type")
                };

                var uri = new Uri($"ms-appx:///{folderPath}/{fileName}");

                var mediaPlayer = new MediaPlayer
                {
                    Source = MediaSource.CreateFromUri(uri),
                    Volume = 1.0
                };

                mediaPlayer.Play();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error playing sound: {ex.Message}");
            }
        }
    }
}
