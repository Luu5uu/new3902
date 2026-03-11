using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Xml.Linq;
using Microsoft.Xna.Framework.Content;

namespace Celeste.Sprites
{
    public static class BangsFrameConfigLoader
    {
        public static Dictionary<string, int[]> Load(ContentManager content, string xmlFileName)
        {
            if (content == null) throw new ArgumentNullException(nameof(content));
            if (string.IsNullOrWhiteSpace(xmlFileName)) throw new ArgumentException("xmlFileName required.");

            // Absolute path: <exe>/Content/<xmlFileName>
            string path = Path.Combine(AppContext.BaseDirectory, content.RootDirectory, xmlFileName);

            XDocument doc;
            using (var fs = File.OpenRead(path))
            {
                doc = XDocument.Load(fs);
            }

            var root = doc.Root ?? throw new InvalidOperationException("BangsFrames XML missing root.");
            if (!root.Name.LocalName.Equals("BangsFrames", StringComparison.OrdinalIgnoreCase))
                throw new InvalidOperationException("Root must be <BangsFrames>.");

            var result = new Dictionary<string, int[]>(StringComparer.OrdinalIgnoreCase);

            foreach (var anim in root.Elements("Anim"))
            {
                string name = (string)anim.Attribute("name")
                    ?? throw new InvalidOperationException("<Anim> missing name.");
                name = name.ToLowerInvariant();

                var frames = new List<int>();
                foreach (var f in anim.Elements("F"))
                {
                    int i = ReadInt(f, "i");
                    if (i < 0) i = 0;
                    if (i > 2) i = 2;
                    frames.Add(i);
                }

                result[name] = frames.ToArray();
            }

            return result;
        }

        private static int ReadInt(XElement e, string attr)
        {
            var a = e.Attribute(attr) ?? throw new InvalidOperationException($"Missing '{attr}'.");
            return int.Parse(a.Value, CultureInfo.InvariantCulture);
        }
    }
}