namespace TheOneStudio.HyperCasual.DrawCarBase.Scripts.Runtime.Scenes.Building
{
    using UnityEngine;
    using Zenject;

    public class BuildingJoyStick : MonoBehaviour
    {
        #region zenject

        private BuildingCarController buildingCarController;

        [Inject]
        public void Init(BuildingCarController buildingCarController) { this.buildingCarController = buildingCarController; }

        #endregion

        private void FixedUpdate()
        {
            var h = UltimateJoystick.GetHorizontalAxis("move");
            var v = UltimateJoystick.GetVerticalAxis("move");
            this.buildingCarController.Moving(v, h);
        }
    }
}