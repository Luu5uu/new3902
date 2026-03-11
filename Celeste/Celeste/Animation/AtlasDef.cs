using System.Collections.Generic;

namespace Celeste.Animation
{
    public sealed class AtlasDef
    {
        public string TextureAsset { get; init; } = string.Empty;
        public List<ClipDef> Clips { get; } = new();
    }

    public sealed class ClipDef
    {
        public string Key { get; init; } = string.Empty;

        public int X { get; init; }
        public int Y { get; init; }
        public int W { get; init; }
        public int H { get; init; }

        public int Frames { get; init; }
        public float Fps { get; init; }
        public bool Loop { get; init; }

        public int FramesPerRow { get; init; }
    }
}