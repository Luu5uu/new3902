using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Celeste.Animation;
using Celeste.Character;


namespace Celeste.MadelineStates
{
    public interface IMadelineState
    {
        public void setState(Madeline m);
        public void update(Madeline m, float dt);
        public void exit(Madeline m);

    }
}
