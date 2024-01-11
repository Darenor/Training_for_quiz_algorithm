using System.Collections.Generic;
using Entitas;

namespace LibraryModule
{
    public class InfoWindowProgressToScrollWindow : ReactiveSystem<ScrollEntity>
    {
        private readonly IGroup<LibraryEntity> _scrollWindow;
        private readonly IGroup<ScrollEntity> _scrolls;

        public InfoWindowProgressToScrollWindow(LibraryContext libraryContext, ScrollContext scrollContext) : base(scrollContext)
        {
            _scrollWindow = libraryContext.GetGroup(LibraryMatcher.AllOf(LibraryMatcher.ScrollWindow));
        }

        protected override ICollector<ScrollEntity> GetTrigger(IContext<ScrollEntity> context)
        {
            return context.CreateCollector(ScrollMatcher.SpecialTrainingProgress.Added());
        }

        protected override bool Filter(ScrollEntity entity)
        {
            return entity.isScroll && entity.isAvailable && entity.isDisplayInfo;
        }

        protected override void Execute(List<ScrollEntity> entities)
        {
            foreach (var scrollEntity in entities)
            {
                foreach (var scrollWindowEntity in _scrollWindow.GetEntities())
                {
                    scrollWindowEntity.ReplaceUIScrollTrainingProgress(scrollEntity.specialTrainingProgress.value);
                }
            }
        }
    }
}