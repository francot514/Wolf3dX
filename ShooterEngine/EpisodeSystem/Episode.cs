#region File Description
//-----------------------------------------------------------------------------
// Episode.cs
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
#endregion

namespace Nexxt.Framework.EpisodeSystem
{
    /// <summary>
    /// Class that holds the information related to a Episode within the game
    /// This holds the list of levels, the currently selected level
    /// </summary>
    public class Episode
    {
        private List<Level> levels;
        private int _currentLevel = 0;

        public string Name { get; set; }
        public bool InitialEpisode { get; set; }
        public int Order { get; set; }

        public Episode()
        {
            levels = new List<Level>();
        }

        public List<Level> Levels
        {
            get { return levels; }
            set { levels = value; }
        }

        public Level Current
        {
            get
            {
                if (levels.Count > 0 && levels.Count > _currentLevel)
                    return levels[_currentLevel];
                else
                {
                    _currentLevel = 0;
                    return null;
                }
            }
        }

        public Level Next()
        {
            _currentLevel += 1;
            return Current;
        }
    }
}