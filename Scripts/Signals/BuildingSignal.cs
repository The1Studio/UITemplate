namespace TheOneStudio.UITemplate.UITemplate.Signals
{
    public class BuildingSignal
    {
        
    }

    public class BuildingOnMouseDownSignal
    {
        
    }

    public class UITemplateUnlockBuildingSignal
    {
        public bool IsUnlockSuccess;

        public UITemplateUnlockBuildingSignal(bool isUnlockSuccess)
        {
            this.IsUnlockSuccess = isUnlockSuccess;
        }
    }
}