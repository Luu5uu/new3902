using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Celeste
{
    public static class LevelConstants
    {
        public const int FirstRoom = 1;
        public const int LastRoom = 6;
        public const int DefaultRoom = 3;

        public const int WorldMapColumns = 50;
        public const int WorldMapRows = 30;
        public const int RoomTransitionOutOfBoundsMargin = 160;
        public const int RoomTransitionEdgeBand = 24;
        public const float RewindDarkOverlayAlpha = 0.6f;

        public static readonly Vector2 DefaultRespawnPoint = new Vector2(200f, 150f);

        public static readonly IReadOnlyDictionary<int, Vector2> RoomRespawnPoints = new Dictionary<int, Vector2>
        {
            { 1, new Vector2(45f, 336f) },
            { 2, new Vector2(70f, 378f) },
            { 3, new Vector2(78f, 396f) },
            { 4, new Vector2(120f, 390f) },
            { 5, new Vector2(120f, 376f) },
            { 6, new Vector2(700f, 320f) }
        };

        public record struct RoomTransitionZone(int SourceRoom, Rectangle TriggerArea, int TargetRoom);
        public record struct EndingZone(int SourceRoom, Rectangle TriggerArea);

        public static readonly IReadOnlyList<RoomTransitionZone> TransitionZones =
            new RoomTransitionZone[]
        {
            new(1, new Rectangle(620, 0, 120, 36), 2),
            new(2, new Rectangle(680, 0, 80, 36), 3),
            new(3, new Rectangle(660, 0, 80, 36), 4),
            new(4, new Rectangle(400, 0, 80, 36), 5),
            new(1, new Rectangle(0, 41, 21, 41), 6),
            new(6, new Rectangle(788, 0, 800, 84), 1)
        };

        public static readonly EndingZone FinalEndingZone = new EndingZone(5, new Rectangle(330, 0, 120, 60));
        
        public const float RewindRecordDuration = 3.0f;
        public const float RewindCooldownDuration = 3.0f;
    }
}
