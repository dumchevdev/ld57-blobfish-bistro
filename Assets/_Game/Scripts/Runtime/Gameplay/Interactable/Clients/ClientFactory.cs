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
            var sprite = clientModel.GetComponent<ClientAnimationClipsComponent>().ClientAnimators.GetRandom();
            var spawnPoint = ServiceLocator<LevelPointsService>.GetService().SpawnClientPoint;
            
            if (_clientsPool.Count > 0)
            {
                clientData = _clientsPool[0];
                clientData.Id = _counter;
                clientData.TableId = -1;
                
                clientData.View.SetAnimator(sprite);
                clientData.Mood = ClientMood.Happy;
                
                var qwer = clientData.View.GetComponent<InteractableObject>();
                qwer.SetId(_counter);
                
                clientData.MoodChecker.StartMoodTimer().Forget();
                clientData.View.transform.position = spawnPoint.position;
                
                clientData.View.gameObject.SetActive(true);
                
                _counter++;
                _clientsPool.RemoveAt(0);

                return clientData;
            }
            
            var clientPrefab = clientModel.GetComponent<PrefabComponent>().Prefab;
            var clientBehaviour = Object.Instantiate(clientPrefab).GetComponent<ClientBehaviour>();
            var movableBehaviour = clientBehaviour.gameObject.AddComponent<MovableBehaviour>();
            
            var interactableObject = clientBehaviour.GetComponent<InteractableObject>();
            interactableObject.SetId(_counter);
            
            clientData = new ClientData(_counter, clientBehaviour, movableBehaviour);
            clientData.View.SetAnimator(sprite);
            clientData.Mood = ClientMood.Happy;
            clientData.MoodChecker.StartMoodTimer().Forget();
            
            clientData.View.transform.position = spawnPoint.position;
            _counter++;
            
            return clientData;
        }

        public void Return(ClientData clientData)
        {
            clientData.Id = -1;
            clientData.TableId = -1;
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