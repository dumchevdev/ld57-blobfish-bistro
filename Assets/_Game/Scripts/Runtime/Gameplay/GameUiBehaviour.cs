using UnityEngine;

namespace Game.Runtime._Game.Scripts.Runtime.Gameplay
{
    public class GameUiBehaviour : MonoBehaviour
    {
        [SerializeField] private GameObject disableUIInput;

        private void Start()
        {
            SetActivateUIInput(false);
        }

        public void SetActivateUIInput(bool activate)
        {
            disableUIInput.SetActive(activate);
        }
    }
}