using System.Collections.Generic;
using System.Linq;
using Entitas;
using ServerSide.Shared;

namespace LibraryModule
{
    public class CloseIfAllTaskCompletedSystem : ReactiveSystem<QuizEntity>
    {
        private readonly IGroup<LibraryEntity> _wordInfoWindow;
        private readonly IGroup<LibraryEntity> _tasksGroup;
        private readonly IGroup<LibraryEntity> _tasksCompleteOrFailedGroup;
        private readonly IGroup<LibraryEntity> _messageForCompletingGroup;
        private readonly IGroup<LibraryEntity> _taskSCompletedOnly;
        private readonly IGroup<LibraryEntity> _messageForFailedGroup;
        private readonly IGroup<ScrollEntity> _learnedWords;
        private readonly IGroup<ScrollEntity> _scrolls;

        public CloseIfAllTaskCompletedSystem(LibraryContext libraryContext, ScrollContext scrollContext, QuizContext quizContext) : base(quizContext)
        {
            _wordInfoWindow = libraryContext.GetGroup(LibraryMatcher.AllOf(LibraryMatcher.WordInfoWindow, LibraryMatcher.Enabled));
            
            _messageForCompletingGroup = libraryContext.GetGroup(LibraryMatcher.AllOf(LibraryMatcher.CompletedMessageAfterTraining, LibraryMatcher.Enabled));

            _messageForFailedGroup = libraryContext.GetGroup(LibraryMatcher.AllOf(LibraryMatcher.FailedMessageAfterTraining, LibraryMatcher.Enabled));
            
            _tasksGroup = libraryContext.GetGroup(LibraryMatcher.AllOf(LibraryMatcher.SpecialTraining, LibraryMatcher.IndexTasks));
            
            
            _taskSCompletedOnly = libraryContext.GetGroup(LibraryMatcher.AllOf(LibraryMatcher.SpecialTraining, LibraryMatcher.IndexTasks, LibraryMatcher.CompleteTask));
            
            _tasksCompleteOrFailedGroup = libraryContext.GetGroup(LibraryMatcher.AllOf(LibraryMatcher.SpecialTraining, LibraryMatcher.IndexTasks).AnyOf(LibraryMatcher.CompleteTask, LibraryMatcher.FailedTask));
            
            _scrolls = scrollContext.GetGroup(ScrollMatcher.AllOf(ScrollMatcher.Scroll, ScrollMatcher.Id,
                ScrollMatcher.Available, ScrollMatcher.DisplayInfo, ScrollMatcher.WordIds, ScrollMatcher.TrainingInProgress));
        }

        protected override ICollector<QuizEntity> GetTrigger(IContext<QuizEntity> context)
        {
            return context.CreateCollector(QuizMatcher.QuizComplete.Added());
        }

        protected override bool Filter(QuizEntity entity)
        {
            return entity.hasGameType && entity.isOpened && entity.hasMode && entity.gameType.Value == QuizGameType.Learning2;
        }

        protected override void Execute(List<QuizEntity> entities)
        {
            var messageWindowForCompletedTrainingEntity = _messageForCompletingGroup.GetEntities().FirstOrDefault();
            var messageWindowForFailedTrainingEntity = _messageForFailedGroup.GetEntities().FirstOrDefault();
            
            foreach (var wordInfoWindowEntity in _wordInfoWindow.GetEntities())
            {
                if (_tasksCompleteOrFailedGroup.GetEntities().Length != _tasksGroup.GetEntities().Length) continue;

                if (_taskSCompletedOnly.GetEntities().Length == _tasksGroup.GetEntities().Length)
                {
                    if (messageWindowForCompletedTrainingEntity != null) messageWindowForCompletedTrainingEntity.ReplaceEnabled(true);
                    wordInfoWindowEntity.ReplaceEnabled(false);
                }
                else
                {
                    if(messageWindowForFailedTrainingEntity != null) messageWindowForFailedTrainingEntity.ReplaceEnabled(true); 
                    wordInfoWindowEntity.ReplaceEnabled(false);
                }
            }

            foreach (var scrollEntity in _scrolls.GetEntities())
            {
                scrollEntity.isTrainingInProgress = false;
            }
        }
    }
}