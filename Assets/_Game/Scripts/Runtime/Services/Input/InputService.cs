using Game.Runtime.ServiceLocator;

namespace Game.Runtime.Services.Input
{
    public class InputService : IService, IUpdatable
    {
        public bool OnMouseClickedAfFrame;

        public void OnUpdate()
        {
            OnMouseClickedAfFrame = UnityEngine.Input.GetMouseButtonDown(0);
        }
    }
}