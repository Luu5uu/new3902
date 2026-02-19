using Celeste.Character;

namespace Celeste.MadelineStates
{
    public interface IMadelineState
    {
        void SetState(Madeline m);
        void Update(Madeline m, float dt);
        void Exit(Madeline m);
    }
}
