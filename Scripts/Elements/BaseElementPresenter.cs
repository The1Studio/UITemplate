namespace TheOneStudio.UITemplate.UITemplate.Elements
{
    using Cysharp.Threading.Tasks;
    using GameFoundation.Scripts.Utilities.ObjectPool;

    public interface IElementPresenter
    {
        IElementView View  { get; }
        object       Model { get; }

        UniTask DestroyAsync();
    }

    public abstract class BaseElementPresenter : IElementPresenter
    {
        public IElementView View  { get; private set; }
        public object       Model { get; private set; }

        public virtual async UniTask DestroyAsync()
        {
            await this.OnDestroyAsync();

            this.View  = null;
            this.Model = null;
        }

        public async UniTask InitializeAsync(IElementView view, object model)
        {
            this.View  = view;
            this.Model = model;

            await this.OnCreateAsync();
        }

        protected virtual UniTask OnCreateAsync()
        {
            return UniTask.CompletedTask;
        }

        protected virtual UniTask OnDestroyAsync()
        {
            return UniTask.CompletedTask;
        }
    }

    public abstract class BasePrefabElementPresenter : BaseElementPresenter
    {
        private readonly ObjectPoolManager objectPoolManager;

        protected BasePrefabElementPresenter(ObjectPoolManager objectPoolManager)
        {
            this.objectPoolManager = objectPoolManager;
        }

        protected abstract string PrefabAddressable { get; }

        public async UniTask InitializeAsync(object model)
        {
            var viewGameObject = await this.objectPoolManager.Spawn(this.PrefabAddressable);
            await this.InitializeAsync(viewGameObject.GetComponent<IElementView>(), model);
        }

        public override async UniTask DestroyAsync()
        {
            var viewGameObject = this.View.GameObject;
            await base.DestroyAsync();
            this.objectPoolManager.Recycle(viewGameObject);
        }
    }

    public abstract class BaseElementPresenter<TView, TModel> : BaseElementPresenter
    {
        public new TView  View  => (TView)base.View;
        public new TModel Model => (TModel)base.Model;
    }

    public abstract class BasePrefabElementPresenter<TView, TModel> : BasePrefabElementPresenter
    {
        protected BasePrefabElementPresenter(ObjectPoolManager objectPoolManager) : base(objectPoolManager)
        {
        }

        public async UniTask InitializeAsync(TModel model)
        {
            await base.InitializeAsync(model);
        }
    }
}