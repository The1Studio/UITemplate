namespace TheOneStudio.UITemplate.UITemplate.Scenes.ATT
{
    using ServiceImplementation.AdsServices.ConsentInformation;
    using UnityEngine;
    using UnityEngine.SceneManagement;
    using UnityEngine.UI;

    public class AppTrackingController : MonoBehaviour
    {
        [SerializeField] private GameObject attView;
        [SerializeField] private Button     btnRequestTracking;

        private void Awake()
        {
            this.CheckRequestTracking();
            this.btnRequestTracking.onClick.AddListener(this.OnClickRequestTracking);
        }

        private void CheckRequestTracking()
        {
            if (AttHelper.IsRequestTrackingComplete())
            {
                this.attView.gameObject.SetActive(false);
                LoadLoadingScene();
            }
            else
            {
                this.attView.gameObject.SetActive(true);
            }
        }

        private async void OnClickRequestTracking()
        {
            this.btnRequestTracking.interactable = false;
            if (!AttHelper.IsRequestTrackingComplete())
            {
#if UNITY_IOS
                Unity.Advertisement.IosSupport.ATTrackingStatusBinding.RequestAuthorizationTracking();
                await Cysharp.Threading.Tasks.UniTask.WaitUntil(AttHelper.IsRequestTrackingComplete);
#endif
            }

            LoadLoadingScene();
        }

        private static void LoadLoadingScene() { SceneManager.LoadScene("0.LoadingScene"); }
    }
}