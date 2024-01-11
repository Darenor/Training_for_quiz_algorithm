using System.Collections.Generic;
using System.Linq;
using Entitas;

namespace LibraryModule
{
    public sealed class ContinueGeneralTrainingSystem : ReactiveSystem<LibraryEntity>
    {
        private readonly IGroup<LibraryEntity> _wordInfoWindow;
        private readonly IGroup<ScrollEntity> _scrolls;
        private readonly IGroup<ScrollEntity> _words;
        private readonly IGroup<LibraryEntity> _quizTasksNativeTrainingGroup;
        private readonly IGroup<LibraryEntity> _quizTasksNTargetTrainingGroup;
        private readonly IGroup<LibraryEntity> _windowWords;

        public ContinueGeneralTrainingSystem(LibraryContext libraryContext, ScrollContext scrollContext) : base(
            libraryContext)
        {
            _wordInfoWindow = libraryContext.GetGroup(LibraryMatcher.AllOf(LibraryMatcher.WordInfoWindow,
                LibraryMatcher.WordInfoNativeAndTargetWord, LibraryMatcher.Enabled, LibraryMatcher.WordIdsForInfoWindow));

            _scrolls = scrollContext.GetGroup(ScrollMatcher.AllOf(ScrollMatcher.Scroll, ScrollMatcher.Id,
                ScrollMatcher.Available, ScrollMatcher.DisplayInfo, ScrollMatcher.WordIds));
            _words = scrollContext.GetGroup(ScrollMatcher.AllOf(ScrollMatcher.Word, ScrollMatcher.WordAvailable,
                ScrollMatcher.Id));
            _quizTasksNativeTrainingGroup = libraryContext.GetGroup(LibraryMatcher.AllOf(LibraryMatcher.SpecialTraining,
                LibraryMatcher.IndexTasks,
                LibraryMatcher.QuizTasksForTraining, LibraryMatcher.NativeTrainingQuiz));
            _quizTasksNTargetTrainingGroup = libraryContext.GetGroup(LibraryMatcher.AllOf(LibraryMatcher.SpecialTraining,
                LibraryMatcher.IndexTasks,
                LibraryMatcher.QuizTasksForTraining, LibraryMatcher.TargetTrainingQuiz));

            _windowWords = libraryContext.GetGroup(LibraryMatcher.AllOf(LibraryMatcher.SpecialTraining,
                LibraryMatcher.InfoWindowTask, LibraryMatcher.IndexTasks, LibraryMatcher.WordInfoTasks).NoneOf(LibraryMatcher.ActiveWord));
        }

        protected override ICollector<LibraryEntity> GetTrigger(IContext<LibraryEntity> context)
        {
            return context.CreateCollector(LibraryMatcher.ContinueButtonAction.Added());
        }

        protected override bool Filter(LibraryEntity entity)
        {
            return true;
        }

        protected override void Execute(List<LibraryEntity> entities)
        {
            var wordInfoScreensAmount = 0;
            foreach (var requestEntity in entities)
            {
                foreach (var displayedScrollEntity in _scrolls.GetEntities())
                {
                    foreach (var wordInfoWindowEntity in _wordInfoWindow.GetEntities())
                    {

                        if (wordInfoWindowEntity.wordIdsForInfoWindow.values != null)
                        {
                            var newWordIds = wordInfoWindowEntity.wordIdsForInfoWindow.values.ToList();
                            var newWord = GameTools.GetRandomElements(wordInfoWindowEntity.wordIdsForInfoWindow.values, 1);

                            foreach (var wordEntity in _words.GetEntities())
                            {
                                if (newWord.Length <= 0)
                                {
                                    
                                }
                                else
                                {
                                    if (wordEntity.id.value != newWord[0]) continue;

                                    wordInfoWindowEntity.ReplaceWordInfoNativeAndTargetWord(wordEntity.wordStaticData.Native, wordEntity.wordStaticData.Target);
                                    wordInfoWindowEntity.ReplaceWordIdForTrainQuiz(wordEntity.id.value);
                                    newWordIds.Remove(wordEntity.id.value);
                                    wordInfoWindowEntity.ReplaceWordIdsForInfoWindow(newWordIds.ToArray());
                                    wordInfoScreensAmount++;
                                        //Придумать как забирать 1 элемент из массива с Квиз заданиями иначе, всегда будет 1 из 7, что не очень круто

                                        // GameTools.QuizRun(new [] {quizTaskEntity.quizTasksForTraining.taskValue});
                                        // Dbg.LogYellow($"{quizTaskEntity.indexTasks.value}");
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}