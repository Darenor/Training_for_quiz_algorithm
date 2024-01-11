using System;
using System.Collections.Generic;
using System.Linq;
using Entitas;
using I2.Loc;

namespace LibraryModule
{
    public class ShowCollectionLabelOnCompleteScreen : ReactiveSystem<LibraryEntity>
    {
        private readonly IGroup<ScrollEntity> _scrolls;
        private readonly IGroup<LibraryEntity> _tasksCompleteOrFailedGroup;
        private readonly IGroup<ScrollEntity> _words;
        private readonly IGroup<LibraryEntity> _tasks;

        public ShowCollectionLabelOnCompleteScreen(LibraryContext libraryContext, ScrollContext scrollContext) : base(libraryContext)
        {
            _scrolls = scrollContext.GetGroup(ScrollMatcher.AllOf(ScrollMatcher.Scroll, ScrollMatcher.Id,
                ScrollMatcher.Available, ScrollMatcher.DisplayInfo, ScrollMatcher.WordIds));
            
            _tasksCompleteOrFailedGroup = libraryContext.GetGroup(LibraryMatcher.AllOf(LibraryMatcher.SpecialTraining, LibraryMatcher.IndexTasks));
            
            _tasks = libraryContext.GetGroup(LibraryMatcher.AllOf(LibraryMatcher.SpecialTraining, LibraryMatcher.CompleteTask));
            
            _words = scrollContext.GetGroup(ScrollMatcher.AllOf(ScrollMatcher.Word, ScrollMatcher.WordAvailable,
                ScrollMatcher.Id, ScrollMatcher.WordLearnedOnTarget, ScrollMatcher.WordLearnedOnNative));
        }

        protected override ICollector<LibraryEntity> GetTrigger(IContext<LibraryEntity> context)
        {
            return context.CreateCollector(LibraryMatcher.Enabled.Added());
        }

        protected override bool Filter(LibraryEntity entity)
        {
            return entity.isCompletedMessageAfterTraining && entity.hasScrollWindowCompleteSpecialTrainingDisplayListener;
        }

        protected override void Execute(List<LibraryEntity> entities)
        {
            foreach (var messageWindowEntity in entities)
            {
                int learnedWords = 0;
                int allTasks = _tasksCompleteOrFailedGroup.GetEntities().Length/3;
                var completelyLearnedWords = new List<ScrollEntity>();
                var listOfCompletedWords = new List<string>();
                var uniqueListOfCompleteWords = new List<string>();
                
                foreach (var displayedScrollEntity in _scrolls.GetEntities())
                {
                    var i = 0;
                    foreach (var wordId in displayedScrollEntity.wordIds.values)
                    {
                        //*E learned words and their count
                        learnedWords = _words.GetEntities().Where(currentWord => currentWord.id.value == wordId).Count(currentWord => currentWord.wordLearnedNumerousTimes.learnedTimes > 0 && currentWord.wordLearnedOnNative.value && currentWord.wordLearnedOnTarget.value && currentWord.wordLearnedWithPictures.value);

                        i = learnedWords + i;
                    }
                    
                    foreach (var taskEntity in _tasks.GetEntities())
                    {
                        //*E string list to display words in screen of words which learned successfully  
                        completelyLearnedWords = _words.GetEntities().Where(w => w.id.value == taskEntity.quizTasksForTraining.taskValue.QuestionWordId && w.wordLearnedOnNative.value && w.wordLearnedOnTarget.value && w.wordLearnedWithPictures.value).ToList();
                        listOfCompletedWords.AddRange(completelyLearnedWords.Select(completeWordEntity => completeWordEntity.wordStaticData.Target + "-" + completeWordEntity.wordStaticData.Native));
                    }
                    
                    uniqueListOfCompleteWords = listOfCompletedWords.Distinct().ToList();

                    switch (LocalizationManager.CurrentLanguage)
                    {
                        case "Russian":
                            messageWindowEntity.ReplaceScrollWindowCompleteSpecialTrainingDisplay(uniqueListOfCompleteWords.Count, allTasks, uniqueListOfCompleteWords, displayedScrollEntity.subject.value, displayedScrollEntity.grade.StringValue, learnedWords > 1 ? "а" : "о");
                            break;
                        case "English (United States)":
                            messageWindowEntity.ReplaceScrollWindowCompleteSpecialTrainingDisplay(uniqueListOfCompleteWords.Count, allTasks, uniqueListOfCompleteWords, displayedScrollEntity.subject.value, displayedScrollEntity.grade.StringValue, learnedWords > 1 ? "s" : "");
                            break;
                    }
                    messageWindowEntity.isResultScreenStart = true;
                }

                
            }
        }
    }
}