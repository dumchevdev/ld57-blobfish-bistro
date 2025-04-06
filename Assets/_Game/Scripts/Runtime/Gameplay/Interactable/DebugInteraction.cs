using Game.Runtime.ServiceLocator;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Game.Runtime.Gameplay.Interactives
{
    public class DebugInteraction : MonoBehaviour, IPointerClickHandler
    {
        public bool isTable;
        
        public void OnPointerClick(PointerEventData eventData)
        {
            if (isTable)
            {
                var table = gameObject.GetComponent<TableBehaviour>();
                var qwer = ServiceLocator<TableService>.GetService().GetTable(table.Id);
                Debug.Log($"Table data: {qwer.Id}, {qwer.Food}, {qwer.Client?.Id}, {qwer.Behaviour._current.DebugName}");
                return;
            }
            else
            {
                var client = gameObject.GetComponent<ClientBehaviour>();
                var qwer = ServiceLocator<ClientsService>.GetService().GetClient(client.Id);
                if (qwer != null)
                    Debug.Log($"Client data: {qwer.Id}, {qwer.TableId}, {qwer.State}");
            }
        }
    }
}