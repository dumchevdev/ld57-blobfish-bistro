using DG.Tweening;
using Game.Runtime.ServiceLocator;

namespace Game.Runtime.Framework.Services.Camera
{
    public class CameraService : IService
    {
        private readonly UnityEngine.Camera _camera;

        public CameraService(UnityEngine.Camera camera)
        {
            _camera = camera;
        }
        
        public void Shake(float duration, float strength)
        {
            _camera.DOShakePosition(duration, strength, 10, 45f);
        }

        public void UIShake()
        {
            Shake(0.025f, 0.4f);
        }
    }
}