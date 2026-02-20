using Microsoft.Xna.Framework.Input;

namespace Celeste.Input
{
    // Command from input layer; consumed by player. No reference to Madeline or keyboard.
    public readonly struct PlayerCommand
    {
        public float MoveX { get; }
        public bool JumpPressed { get; }
        public bool DashPressed { get; }
        public bool DeathPressed { get; }   // NEW

        public PlayerCommand(float moveX, bool jumpPressed, bool dashPressed, bool deathPressed)
        {
            MoveX = moveX;
            JumpPressed = jumpPressed;
            DashPressed = dashPressed;
            DeathPressed = deathPressed;
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

            bool deathPressed = current.IsKeyDown(Keys.T) && !previous.IsKeyDown(Keys.T); // NEW

            return new PlayerCommand(moveX, jumpPressed, dashPressed, deathPressed);
        }
    }
}