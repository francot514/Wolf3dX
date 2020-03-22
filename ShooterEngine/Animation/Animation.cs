using System;
using System.Collections.Generic;
using System.Text;

namespace Nexxt.Engine.Animations
{
    public class Animation
    {
        #region Fields
        public String Name;
        KeyFrame[] KeyFrames;
        public bool MustBeAnimatedFromDistinctAngles = false;
        #endregion

        #region Constructor
        public Animation()
        {
            Name = "";
            KeyFrames = new KeyFrame[64];
            for (int i = 0; i < KeyFrames.Length; i++)
                KeyFrames[i] = new KeyFrame();
        } 
        #endregion

        #region Methods
        public KeyFrame GetKeyFrame(int idx)
        {
            return KeyFrames[idx];
        }

        public void SetKeyFrame(int idx, KeyFrame _keyFrame)
        {
            KeyFrames[idx] = _keyFrame;
        }

        public KeyFrame[] getKeyFrameArray()
        {
            return KeyFrames;
        } 
        #endregion
    }
}
