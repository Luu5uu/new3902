using System.Collections.Generic;

namespace Celeste.Animation
{
    /// <summary>
    /// A lightweight container for all loaded animation clips.
    /// Provides a single place to retrieve clips by key.
    /// </summary>
    /// <author> Albert Liu </author>
    public sealed class AnimationCatalog
    {
        /// <summary>
        /// All loaded clips, indexed by a stable string key.
        /// </summary>
        public Dictionary<string, AnimationClip> Clips { get; } = new Dictionary<string, AnimationClip>();

        /// <summary>
        /// Indexer convenience. Throws if key is missing.
        /// </summary>
        public AnimationClip this[string key] => Clips[key];
    }
}