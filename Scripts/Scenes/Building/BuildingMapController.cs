namespace TheOneStudio.HyperCasual.DrawCarBase.Scripts.Runtime.Scenes.Building
{
    using System.Collections.Generic;
    using UnityEngine;
    using Zenject;

    public class BuildingMapController : MonoBehaviour
    {
        #region Zenject

        public List<BuildingBuildingElement> buildingElements { get; private set; } = new();

        [Inject]
        public void OnInit()
        {
            var buildingArrays = this.GetComponentsInChildren<BuildingBuildingElement>();

            foreach (var buildingElement in buildingArrays)
            {
                this.buildingElements.Add(buildingElement);
            }
        }

        #endregion
    }
}