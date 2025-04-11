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
    public class CustomerFactory
    {
        private int _counter;
        //private readonly List<ClientData> _clientsPool = new();
        
        public CustomerData CreateClient(CMSEntity clientModel)
        {
            CustomerData customerData = null;
            
            // if (_clientsPool.Count > 0)
            // {
            //     clientData = _clientsPool[0];
            //     ConfigureClient(clientData, clientModel);
            //     
            //     _clientsPool.RemoveAt(0);
            //     return clientData;
            // }
            
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
            var animator = clientModel.GetComponent<ClientAnimationClipsComponent>().ClientAnimators.GetRandom();
            
            customerData.Id = _counter;
            customerData.View.Id = _counter;
            _counter++;

            customerData.View.SetAnimator(animator);
            customerData.Mood = CustomerMood.Happy;
            
            customerData.MoodChecker.StartMoodTimer().Forget();
            customerData.View.transform.position = spawnPoint.position;
            
            customerData.StateMachine.ChangeState<EmptyClientState>();
            customerData.View.ResetBehaviour();
            customerData.View.gameObject.SetActive(true);
        }

        // public void Return(ClientData clientData)
        // {
        //     clientData.Id = -1;
        //     clientData.View.Id = -1;
        //     clientData.View.gameObject.SetActive(false);
        //     
        //     _clientsPool.Add(clientData);
        // }

        // public void Dispose()
        // {
        //     foreach (var client in _clientsPool)
        //         client.Dispose();
        // }
    }
}