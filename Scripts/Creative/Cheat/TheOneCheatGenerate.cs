namespace TheOneStudio.UITemplate.UITemplate.Creative.Cheat
{
    using GameFoundation.DI;
    using GameFoundation.Scripts.AssetLibrary;

    public class TheOneCheatGenerate : IInitializable
    {
        private readonly IGameAssets gameAssets;

        public TheOneCheatGenerate(IGameAssets gameAssets) { this.gameAssets = gameAssets; }

        public void Initialize()
        {
            this.gameAssets.InstantiateAsync(nameof(TheOneCheatView), default, default);
        }
    }
}