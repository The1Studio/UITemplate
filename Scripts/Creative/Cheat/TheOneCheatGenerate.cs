namespace TheOneStudio.UITemplate.UITemplate.Creative.Cheat
{
    using GameFoundation.DI;
    using GameFoundation.Scripts.AssetLibrary;
    using UnityEngine.Scripting;

    public class TheOneCheatGenerate : IInitializable
    {
        private readonly IGameAssets gameAssets;

        [Preserve]
        public TheOneCheatGenerate(IGameAssets gameAssets) { this.gameAssets = gameAssets; }

        public void Initialize()
        {
            this.gameAssets.InstantiateAsync(nameof(TheOneCheatView), default, default);
        }
    }
}