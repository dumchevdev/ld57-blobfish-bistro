using System;
using System.Collections.Generic;
using System.Reflection;
using Game.Runtime.CMS;
using Game.Runtime.ServiceLocator;
using Game.Runtime.Utils.Helpers;
using UnityEngine;

namespace Game.Runtime.Runners
{
    public class GameRunner : MonoBehaviour
    {
        private static bool _isRunning;
        
        private readonly List<Type> _cachedServices = new();
        private readonly List<IUpdatable> _updatableServices = new();

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void InstantiateAutoSaveSystem()
        {
            if (!_isRunning)
            {
                GameObject servicedMain = new GameObject("GameRunner");
                servicedMain.AddComponent<GameRunner>();
                
                DontDestroyOnLoad(servicedMain);
                _isRunning = true;
            }
        }
        
        private void Awake()
        {
            CMSProvider.Load();
            RegisterServices();
        }

        private void Update()
        {
            foreach (var service in _updatableServices)
                service.OnUpdate();
        }

        private void RegisterServices()
        {
            var serviceTypes = Helpers.ReflectionHelper.FindAllImplementations<IService>();
            foreach (Type serviceType in serviceTypes)
            {
                try
                {
                    var service = Activator.CreateInstance(serviceType);
                    RegisterInServiceLocator(serviceType, service);
            
                    if (service is IUpdatable updatableService)
                        _updatableServices.Add(updatableService);
                    
                }
                catch (Exception ex)
                {
                    Debug.LogError($"Failed to register service {serviceType.Name}: {ex.Message}");
                }
            }
        }
        
        private void RegisterInServiceLocator(Type serviceType, object serviceInstance)
        {
            var registerMethod = typeof(ServiceLocator<>)
                .MakeGenericType(serviceType)
                .GetMethod("RegisterService", BindingFlags.Public | BindingFlags.Static);

            if (registerMethod != null)
            {
                registerMethod.Invoke(null, new[] { serviceInstance });
                _cachedServices.Add(serviceType);
            }
        }
        
        private void UnregisterInServiceLocator(Type serviceType)
        {
            var unregisterMethod = typeof(ServiceLocator<>)
                .MakeGenericType(serviceType)
                .GetMethod("UnregisterService", BindingFlags.Public | BindingFlags.Static);
    
            unregisterMethod?.Invoke(null, null);
        }

        private void UnregisterServices()
        {
            foreach (var cachedService in _cachedServices)
            {
                UnregisterInServiceLocator(cachedService);
            }
            
            _cachedServices.Clear();
            _updatableServices.Clear();
        }

        private void OnDestroy()
        {
            UnregisterServices();
            _isRunning = false;
        }
    }
}