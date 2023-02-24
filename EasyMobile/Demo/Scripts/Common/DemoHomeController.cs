using UnityEngine;
using UnityEngine.SceneManagement;

namespace EasyMobile.Demo
{
    public class DemoHomeController : MonoBehaviour
    {
        void Awake()
        {
            // Initialize EM runtime.
            if (!RuntimeManager.IsInitialized())
                RuntimeManager.Init();
        }

        void Update()
        {
            #if UNITY_ANDROID
            if (Input.GetKeyUp(KeyCode.Escape))
            {   
                // Ask if user wants to exit
                NativeUI.AlertPopup alert = NativeUI.ShowTwoButtonAlert("Exit App",
                                                "Do you want to exit?",
                                                "Yes", 
                                                "No");

                if (alert != null)
                    alert.OnComplete += delegate (int button)
                    { 
                        if (button == 0)
                            Application.Quit();
                    };
            }

            #endif
        }

        public void Restart()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }
}

