using Game.Runtime.StateMachine;

namespace Game.Runtime.Gameplay.Interactives
{
    public class EatingOrderFoodClientState : State<ClientData>
    {
        public override void OnEnter()
        {
            Context.MoodChecker.StopMoodTimer();
            Context.View.InteractionStrategy = new MoveToClientInteraction();
        }
    }
}