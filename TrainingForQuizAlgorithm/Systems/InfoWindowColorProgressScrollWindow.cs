using System.Collections.Generic;
using Entitas;
using ScrollModule;

namespace LibraryModule
{
    public sealed class InfoWindowColorProgressScrollWindow : ReactiveSystem<LibraryEntity>
    {
        private readonly LibraryConfig _libraryConfig;

        public InfoWindowColorProgressScrollWindow(LibraryContext libraryContext, LibraryConfig libraryConfig) : base(libraryContext)
        {
            _libraryConfig = libraryConfig;
        }

        protected override ICollector<LibraryEntity> GetTrigger(IContext<LibraryEntity> context)
        {
            return context.CreateCollector(LibraryMatcher.UIScrollTrainingProgress);
        }

        protected override bool Filter(LibraryEntity entity)
        {
            return entity.isScrollWindow;
        }

        protected override void Execute(List<LibraryEntity> entities)
        {
            foreach (var scrollWindowEntity in entities)
            {
                scrollWindowEntity.ReplaceUIScrollTrainingProgressColor(_libraryConfig.GetLearningProgressColor(scrollWindowEntity.uIScrollTrainingProgress.value));
                break;
            }
        }
    }
}