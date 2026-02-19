using Microsoft.Xna.Framework.Input;

namespace Celeste.Input
{
    // Command from input layer; consumed by player. No reference to Madeline or keyboard.
    public readonly struct PlayerCommand
    {
        public float MoveX { get; }
        public bool JumpPressed { get; }
        public bool DashPressed { get; }

        public PlayerCommand(float moveX, bool jumpPressed, bool dashPressed)
        {
            MoveX = moveX;
            JumpPressed = jumpPressed;
            DashPressed = dashPressed;
        }

        // Arrow + WASD move; Space jump; Enter/Z/N dash.
        public static PlayerCommand FromKeyboard(KeyboardState current, KeyboardState previous)
        {
            float moveX = 0f;
            if (current.IsKeyDown(Keys.D) || current.IsKeyDown(Keys.Right)) moveX += 1f;
            if (current.IsKeyDown(Keys.A) || current.IsKeyDown(Keys.Left))  moveX -= 1f;

            bool jumpPressed = current.IsKeyDown(Keys.Space) && !previous.IsKeyDown(Keys.Space);
            bool dashPressed = (current.IsKeyDown(Keys.Enter) || current.IsKeyDown(Keys.Z) || current.IsKeyDown(Keys.N))
                && !(previous.IsKeyDown(Keys.Enter) || previous.IsKeyDown(Keys.Z) || previous.IsKeyDown(Keys.N));

            return new PlayerCommand(moveX, jumpPressed, dashPressed);
        }
    }
}
