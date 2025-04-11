using System;

namespace Game.Runtime._Game.Scripts.Runtime.ServiceLocator
{
    public static class ServiceLocator<T> where T : class, IService
    {
        private static T _service;

        public static void RegisterService(T service) => _service = service ?? throw new ArgumentNullException(nameof(service));
        public static T GetService() => _service ?? throw new InvalidOperationException($"Service of type {typeof(T)} is not registered.");
        
        public static void UnregisterService()
        {
            if (_service is IDisposable disposable)
                disposable.Dispose();
            
            _service = null;
        }
    }
}