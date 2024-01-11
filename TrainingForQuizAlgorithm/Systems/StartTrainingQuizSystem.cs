using System;
using System.Collections.Generic;
using Entitas;
using QuizModule;
using ServerSide.Shared;
using Random = UnityEngine.Random;

namespace LibraryModule
{
    public class StartTrainingQuizSystem : ReactiveSystem<LibraryEntity>
    {
        private readonly IGroup<LibraryEntity> _wordInfoWindow;
        private readonly IGroup<ScrollEntity> _displayedScrolls;

        public StartTrainingQuizSystem(LibraryContext libraryContext, ScrollContext scrollContext) : base(
            libraryContext)
        {
            _wordInfoWindow = libraryContext.GetGroup(LibraryMatcher.AllOf(LibraryMatcher.WordInfoWindow,
                LibraryMatcher.WordInfoNativeAndTargetWord, LibraryMatcher.Enabled,
                LibraryMatcher.WordIdsForInfoWindow));

            _displayedScrolls = scrollContext.GetGroup(ScrollMatcher.AllOf(ScrollMatcher.Scroll, ScrollMatcher.Id,
                ScrollMatcher.Available, ScrollMatcher.DisplayInfo, ScrollMatcher.WordIds));
        }

        protected override ICollector<LibraryEntity> GetTrigger(IContext<LibraryEntity> context)
        {
            return context.CreateCollector(LibraryMatcher.StartQuizTraining.Added());
        }

        protected override bool Filter(LibraryEntity entity)
        {
            return true;
        }

        protected override void Execute(List<LibraryEntity> entities)
        {
            foreach (var wordInfoEntity in _wordInfoWindow.GetEntities())
            {
                foreach (var displayedScrollEntity in _displayedScrolls.GetEntities())
                { 
                    bool quizKind = Random.value > 0.5f;

                    GameTools.CreateQuizTasks(QuizMode.Player, QuizGameType.Learning2,
                        wordInfoEntity.wordIdForTrainQuiz.value, displayedScrollEntity.id.value,
                        trainTask =>
                        {
                            GameTools.QuizRun(new[] { trainTask });
                        }, answersCount: 4, isNative: quizKind);
                }
            }
        }
    }
}