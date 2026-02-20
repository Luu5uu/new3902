using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Celeste.Animation;

namespace Celeste.DeathAnimation
{
    /// <summary>
    /// Death sprite playback anchored at TOP-LEFT to match Madeline.position semantics.
    /// </summary>
    public sealed class DeathSpritePlayer
    {
        private readonly ClipPlayer _player;
        private readonly AnimationClip _clip;

        /// <summary>
        /// TOP-LEFT world position of the sprite (same semantics as Madeline.position).
        /// </summary>
        public Vector2 TopLeft { get; private set; }

        public float Scale { get; private set; }

        public bool IsFinished => !_player.IsPlaying;

        public int FrameWidth  => _clip.FrameWidth;
        public int FrameHeight => _clip.FrameHeight;

        public DeathSpritePlayer(AnimationClip clip, Vector2 topLeft, float scale)
        {
            _clip = clip;
            _player = new ClipPlayer(clip, overrideLoop: false);

            TopLeft = topLeft;
            Scale = scale;
        }

        public void Update(float dt)
        {
            _player.Update(dt);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            // ✅ origin = Vector2.Zero to match other states' AnimationPlayer.draw
            _player.Draw(
                spriteBatch,
                TopLeft,
                Scale,
                origin: Vector2.Zero,
                color: Color.White
            );
        }

        /// <summary>
        /// Returns the geometric center of the sprite in world space (scaled).
        /// Useful for particle spawn centers.
        /// </summary>
        public Vector2 GetScaledCenter()
        {
            return TopLeft + new Vector2(FrameWidth * Scale * 0.5f, FrameHeight * Scale * 0.5f);
        }
    }
}