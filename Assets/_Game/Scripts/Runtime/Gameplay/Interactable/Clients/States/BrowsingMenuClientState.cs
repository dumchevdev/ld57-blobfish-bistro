using Game.Runtime.StateMachine;

namespace Game.Runtime.Gameplay.Interactives
{
    public class BrowsingMenuClientState : State<ClientData>
    {
        public override void OnEnter()
        {
            Context.MoodChecker.ResetMoodTimer();
            Context.View.Settings.IsClickable = true;
            Context.View.Settings.IsHighlightable = true;
            Context.View.InteractionStrategy = new MoveToClientInteraction();
        }
    }
}