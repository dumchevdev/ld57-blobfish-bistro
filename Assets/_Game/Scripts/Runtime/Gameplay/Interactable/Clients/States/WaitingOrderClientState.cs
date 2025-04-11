using Game.Runtime.StateMachine;

namespace Game.Runtime.Gameplay.Interactives
{
    public class WaitingOrderClientState : State<ClientData>
    {
        public override void OnEnter()
        {
            Context.MoodChecker.ResetMoodTimer();
            Context.View.ShowHintWarning(true);
            Context.View.InteractionStrategy = new TakeOrderClientInteraction();
        }
        
        public override void OnExit()
        {
            Context.View.ShowHintWarning(false);
        }
    }
}