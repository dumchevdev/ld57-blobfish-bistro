using System;
using Game.Runtime.Gameplay.Character;
using Game.Runtime.StateMachine;

namespace Game.Runtime.Gameplay.Interactives
{
    public class ClientData : IDisposable
    {
        public int Id;

        public ClientMood Mood 
        {
            get => _mood;
            set
            {
                View.SetMood(value);
                _mood = value;
            }
        }
        private ClientMood _mood;
        
        public readonly MovableBehaviour Movable;
        public readonly ClientMoodChecker MoodChecker;
        public readonly ClientBehaviour View;
        public readonly StateMachine<ClientData> StateMachine;
        
        public ClientData(ClientBehaviour view, MovableBehaviour movable)
        {
            View = view;
            Movable = movable;
            MoodChecker = new ClientMoodChecker(this);
            
            StateMachine = new StateMachine<ClientData>(this);
            StateMachine.AddState(new BrowsingMenuClientState());
            StateMachine.AddState(new EatingOrderFoodClientState());
            StateMachine.AddState(new EmptyClientState());
            StateMachine.AddState(new LeavingClientState());
            StateMachine.AddState(new WaitingFoodClientState());
            StateMachine.AddState(new WaitingInQueueClientState());
            StateMachine.AddState(new WaitingOrderClientState());
        }

        public void Dispose()
        {
            Movable.Dispose();
            MoodChecker.Dispose();
            View.ResetBehaviour();
        }
    }
}