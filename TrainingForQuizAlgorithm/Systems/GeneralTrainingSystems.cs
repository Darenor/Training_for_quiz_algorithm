using Entitas;
using ScrollModule;

namespace LibraryModule
{ 
    public class GeneralTrainingSystems : Systems
    {
        public GeneralTrainingSystems(Contexts contexts, LibraryConfig libraryConfig)
        {
            
            Add(new GenerateTasksForTraining(contexts.library, contexts.scroll));

            Add(new ShuffleGroupOfTasksSystem(contexts.library, contexts.scroll, contexts.quiz));
            // Add(new LoadOfImagesComplete(contexts.library, contexts.quiz)); Load tasks with picture
            // Add(new StartGeneralTraining(contexts.library, contexts.scroll));
            
            Add(new QuizAnswerCompleteOrFailedSystem(contexts.quiz, contexts.library));
            // Add(new InfoWindowTaskComplete(contexts.library));
            
            Add(new CloseTaskQuizSystem(contexts.quiz, contexts.library, contexts.scroll));
            
            //*E React on trigger CompleteTask or FailedTask
            Add(new WordsLearnedOnOneOfLanguagesSystem(contexts.library, contexts.scroll));
            Add(new WordMarkAsLearnedCompletelySystem(contexts.scroll, contexts.quiz));
            Add(new QuizCompleteTrainingSystem(contexts.library, contexts.scroll));
            Add(new WordMarkedAsLearnedCompletelyOnLoadSystem(contexts.library, contexts.scroll));

            Add(new UpdateScrollTrainingInfoSystem(contexts.scroll, contexts.library));

            //*E Change progress bar on scroll window and scroll widgets
            Add(new InfoWindowProgressToScrollWindow(contexts.library, contexts.scroll));
            Add(new InfoWindowColorProgressScrollWindow(contexts.library, libraryConfig));
            
            Add(new CloseIfAllTaskCompletedSystem(contexts.library, contexts.scroll, contexts.quiz));
            
            //*E Show ending screen after all tasks complete
            Add(new ShowCollectionLabelOnFailedScreen(contexts.library, contexts.scroll));
            Add(new ShowCollectionLabelOnCompleteScreen(contexts.library, contexts.scroll));

            Add(new RestartTrainingReactSystem(contexts.library, contexts.scroll));
            Add(new DestroyTasksAfterResultScreenReactiveSystem(contexts.library, contexts.scroll));

            Add(new OnReconnectRestartGeneralTrainingSystem(contexts.network, contexts.library, contexts.scroll));

            Add(new TaskWithIconsAnswersIsActiveReactiveSystem(contexts));
        }
    }
}