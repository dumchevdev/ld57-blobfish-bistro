using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Game.Runtime.CMS;
using Game.Runtime.CMS.Commons;
using Game.Runtime.CMS.Components.Gameplay;
using Game.Runtime.Gameplay.Character;
using Game.Runtime.Gameplay.Level;
using Game.Runtime.ServiceLocator;
using Game.Runtime.Utils.Extensions;
using Object = UnityEngine.Object;

namespace Game.Runtime.Gameplay.Interactives
{
    public class ClientFactory : IDisposable
    {
        private int _counter;
        private readonly List<ClientData> _clientsPool = new();
        
        public ClientData CreateClient(CMSEntity clientModel)
        {
            ClientData clientData = null;
            
            if (_clientsPool.Count > 0)
            {
                clientData = _clientsPool[0];
                ConfigureClient(clientData, clientModel);
                
                _clientsPool.RemoveAt(0);
                return clientData;
            }
            
            var clientPrefab = clientModel.GetComponent<PrefabComponent>().Prefab;
            var clientBehaviour = Object.Instantiate(clientPrefab).GetComponent<ClientBehaviour>();
            var movableBehaviour = clientBehaviour.gameObject.AddComponent<MovableBehaviour>();
            
            clientData = new ClientData(clientBehaviour, movableBehaviour);
            ConfigureClient(clientData, clientModel);
            
            return clientData;
        }

        private void ConfigureClient(ClientData clientData, CMSEntity clientModel)
        {
            var spawnPoint = ServiceLocator<LevelPointsService>.GetService().SpawnClientPoint;
            var animator = clientModel.GetComponent<ClientAnimationClipsComponent>().ClientAnimators.GetRandom();
            
            clientData.Id = _counter;
            clientData.View.Id = _counter;
            _counter++;

            clientData.View.SetAnimator(animator);
            clientData.Mood = ClientMood.Happy;
            
            clientData.MoodChecker.StartMoodTimer().Forget();
            clientData.View.transform.position = spawnPoint.position;
            
            clientData.StateMachine.ChangeState<EmptyClientState>();
            clientData.View.gameObject.SetActive(true);
        }

        public void Return(ClientData clientData)
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