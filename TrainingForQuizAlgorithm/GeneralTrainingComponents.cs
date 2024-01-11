using System.Collections.Generic;
using Entitas;
using Entitas.CodeGeneration.Attributes;
using QuizModule;
using ServerSide.Shared;

namespace LibraryModule
{

    #region wordInfoComponent

    [Library]
    public sealed class WordInfoWindowComponent : IComponent
    {
    }

    [Library]
    public sealed class IncorrectAnswerWindowComponent : IComponent
    {
    }

    [Library, Event(EventTarget.Self)]
    public sealed class WordInfoNativeAndTargetWordComponent : IComponent
    {
        public string native;
        public string target;
    }

    [Library, Event(EventTarget.Self)]
    public sealed class IncorrectAnswerNativeAndTargetWordComponent : IComponent
    {
        public string nativeIncorrect;
        public string targetIncorrect;
    }

    [Library, Event(EventTarget.Self)]
    public sealed class WordIdsForInfoWindowComponent : IComponent
    {
        public int[] values;
    }

    [Library]
    public sealed class WordIdForTrainQuizComponent : IComponent
    {
        public int value;
    }

    #endregion

    #region specialTrainingComponents

    [Library]
    public sealed class SpecialTrainingProgressComponent : IComponent
    {
        public int value;
    }

    [Library]
    public sealed class AmountOfWordsForSpecialTrainingComponent : IComponent
    {
        public int value;
    }
    #endregion

    #region buttonComponents

    [Library]
    public sealed class ContinueButtonComponent : IComponent
    {
    }

    [Library, FlagPrefix("request"), Cleanup(CleanupMode.DestroyEntity)]
    public sealed class ContinueButtonActionComponent : IComponent
    {
    }
    
    [Library, FlagPrefix("request"), Cleanup(CleanupMode.DestroyEntity)]
    public sealed class ContinueAfterMistakeQuizTrainingComponent : IComponent
    {
    }

    [Library, FlagPrefix("request"), Cleanup(CleanupMode.DestroyEntity)]
    public sealed class CloseButtonActionComponent : IComponent
    {
    }

    #endregion
    
    
    #region quizTask

    [Library, FlagPrefix("request"), Cleanup(CleanupMode.DestroyEntity)]
    public sealed class StartQuizTrainingComponent : IComponent
    {
    }

    //* QuizTask and InfoWindowTask entity components

    [Library]
    public sealed class QuizTasksForTrainingComponent : IComponent
    {
        public QuizTask taskValue;
    }

    [Library, Event(EventTarget.Self)]
    public sealed class IndexTasksComponent : IComponent
    {
        public int value;
    }
    
    [Library]
    public sealed class QuizTasksComponent : IComponent
    {
        public int value;
    }
    
    
    [Library, Cleanup(CleanupMode.RemoveComponent)]
    public sealed class QuizIconsLoadedComponent : IComponent
    {
        
    }
    
    [Library]
    public sealed class OrderComponent : IComponent
    {
        public int value;
    }

    [Library]
    public sealed class WordInfoTasksComponent : IComponent
    {
        public int value;
    }

    [Library]
    public sealed class SpecialTrainingComponent : IComponent
    {
    }

    [Library]
    public sealed class NativeTrainingQuizComponent : IComponent
    {
    }
    
    [Library]
    public sealed class TargetTrainingQuizComponent : IComponent
    {
    }

    [Library]
    public sealed class PictureTrainingQuizComponent : IComponent
    {
    }

    [Library, Cleanup(CleanupMode.RemoveComponent), FlagPrefix("request")]
    public sealed class QuizTasksGeneratedComponent : IComponent
    {
    }
    
    [Library, Cleanup(CleanupMode.DestroyEntity), FlagPrefix("request")]
    public sealed class TrainingTasksAllDoneComponent : IComponent
    {
    }

    #endregion

    #region groupComponent

    [Library]
    public sealed class GroupIndexComponent : IComponent
    {
        public int value;
    }

    [Library, Cleanup(CleanupMode.RemoveComponent)]
    public sealed class MoveToAnotherGroupComponent : IComponent
    {
        
    }

    [Library]
    public sealed class FirstGroupMarkComponent : IComponent
    {
        
    }

    [Library]
    public sealed class SecondGroupMarkComponent : IComponent
    {
        
    }

    [Library]
    public sealed class ThirdGroupMarkComponent : IComponent
    {
        
    }

    #endregion

    #region messagesComponent

    [Library]
    public sealed class CompletedMessageAfterTrainingComponent : IComponent
    {
        
    }

    [Library]
    public sealed class FailedMessageAfterTrainingComponent : IComponent
    {
        
    }
    
     
    [Library, Event(EventTarget.Self)]
    public sealed class ScrollWindowFailedSpecialTrainingDisplayComponent : IComponent
    {
        public int failedTasksCount;
        public int totalTasksCount;
        public List<string> wordsFailed;
        public string scrollSubject;
        public string endingOfTheWord;
    }
    
    [Library, Event(EventTarget.Self)]
    public sealed class ScrollWindowCompleteSpecialTrainingDisplayComponent : IComponent
    {
        public int completeTasks;
        public int totalTasksCount;
        public List<string> wordsSucceed;
        public string scrollSubject;
        public string scrollGrade;
        public string endingOfTheWord;

    }
    
    #endregion
    
    #region others

    [Library, Event(EventTarget.Self)]
    public sealed class ActiveWordComponent : IComponent
    {
    }

    [Library]
    public sealed class InfoWindowTaskComponent : IComponent
    {
    }

    [Library, Event(EventTarget.Self)]
    public sealed class ArraysOfTasksIndexesComponent : IComponent
    {
        public int[] values;
    }

    [Library, Event(EventTarget.Self)]
    public sealed class CompleteTaskComponent : IComponent
    {
        
    }
    
    [Library, Event(EventTarget.Self)]
    public sealed class FailedTaskComponent : IComponent
    {
        
    }

    [Library, FlagPrefix("request")]
    public sealed class TaskCreatedComponent : IComponent
    {
        
    }

    [Library, Event(EventTarget.Self)]
    public sealed class ObjectsBlockUnusedInfoComponent : IComponent
    {
        public bool value;
    }

    [Library]
    public sealed class DealyBeforeOpenNewTaskComponent : IComponent
    {
        public float value;
    }

    [Library]
    public sealed class WordLearnedSuccessfullyComponent : IComponent
    {
        
    }

    [Library, Cleanup(CleanupMode.RemoveComponent)]
    public sealed class ResultScreenStartComponent : IComponent
    {
        
    }
    
    
    
    #endregion

  
}