using System.Collections.Generic;
using Entitas;

namespace LibraryModule
{
    public class TaskMarkedAndAddedDelayForNewOneSystem : ReactiveSystem<LibraryEntity>
    {
        public TaskMarkedAndAddedDelayForNewOneSystem(IContext<LibraryEntity> context) : base(context)
        {
            
        }

        protected override ICollector<LibraryEntity> GetTrigger(IContext<LibraryEntity> context)
        {
            return context.CreateCollector(LibraryMatcher.CompleteTask.Added(), LibraryMatcher.FailedTask.Added());
        }
        
        protected override bool Filter(LibraryEntity entity)
        {
            return entity.isSpecialTraining && entity.isInfoWindowTask == false;
        }

        protected override void Execute(List<LibraryEntity> entities)
        {
            foreach (var entity in entities)
            {
                entity.ReplaceDealyBeforeOpenNewTask(0.8f);
            }
        }
    }
}