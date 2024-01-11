using System.Collections.Generic;
using System.Linq;
using Entitas;

namespace LibraryModule
{
    public class ShowCollectionLabelOnFailedScreen : ReactiveSystem<LibraryEntity>
    {
        private readonly IGroup<ScrollEntity> _scrolls;
        private readonly IGroup<LibraryEntity> _tasks;
        private readonly IGroup<ScrollEntity> _words;
        private readonly IGroup<LibraryEntity> _failedTasks;
        private readonly IGroup<LibraryEntity> _tasksCompleteOrFailedGroup;

        public ShowCollectionLabelOnFailedScreen(LibraryContext libraryContext, ScrollContext scrollContext) : base(libraryContext)
        {
            _scrolls = scrollContext.GetGroup(ScrollMatcher.AllOf(ScrollMatcher.Scroll, ScrollMatcher.Id,
                ScrollMatcher.Available, ScrollMatcher.DisplayInfo, ScrollMatcher.WordIds));

            _failedTasks = libraryContext.GetGroup(LibraryMatcher.AllOf(LibraryMatcher.SpecialTraining, LibraryMatcher.FailedTask));
            
            _tasks = libraryContext.GetGroup(LibraryMatcher.AllOf(LibraryMatcher.SpecialTraining, LibraryMatcher.IndexTasks));

            _words = scrollContext.GetGroup(ScrollMatcher.AllOf(ScrollMatcher.Word, ScrollMatcher.WordAvailable,
                ScrollMatcher.Id, ScrollMatcher.WordLearnedOnTarget, ScrollMatcher.WordLearnedOnNative, ScrollMatcher.WordLearnedWithPictures));
        }

        protected override ICollector<LibraryEntity> GetTrigger(IContext<LibraryEntity> context)
        {
            return context.CreateCollector(LibraryMatcher.Enabled.Added());
        }

        protected override bool Filter(LibraryEntity entity)
        {
            return entity.isFailedMessageAfterTraining && entity.hasScrollWindowFailedSpecialTrainingDisplayListener;
        }

        protected override void Execute(List<LibraryEntity> entities)
        {
            foreach (var messageWindowEntity in entities)
            {
                int learnedWords = 0;
                int y = 0;
                int allTasks = _tasks.GetEntities().Length/3;
                var failedLearnedWords = new List<ScrollEntity>();
                var listOfFailedWords = new List<string>();
                
                foreach (var displayedScrollEntity in _scrolls.GetEntities())
                {

                    foreach (var taskFailedEntity in _failedTasks.GetEntities())
                    {
                        //*E failed words during Special Training, count by learned components if one of them false, means word is failed
                        failedLearnedWords = _words.GetEntities().Where(w => w.id.value == taskFailedEntity.quizTasksForTraining.taskValue.QuestionWordId
                                                                             && (w.wordLearnedOnNative.value == false || w.wordLearnedOnTarget.value == false || w.wordLearnedWithPictures.value == false)).ToList();
                        listOfFailedWords.AddRange(failedLearnedWords.Select(failedWordEntity => failedWordEntity.wordStaticData.Target + "-" + failedWordEntity.wordStaticData.Native));
                    }

                    var uniqueListOfFailedWords = listOfFailedWords.Distinct();
                    
                    //*E Show which in string words are failed and how many 
                    messageWindowEntity.ReplaceScrollWindowFailedSpecialTrainingDisplay(uniqueListOfFailedWords.Count(), allTasks, uniqueListOfFailedWords.ToList(), displayedScrollEntity.subject.value, learnedWords > 4 ? "вах" : "ов");

                    messageWindowEntity.isResultScreenStart = true;
                }
            }
        }
    }
}