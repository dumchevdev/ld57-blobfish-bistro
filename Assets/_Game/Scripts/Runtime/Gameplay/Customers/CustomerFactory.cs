using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Game.Runtime._Game.Scripts.Runtime.CMS;
using Game.Runtime._Game.Scripts.Runtime.CMS.Components.Commons;
using Game.Runtime._Game.Scripts.Runtime.CMS.Components.Gameplay;
using Game.Runtime._Game.Scripts.Runtime.Gameplay.Customers.States;
using Game.Runtime._Game.Scripts.Runtime.Gameplay.Level;
using Game.Runtime._Game.Scripts.Runtime.Gameplay.Unit;
using Game.Runtime._Game.Scripts.Runtime.ServiceLocator;
using Game.Runtime._Game.Scripts.Runtime.Utils.Extensions;
using Object = UnityEngine.Object;

namespace Game.Runtime._Game.Scripts.Runtime.Gameplay.Customers
{
    public class CustomerFactory : IDisposable
    {
        private int _counter;
        private readonly List<CustomerData> _clientsPool = new();
        
        public CustomerData CreateClient(CMSEntity clientModel)
        {
            CustomerData customerData = null;
            
            for (int i = 0; i < _clientsPool.Count; i++)
            {
                if (!_clientsPool[i].View.gameObject.activeInHierarchy)
                {
                    customerData = _clientsPool[i];
                    ConfigureClient(customerData, clientModel);
                    _clientsPool.RemoveAt(i);
                    return customerData;
                }
            }
            
            var clientPrefab = clientModel.GetComponent<PrefabComponent>().Prefab;
            var clientBehaviour = Object.Instantiate(clientPrefab).GetComponent<CustomerBehaviour>();
            var movableBehaviour = clientBehaviour.gameObject.AddComponent<MovableBehaviour>();
            
            customerData = new CustomerData(clientBehaviour, movableBehaviour);
            ConfigureClient(customerData, clientModel);
            
            return customerData;
        }

        private void ConfigureClient(CustomerData customerData, CMSEntity clientModel)
        {
            var spawnPoint = ServiceLocator<LevelPointsService>.GetService().SpawnClientPoint;
            var animator = clientModel.GetComponent<CustomerAnimatorsComponent>().OverrideAnimators.GetRandom();
            
            customerData.Id = _counter;
            customerData.View.Id = _counter;
            _counter++;

            customerData.MoodChecker.ResetMoodTimer();
            customerData.StateMachine.ChangeState<EmptyClientState>();
            
            customerData.View.ResetBehaviour();
            customerData.View.SetAnimator(animator);
            customerData.View.transform.position = spawnPoint.position;
            customerData.View.gameObject.SetActive(true);
        }

        public void Return(CustomerData clientData)
        {
            clientData.Id = -1;
            clientData.View.Id = -1;
            clientData.View.gameObject.SetActive(false);
            
            _clientsPool.Add(clientData);
        }

        public void Dispose()
        {
            foreach (var client in _clientsPool)
                client.Dispose();
        }
    }
}