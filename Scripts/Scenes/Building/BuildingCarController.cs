namespace TheOneStudio.HyperCasual.DrawCarBase.Scripts.Runtime.Scenes.Building
{
    using GameFoundation.Scripts.Utilities.ObjectPool;
    using TheOneStudio.UITemplate.UITemplate.Models.Controllers;
    using UnityEngine;
    using Zenject;

    public class BuildingCarController : MonoBehaviour
    {
        #region View

        public Transform carholder;

        #endregion

        public  float     movementSpeed = 10f;
        private Rigidbody rig => this.GetComponent<Rigidbody>();

        [Inject] private ObjectPoolManager                 objectPoolManager;
        [Inject] private UITemplateInventoryDataController inventoryDataController;
        [Inject] private BuildingRunningObjectFactory      buildingRunningObjectFactory;

        [Inject]
        public async void Init()
        {

            var runningObiect = await buildingRunningObjectFactory.Create();
            runningObiect.transform.SetParent(this.carholder);
            runningObiect.transform.localPosition = Vector3.zero;
            runningObiect.transform.localRotation = Quaternion.identity;
        }

        public void Moving(float v, float h)
        {
            if (h == 0 || v == 0)
            {
                this.rig.velocity       = Vector3.zero;
                this.rig.freezeRotation = true;

                return;
            }

            this.rig.freezeRotation = false;
            this.rig.velocity       = (new Vector3(-v, 0, h) * this.movementSpeed).normalized * this.movementSpeed;
            var lookRot = new Vector3(v, 0, -h);

            if (lookRot == Vector3.zero)
            {
                return;
            }

            this.transform.LookAt(this.transform.position + lookRot);
        }

        private void OnTriggerStay(Collider other)
        {
            var script = other.gameObject.GetComponent<BuildingBuildingElement>();

            if (script != null)
            {
                script.CheckToUnlockBuilding();
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            var script = other.gameObject.GetComponent<BuildingBuildingElement>();

            if (script != null)
            {
                script.OnCarEnter();
            }
        }

        private void OnTriggerExit(Collider other)
        {
            var script = other.gameObject.GetComponent<BuildingBuildingElement>();

            if (script != null)
            {
                script.OnCarExit();
            }
        }
    }
}