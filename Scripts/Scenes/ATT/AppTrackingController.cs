namespace TheOneStudio.UITemplate.UITemplate.Scenes.ATT
{
    using GameFoundation.DI;
    using ServiceImplementation.AdsServices.ConsentInformation;
    using UnityEngine;
    using UnityEngine.SceneManagement;
    using UnityEngine.UI;

    public class AppTrackingController : MonoBehaviour
    {
        [SerializeField] private GameObject attView;
        [SerializeField] private GameObject goLoadingScreen;
        [SerializeField] private Button     btnRequestTracking;

        private AppTrackingServices appTrackingServices;
        private AppTrackingServices AppTrackingServices => this.appTrackingServices ??= this.GetCurrentContainer().Resolve<AppTrackingServices>();

        private void Awake()
        {
            this.SetActiveLoadingScreen(false);
            this.CheckRequestTracking();
            this.btnRequestTracking.onClick.AddListener(this.OnClickRequestTracking);
        }

        private void CheckRequestTracking()
        {
            if (this.AppTrackingServices.IsTrackingComplete())
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
            // if (!this.AppTrackingServices.IsTrackingComplete())
            // {
                this.SetActiveLoadingScreen(true);
                await this.AppTrackingServices.RequestConsentAndTracking();
                this.SetActiveLoadingScreen(false);
            // }

            LoadLoadingScene();
        }

        private void SetActiveLoadingScreen(bool isActive)
        {
            if (this.goLoadingScreen) this.goLoadingScreen.SetActive(isActive);
        }

        private static void LoadLoadingScene()
        {
            SceneManager.LoadScene("0.LoadingScene");
        }
    }
}