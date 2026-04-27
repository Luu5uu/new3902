using Celeste.Character;

namespace Celeste.MadelineStates
{
    public sealed class StarFlyState : IMadelineState
    {
        public void SetState(Madeline m)
        {
            m.BeginStarFly();
        }

        public void Update(Madeline m, float dt)
        {
            m.UpdateStarFly(dt);
        }

        public void Exit(Madeline m)
        {
            m.EndStarFly();
        }
    }
}
