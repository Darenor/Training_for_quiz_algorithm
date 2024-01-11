using System.Collections.Generic;
using Entitas;
using ServerSide.Shared;

namespace LibraryModule
{
    public class CloseTaskQuizSystem : ReactiveSystem<QuizEntity>
    {
        private readonly IGroup<LibraryEntity> _wordInfoWindow;
        private readonly IGroup<LibraryEntity> _specialTrainingTasks;
        private readonly IGroup<ScrollEntity> _learnedWords;
        private readonly IGroup<ScrollEntity> _scrolls;

        public CloseTaskQuizSystem(QuizContext quizContext, LibraryContext libraryContext, ScrollContext scrollContext) : base(quizContext)
        {
            _wordInfoWindow = libraryContext.GetGroup(LibraryMatcher.AllOf(LibraryMatcher.WordInfoWindow,
                LibraryMatcher.WordInfoNativeAndTargetWord, LibraryMatcher.Enabled));

            _specialTrainingTasks = libraryContext.GetGroup(LibraryMatcher.AllOf(LibraryMatcher.SpecialTraining));
            
            _learnedWords = scrollContext.GetGroup(ScrollMatcher.AllOf(ScrollMatcher.Word, ScrollMatcher.WordAvailable,
                ScrollMatcher.Id, ScrollMatcher.WordLearnedOnTarget, ScrollMatcher.WordLearnedOnNative, ScrollMatcher.WordLearnedWithPictures));
            
            _scrolls = scrollContext.GetGroup(ScrollMatcher.AllOf(ScrollMatcher.Scroll, ScrollMatcher.Id,
                ScrollMatcher.Available, ScrollMatcher.DisplayInfo, ScrollMatcher.WordIds, ScrollMatcher.TrainingInProgress));
        }

        protected override ICollector<QuizEntity> GetTrigger(IContext<QuizEntity> context)
        {
            return context.CreateCollector(QuizMatcher.AbortPlay.Added());
        }

        protected override bool Filter(QuizEntity entity)
        {
            return entity.gameType.Value == QuizGameType.Learning2;
        }

        protected override void Execute(List<QuizEntity> entities)
        {
            foreach (var wordInfoWindowEntity in _wordInfoWindow.GetEntities())
            {
                wordInfoWindowEntity.ReplaceEnabled(false);
                wordInfoWindowEntity.ReplaceWordInfoNativeAndTargetWord("", "");

                // foreach (var words in _learnedWords.GetEntities())
                // {
                //     words.ReplaceWordLearnedOnNative(false);
                //     words.ReplaceWordLearnedOnTarget(false);
                //     words.ReplaceWordLearnedWithPictures(false);
                // }

                foreach (var specialTrainingEntity in _specialTrainingTasks.GetEntities())
                {
                    specialTrainingEntity.Destroy();
                }

                foreach (var scrollEntity in _scrolls.GetEntities())
                {
                    scrollEntity.isTrainingInProgress = false;
                }
            }
        }
    }
}