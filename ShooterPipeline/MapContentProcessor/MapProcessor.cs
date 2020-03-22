#region File Description
//-----------------------------------------------------------------------------
// MapProcessor.cs
//
// Wolfenstein3DX
// Copyright (C) Nexxt Studios - 2009
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline.Processors;
using System.Diagnostics;
using System.Reflection;
using Nexxt.Pipeline.GameObjectContentProcessor;
using Nexxt.Common.Enums; 
#endregion

namespace Nexxt.Pipeline.MapContentProcessor
{
	[ContentProcessor(DisplayName = "Map Processor")]
	public class MapProcessor : ContentProcessor<string, MapContent>
	{
		public override MapContent Process(string input, ContentProcessorContext context)
		{
            //System.Diagnostics.Debugger.Launch();

			MapContent content = new MapContent();

            string[] lines = ParseMapDefinition(input);

			//first up is the textures used
			int line = 0;
			do
			{
				//load the texture from the file
				ExternalReference<TextureContent> tex =
					context.BuildAsset<TextureContent, TextureContent>(
						new ExternalReference<TextureContent>(
							Path.Combine(Directory.GetCurrentDirectory(), "Textures\\" + lines[line])),
						"TextureProcessor");

				//add it to our map content
				content.Textures.Add(tex);

				//move to the next line
				line++;
			} while (!string.IsNullOrEmpty(lines[line])); //we're done when we hit an empty line

			//skip over the empty line
			line++;

			//read in the background file
			content.Background = context.BuildAsset<TextureContent, TextureContent>(
				new ExternalReference<TextureContent>(
					Path.Combine(Directory.GetCurrentDirectory(), "Backgrounds\\" + lines[line])),
				"TextureProcessor");
            //skip blank line and load the ambient music for the map
            line += 2;

            content.AmbientAudio = lines[line];

            //skip the blank line and load the Spawn Point for the player
            line += 2;
            content.SpawnPoint = GetSpawnPoint(lines[line], lines[line + 1]);
            line++;

			//skip over the blank line and get to the sprite definition or the layout
            line += 2;

            while (lines[line].StartsWith("["))
            {
                //Sprite Handling
                GameObjectContent gameObject = new GameObjectContent();
                gameObject.Context = context;
                Type gameObjType = gameObject.GetType();
                PropertyInfo objectProperty;
                string propertyName = "";
                string propertyValue = "";
                gameObject.Name = lines[line].Substring(1, lines[line].Length - 2);
                line++;
                do
                {
                    // Read the property and assign to the object, since this is done in compilation time
                    // not while the game is running performance is not a big issue so we can do this by
                    // reflection, is slower but gives us a lot of flexibility
                    if (ParseProperty(lines[line], ref propertyName, ref propertyValue))
                    {
                        //Well formed property assignment
                        objectProperty = gameObjType.GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance);
                        if (objectProperty != null)
                        {
                            //Property found assign directly
                            objectProperty.SetValue(gameObject, Convert.ChangeType(propertyValue, objectProperty.PropertyType.UnderlyingSystemType), null);
                            System.Diagnostics.Debug.WriteLine(string.Format("Property: {0}, Value: {1}", propertyName, propertyValue));
                        }
                        else
                        {
                            //the property could not be found add this to the object state bag
                            gameObject.StateBag.Add(propertyName, propertyValue);
                        }
                    }
                    line++;
                } while (!string.IsNullOrEmpty(lines[line]));
                content.Sprites.Add(gameObject);
                line++;
            }
			
            //Finished processing sprites load the layout
			do
			{
				//read in all the cells in the line
				string[] cells = lines[line].Split(' ');

				//parse all those cells into integer indexes
				int[] cellIndexes = new int[cells.Length];
				for (int i = 0; i < cellIndexes.Length; i++)
					cellIndexes[i] = int.Parse(cells[i]);

				//add the row to the map content
				content.Rows.Add(cellIndexes);

				//move to the next line
				line++;
			} while (line < lines.Length && !string.IsNullOrEmpty(lines[line])); //keep going until we're out of layout lines

			//return the map content to the writer
			return content;
		}

        private Vector2 GetSpawnPoint(string coord1, string coord2)
        {
            float x, y;
            if (coord1.StartsWith("SpawnPoint.X"))
            {
                if (!float.TryParse(coord1.Substring(coord1.IndexOf('=') + 1), System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out x))
                    x = 0.0f;
                if (!float.TryParse(coord2.Substring(coord1.IndexOf('=') + 1), System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out y))
                    y = 0.0f;
            }
            else
            {
                if (!float.TryParse(coord1.Substring(coord1.IndexOf('=') + 1), System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out y))
                    y = 0.0f;
                if (!float.TryParse(coord2.Substring(coord1.IndexOf('=') + 1), System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out x))
                    x = 0.0f;
            }
            return new Vector2(x, y);
        }

        private bool ParseProperty(string line, ref string propertyName, ref string propertyValue)
        {
            int indexOfEq;
            indexOfEq = line.IndexOf("=");

            if (indexOfEq < 0 || indexOfEq >= line.Length)
                //Bad formed property string
                return false;

            propertyName = line.Substring(0, line.IndexOf("=") - 1).Trim();
            propertyValue = line.Substring(line.IndexOf("=") + 1).Trim();
            return true;
        }

        private string[] ParseMapDefinition(string input)
        {
            //split the text by the end-line character
            string[] lines = input.Split('\n');
            List<string> linesCopy = new List<string>();
            //remove all the \r characters and trim the lines
            for (int i = 0; i < lines.Length; i++)
                lines[i] = lines[i].Replace('\r', ' ').Trim();

            for (int i = 0; i < lines.Length; i++)
            {
                if (!lines[i].StartsWith("//"))
                    linesCopy.Add(lines[i]);
            }
            return linesCopy.ToArray();
        }
    }

    
}