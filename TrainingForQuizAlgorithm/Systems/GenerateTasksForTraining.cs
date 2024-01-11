using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Entitas;
using QuizModule;
using ServerSide.Shared;
using Random = System.Random;

namespace LibraryModule
{
    public sealed class GenerateTasksForTraining : ReactiveSystem<ScrollEntity>
    {
        private readonly IGroup<LibraryEntity> _customScrollWarningWindow;
        private readonly IGroup<ScrollEntity> _scrolls;
        private readonly IGroup<ScrollEntity> _words;
        private List<int> _newWordIds;

        public GenerateTasksForTraining(LibraryContext libraryContext, ScrollContext scrollContext) : base(
            scrollContext)
        {
            _customScrollWarningWindow = libraryContext.GetGroup(LibraryMatcher.CustomScrollWarningWindow);
            _scrolls = scrollContext.GetGroup(ScrollMatcher.AllOf(ScrollMatcher.Scroll, ScrollMatcher.Id,
                ScrollMatcher.Available, ScrollMatcher.DisplayInfo, ScrollMatcher.WordIds).NoneOf(ScrollMatcher.TrainingInProgress));
            _words = scrollContext.GetGroup(ScrollMatcher.AllOf(ScrollMatcher.Word, ScrollMatcher.WordAvailable,
                ScrollMatcher.Id, ScrollMatcher.WordLearnedNumerousTimes));
        }

        protected override ICollector<ScrollEntity> GetTrigger(IContext<ScrollEntity> context)
        {
            return context.CreateCollector(ScrollMatcher.StartTrainingWithWholeScrollLanguage.Added());
        }

        protected override bool Filter(ScrollEntity entity)
        {
            return true;
        }

        protected override void Execute(List<ScrollEntity> entities)
        {
            foreach (var scrollEntity in _scrolls.GetEntities())
            {
                scrollEntity.isTrainingInProgress = true;
                if (scrollEntity.isUsable == false)
                {
                    foreach (var customScrollWarningWindowEntity in _customScrollWarningWindow.GetEntities())
                    {
                        customScrollWarningWindowEntity.setEnable = true;
                    }

                    break;
                }

                var orderOfWordsInScroll = 0;

                List<WordsOrderInScroll> allWordsInScroll = new List<WordsOrderInScroll>();
                
                //*E Find every word in scroll wordIds and sort them by how they sorted in scroll 
                for (int i = 0; i < scrollEntity.wordIds.values.Length; i++)
                {
                    var wordEntity = _words.GetEntities().FirstOrDefault(word => word.id.value == scrollEntity.wordIds.values[i]);
                    allWordsInScroll.Add(new WordsOrderInScroll(wordEntity!.id.value, i, wordEntity.wordLearnedNumerousTimes.learnedTimes));
                }
                
                //*E Resorting order if there are words with components learning times more than 0
                //*E Every words with more learning times go upper while others go down and algorithm will work with it until their learning time less than 4,
                //*E however if there is not enough words for training it will added words from list of completely learned words
                
                //*E Visually nothing changes because its only for algorithms 
                
                var newSortingOrder = allWordsInScroll.OrderByDescending(x => x.WordLearnedTimes).ThenBy(x => x.WordOrderInScroll).ToArray();

                for (int i = 0; i < newSortingOrder.Length; i++)
                {
                    allWordsInScroll[i].SortOrder = newSortingOrder[i].WordOrderInScroll;
                }
                
                var wordsForQuiz = GetWordsForQuiz(allWordsInScroll);
                
                var countOfTasks = 0;
                var countOfQuizWindows = 0;
                foreach (var groupWords in wordsForQuiz)
                {
                    //*E Create quizTask
                    //*E For each word 3 windows of training Native, Target and Pictures tasks
                    //*E Also add indexes to tasks and windows
                    
                    for (var quizIndex = 0; quizIndex < 3; quizIndex++)
                    {
                        var quizTaskEntity = Contexts.sharedInstance.library.CreateEntity();
                        quizTaskEntity.isSpecialTraining = true;
                        quizTaskEntity.isNativeTrainingQuiz = quizIndex == 0;
                        quizTaskEntity.isTargetTrainingQuiz = quizIndex == 1;
                        quizTaskEntity.isPictureTrainingQuiz = quizIndex == 2;
                        GameTools.CreateQuizTasks(QuizMode.Player, QuizGameType.Learning2,
                            groupWords.WordId,
                            scrollEntity.id.value,
                            quizTask => {
                                quizTaskEntity.ReplaceQuizTasksForTraining(quizTask);
                                
                                    countOfTasks++;

                                    if (countOfTasks > 14)
                                    {
                                        quizTaskEntity.requestTaskCreated = true;
                                    }
                            },
                            scrollEntity.isCustom,
                            isNative: quizIndex == 0);
                
                        quizTaskEntity.ReplaceQuizTasks(quizIndex);
                        quizTaskEntity.ReplaceIndexTasks(countOfQuizWindows);
                    }
                
                    countOfQuizWindows++;
                }
            }
        }

        private List<WordsOrderInScroll> GetWordsForQuiz(List<WordsOrderInScroll> allWordsInScroll)
        {
            var learnedTimesRequiresForCompletelyStudiedWord = 3;
            List<WordsOrderInScroll> notCompleteWordsInScroll = allWordsInScroll.OrderByDescending(w => w.WordLearnedTimes).SkipWhile(w=>w.WordLearnedTimes>learnedTimesRequiresForCompletelyStudiedWord - 1).ToList();
            if (notCompleteWordsInScroll.Count < 10)
            {
                //*E Nonstandard Algorithm if words which is not learned left less than 10
            
                var wordsThatLeft = notCompleteWordsInScroll.OrderBy(w => w.WordOrderInScroll).ThenBy(w => w.WordLearnedTimes).ToList();
                
                var wordForQuiz = new List<WordsOrderInScroll>();
                
                var firstGroup = wordsThatLeft.Take(5).ToList();
                
                firstGroup.Shuffle();
                
                var shuffledAllWordsInScroll = new List<WordsOrderInScroll>(allWordsInScroll);
                
                shuffledAllWordsInScroll.Shuffle();
                    
                if (wordsThatLeft.ToArray().Length < 6)
                {
                    //*E Algorithm if words which is not learned left less than 5
                    
                    int cnt = 5 - wordsThatLeft.ToArray().Length;
                    if (cnt > 0)
                    {
                        List<WordsOrderInScroll> orderInScrolls = shuffledAllWordsInScroll.Where(x => x.WordLearnedTimes == learnedTimesRequiresForCompletelyStudiedWord).Take(cnt).ToList();
                        wordForQuiz.AddRange(wordsThatLeft);
                        wordForQuiz.AddRange(orderInScrolls);
                    }
                    else
                    {
                        wordForQuiz.AddRange(wordsThatLeft);
                    }
                }
                else
                {
                    //*E Algorithm to add words, however if words for second group of words left less than necessary, now its less than 2

                    var secondGroup = wordsThatLeft.GetRange(5, wordsThatLeft.Count - 5).Take(5).ToList();
                    secondGroup.Shuffle();

                    var random3ElementFromFirstGroup = firstGroup.Take(3).ToList();
                    
                    var random2ElementSecondGroup = secondGroup.Take(2).ToList();
                    Dbg.LogCyan($"{secondGroup.Count}, {random2ElementSecondGroup.Count}");


                    wordForQuiz.AddRange(random3ElementFromFirstGroup);
                    
                    //*E Add additional words if its less than requires of second group random, right now its 2 
                    int cnt = 2 - secondGroup.ToArray().Length;
                    if (cnt > 0)
                    {
                        List<WordsOrderInScroll> orderInScrolls = shuffledAllWordsInScroll.Where(x => x.WordLearnedTimes == learnedTimesRequiresForCompletelyStudiedWord).Take(cnt).ToList();

                        random2ElementSecondGroup.AddRange(orderInScrolls);
                       
                        wordForQuiz.AddRange(random2ElementSecondGroup);
                        Dbg.Log($"{wordForQuiz.Count}, {cnt}, {random2ElementSecondGroup.Count} random, {orderInScrolls.Count} orderInScroll");
                    }
                    else
                    {
                        wordForQuiz.AddRange(random2ElementSecondGroup);
                    }
                }

                return wordForQuiz;
            }
            else
            {
                // Standard algorithm for 10 words for special training 
                List<WordsOrderInScroll> firstGroup = notCompleteWordsInScroll.Take(5).ToList();
                
                List<WordsOrderInScroll> secondGroup = notCompleteWordsInScroll.GetRange(5, 5).Take(5).ToList();
                
                firstGroup.Shuffle();
                secondGroup.Shuffle();

                var random3ElementFromFirstGroup = firstGroup.Take(3).ToList();
                
                var random2ElementFromSecondGroup = secondGroup.Take(2).ToList();

                var wordsForQuiz = new List<WordsOrderInScroll>();
                
                wordsForQuiz.AddRange(random3ElementFromFirstGroup);
                wordsForQuiz.AddRange(random2ElementFromSecondGroup);

                return wordsForQuiz;
            }
        }
        
        //*E Class for words order and sorting in scroll with their order in scroll.WordIds.values
        private sealed class WordsOrderInScroll
        {
            public int WordId;
            public int WordOrderInScroll;
            public int SortOrder;
            public int WordLearnedTimes;

            public WordsOrderInScroll(int wordId, int wordOrderInScroll, int wordLearnedTimes)
            {
                WordId = wordId;
                WordOrderInScroll = wordOrderInScroll;
                WordLearnedTimes = wordLearnedTimes;
            }
        }
    }
}