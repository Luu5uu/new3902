using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Celeste.Animation;

namespace Celeste.Items
{
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
            Draw(spriteBatch, Position, Scale, Color.White);
        }

        public void Draw(SpriteBatch spriteBatch, Color tint)
        {
            if (spriteBatch == null) throw new ArgumentNullException(nameof(spriteBatch));
            Draw(spriteBatch, Position, Scale, tint);
        }

        public void Draw(
            SpriteBatch spriteBatch,
            Vector2 position,
            float scale = 1f,
            Color? tint = null,
            SpriteEffects effects = SpriteEffects.None,
            float layerDepth = 0f)
        {
            if (spriteBatch == null) throw new ArgumentNullException(nameof(spriteBatch));

            Rectangle src = _clip.GetSourceRect(_animator.FrameIndex);
            Color drawColor = tint ?? Color.White;

            spriteBatch.Draw(
                _clip.Texture,
                position,
                src,
                drawColor,
                0f,
                Vector2.Zero,
                scale,
                effects,
                layerDepth);
        }
    }
}