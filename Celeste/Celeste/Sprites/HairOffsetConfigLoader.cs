using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Xml.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace Celeste.Sprites
{
    public static class HairOffsetConfigLoader
    {
        public static Dictionary<string, Vector2[]> Load(ContentManager content, string xmlFileName)
        {
            if (content == null) throw new ArgumentNullException(nameof(content));
            if (string.IsNullOrWhiteSpace(xmlFileName)) throw new ArgumentException("xmlFileName required.");

            string path = Path.Combine(AppContext.BaseDirectory, content.RootDirectory, xmlFileName);

            XDocument doc;
            using (var fs = File.OpenRead(path))
            {
                doc = XDocument.Load(fs);
            }

            var root = doc.Root ?? throw new InvalidOperationException("HairOffsets XML missing root.");
            if (!root.Name.LocalName.Equals("HairOffsets", StringComparison.OrdinalIgnoreCase))
                throw new InvalidOperationException("Root must be <HairOffsets>.");

            var result = new Dictionary<string, Vector2[]>(StringComparer.OrdinalIgnoreCase);

            foreach (var anim in root.Elements())
            {
                if (!anim.Name.LocalName.Equals("Anim", StringComparison.OrdinalIgnoreCase))
                    continue;

                string name = (string)anim.Attribute("name")
                    ?? throw new InvalidOperationException("<Anim> missing name.");
                name = name.Trim().ToLowerInvariant();

                var frames = new List<Vector2>();
                foreach (var f in anim.Elements())
                {
                    if (!f.Name.LocalName.Equals("F", StringComparison.OrdinalIgnoreCase))
                        continue;

                    float dx = ReadFloat(f, "dx");
                    float dy = ReadFloat(f, "dy");
                    frames.Add(new Vector2(dx, dy));
                }

                result[name] = frames.ToArray();
            }

            return result;
        }

        private static float ReadFloat(XElement e, string attr)
        {
            var a = e.Attribute(attr) ?? throw new InvalidOperationException($"Missing '{attr}'.");
            return float.Parse(a.Value, CultureInfo.InvariantCulture);
        }
    }
}