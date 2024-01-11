using System.Collections.Generic;
using System.Linq;
using Entitas;

namespace LibraryModule
{
    public class TaskFailedShowRightAnswerSystem : ReactiveSystem<LibraryEntity>
    {
        private readonly IGroup<LibraryEntity> _incorretInfoWindow;
        private readonly IGroup<QuizEntity> _optionSelectedByPlayer;
        private readonly IGroup<ScrollEntity> _words;
        private readonly IGroup<QuizEntity> _optionCorrectInQuiz;

        public TaskFailedShowRightAnswerSystem(LibraryContext libraryContext, QuizContext quizContext, ScrollContext scrollContext) : base(libraryContext)
        {
            _incorretInfoWindow = libraryContext.GetGroup(LibraryMatcher.AllOf(LibraryMatcher.IncorrectAnswerWindow,
                LibraryMatcher.Enabled));

            _optionSelectedByPlayer = quizContext.GetGroup(QuizMatcher.AllOf(QuizMatcher.Option,
                QuizMatcher.Index, QuizMatcher.OptionSelectedByPlayer));

            _optionCorrectInQuiz = quizContext.GetGroup(QuizMatcher.AllOf(QuizMatcher.Option, QuizMatcher.OptionCorrect));
            
            _words = scrollContext.GetGroup(ScrollMatcher.AllOf(ScrollMatcher.Word, ScrollMatcher.WordAvailable,
                ScrollMatcher.Id));
        }

        protected override ICollector<LibraryEntity> GetTrigger(IContext<LibraryEntity> context)
        {
            return context.CreateCollector(LibraryMatcher.FailedTask);
        }

        protected override bool Filter(LibraryEntity entity)
        {
            return entity.isSpecialTraining && entity.isActiveWord;
        }

        protected override void Execute(List<LibraryEntity> entities)
        {
            var optionCorrectEntity = _optionCorrectInQuiz.GetEntities().FirstOrDefault();
            foreach (var incorrectInfoWindowEntity in _incorretInfoWindow.GetEntities())
            {
                incorrectInfoWindowEntity.ReplaceEnabled(true);
                foreach (var optionEntity in _optionSelectedByPlayer.GetEntities())
                {
                    foreach (var wordEntity in _words.GetEntities())
                    {
                        if (wordEntity.id.value == optionEntity.wordId.value)
                            incorrectInfoWindowEntity.ReplaceIncorrectAnswerNativeAndTargetWord(
                                wordEntity.wordStaticData.Native, wordEntity.wordStaticData.Target);
                        if (optionCorrectEntity != null && wordEntity.id.value == optionCorrectEntity.wordId.value)
                            incorrectInfoWindowEntity.ReplaceWordInfoNativeAndTargetWord(
                                wordEntity.wordStaticData.Native, wordEntity.wordStaticData.Target);
                    }
                }
            }
        }
    }
}