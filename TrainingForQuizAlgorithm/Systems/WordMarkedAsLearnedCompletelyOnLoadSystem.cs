using System;
using System.Collections.Generic;
using System.Linq;
using Entitas;
using UnityEngine;

namespace LibraryModule
{
    public class WordMarkedAsLearnedCompletelyOnLoadSystem : ReactiveSystem<ScrollEntity>
    {
        private readonly IGroup<ScrollEntity> _learnedWords;
        private readonly IGroup<ScrollEntity> _scrolls;
        private readonly IGroup<ScrollEntity> _words;
        private readonly IGroup<LibraryEntity> _scrollWindow;

        public WordMarkedAsLearnedCompletelyOnLoadSystem(LibraryContext libraryContext, ScrollContext scrollContext) : base(scrollContext)
        {
            _learnedWords = scrollContext.GetGroup(ScrollMatcher.AllOf(ScrollMatcher.Word, ScrollMatcher.WordAvailable,
                ScrollMatcher.Id, ScrollMatcher.WordLearnedOnNative, ScrollMatcher.WordLearnedOnTarget, ScrollMatcher.WordLearnedNumerousTimes));

            _scrolls = scrollContext.GetGroup(ScrollMatcher.AllOf(ScrollMatcher.Scroll, ScrollMatcher.Id,
                ScrollMatcher.WordIds));

            _words = scrollContext.GetGroup(ScrollMatcher.AllOf(ScrollMatcher.Word, ScrollMatcher.WordAvailable,
                ScrollMatcher.Id));


            _scrollWindow = libraryContext.GetGroup(LibraryMatcher.AllOf(LibraryMatcher.ScrollWindow));
        }

        protected override ICollector<ScrollEntity> GetTrigger(IContext<ScrollEntity> context)
        {
            return context.CreateCollector(ScrollMatcher.WordGeneralTrainingProgress.Added());
        }

        protected override bool Filter(ScrollEntity entity)
        {
            return entity.isScroll;
        }

        protected override void Execute(List<ScrollEntity> entities)
        {
            foreach (var scrollEntity in _scrolls.GetEntities())
            {
                int sum = 0;
                foreach (var scroll in _scrolls.GetEntities().Where(w => w.id.value == scrollEntity.id.value && w.type.value == scrollEntity.type.value))
                {
                    sum += scroll.wordGeneralTrainingProgress.wordsLearningProgress.Values.Sum();
                }
                
                //*E Count all progress on the load of the game for basic training
                var currentProgress = scrollEntity.wordIds.values.Length == 0 ? 0 : (int)(sum / ((float)scrollEntity.wordIds.values.Length * 3) * 100);

                foreach (var scrollWindowEntity in _scrollWindow.GetEntities())
                {
                    var y = 0;

                    if (currentProgress != 0)
                    {
                        y = currentProgress != 100 ? Mathf.Clamp(currentProgress, 10, 90) : 100;
                    }
                    else
                    {
                        y = 0;
                    }

                    scrollWindowEntity.ReplaceUIScrollTrainingProgress(y);
                    scrollEntity.ReplaceSpecialTrainingProgress(y, sum);
                }

            }
        }
    }
}