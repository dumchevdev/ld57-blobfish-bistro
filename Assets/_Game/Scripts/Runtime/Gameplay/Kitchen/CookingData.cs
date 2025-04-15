using Game.Runtime._Game.Scripts.Runtime.CMS.Components.Gameplay;
using Game.Runtime._Game.Scripts.Runtime.Gameplay.Dishes;

namespace Game.Runtime._Game.Scripts.Runtime.Gameplay.Kitchen
{
    public class CookingData
    {
        public readonly DinnerComponent Model;
        public readonly DinnerBehaviour DinnerBehaviour;
        public readonly DinnerPointData DinnerPointData;

        public CookingData(DinnerComponent model, DinnerBehaviour dinnerBehaviour, DinnerPointData dinnerPointData)
        {
            Model = model;
            DinnerBehaviour = dinnerBehaviour;
            DinnerPointData = dinnerPointData;
        }
    }
}