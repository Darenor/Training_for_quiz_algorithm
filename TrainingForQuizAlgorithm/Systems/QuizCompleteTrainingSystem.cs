using System.Collections.Generic;
using System.Linq;
using Entitas;
using UnityEngine;

namespace LibraryModule
{
    public class QuizCompleteTrainingSystem : ReactiveSystem<LibraryEntity>
    {
        private readonly IGroup<LibraryEntity> _wordInfoWindow;
        private readonly IGroup<ScrollEntity> _scrolls;
        private readonly IGroup<ScrollEntity> _learnedWords;
        private readonly IGroup<LibraryEntity> _scrollWindow;

        public QuizCompleteTrainingSystem(LibraryContext libraryContext, ScrollContext scrollContext) : base(libraryContext)
        {
            _wordInfoWindow = libraryContext.GetGroup(LibraryMatcher.AllOf(LibraryMatcher.WordInfoWindow,
                LibraryMatcher.WordInfoNativeAndTargetWord, LibraryMatcher.Enabled));

            _scrolls = scrollContext.GetGroup(ScrollMatcher.AllOf(ScrollMatcher.Scroll, ScrollMatcher.Id,
                ScrollMatcher.Available, ScrollMatcher.DisplayInfo, ScrollMatcher.WordIds));

            _learnedWords = scrollContext.GetGroup(ScrollMatcher.AllOf(ScrollMatcher.Word, ScrollMatcher.WordAvailable,
                ScrollMatcher.Id, ScrollMatcher.WordLearnedOnTarget, ScrollMatcher.WordLearnedOnNative));

            _scrollWindow = libraryContext.GetGroup(LibraryMatcher.AllOf(LibraryMatcher.ScrollWindow));
        }

        protected override ICollector<LibraryEntity> GetTrigger(IContext<LibraryEntity> context)
        {
            return context.CreateCollector(LibraryMatcher.TrainingTasksAllDone.Added());
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


                //*E Count basic training progress in the end of sequence
                var currentProgress = scrollEntity.wordIds.values.Length == 0 ? 0 : (int)(i / ((float)scrollEntity.wordIds.values.Length * 3) * 100);

                foreach (var scrollWindowEntity in _scrollWindow.GetEntities())
                {
                    var progress = 0;

                    if (currentProgress != 0)
                    {
                        progress = currentProgress != 100 ? Mathf.Clamp(currentProgress, 10, 90) : 100;
                    }
                    else
                    {
                        progress = 0;
                    }

                    scrollWindowEntity.ReplaceUIScrollTrainingProgress(progress);
                    scrollEntity.ReplaceSpecialTrainingProgress(progress, i);

                    var scrollType = GameTools.GetServerScrollCategoryType(scrollEntity.type.value);

                    ServerUnityInstance.RealServerApi.SetScrollsTrainingProgress((uint)scrollEntity.id.value, (uint)scrollEntity.specialTrainingProgress.value,
                        (uint)scrollEntity.hogStudyProgress.progress,
                        (uint)scrollEntity.hogStudyProgress.scores, (uint)scrollEntity.hog2StudyProgress.progress, (uint)scrollEntity.hog2StudyProgress.scores,
                        (uint)scrollEntity.nativeLearningProgress.value, (uint)scrollEntity.targetLearningProgress.value, (uint)scrollEntity.picturesLearningProgress.value,
                        scrollType);
                }

                scrollEntity.isUpdated = true;
            }
        }
    }
}