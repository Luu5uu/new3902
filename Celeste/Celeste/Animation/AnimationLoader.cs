using System;
using Microsoft.Xna.Framework.Content;

namespace Celeste.Animation
{
    public static class AnimationLoader
    {
        public static AnimationCatalog LoadAll(ContentManager content)
        {
            if (content == null) throw new ArgumentNullException(nameof(content));

            // Atlas.xml placed under Content/Atlas.xml and copied to output.
            return AtlasLoader.LoadFromAtlas(content, "Atlas.xml");
        }
    }
}