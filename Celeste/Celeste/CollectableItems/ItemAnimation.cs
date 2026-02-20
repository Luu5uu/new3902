using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Celeste.Animation;

namespace Celeste.CollectableItems
{
    // Item animation: clip + animator. IDrawable/IUpdateable; set Position/Scale or use Draw(sb, pos, scale).
    public sealed class ItemAnimation : Celeste.GamePlay.IUpdateable, Celeste.GamePlay.IDrawable
    {
        private readonly AnimationClip _clip;
        private readonly ItemAnimator _animator;

        public ItemAnimation(AnimationClip clip)
        {
            _clip = clip ?? throw new ArgumentNullException(nameof(clip));
            _animator = new ItemAnimator(_clip);
        }

        public AnimationClip Clip => _clip;

        public Vector2 Position { get; set; }
        public float Scale { get; set; } = 1f;

        public void Reset() => _animator.Reset();

        public void Update(GameTime gameTime) => _animator.Update(gameTime);

        public void Draw(SpriteBatch spriteBatch)
        {
            if (spriteBatch == null) throw new ArgumentNullException(nameof(spriteBatch));
            Draw(spriteBatch, Position, Scale);
        }

        // Draw at position/scale without setting Position/Scale.
        public void Draw(SpriteBatch spriteBatch, Vector2 position, float scale = 1f,
            SpriteEffects effects = SpriteEffects.None, float layerDepth = 0f)
        {
            if (spriteBatch == null) throw new ArgumentNullException(nameof(spriteBatch));
            Rectangle src = _clip.GetSourceRect(_animator.FrameIndex);
            spriteBatch.Draw(_clip.Texture, position, src, Color.White, 0f, Vector2.Zero, scale, effects, layerDepth);
        }
    }
}