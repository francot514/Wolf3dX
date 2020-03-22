using System;
using System.Collections.Generic;
using System.Text;
using Nexxt.Engine.Animations.Script;

namespace Nexxt.Engine.Animations
{
    public class KeyFrame
    {
        #region Fields
        public int Frame;
        public float Duration;

        ScriptLine[] scripts; 
        #endregion

        #region Constructor
        public KeyFrame()
        {
            Frame = -1;
            Duration = 0;

            scripts = new ScriptLine[4];
            for (int i = 0; i < scripts.Length; i++)
                scripts[i] = null;
        } 
        #endregion

        #region Methods
        public void SetScript(int idx, String val)
        {
            scripts[idx] = new ScriptLine(val);
        }

        public ScriptLine GetScript(int idx)
        {
            return scripts[idx];
        }

        public ScriptLine[] GetScriptArray()
        {
            return scripts;
        } 
        #endregion
    }
}
