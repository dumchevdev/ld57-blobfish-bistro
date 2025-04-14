using System;
using Game.Runtime._Game.Scripts.Runtime.Gameplay.Customers.States;
using Game.Runtime._Game.Scripts.Runtime.Gameplay.Unit;
using Game.Runtime._Game.Scripts.Runtime.StateMachine;

namespace Game.Runtime._Game.Scripts.Runtime.Gameplay.Customers
{
    public class CustomerData : IDisposable
    {
        public int Id;
        public string ViewId;
        
        public readonly MovableBehaviour Movable;
        public readonly CustomerMoodChecker MoodChecker;
        public readonly CustomerBehaviour Behaviour;
        public readonly StateMachine<CustomerData> StateMachine;
        
        public CustomerData(CustomerBehaviour behaviour, MovableBehaviour movable)
        {
            Behaviour = behaviour;
            Movable = movable;
            
            MoodChecker = new CustomerMoodChecker(this);
            StateMachine = new StateMachine<CustomerData>(this);
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
        }
    }
}