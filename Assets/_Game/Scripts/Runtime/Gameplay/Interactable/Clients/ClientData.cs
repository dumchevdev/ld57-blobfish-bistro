using System;
using Game.Runtime.Gameplay.Character;

namespace Game.Runtime.Gameplay.Interactives
{
    public class ClientData : IDisposable
    {
        public int Id;
        public int TableId;

        public ClientMood Mood 
        {
            get => _mood;
            set
            {
                View.SetMood(value);
                _mood = value;
            }
        }

        public ClientState State
        {
            get => _state;
            set
            {
                StateChecker.HandleStateChanged(value);
                _state = value;
            }
        }

        public ClientOrderData OrderData;
        public ClientBehaviour View;
        public MovableBehaviour Movable;
        public ClientStateChecker StateChecker;
        public ClientMoodChecker MoodChecker;

        private ClientState _state;
        private ClientMood _mood;
        
        public ClientData(int id, ClientBehaviour view, MovableBehaviour movable)
        {
            Id = id;
            View = view;
            Movable = movable;
            StateChecker = new ClientStateChecker(this);
            MoodChecker = new ClientMoodChecker(this);
            TableId = -1;
        }

        public void Dispose()
        {
            Movable.Dispose();
            MoodChecker.Dispose();
            StateChecker.Dispose();
        }
    }
}