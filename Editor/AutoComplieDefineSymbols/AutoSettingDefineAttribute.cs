namespace UITemplate.Editor.AutoComplieDefineSymbols
{
    using UnityEngine;

    public class AutoSettingDefineAttribute : PropertyAttribute
    {
        public bool IsDownDown;

        public AutoSettingDefineAttribute(bool isDownDown) { this.IsDownDown = isDownDown; }
    }
}