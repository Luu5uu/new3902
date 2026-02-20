using Microsoft.Xna.Framework.Input;
using System;

namespace Celeste.Input
{
    // Command from input layer; consumed by player. No reference to Madeline or keyboard.
    public readonly struct PlayerCommand
    {
        public float MoveX { get; }
        public bool JumpPressed { get; }
        public bool DashPressed { get; }
        public bool DeathPressed { get; }

        public bool ClimbHeld { get; }
        // NEW

        public PlayerCommand(float moveX, bool jumpPressed, bool dashPressed, bool deathPressed, bool climbheld)
        {
            MoveX = moveX;
            JumpPressed = jumpPressed;
            DashPressed = dashPressed;
            DeathPressed = deathPressed;
            ClimbHeld = climbheld;
        }

        // Arrow + WASD move; Space jump; Enter/Z/N dash; T death.
        public static PlayerCommand FromKeyboard(KeyboardState current, KeyboardState previous)
        {
            float moveX = 0f;
            if (current.IsKeyDown(Keys.D) || current.IsKeyDown(Keys.Right)) moveX += 1f;
            if (current.IsKeyDown(Keys.A) || current.IsKeyDown(Keys.Left))  moveX -= 1f;

            bool jumpPressed = current.IsKeyDown(Keys.Space) && !previous.IsKeyDown(Keys.Space);

            bool dashPressed =
                (current.IsKeyDown(Keys.Enter) || current.IsKeyDown(Keys.Z) || current.IsKeyDown(Keys.N))
                && !(previous.IsKeyDown(Keys.Enter) || previous.IsKeyDown(Keys.Z) || previous.IsKeyDown(Keys.N));

            bool deathPressed = current.IsKeyDown(Keys.E) && !previous.IsKeyDown(Keys.E); // NEW

            bool climbheld = current.IsKeyDown(Keys.W);

            return new PlayerCommand(moveX, jumpPressed, dashPressed, deathPressed, climbheld);
        }
    }
}