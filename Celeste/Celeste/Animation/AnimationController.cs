using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Celeste.Animation
{
    // State machine that routes updates/rendering to the active AutoAnimation.
    public sealed class AnimationController<TState> where TState : notnull
    {
        private readonly Dictionary<TState, AutoAnimation> _animations = new();

        public TState CurrentState { get; private set; } = default!;
        private bool _hasState = false;

        public bool HasState(TState state) => _animations.ContainsKey(state);
        public bool HasAnyState => _hasState;

        public void Register(TState state, AutoAnimation animation, bool setAsDefault = false)
        {
            if (animation == null) throw new ArgumentNullException(nameof(animation));
            _animations[state] = animation;

            if (setAsDefault || !_hasState)
            {
                CurrentState = state;
                _hasState = true;
                animation.Play();
            }
        }

        public void SetState(TState state, bool restart = false)
        {
            if (!_animations.TryGetValue(state, out AutoAnimation next))
                return; // State not registered (e.g. legacy has no *_static_hair clip for this state).

            if (!_hasState)
            {
                CurrentState = state;
                _hasState = true;
                if (restart) next.Stop();
                next.Play();
                return;
            }

            if (EqualityComparer<TState>.Default.Equals(CurrentState, state) && !restart)
                return;

            CurrentState = state;
            if (restart) next.Stop();
            next.Play();
        }

        public void Update(GameTime gameTime)
        {
            if (!_hasState) return;
            _animations[CurrentState].Update(gameTime);
        }

        public void Draw(SpriteBatch spriteBatch, Vector2 position, Color color, float scale, SpriteEffects effects)
        {
            if (!_hasState) return;
            _animations[CurrentState].Draw(spriteBatch, position, color, scale, effects);
        }

        public void Draw(SpriteBatch spriteBatch, Vector2 position, Color color, Vector2 scale)
        {
            if (!_hasState) return;
            _animations[CurrentState].Draw(spriteBatch, position, color, scale);
        }

        public AutoAnimation Get(TState state) => _animations[state];

        public bool IsFinished => !_hasState || !_animations[CurrentState].IsPlaying;
    }
}
