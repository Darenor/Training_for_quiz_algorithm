using System.Collections.Generic;
using System.Linq;
using Entitas;

namespace LibraryModule
{
    public class TaskWithIconsAnswersIsActiveReactiveSystem : ReactiveSystem<QuizEntity>
    {
        private readonly IGroup<LibraryEntity> _trainingTasks;
        private readonly IGroup<QuizEntity> _quiz;

        public TaskWithIconsAnswersIsActiveReactiveSystem(Contexts contexts) : base(contexts.quiz)
        {
            _trainingTasks = contexts.library.GetGroup(LibraryMatcher.AllOf(LibraryMatcher.SpecialTraining,
                LibraryMatcher.IndexTasks, LibraryMatcher.QuizTasks));
            _quiz = contexts.quiz.GetGroup(QuizMatcher.Quiz);
        }

        protected override ICollector<QuizEntity> GetTrigger(IContext<QuizEntity> context)
        {
            return context.CreateCollector(QuizMatcher.ActiveTask.Added());
        }

        protected override bool Filter(QuizEntity entity)
        {
            return true;
        }

        protected override void Execute(List<QuizEntity> entities)
        {
            var quiz = _quiz.GetEntities().FirstOrDefault();
            foreach (var quizEntity in entities)
            {
                foreach (var tTEntity in _trainingTasks.GetEntities())
                {
                    // if (tTEntity.isPictureTrainingQuiz && quizEntity.task.Value.QuestionWordId == tTEntity.quizTasksForTraining.taskValue.QuestionWordId)
                    // {
                    //     quiz.ReplaceAnswersWithIconsQuiz(true);
                    // }
                    // else
                    // {
                    //     quiz.ReplaceAnswersWithIconsQuiz(false);
                    // }
                }
            }
        }
    }
}