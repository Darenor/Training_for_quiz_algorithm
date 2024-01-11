using System;
using System.Collections.Generic;
using System.Linq;
using Entitas;
using QuizModule;
using ServerSide.Shared;

namespace LibraryModule
{
    public class WordMarkAsLearnedCompletelySystem : ReactiveSystem<QuizEntity>
    {
        private readonly IGroup<ScrollEntity> _words;
        private readonly IGroup<ScrollEntity> _displayScrollGroup;
        private readonly IGroup<ScrollEntity> _displayScrollCustomGroup;

        private Dictionary<int, int> _wordIdsToSend = new Dictionary<int, int>();
        public WordMarkAsLearnedCompletelySystem(ScrollContext scrollContext, QuizContext quizContext) : base(quizContext)
        {
            _words = scrollContext.GetGroup(ScrollMatcher.AllOf(ScrollMatcher.Word, ScrollMatcher.WordAvailable,
                ScrollMatcher.Id));

            _displayScrollGroup = scrollContext.GetGroup(ScrollMatcher.AllOf(ScrollMatcher.Scroll, ScrollMatcher.DisplayInfo, ScrollMatcher.Id, ScrollMatcher.WordIds));
            _displayScrollCustomGroup = scrollContext.GetGroup(ScrollMatcher.AllOf(ScrollMatcher.Scroll, ScrollMatcher.DisplayInfo, ScrollMatcher.Id, ScrollMatcher.WordIds, ScrollMatcher.Custom));
        }

        protected override ICollector<QuizEntity> GetTrigger(IContext<QuizEntity> context)
        {
            return context.CreateCollector(QuizMatcher.QuizComplete.Added(), QuizMatcher.AbortPlay.Added());
        }

        protected override bool Filter(QuizEntity entity)
        {
            return entity.isQuiz && entity.isOpened && entity.hasMode && entity.hasGameType && entity.gameType.Value == QuizGameType.Learning2;
        }

        protected override void Execute(List<QuizEntity> entities)
        {
            foreach (var scrollEntity in _displayScrollGroup.GetEntities())
            {
                var customScroll = _displayScrollCustomGroup.GetEntities().FirstOrDefault();
                // scroll
                for (int i = 0; i < scrollEntity!.wordIds.values.Length; i++)
                {
                    int wordId = scrollEntity.wordIds.values[i];
                    // all words
                    foreach (var wordEntity in _words.GetEntities().Where(word => word.id.value == wordId
                                                                                  && word.wordLearnedOnNative.value
                                                                                  && word.wordLearnedOnTarget.value
                                                                                  && word.wordLearnedWithPictures.value
                                                                                  && word.wordLearnedNumerousTimes.learnedTimes < 3))
                    {
                        wordEntity.ReplaceWordLearnedNumerousTimes(wordEntity.wordLearnedNumerousTimes.learnedTimes + 1);
                        _wordIdsToSend.Add(wordId, wordEntity.wordLearnedNumerousTimes.learnedTimes);
                        if(scrollEntity.isCustom == false)
                        {
                            scrollEntity.ReplaceWordGeneralTrainingProgress(_wordIdsToSend);
                        }
                        else
                        {
                            customScroll.ReplaceWordGeneralTrainingProgress(_wordIdsToSend);
                        }
                        break;

                    }
                }

                ServerUnityInstance.RealServerApi.AddWordProgress(_wordIdsToSend, GameTools.GetServerScrollCategoryType(scrollEntity.type.value));

                Contexts.sharedInstance.library.CreateEntity().requestTrainingTasksAllDone = true;

                scrollEntity.isUpdated = true;
            }
        }
    }
}