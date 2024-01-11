using System;
using System.Collections.Generic;
using Entitas;

namespace LibraryModule
{
    public class OnCompletedAllGroupTaskActivateNewOneSystem : ReactiveSystem<LibraryEntity>
    {
        private readonly IGroup<LibraryEntity> _taskFirstGroup;
        private readonly IGroup<LibraryEntity> _taskSecondGroup;
        private readonly IGroup<LibraryEntity> _taskThirdGroup;

        public OnCompletedAllGroupTaskActivateNewOneSystem(LibraryContext libraryContext) : base(libraryContext)
        {
            _taskFirstGroup = libraryContext.GetGroup(LibraryMatcher.AllOf(LibraryMatcher.SpecialTraining, LibraryMatcher.FirstGroupMark).NoneOf(LibraryMatcher.CompleteTask, LibraryMatcher.FailedTask));
            
            _taskSecondGroup = libraryContext.GetGroup(LibraryMatcher.AllOf(LibraryMatcher.SpecialTraining, LibraryMatcher.SecondGroupMark).NoneOf(LibraryMatcher.FirstGroupMark));
            
            _taskThirdGroup = libraryContext.GetGroup(LibraryMatcher.AllOf(LibraryMatcher.SpecialTraining, LibraryMatcher.ThirdGroupMark).NoneOf(LibraryMatcher.FirstGroupMark));

        }

        protected override ICollector<LibraryEntity> GetTrigger(IContext<LibraryEntity> context)
        {
            return context.CreateCollector(LibraryMatcher.CompleteTask);
        }

        protected override bool Filter(LibraryEntity entity)
        {
            return entity.isNativeTrainingQuiz || entity.isTargetTrainingQuiz;
        }

        protected override void Execute(List<LibraryEntity> entities)
        {
            foreach (var firstTaskGroup in entities)
            {
                if (_taskFirstGroup.GetEntities().Length != 0) continue;
                if (_taskSecondGroup.GetEntities().Length != 0)
                {
                    foreach (var secondGroupTaskEntity in _taskSecondGroup.GetEntities())
                    {
                        secondGroupTaskEntity.isFirstGroupMark = true;
                        secondGroupTaskEntity.isSecondGroupMark = false;
                    }
                }
                else
                {
                    foreach (var thirdGroupTaskEntity in _taskThirdGroup.GetEntities())
                    {
                        thirdGroupTaskEntity.isFirstGroupMark = true;
                        thirdGroupTaskEntity.isThirdGroupMark = false;
                    }
                }
            }
        }
    }
}