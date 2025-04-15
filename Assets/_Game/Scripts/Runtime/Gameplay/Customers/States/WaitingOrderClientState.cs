using Game.Runtime._Game.Scripts.Runtime.Gameplay.Customers.Strategies;
using Game.Runtime._Game.Scripts.Runtime.ServiceLocator;
using Game.Runtime._Game.Scripts.Runtime.Services.Audio;
using Game.Runtime._Game.Scripts.Runtime.StateMachine;
using Game.Runtime.CMS;

namespace Game.Runtime._Game.Scripts.Runtime.Gameplay.Customers.States
{
    public class WaitingOrderClientState : State<CustomerData>
    {
        public override void OnEnter()
        {
            ServicesProvider.GetService<AudioService>().Play(CMSPrefabs.Audio.SFX.SFXBubble);
            Context.MoodChecker.ResetMoodTimer();
            Context.Behaviour.ShowHintWarning(true);
            Context.Behaviour.InteractionStrategy = new TakeOrderCustomerInteraction();
        }
        
        public override void OnExit()
        {
            Context.Behaviour.ShowHintWarning(false);
        }
    }
}