using System.Collections.Generic;
using Entitas;
using ServerSide.Shared;

namespace LibraryModule
{
        public class QuizAnswerCompleteOrFailedSystem : ReactiveSystem<QuizEntity>
        {
            private readonly IGroup<LibraryEntity> _taskTrainingWindows;

            public QuizAnswerCompleteOrFailedSystem(QuizContext quizContexts, LibraryContext libraryContext) : base(quizContexts)
            {
                _taskTrainingWindows = libraryContext.GetGroup(LibraryMatcher.AllOf(LibraryMatcher.SpecialTraining,
                    LibraryMatcher.IndexTasks, LibraryMatcher.Order).NoneOf(LibraryMatcher.CompleteTask,
                    LibraryMatcher.FailedTask));
                
            }

            protected override ICollector<QuizEntity> GetTrigger(IContext<QuizEntity> context)
            {
                return context.CreateCollector(QuizMatcher.TaskFailedPlayer.Added(), QuizMatcher.TaskCompletePlayer.Added());
            }

            protected override bool Filter(QuizEntity entity)
            {
                return entity.hasTask && entity.task.Value.QuizGameType == QuizGameType.Learning2;
            }

            protected override void Execute(List<QuizEntity> entities)
            {
                foreach (var quiz in entities)  
                {
                    foreach (var taskTrainingEntity in _taskTrainingWindows.GetEntities())
                    {
                        if(quiz.index.Value != taskTrainingEntity.order.value - 1 ) continue;

                        if (quiz.hasTaskCompletePlayer)
                        {
                            taskTrainingEntity.isCompleteTask = true;
                        }

                        if (quiz.hasTaskFailedPlayer)
                        {
                            taskTrainingEntity.isFailedTask = true;
                        }

                        // taskTrainingEntity.isActiveWord = false;
                    }
                }
            }
        }
}