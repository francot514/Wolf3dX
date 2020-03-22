#region File Description
//-----------------------------------------------------------------------------
// SoundManager.cs
//
// Wolfenstein3DX
// Copyright (C) Nexxt Studios - 2009
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Audio;    
#endregion

namespace Nexxt.Engine.Sound
{
    /// <summary>
    /// Controls the audio for the game
    /// </summary>
    public static class SoundManager
    {
        //the sound effects for our game
        static Dictionary<string, SoundEffect> sounds =
            new Dictionary<string, SoundEffect>();

        //the music for our game
        static Song currentSong;

        //a content manager for later loading sound effects
        static ContentManager content;

        //we now store the sound volume in here
        static float soundVolume = 1f;

        static SoundEffectInstance currentSoundEffect;

        static SoundEffect effect;

        /// <summary>
        /// Initializes the manager.
        /// </summary>
        /// <param name="game">The game instance.</param>
        public static void Initialize(Game game)
        {
            content = game.Content;
        }

        /// <summary>
        /// Plays a sound with a given name.
        /// </summary>
        /// <param name="name">The name of the sound to play.</param>
        public static void PlaySound(string name, bool overPlay)
        {
            if (sounds.TryGetValue(name, out effect))
            {
                 //validates if an effect is being played 
                if (currentSoundEffect != null)
                {
                    if (!overPlay)
                    {
                        if (currentSoundEffect.State != SoundState.Playing)
                        {
                            currentSoundEffect = effect.CreateInstance();
                            currentSoundEffect.Play();
                        }
                    }
                    else
                    {
                        currentSoundEffect = effect.CreateInstance();
                        currentSoundEffect.Play();
                    }
                }
                 //plays the effect for the first time
                else
                {
                    currentSoundEffect = effect.CreateInstance();
                    currentSoundEffect.Play();
                }
            }
        }


        /// <summary>
        /// Sets the background music to the sound with the given name.
        /// </summary>
        /// <param name="name">The name of the music to play.</param>
        public static void PlayMusic(string name)
        {
            currentSong = null;

            try
            {
                currentSong = content.Load<Song>(name);
            }
            catch (Exception e)
            {
#if ZUNE
			//on the Zune we can go through the MediaLibrary to attempt
			//to find a matching song name. this functionality doesn't
			//exist on Windows or Xbox 360 at this time
			MediaLibrary ml = new MediaLibrary();
			foreach (Song song in ml.Songs)
				if (song.Name == name)
					currentSong = song;
#endif

                //if we didn't find the song, rethrow the exception
                if (currentSong == null)
                    throw e;
            }
            MediaPlayer.Play(currentSong);
        }

     

        /// <summary>
        /// Loads a sound for playing
        /// </summary>
        /// <param name="assetName">The asset name of the sound</param>
        public static void LoadSound(string assetName)
        {
            sounds.TryGetValue(assetName, out effect);

            if (effect == null)
            {
                //load the sound into our dictionary
                sounds.Add(assetName, content.Load<SoundEffect>(assetName));
            }   
        }

        /// <summary>
        /// Stops the background music.
        /// </summary>
        public static void StopMusic()
        {
            MediaPlayer.Stop();
        }

        /// <summary>
        /// Sets the sound FX volume level.
        /// </summary>
        /// <param name="volume">A volume level in the range [0, 1].</param>
        public static void SetSoundFXVolume(float volume)
        {
            soundVolume = volume;
        }

        /// <summary>
        /// Sets the music volume level.
        /// </summary>
        /// <param name="volume">A volume level in the range [0, 1].</param>
        public static void SetMusicVolume(float volume)
        {
            MediaPlayer.Volume = volume;
        }
    }
}