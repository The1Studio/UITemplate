namespace TheOneStudio.UITemplate.Quests
{
    using System.Collections.Generic;
    using System.Linq;
    using GameFoundation.Scripts.Utilities.Extension;
    using TheOneStudio.UITemplate.Quests.Data;
    using Zenject;

    public class UITemplateQuestManager : IInitializable, ITickable
    {
        private readonly IInstantiator            instantiator;
        private readonly UITemplateQuestBlueprint questBlueprint;
        private readonly UITemplateQuestProgress  questProgress;

        private readonly Dictionary<string, UITemplateQuestController> controllers = new Dictionary<string, UITemplateQuestController>();

        public UITemplateQuestManager(
            IInstantiator            instantiator,
            UITemplateQuestBlueprint questBlueprint,
            UITemplateQuestProgress  questProgress
        )
        {
            this.instantiator   = instantiator;
            this.questBlueprint = questBlueprint;
            this.questProgress  = questProgress;
        }

        void IInitializable.Initialize()
        {
            this.questBlueprint.Keys.ForEach(this.InstantiateHandler);
        }

        void ITickable.Tick()
        {
            foreach (var (id, controller) in this.controllers.ToArray())
            {
                if (!controller.CanBeReset()) continue;
                controller.Dispose();
                this.controllers.Remove(id);
                this.questProgress.Storage.Remove(id);
                this.InstantiateHandler(id);
            }
            foreach (var controller in this.controllers.Values)
            {
                controller.UpdateStatus();
            }
        }

        public UITemplateQuestController GetController(string id)
        {
            return this.controllers[id];
        }

        public IEnumerable<UITemplateQuestController> GetAllControllers()
        {
            return this.controllers.Values;
        }

        private void InstantiateHandler(string id)
        {
            var record     = this.questBlueprint[id];
            var progress   = this.questProgress.Storage.GetOrAdd(record.Id, () => new UITemplateQuestProgress.Quest(record));
            var controller = this.instantiator.Instantiate<UITemplateQuestController>();
            controller.Record   = record;
            controller.Progress = progress;
            controller.Initialize();
            this.controllers.Add(id, controller);
        }
    }
}