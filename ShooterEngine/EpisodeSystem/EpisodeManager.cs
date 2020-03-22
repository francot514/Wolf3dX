#region File Description
//-----------------------------------------------------------------------------
// EpisodeManager.cs
//
// Wolfenstein3DX
// Copyright (C) Nexxt Studios - 2009
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using Nexxt.Common.Enums;
#endregion

namespace Nexxt.Framework.EpisodeSystem
{
    /// <summary>
    /// Class that represents the Episode System for the Game Holds
    /// the information about the currently selected (playing?) episode
    /// </summary>
    public class EpisodeManager
    {
        public Difficulty SelectedDifficulty { get; set; }

        private List<Episode> episodes;
        private int _currentEpisode = 0;

        public List<Episode> Episodes
        {
            get
            {
                return episodes;
            }
            set
            {
                episodes = value;
            }
        }

        public EpisodeManager()
        {
            episodes = new List<Episode>();
            SelectedDifficulty = Difficulty.None;
        }

        public Episode Current
        {
            get
            {
                if (episodes.Count > 0 && episodes.Count > _currentEpisode)
                    return episodes[_currentEpisode];
                else
                {
                    _currentEpisode = 0;
                    return null;
                }
            }
        }

        public Episode Next()
        {
            _currentEpisode += 1;
            return Current;
        }
    }
}
