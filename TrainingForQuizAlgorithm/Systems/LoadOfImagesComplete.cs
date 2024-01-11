using System.Collections.Generic;
using System.Linq;
using Entitas;
using static LibraryMatcher;

namespace LibraryModule
{
    public class LoadOfImagesComplete : ReactiveSystem<LibraryEntity>
    {
        private readonly IGroup<LibraryEntity> _tasksWithIconsGroup;
        private readonly IGroup<QuizEntity> _tasksGroup;
        private readonly IGroup<QuizEntity> _quiz;

        public LoadOfImagesComplete(LibraryContext libraryContext, QuizContext quizContext) : base(libraryContext)
        {
            _tasksWithIconsGroup = libraryContext.GetGroup(AllOf(SpecialTraining, IndexTasks, QuizTasks, PictureTrainingQuiz));

            _tasksGroup = quizContext.GetGroup(QuizMatcher.AllOf(QuizMatcher.Task));
            _quiz = quizContext.GetGroup(QuizMatcher.Quiz);
        }

        protected override ICollector<LibraryEntity> GetTrigger(IContext<LibraryEntity> context)
        {
            return context.CreateCollector(QuizIconsLoaded);
        }

        protected override bool Filter(LibraryEntity entity)
        {
            return true;
        }

        protected override void Execute(List<LibraryEntity> entities)
        {
            var quizEntity = _quiz.GetEntities().FirstOrDefault(); 
            foreach (var quizIconsEntity in _tasksGroup.GetEntities())
            {
                var sprite = AssetsProvider.WordIconsAssetsLoader.GetLoadedData().FirstOrDefault(e => e.name == $"{quizIconsEntity.task.Value.QuestionWordId}");
                if (quizEntity != null) quizEntity.ReplaceQuizIcon(sprite);
            }
        }
    }
}