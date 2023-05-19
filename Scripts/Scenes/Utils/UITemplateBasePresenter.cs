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

    public class UITemplateBaseScreenUtils
    {
        private readonly IAnalyticServices       analyticService;
        private readonly UITemplateSoundServices soundServices;
        private Dictionary<GameObject,bool>              originalStates = new();
        
        public UITemplateBaseScreenUtils()
        {
            var diContainer = ZenjectUtils.GetCurrentContainer();
            this.analyticService = diContainer.Resolve<IAnalyticServices>();
            this.soundServices   = diContainer.Resolve<UITemplateSoundServices>();
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

#if CREATIVE
        public void SetupCreativeMode<TView>(BaseScreenPresenter<TView> presenter) where TView : IScreenView
        {
            var           creativeService = ZenjectUtils.GetCurrentContainer().Resolve<CreativeService>();
            creativeService.OnTripleTap.AddListener(() =>
            {
                // If the view is not marked as HideOnCreative, then do nothing with it
                if (presenter.View.GetType().GetCustomAttribute<CreativeAttribute>() is { HideOnCreative: false }) return;
                var oldActiveStates = new Dictionary<GameObject, bool>();

                // At First, set all active state to false
                if (!creativeService.IsShowUI)
                {
                    this.originalStates.Clear();
                }
                SetActiveRecursive(presenter.View.RectTransform, creativeService.IsShowUI, oldActiveStates);
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
                    var gameObject = transform.gameObject;
                    if (!active)
                    {
                        if (oldActiveStates is not null)
                            oldActiveStates[gameObject] = transform.gameObject.activeSelf;
                        if (!this.originalStates.ContainsKey(transform.gameObject))
                        {
                            this.originalStates.Add(transform.gameObject,gameObject.activeSelf);
                        }
                        transform.gameObject.SetActive(active);
                    }
                    else
                    {
                        if (this.originalStates is not null)
                        {
                            transform.gameObject.SetActive(this.originalStates[transform.gameObject]);
                        }
                    }
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
#endif

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
#if CREATIVE
            UITemplateBaseScreenUtils.Instance.SetupCreativeMode(this);
#endif
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
#if CREATIVE
            UITemplateBaseScreenUtils.Instance.SetupCreativeMode(this);
#endif
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
#if CREATIVE
            UITemplateBaseScreenUtils.Instance.SetupCreativeMode(this);
#endif
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
#if CREATIVE
            UITemplateBaseScreenUtils.Instance.SetupCreativeMode(this);
#endif
        }
    }
}