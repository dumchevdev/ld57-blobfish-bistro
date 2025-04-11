using Game.Runtime.StateMachine;
using UnityEngine;

namespace Game.Runtime.Gameplay.Interactives
{
    public class WaitingInQueueClientState : State<ClientData>
    {
        public override void OnEnter()
        {
            Context.View.Settings.IsClickable = true;
            Context.View.Settings.IsHighlightable = true;
            Context.View.InteractionStrategy = new SelectedClientInteraction();
        }

        public override void OnExit()
        {
            Context.View.InteractionStrategy = new EmptyClientInteraction();
        }
    }
}