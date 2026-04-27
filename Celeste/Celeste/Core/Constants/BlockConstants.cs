namespace Celeste
{
    /// <summary>
    /// Constants for blocks/obstacles: display position, movement, and future physics/tuning.
    /// </summary>
    public static class BlockConstants
    {
        // Single block/obstacle display position (T/Y cycle which block is shown here)
        public const float BlockDisplayX = 480f;
        public const float BlockDisplayY = 120f;

        public const float SpringTriggerInsetNative = 2f;
        public const float SpringTriggerTopOffsetNative = 12f;
        public const float SpringTriggerMinHeightNative = 3f;
        public const int SpringFootTriggerTolerancePixels = 4;
    }
}
