namespace TheOneStudio.UITemplate.UITemplate.Installers
{
    using BlueprintFlow.Signals;
    using GameFoundation.Scripts.Utilities.Extension;
    using Zenject;

    public class UITemplateDeclareSignalInstaller : Installer<UITemplateDeclareSignalInstaller>
    {
        public override void InstallBindings()
        {
            var signalList = ReflectionUtils.GetAllDerivedTypes<ISignal>();

            foreach (var localDataType in signalList)
            {
                this.Container.DeclareSignal(localDataType);
            }
        }
    }
}