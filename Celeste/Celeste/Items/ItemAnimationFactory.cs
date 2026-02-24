using System;
using Celeste.Animation;

namespace Celeste.Items
{
    /// <summary>
    /// Creates item animations from the loaded AnimationCatalog.
    /// </summary>
    /// <author> Albert Liu </author>
    public static class ItemAnimationFactory
    {
        public static ItemAnimation CreateNormalStaw(AnimationCatalog catalog)
            => new ItemAnimation(GetClip(catalog, AnimationKeys.ItemNormalStaw));

        public static ItemAnimation CreateFlyStaw(AnimationCatalog catalog)
            => new ItemAnimation(GetClip(catalog, AnimationKeys.ItemFlyStaw));

        public static ItemAnimation CreateCrystal(AnimationCatalog catalog)
            => new ItemAnimation(GetClip(catalog, AnimationKeys.ItemCrystal));

        private static AnimationClip GetClip(AnimationCatalog catalog, string key)
        {
            if (catalog == null) throw new ArgumentNullException(nameof(catalog));

            if (!catalog.Clips.TryGetValue(key, out var clip))
                throw new ArgumentException(
                    $"Missing clip for key '{key}'. Did you load it in AnimationLoader?",
                    nameof(key));

            return clip;
        }
    }
}
