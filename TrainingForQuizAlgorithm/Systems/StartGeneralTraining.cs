using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Entitas;
using QuizModule;
using UnityEngine;

namespace LibraryModule
{
    public sealed class StartGeneralTraining : ReactiveSystem<LibraryEntity>
    {
        private readonly IGroup<LibraryEntity> _customScrollWarningWindow;
        private readonly IGroup<ScrollEntity> _scrolls;
        private readonly IGroup<ScrollEntity> _words;
        private readonly IGroup<LibraryEntity> _wordInfoWindow;
        private List<int> _newWordIds;
        private readonly IGroup<LibraryEntity> _taskFirstGroupWindows;
        private readonly IGroup<LibraryEntity> _taskCreatedGroup;

        public StartGeneralTraining(LibraryContext libraryContext, ScrollContext scrollContext) : base(libraryContext)
        {
            _customScrollWarningWindow = libraryContext.GetGroup(LibraryMatcher.CustomScrollWarningWindow);
            _scrolls = scrollContext.GetGroup(ScrollMatcher.AllOf(ScrollMatcher.Scroll, ScrollMatcher.Id,
                ScrollMatcher.Available, ScrollMatcher.DisplayInfo, ScrollMatcher.WordIds));
            _words = scrollContext.GetGroup(ScrollMatcher.AllOf(ScrollMatcher.Word, ScrollMatcher.WordAvailable,
                ScrollMatcher.Id));

            _wordInfoWindow = libraryContext.GetGroup(LibraryMatcher.AllOf(LibraryMatcher.WordInfoWindow,
                LibraryMatcher.WordInfoNativeAndTargetWord, LibraryMatcher.Enabled));

            _taskFirstGroupWindows = libraryContext.GetGroup(LibraryMatcher.AllOf(LibraryMatcher.SpecialTraining,
                LibraryMatcher.IndexTasks, LibraryMatcher.FirstGroupMark).NoneOf(LibraryMatcher.CompleteTask, LibraryMatcher.FailedTask));
            
            _taskCreatedGroup = libraryContext.GetGroup(LibraryMatcher.AllOf(LibraryMatcher.SpecialTraining,
                LibraryMatcher.IndexTasks).NoneOf(LibraryMatcher.InfoWindowTask));
        }

        protected override ICollector<LibraryEntity> GetTrigger(IContext<LibraryEntity> context)
        {
            return context.CreateCollector(LibraryMatcher.TaskCreated.Added());
        }

        protected override bool Filter(LibraryEntity entity)
        {
            return true;
        }

        protected override void Execute(List<LibraryEntity> entities)
        {
            var index = 0;
            foreach (var requestEntity in entities)
            {
                foreach (var scrollEntity in _scrolls.GetEntities())
                {
                    if (scrollEntity.isUsable == false)
                    {
                        foreach (var customScrollWarningWindowEntity in _customScrollWarningWindow.GetEntities())
                        {
                            customScrollWarningWindowEntity.setEnable = true;
                        }

                        break;
                    }

                    
                    foreach (var wordInfoWindowEntity in _wordInfoWindow.GetEntities())
                    {
                        if (wordInfoWindowEntity.wordIdsForInfoWindow.values == null)
                        {
                            wordInfoWindowEntity.ReplaceWordIdsForInfoWindow(scrollEntity.wordIds.values);
                        }

                        if (wordInfoWindowEntity.wordIdsForInfoWindow.values != null)
                        {
                            var trainingWord = GameTools.GetRandomElements(wordInfoWindowEntity.wordIdsForInfoWindow.values, 1);

                            var newWordIds = wordInfoWindowEntity.wordIdsForInfoWindow.values.ToList();

                            foreach (var wordEntity in _words.GetEntities())
                            {
                                if (wordEntity.id.value != trainingWord[0]) continue;
                                
                                //* Pick random task from pull
                                wordInfoWindowEntity.ReplaceEnabled(true);

                                if (_taskFirstGroupWindows.GetEntities().Length == 0) continue;
                                var tasksEntity = _taskFirstGroupWindows.GetEntities().ElementAt(Random.Range(0, _taskFirstGroupWindows.GetEntities().Length));

                                tasksEntity.isActiveWord = true;
                                if (tasksEntity.isNativeTrainingQuiz || tasksEntity.isTargetTrainingQuiz)
                                {
                                    wordInfoWindowEntity.ReplaceObjectsBlockUnusedInfo(true);
                                    GameTools.QuizRun(new[] { tasksEntity.quizTasksForTraining.taskValue });
                                }
                                else
                                {
                                    wordInfoWindowEntity.ReplaceObjectsBlockUnusedInfo(false);
                                    wordInfoWindowEntity.ReplaceWordInfoNativeAndTargetWord(wordEntity.wordStaticData.Native, wordEntity.wordStaticData.Target);
                                    wordInfoWindowEntity.ReplaceWordIdForTrainQuiz(tasksEntity.indexTasks.value);
                                }

                                wordInfoWindowEntity.ReplaceWordIdsForInfoWindow(newWordIds.ToArray());
                                break;
                            }
                        }
                    }
                }
            }
        }
    }
}