namespace TheOneStudio.UITemplate.UITemplate.Interfaces
{
    public interface IFlashLight
    {
        void TurnOn();
        void TurnOff();

        void AutoOnOff(float time=0.1f);
    }
}