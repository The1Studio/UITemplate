using UnityEngine;

namespace UITemplate.Packages.UnityLogsViewer.Reporter
{
    public class ReporterGUI : MonoBehaviour
    {
        Reporter reporter;

        void Awake() { this.reporter = this.gameObject.GetComponent<Reporter>(); }

        void OnGUI() { this.reporter.OnGUIDraw(); }
    }
}