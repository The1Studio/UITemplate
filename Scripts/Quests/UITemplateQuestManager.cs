namespace TheOneStudio.UITemplate.Quests
{
    using System.Collections.Generic;
    using System.Linq;
    using GameFoundation.Scripts.Utilities.Extension;
    using TheOneStudio.UITemplate.Quests.Data;
    using Zenject;

    public class UITemplateQuestManager : IInitializable, ITickable
    {
        private readonly IInstantiator  instantiator;
        private readonly QuestBlueprint questBlueprint;
        private readonly QuestProgress  questProgress;

        private readonly Dictionary<string, UITemplateQuestController> controllers;

        public UITemplateQuestManager(
            IInstantiator  instantiator,
            QuestBlueprint questBlueprint,
            QuestProgress  questProgress
        )
        {
            this.instantiator   = instantiator;
            this.questBlueprint = questBlueprint;
            this.questProgress  = questProgress;
            this.controllers    = new Dictionary<string, UITemplateQuestController>();
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
            var record     = this.questBlueprint[id].Record;
            var progress   = this.questProgress.Storage.GetOrAdd(record.Id, () => new QuestProgress.Quest(record));
            var controller = this.instantiator.Instantiate<UITemplateQuestController>(new object[] { record, progress });
            controller.Record   = record;
            controller.Progress = progress;
            controller.Initialize();
            this.controllers.Add(id, controller);
        }
    }
}