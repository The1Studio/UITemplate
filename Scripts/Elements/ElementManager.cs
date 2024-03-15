namespace TheOneStudio.UITemplate.UITemplate.Elements
{
    using Cysharp.Threading.Tasks;
    using Zenject;

    public class ElementManager
    {
        private readonly DiContainer container;

        protected ElementManager(DiContainer container)
        {
            this.container = container;
        }

        public async UniTask<TPresenter> CreateAsync<TPresenter>(IElementView view, object model)
            where TPresenter : BaseElementPresenter
        {
            var presenter = this.container.Instantiate<TPresenter>();
            await presenter.InitializeAsync(view, model);
            return presenter;
        }

        public async UniTask<TPresenter> CreateAsync<TPresenter>(object model)
            where TPresenter : BasePrefabElementPresenter
        {
            var presenter = this.container.Instantiate<TPresenter>();
            await presenter.InitializeAsync(model);
            return presenter;
        }

        public async UniTask DestroyAsync<TPresenter>(TPresenter presenter)
            where TPresenter : BaseElementPresenter
        {
            await ((IElementPresenter)presenter).DestroyAsync();
        }
    }
}