using System.Collections.Generic;
using Entitas;
using UnityEngine;

namespace LibraryModule
{
    public class OpenNewTaskSystem : ReactiveSystem<LibraryEntity>
    {
        private readonly IGroup<LibraryEntity> _taskCreatedGroup;

        public OpenNewTaskSystem(LibraryContext libraryContext) : base(libraryContext)
        {
            _taskCreatedGroup = libraryContext.GetGroup(LibraryMatcher.AllOf(LibraryMatcher.SpecialTraining,
                LibraryMatcher.IndexTasks).NoneOf(LibraryMatcher.InfoWindowTask));
        }

        protected override ICollector<LibraryEntity> GetTrigger(IContext<LibraryEntity> context)
        {
            return context.CreateCollector(LibraryMatcher.DealyBeforeOpenNewTask.Added());
        }

        protected override bool Filter(LibraryEntity entity)
        {
            return entity.hasDealyBeforeOpenNewTask;
        }

        protected override void Execute(List<LibraryEntity> entities)
        {
            foreach (var entity in entities)
            {
                entity.ReplaceDealyBeforeOpenNewTask(entity.dealyBeforeOpenNewTask.value - Time.deltaTime);
                
                if (entity.dealyBeforeOpenNewTask.value <= 0)
                {
                    LibraryEntity _continueEnt = Contexts.sharedInstance.library.CreateEntity();
                    _continueEnt.requestTaskCreated = true;
                    _continueEnt.ReplaceIndexTasks(_taskCreatedGroup.GetEntities().Length - 1);;
                    entity.RemoveDealyBeforeOpenNewTask();
                }
            }
        }
    }
}