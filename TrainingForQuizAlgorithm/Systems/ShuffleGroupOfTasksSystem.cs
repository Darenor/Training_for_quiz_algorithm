using System;
using System.Collections.Generic;
using System.Linq;
using Entitas;
using UnityEngine;

namespace LibraryModule
{
    public class ShuffleGroupOfTasksSystem : ReactiveSystem<LibraryEntity>
    {
        private readonly IGroup<LibraryEntity> _taskGroup;
        private readonly IGroup<LibraryEntity> _taskPicturesGroup;
        
        private readonly Dictionary<string, int> _dictionary = new Dictionary<string, int>()
        {
            {"00", 1},
            {"01", 3},
            {"02", 7},
            {"10", 2},
            {"11", 5},
            {"12", 8},
            {"20", 4},
            {"21", 9},
            {"22", 14},
            {"30", 6},
            {"31", 11},
            {"32", 13},
            {"40", 10},
            {"41", 12},
            {"42", 15}
        };

        private readonly IGroup<LibraryEntity> _failedWindowGroup;
        private readonly QuizContext _quizContext;
        private readonly IGroup<QuizEntity> _quiz;

        public ShuffleGroupOfTasksSystem(LibraryContext libraryContext, ScrollContext scrollContext, QuizContext quizContext) : base(libraryContext)
        {
            _quizContext = quizContext;
            
            _taskGroup = libraryContext.GetGroup(LibraryMatcher.AllOf(LibraryMatcher.SpecialTraining, LibraryMatcher.IndexTasks, LibraryMatcher.QuizTasks));
            
            _taskPicturesGroup = libraryContext.GetGroup(LibraryMatcher.AllOf(LibraryMatcher.SpecialTraining, LibraryMatcher.IndexTasks, LibraryMatcher.QuizTasks, LibraryMatcher.PictureTrainingQuiz));

            _failedWindowGroup = libraryContext.GetGroup(LibraryMatcher.AllOf(LibraryMatcher.FailedMessageAfterTraining, LibraryMatcher.Enable));


            _quiz = quizContext.GetGroup(QuizMatcher.Quiz);
        }

        protected override ICollector<LibraryEntity> GetTrigger(IContext<LibraryEntity> context)
        {
            return context.CreateCollector(LibraryMatcher.TaskCreated.Added());
        }

        protected override bool Filter(LibraryEntity entity)
        {
            return true;
        }

        protected override void Execute(List<LibraryEntity> entities)
        {
            //*E System will check if key of dictionary equal to index of task and index of window, to add order to entity
            foreach (var taskEntity in _taskGroup.GetEntities())
            {
                var key = $"{taskEntity.indexTasks.value}{taskEntity.quizTasks.value}";

                if (_dictionary.TryGetValue(key, out int order))
                {
                    taskEntity.ReplaceOrder(order);
                }
            }

            var quiz = _quiz.GetEntities().FirstOrDefault();
            
            var quizTasks = _taskGroup.GetEntities().OrderBy(e => e.order.value).Select(e => e.quizTasksForTraining.taskValue).ToArray();
            var picQuizTasks = _taskGroup.GetEntities().Where(e => e.isPictureTrainingQuiz).Select(e => e.quizTasksForTraining.taskValue.AnswersIds);

            var newWordIds = new List<int>();
            foreach (var wordId in picQuizTasks)
            {
                newWordIds.AddRange(wordId);
            }
            //Debug.Log(5);
            AssetsProvider.WordIconsAssetsLoader.Load(newWordIds.ConvertToString(), result =>
            {
                GameTools.QuizRun(quizTasks, true);
            });
            

            foreach (var windowEntity in _failedWindowGroup.GetEntities())
            {
                if (windowEntity.enabled.value == false) continue;
                windowEntity.ReplaceEnabled(false);
            }
        }
    }
}