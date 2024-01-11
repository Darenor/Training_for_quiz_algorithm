using System.Collections.Generic;
using Entitas;

namespace LibraryModule
{
    public class OnReconnectRestartGeneralTrainingSystem : ReactiveSystem<NetworkEntity>
    {
        private readonly IGroup<LibraryEntity> _wordInfoWindow;
        private readonly IGroup<LibraryEntity> _specialTrainingTasks;
        private readonly IGroup<ScrollEntity> _learnedWords;
        private readonly IGroup<ScrollEntity> _scrolls;

        public OnReconnectRestartGeneralTrainingSystem(NetworkContext networkContext, LibraryContext libraryContext, ScrollContext scrollContext) : base(networkContext)
        {
            _wordInfoWindow = libraryContext.GetGroup(LibraryMatcher.AllOf(LibraryMatcher.WordInfoWindow,
                LibraryMatcher.WordInfoNativeAndTargetWord, LibraryMatcher.Enabled));

            _specialTrainingTasks = libraryContext.GetGroup(LibraryMatcher.AllOf(LibraryMatcher.SpecialTraining));
            
            _learnedWords = scrollContext.GetGroup(ScrollMatcher.AllOf(ScrollMatcher.Word, ScrollMatcher.WordAvailable,
                ScrollMatcher.Id, ScrollMatcher.WordLearnedOnTarget, ScrollMatcher.WordLearnedOnNative, ScrollMatcher.WordLearnedWithPictures));
            
            _scrolls = scrollContext.GetGroup(ScrollMatcher.AllOf(ScrollMatcher.Scroll, ScrollMatcher.Id,
                ScrollMatcher.Available, ScrollMatcher.DisplayInfo, ScrollMatcher.WordIds, ScrollMatcher.TrainingInProgress));
        }

        protected override ICollector<NetworkEntity> GetTrigger(IContext<NetworkEntity> context)
        {
            return context.CreateCollector(NetworkMatcher.ServerReconnected.Added());
        }

        protected override bool Filter(NetworkEntity entity)
        {
            return entity.isServer;
        }

        protected override void Execute(List<NetworkEntity> entities)
        {
            foreach (var wordInfoWindowEntity in _wordInfoWindow.GetEntities())
            {
                wordInfoWindowEntity.ReplaceEnabled(false);
                wordInfoWindowEntity.ReplaceWordInfoNativeAndTargetWord("", "");

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