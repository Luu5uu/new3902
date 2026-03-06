using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Celeste.Animation
{
    public static class AtlasLoader
    {
        public static AnimationCatalog LoadFromAtlas(ContentManager content, string atlasXmlFileName)
        {
            if (content == null) throw new ArgumentNullException(nameof(content));
            if (string.IsNullOrWhiteSpace(atlasXmlFileName))
                throw new ArgumentException("atlasXmlFileName is required.");

            // Read Atlas.xml from the Content folder at runtime
            string atlasPath = Path.Combine(content.RootDirectory, atlasXmlFileName);

            XDocument doc;
            using (Stream s = TitleContainer.OpenStream(atlasPath))
            {
                doc = XDocument.Load(s);
            }

            XElement atlas = doc.Root ?? throw new InvalidOperationException("Atlas XML missing root element.");
            if (atlas.Name != "Atlas")
                throw new InvalidOperationException("Atlas XML root element must be <Atlas>.");

            string textureAsset = (string)atlas.Attribute("texture")
                ?? throw new InvalidOperationException("<Atlas> must have attribute 'texture'.");

            Texture2D sheet = content.Load<Texture2D>(textureAsset);

            var catalog = new AnimationCatalog();

            foreach (XElement e in atlas.Elements("Clip"))
            {
                ClipDef def = ParseClipDef(e);

                ValidateClipDef(def, sheet);

                var clip = new AnimationClip
                {
                    Texture = sheet,
                    StartX = def.X,
                    StartY = def.Y,
                    FrameWidth = def.W,
                    FrameHeight = def.H,
                    FrameCount = def.Frames,
                    FramesPerRow = def.FramesPerRow > 0 ? def.FramesPerRow : def.Frames,
                    Fps = def.Fps > 0 ? def.Fps : 12f,
                    Loop = def.Loop
                };

                // Use key exactly as provided in XML (should match AnimationKeys)
                catalog.Clips[def.Key] = clip;
            }

            return catalog;
        }

        private static ClipDef ParseClipDef(XElement e)
        {
            string key = (string)e.Attribute("key") ?? throw new InvalidOperationException("<Clip> missing key.");

            int x = ReadInt(e, "x");
            int y = ReadInt(e, "y");
            int w = ReadInt(e, "w");
            int h = ReadInt(e, "h");
            int frames = ReadInt(e, "frames");

            float fps = ReadFloat(e, "fps", 12f);
            bool loop = ReadBool(e, "loop", true);
            int fpr = ReadInt(e, "framesPerRow", frames);

            return new ClipDef
            {
                Key = key,
                X = x,
                Y = y,
                W = w,
                H = h,
                Frames = frames,
                Fps = fps,
                Loop = loop,
                FramesPerRow = fpr
            };
        }

        private static void ValidateClipDef(ClipDef def, Texture2D sheet)
        {
            if (def.W <= 0 || def.H <= 0) throw new ArgumentOutOfRangeException(def.Key, "Frame size must be > 0.");
            if (def.Frames <= 0) throw new ArgumentOutOfRangeException(def.Key, "Frames must be > 0.");
            if (def.X < 0 || def.Y < 0) throw new ArgumentOutOfRangeException(def.Key, "x/y must be >= 0.");

            int fpr = def.FramesPerRow > 0 ? def.FramesPerRow : def.Frames;
            int rows = (def.Frames + fpr - 1) / fpr;

            int clipPixelWidth = fpr * def.W;
            int clipPixelHeight = rows * def.H;

            // Ensure the rectangle that covers ALL frames fits inside the sheet.
            if (def.X + clipPixelWidth > sheet.Width || def.Y + clipPixelHeight > sheet.Height)
            {
                throw new InvalidOperationException(
                    $"Clip '{def.Key}' exceeds sheet bounds. " +
                    $"ClipArea=({def.X},{def.Y},{clipPixelWidth},{clipPixelHeight}), " +
                    $"Sheet=({sheet.Width},{sheet.Height}).");
            }
        }

        private static int ReadInt(XElement e, string name, int? defaultValue = null)
        {
            XAttribute a = e.Attribute(name);
            if (a == null)
            {
                if (defaultValue.HasValue) return defaultValue.Value;
                throw new InvalidOperationException($"<Clip> missing '{name}'.");
            }
            return int.Parse(a.Value, CultureInfo.InvariantCulture);
        }

        private static float ReadFloat(XElement e, string name, float defaultValue)
        {
            XAttribute a = e.Attribute(name);
            if (a == null) return defaultValue;
            return float.Parse(a.Value, CultureInfo.InvariantCulture);
        }

        private static bool ReadBool(XElement e, string name, bool defaultValue)
        {
            XAttribute a = e.Attribute(name);
            if (a == null) return defaultValue;
            return bool.Parse(a.Value);
        }
    }
}