using System.Collections.Generic;
using Entitas;

namespace LibraryModule
{
    public class DestroyTasksAfterResultScreenReactiveSystem : ReactiveSystem<LibraryEntity>
    {
        private readonly IGroup<LibraryEntity> _tasksCompleteOrFailedGroup;
        private readonly IGroup<ScrollEntity> _learnedWords;

        public DestroyTasksAfterResultScreenReactiveSystem(LibraryContext libraryContext, ScrollContext scrollContext) : base(libraryContext)
        {
            _tasksCompleteOrFailedGroup = libraryContext.GetGroup(LibraryMatcher.AllOf(LibraryMatcher.SpecialTraining, LibraryMatcher.IndexTasks));
            
            _learnedWords = scrollContext.GetGroup(ScrollMatcher.AllOf(ScrollMatcher.Word, ScrollMatcher.WordAvailable,
                ScrollMatcher.Id, ScrollMatcher.WordLearnedOnTarget, ScrollMatcher.WordLearnedOnNative, ScrollMatcher.WordLearnedWithPictures));
        }

        protected override ICollector<LibraryEntity> GetTrigger(IContext<LibraryEntity> context)
        {
            return context.CreateCollector(LibraryMatcher.ResultScreenStart.Added());
        }

        protected override bool Filter(LibraryEntity entity)
        {
            return true;
        }

        protected override void Execute(List<LibraryEntity> entities)
        {
            foreach (var taskEntity in _tasksCompleteOrFailedGroup.GetEntities())
            {
                taskEntity.Destroy();
            }
            
            foreach (var words in _learnedWords.GetEntities())
            {
                words.ReplaceWordLearnedOnNative(false);
                words.ReplaceWordLearnedOnTarget(false);
                words.ReplaceWordLearnedWithPictures(false);
            }
        }
    }
}