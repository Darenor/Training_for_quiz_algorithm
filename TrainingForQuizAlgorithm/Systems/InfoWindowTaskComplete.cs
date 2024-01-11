using System.Collections.Generic;
using Entitas;

namespace LibraryModule
{
    public class InfoWindowTaskComplete : ReactiveSystem<LibraryEntity>
    {
        private readonly IGroup<LibraryEntity> _windowTask;

        public InfoWindowTaskComplete(LibraryContext libraryContext) : base(libraryContext)
        {
            _windowTask = libraryContext.GetGroup(LibraryMatcher.AllOf(LibraryMatcher.SpecialTraining,
                LibraryMatcher.InfoWindowTask, LibraryMatcher.IndexTasks, LibraryMatcher.ActiveWord).NoneOf(LibraryMatcher.CompleteTask, LibraryMatcher.FailedTask));
        }

        protected override ICollector<LibraryEntity> GetTrigger(IContext<LibraryEntity> context)
        {
            return context.CreateCollector(LibraryMatcher.ContinueButtonAction.Added());
        }

        protected override bool Filter(LibraryEntity entity)
        {
            return true;
        }

        protected override void Execute(List<LibraryEntity> entities)
        {
            foreach (var windowTaskEntity in _windowTask.GetEntities())
            {
                windowTaskEntity.isCompleteTask = true;
                windowTaskEntity.isActiveWord = false;
            }
        }
    }
}