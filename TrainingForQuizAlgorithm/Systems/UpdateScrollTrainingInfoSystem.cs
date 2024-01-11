using System;
using System.Collections.Generic;
using System.Linq;
using Entitas;
using UnityEngine;

namespace LibraryModule
{
    public class UpdateScrollTrainingInfoSystem : ReactiveSystem<LibraryEntity>
    {
        private readonly IGroup<ScrollEntity> _learnedWords;
        private readonly IGroup<LibraryEntity> _wordInfoWindow;
        private readonly IGroup<ScrollEntity> _scrolls;
        private readonly IGroup<LibraryEntity> _scrollWindow;

        public UpdateScrollTrainingInfoSystem(ScrollContext scrollContext, LibraryContext libraryContext) : base(libraryContext)
        {
            _wordInfoWindow = libraryContext.GetGroup(LibraryMatcher.AllOf(LibraryMatcher.WordInfoWindow,
                LibraryMatcher.WordInfoNativeAndTargetWord, LibraryMatcher.Enabled));

            _learnedWords = scrollContext.GetGroup(ScrollMatcher.AllOf(ScrollMatcher.Word, ScrollMatcher.WordAvailable,
                ScrollMatcher.Id, ScrollMatcher.WordLearnedNumerousTimes));
            
            _scrolls = scrollContext.GetGroup(ScrollMatcher.AllOf(ScrollMatcher.Scroll, ScrollMatcher.Id,
                ScrollMatcher.Available, ScrollMatcher.DisplayInfo, ScrollMatcher.WordIds));
            
            _scrollWindow = libraryContext.GetGroup(LibraryMatcher.AllOf(LibraryMatcher.ScrollWindow));

        }

        protected override ICollector<LibraryEntity> GetTrigger(IContext<LibraryEntity> context)
        {
            return context.CreateCollector(LibraryMatcher.ScrollWindowOpened.Added(), LibraryMatcher.WordWidgetAddToCustomScrollMode.Added());
        }

        protected override bool Filter(LibraryEntity entity)
        {
            return true;
        }

        protected override void Execute(List<LibraryEntity> entities)
        {
            foreach (var scrollEntity in _scrolls.GetEntities())
            {
                var i = 0;
                foreach (var wordId in scrollEntity.wordIds.values)
                {
                    var learnedWordsCount = _learnedWords.GetEntities().Where(w => w.id.value == wordId).Sum(w => w.wordLearnedNumerousTimes.learnedTimes);
                    
                    i = learnedWordsCount + i;
                }

                foreach (var wordInfoWindowEntity in _wordInfoWindow.GetEntities())
                {
                    //*after open scroll window and widgets changes count progress for each scroll
                    var currentProgress = scrollEntity.wordIds.values.Length == 0 ? 0 : (int)(i / ((float)scrollEntity.wordIds.values.Length * 3) * 100);
                    
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
                        scrollEntity.ReplaceSpecialTrainingProgress(y, i);
                    }
                    
                    scrollEntity.isUpdated = true;
                }
            }
        }
    }
}