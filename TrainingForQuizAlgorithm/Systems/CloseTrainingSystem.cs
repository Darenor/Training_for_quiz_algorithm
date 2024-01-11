using System.Collections.Generic;
using Entitas;

namespace LibraryModule
{
    public class CloseTrainingSystem : ReactiveSystem<LibraryEntity>
    {
        private readonly IGroup<LibraryEntity> _wordInfoWindow;
        private readonly IGroup<LibraryEntity> _specialTrainingTasks;

        public CloseTrainingSystem(LibraryContext libraryContext) : base(libraryContext)
        {
            _wordInfoWindow = libraryContext.GetGroup(LibraryMatcher.AllOf(LibraryMatcher.WordInfoWindow,
                LibraryMatcher.WordInfoNativeAndTargetWord, LibraryMatcher.Enabled));

            _specialTrainingTasks = libraryContext.GetGroup(LibraryMatcher.AllOf(LibraryMatcher.SpecialTraining));
        }

        protected override ICollector<LibraryEntity> GetTrigger(IContext<LibraryEntity> context)
        {
            return context.CreateCollector(LibraryMatcher.CloseButtonAction.Added());
        }

        protected override bool Filter(LibraryEntity entity)
        {
            return true;
        }

        protected override void Execute(List<LibraryEntity> entities)
        {
            foreach (var wordInfoWindowEntity in _wordInfoWindow.GetEntities())
            {
                wordInfoWindowEntity.ReplaceEnabled(false);
                wordInfoWindowEntity.ReplaceWordInfoNativeAndTargetWord("", "");

                foreach (var specialTrainingEntity in _specialTrainingTasks.GetEntities())
                {
                    specialTrainingEntity.Destroy();
                }
            }
        }
    }
}