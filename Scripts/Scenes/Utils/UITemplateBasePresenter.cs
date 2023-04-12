namespace TheOneStudio.UITemplate.UITemplate.Scenes.Utils
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using Core.AnalyticServices;
    using GameFoundation.Scripts.UIModule.ScreenFlow.BaseScreen.Presenter;
    using GameFoundation.Scripts.UIModule.ScreenFlow.BaseScreen.View;
    using GameFoundation.Scripts.UIModule.ScreenFlow.Managers;
    using GameFoundation.Scripts.Utilities.Extension;
    using GameFoundation.Scripts.Utilities.LogService;
    using TheOneStudio.UITemplate.UITemplate.Scripts.Services;
    using TheOneStudio.UITemplate.UITemplate.Services;
    using TheOneStudio.UITemplate.UITemplate.ThirdPartyServices.AnalyticEvents.Wido;
    using UnityEngine;
    using UnityEngine.UI;
    using Zenject;
    using Object = System.Object;

    internal class UITemplateBaseScreenUtils
    {
        private readonly IAnalyticServices       analyticService;
        private readonly UITemplateSoundServices soundServices;

#if CREATIVE
        private readonly CreativeService creativeService;
#endif

        public UITemplateBaseScreenUtils()
        {
            var diContainer = ZenjectUtils.GetCurrentContainer();
            this.analyticService = diContainer.Resolve<IAnalyticServices>();
            this.soundServices   = diContainer.Resolve<UITemplateSoundServices>();

#if CREATIVE
            this.creativeService = diContainer.Resolve<CreativeService>();
#endif
        }

        public static UITemplateBaseScreenUtils Instance { get; set; }

        public static void Init()
        {
            Instance ??= new UITemplateBaseScreenUtils();
        }

        public static void ReInit()
        {
            Instance = new UITemplateBaseScreenUtils();
        }

        private void OnClickButton(string screenName, Button button)
        {
            Init();
            this.soundServices.PlaySoundClick();
            this.analyticService.Track(new ButtonClick(screenName, button.gameObject.name));
        }

        public void BindOnClickButton(string screenName, Button[] buttons)
        {
            foreach (var button in buttons) button.onClick.AddListener(() => this.OnClickButton($"{SceneDirector.CurrentSceneName}/{screenName}", button));
        }

        public void SetupCreativeMode<TView>(BaseScreenPresenter<TView> presenter) where TView : IScreenView
        {
            this.creativeService.OnTripleTap.AddListener(() =>
            {
                // If the view is not marked as HideOnCreative, then do nothing with it
                if (presenter.View.GetType().GetCustomAttribute<CreativeAttribute>() is { HideOnCreative: false }) return;

                var oldActiveStates = new Dictionary<GameObject, bool>();

                // At First, set all active state to false
                SetActiveRecursive(presenter.View.RectTransform, this.creativeService.IsShowUI, oldActiveStates);

                // Retrieve all fields from the view
                foreach (var fieldInfo in GetAllFieldInfosIncludeBaseClass(presenter.View.GetType()))
                {
                    var gameObject = GetGameObjectFromFieldInfo(fieldInfo);

                    if (gameObject is null)
                    {
                        continue;
                    }

                    var creativeAttribute = fieldInfo.GetCustomAttribute<CreativeAttribute>() ?? new CreativeAttribute();

                    // If the field is not marked as HideOnCreative, then set the active state to the old active state
                    if (creativeAttribute is { HideOnCreative: false })
                    {
                        SetActiveForBranch(gameObject.transform, presenter.View.RectTransform.parent, oldActiveStates[gameObject]);
                    }
                }
            });

            void SetActiveRecursive(Transform transform, bool active, Dictionary<GameObject, bool> oldActiveStates = null)
            {
                try
                {
                    if (oldActiveStates is not null)
                        oldActiveStates[transform.gameObject] = transform.gameObject.activeSelf;
                    transform.gameObject.SetActive(active);
                    foreach (Transform child in transform)
                    {
                        SetActiveRecursive(child, active, oldActiveStates);
                    }
                }
                catch (Exception)
                {
                    // ignored
                }
            }

            void SetActiveForBranch(Transform transform, Transform topParent, bool active)
            {
                transform.gameObject.SetActive(active);
                foreach (Transform child in transform)
                {
                    SetActiveRecursive(child, active);
                }

                var parent = transform.parent;
                while (parent != topParent)
                {
                    parent.gameObject.SetActive(active);
                    parent = parent.parent;
                }
            }

            GameObject GetGameObjectFromFieldInfo(FieldInfo fieldInfo)
            {
                try
                {
                    var value = fieldInfo.GetValue(presenter.View);
                    if (value is GameObject gameObject)
                    {
                        return gameObject;
                    }

                    if (value is Component component)
                    {
                        return component.gameObject;
                    }
                }
                catch (Exception)
                {
                    // ignored
                }

                return null;
            }

            FieldInfo[] GetAllFieldInfosIncludeBaseClass(Type type)
            {
                if (type == null || type == typeof(object))
                {
                    return Array.Empty<FieldInfo>();
                }

                var baseClassFieldInfos = GetAllFieldInfosIncludeBaseClass(type.BaseType);
                var thisClassFieldInfos = type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                var fieldInfosSet       = new HashSet<FieldInfo>(baseClassFieldInfos, new FieldInfoComparer());
                fieldInfosSet.UnionWith(thisClassFieldInfos);
                return fieldInfosSet.ToArray();
            }
        }

        private class FieldInfoComparer : IEqualityComparer<FieldInfo>
        {
            public bool Equals(FieldInfo x, FieldInfo y)
            {
                if (x is null || y is null)
                {
                    return false;
                }

                return x.Name == y.Name && x.DeclaringType == y.DeclaringType;
            }

            public int GetHashCode(FieldInfo obj)
            {
                return obj.Name.GetHashCode() ^ (obj.DeclaringType?.GetHashCode() ?? 0);
            }
        }
    }

    public abstract class UITemplateBaseScreenPresenter<TView> : BaseScreenPresenter<TView> where TView : IScreenView
    {
        protected UITemplateBaseScreenPresenter(SignalBus signalBus) : base(signalBus)
        {
            UITemplateBaseScreenUtils.Init();
        }

        protected override void OnViewReady()
        {
            base.OnViewReady();
            UITemplateBaseScreenUtils.Instance.BindOnClickButton(this.View.GetType().Name, this.View.RectTransform.GetComponentsInChildren<Button>());
            UITemplateBaseScreenUtils.Instance.SetupCreativeMode(this);
        }
    }

    public abstract class UITemplateBaseScreenPresenter<TView, TModel> : BaseScreenPresenter<TView, TModel> where TView : IScreenView
    {
        protected UITemplateBaseScreenPresenter(SignalBus signalBus, ILogService logger) : base(signalBus, logger)
        {
            UITemplateBaseScreenUtils.Init();
        }

        protected override void OnViewReady()
        {
            base.OnViewReady();
            UITemplateBaseScreenUtils.Instance.BindOnClickButton(this.View.GetType().Name, this.View.RectTransform.GetComponentsInChildren<Button>());
            UITemplateBaseScreenUtils.Instance.SetupCreativeMode(this);
        }
    }

    public abstract class UITemplateBasePopupPresenter<TView> : BasePopupPresenter<TView> where TView : IScreenView
    {
        protected UITemplateBasePopupPresenter(SignalBus signalBus) : base(signalBus)
        {
            UITemplateBaseScreenUtils.Init();
        }

        protected override void OnViewReady()
        {
            base.OnViewReady();
            UITemplateBaseScreenUtils.Instance.BindOnClickButton(this.View.GetType().Name, this.View.RectTransform.GetComponentsInChildren<Button>());
            UITemplateBaseScreenUtils.Instance.SetupCreativeMode(this);
        }
    }

    public abstract class UITemplateBasePopupPresenter<TView, TModel> : BasePopupPresenter<TView, TModel> where TView : IScreenView
    {
        protected UITemplateBasePopupPresenter(SignalBus signalBus, ILogService logger) : base(signalBus, logger)
        {
            UITemplateBaseScreenUtils.Init();
        }

        protected override void OnViewReady()
        {
            base.OnViewReady();
            UITemplateBaseScreenUtils.Instance.BindOnClickButton(this.View.GetType().Name, this.View.RectTransform.GetComponentsInChildren<Button>());
            UITemplateBaseScreenUtils.Instance.SetupCreativeMode(this);
        }
    }
}