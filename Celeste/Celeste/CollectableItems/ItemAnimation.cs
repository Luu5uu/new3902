using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Celeste.Animation;

namespace Celeste.CollectableItems
{
    /// <summary>
    /// Renderable animation wrapper for items: owns clip + animator.
    /// No gameplay logic (no collisions, no collect state).
    /// </summary>
    /// <author> Albert Liu </author>
    public sealed class ItemAnimation
    {
        private readonly AnimationClip _clip;
        private readonly ItemAnimator _animator;

        public ItemAnimation(AnimationClip clip)
        {
            _clip = clip ?? throw new ArgumentNullException(nameof(clip));
            _animator = new ItemAnimator(_clip);
        }

        public AnimationClip Clip => _clip;

        public void Reset() => _animator.Reset();

        public void Update(GameTime gameTime) => _animator.Update(gameTime);

        /// <summary>
        /// Draw at a position with optional scale & sprite effects.
        /// Caller can decide world transform; this method stays minimal.
        /// </summary>
        public void Draw(
            SpriteBatch spriteBatch,
            Vector2 position,
            float scale = 1f,
            SpriteEffects effects = SpriteEffects.None,
            float layerDepth = 0f)
        {
            if (spriteBatch == null) throw new ArgumentNullException(nameof(spriteBatch));

            Rectangle src = _clip.GetSourceRect(_animator.FrameIndex);

            spriteBatch.Draw(
                _clip.Texture,
                position,
                src,
                Color.White,
                rotation: 0f,
                origin: Vector2.Zero,
                scale: scale,
                effects: effects,
                layerDepth: layerDepth
            );
        }
    }
}