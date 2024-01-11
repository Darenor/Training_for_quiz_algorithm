using System.Collections.Generic;
using Entitas;

namespace LibraryModule
{
    public class WordsLearnedOnOneOfLanguagesSystem : ReactiveSystem<LibraryEntity>
    {
        private readonly IGroup<ScrollEntity> _words;

        public WordsLearnedOnOneOfLanguagesSystem(LibraryContext libraryContext, ScrollContext scrollContext) : base(libraryContext)
        {
            _words = scrollContext.GetGroup(ScrollMatcher.AllOf(ScrollMatcher.Word, ScrollMatcher.WordAvailable,
                ScrollMatcher.Id));
        }

        protected override ICollector<LibraryEntity> GetTrigger(IContext<LibraryEntity> context)
        {
            return context.CreateCollector(LibraryMatcher.CompleteTask.Added());
        }

        protected override bool Filter(LibraryEntity entity)
        {
            return (entity.isTargetTrainingQuiz || entity.isNativeTrainingQuiz || entity.isPictureTrainingQuiz) && entity.hasQuizTasksForTraining;
        }

        protected override void Execute(List<LibraryEntity> entities)
        {
            foreach (var quizTasks in entities)
            {
                foreach (var wordEntity in _words.GetEntities())
                {
                    //*E If all word learned components are true than word goes to learning progress
                    if(quizTasks.quizTasksForTraining.taskValue.QuestionWordId != wordEntity.id.value) continue;

                    if (quizTasks.isNativeTrainingQuiz)
                    {
                        wordEntity.ReplaceWordLearnedOnNative(true);
                    }
                    
                    if (quizTasks.isTargetTrainingQuiz)
                    {
                        wordEntity.ReplaceWordLearnedOnTarget(true);
                    }

                    if (quizTasks.isPictureTrainingQuiz)
                    {
                        wordEntity.ReplaceWordLearnedWithPictures(true);
                    }
                }
            }
        }
    }
}