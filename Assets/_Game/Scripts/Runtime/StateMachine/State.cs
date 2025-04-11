namespace Game.Runtime._Game.Scripts.Runtime.StateMachine
{
    public abstract class State<T> where T : class
    {
        protected T Context { get; private set; }

        public abstract void OnEnter();
        public virtual void OnExit() { }
    
        public void SetContext(T context)
        {
            Context = context;
        }
    }
}