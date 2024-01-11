using System;
using System.Collections.Generic;
using Entitas;

namespace LibraryModule
{
    public class RestartTrainingReactSystem : ReactiveSystem<ScrollEntity>
    {
        private readonly IGroup<LibraryEntity> _tasksGroup;

        public RestartTrainingReactSystem(LibraryContext libraryContext, ScrollContext scrollContext) : base(scrollContext)
        {
            _tasksGroup = libraryContext.GetGroup(LibraryMatcher.AllOf(LibraryMatcher.SpecialTraining, LibraryMatcher.IndexTasks));
        }

        protected override ICollector<ScrollEntity> GetTrigger(IContext<ScrollEntity> context)
        {
            return context.CreateCollector(ScrollMatcher.RestartTrainingWithWholeScrollLanguage);
        }

        protected override bool Filter(ScrollEntity entity)
        {
            return true;
        }

        protected override void Execute(List<ScrollEntity> entities)
        {
            foreach (var taskEntity in _tasksGroup.GetEntities())
            {
                taskEntity.Destroy();
            }
            
            Contexts.sharedInstance.scroll.CreateEntity().requestStartTrainingWithWholeScrollLanguage = true;
        }
    }
}