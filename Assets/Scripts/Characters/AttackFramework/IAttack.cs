using System;
using AudioFramework;

namespace Characters.AttackFramework
{
    //And the award for most disgusting interface goes to IAttack... I have joker face paint on rn
    //This was supposed to be just a state interface but degraded into whatever monster this is
    public interface IAttack
    {
        public void Enter(Metronome.Accuracy accuracy);

        public void Tick();

        public void Exit();

        public bool ShouldExit();

        public bool CanQueueMoveAfterwards();

        public void QueueMove(Action move);
    }
}