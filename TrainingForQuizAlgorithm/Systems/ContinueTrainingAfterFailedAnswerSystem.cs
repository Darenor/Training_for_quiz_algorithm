using System.Collections.Generic;
using Entitas;

namespace LibraryModule
{
    public class ContinueTrainingAfterFailedAnswerSystem : ReactiveSystem<LibraryEntity>
    {
        private LibraryContext _libraryContext;
        private readonly IGroup<LibraryEntity> _incorretInfoWindow;

        public ContinueTrainingAfterFailedAnswerSystem(LibraryContext libraryContext) : base(libraryContext)
        {
            _libraryContext = libraryContext;
            
            _incorretInfoWindow = libraryContext.GetGroup(LibraryMatcher.AllOf(LibraryMatcher.IncorrectAnswerWindow,
                LibraryMatcher.Enabled));
        }

        protected override ICollector<LibraryEntity> GetTrigger(IContext<LibraryEntity> context)
        {
            return context.CreateCollector(LibraryMatcher.ContinueAfterMistakeQuizTraining.Added());
        }

        protected override bool Filter(LibraryEntity entity)
        {
            return true;
        }

        protected override void Execute(List<LibraryEntity> entities)
        {
            foreach (var entity in entities)
            {
               var requestEntity = _libraryContext.CreateEntity();
               requestEntity.requestTaskCreated = true;
               requestEntity.ReplaceIndexTasks(13);
               requestEntity.requestContinueButtonAction = true;
               foreach (var incorrectInfoEntity in _incorretInfoWindow.GetEntities())
               {
                   incorrectInfoEntity.ReplaceEnabled(false);
               }
            }
        }
    }
}