using System.Collections.Generic;
using Entitas;
using ServerSide.Shared;
using UnityEngine;

namespace LibraryModule
{
    public class MoveTaskOnFailedToNewGroup : ReactiveSystem<LibraryEntity>
    {
        private readonly IGroup<LibraryEntity> _taskSecondGroup;
        private readonly IGroup<LibraryEntity> _taskThirdGroup;

        public MoveTaskOnFailedToNewGroup(LibraryContext libraryContext) : base(libraryContext)
        {
            _taskSecondGroup = libraryContext.GetGroup(LibraryMatcher.AllOf(LibraryMatcher.SpecialTraining, LibraryMatcher.SecondGroupMark).NoneOf(LibraryMatcher.FirstGroupMark));
            
            _taskThirdGroup = libraryContext.GetGroup(LibraryMatcher.AllOf(LibraryMatcher.SpecialTraining, LibraryMatcher.ThirdGroupMark).NoneOf(LibraryMatcher.FirstGroupMark));
        }
        
        protected override ICollector<LibraryEntity> GetTrigger(IContext<LibraryEntity> context)
        {
            return context.CreateCollector(LibraryMatcher.MoveToAnotherGroup.Added());
        }
        protected override bool Filter(LibraryEntity entity)
        {
           return entity.isSpecialTraining;
        }
        protected override void Execute(List<LibraryEntity> entities)
        {
            foreach (var taskFailedEntity in entities)
            {
                if (_taskSecondGroup.GetEntities().Length != 0)
                {
                    foreach (var secondGroupEntity in _taskSecondGroup.GetEntities())
                    {
                        secondGroupEntity.isFirstGroupMark = true;
                        secondGroupEntity.isSecondGroupMark = false;
                        break;
                    }
                }
                else
                {
                    foreach (var thirdGroupEntity in _taskThirdGroup.GetEntities())
                    {
                        thirdGroupEntity.isFirstGroupMark = true;
                        thirdGroupEntity.isThirdGroupMark = false;
                        break;
                    }
                }
            }
        }
    }
}
