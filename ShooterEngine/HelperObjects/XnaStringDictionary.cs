#region File Description
//-----------------------------------------------------------------------------
// XnaStringDictionary.cs
//
// Wolfenstein3DX
// Copyright (C) Nexxt Studios - 2009
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using System.Collections.Specialized;
using System.Text;
using System.Collections.Generic; 
#endregion

namespace Nexxt.Framework.HelperObjects
{
    public class XnaStringDictionary : StringDictionary
    {
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder("╞");
            foreach (KeyValuePair<string,string> item in this)
            {
                sb.AppendFormat("{0}¼{1}å", item.Key, item.Value);
            }
            if (this.Count > 0)
            {
                sb.Remove(sb.Length - 1, 1);
            }
            return sb.ToString();
        }

        public void FromString(string content)
        {
            content = content.Substring(1);
            if (!string.IsNullOrEmpty(content))
            {
                string[] keypairvalues = content.Split('å');
                string[] values;
                for (int i = 0; i < keypairvalues.Length; i++)
                {
                    values = keypairvalues[i].Split('¼');
                    this.Add(values[0], values[1]);
                }
            }
        }
    }
}
