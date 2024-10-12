using UnityEngine;
using System.Collections;

public class ReporterGUI : MonoBehaviour
{
    private Reporter reporter;

    private void Awake()
    {
        this.reporter = this.gameObject.GetComponent<Reporter>();
    }

    private void OnGUI()
    {
        this.reporter.OnGUIDraw();
    }
}