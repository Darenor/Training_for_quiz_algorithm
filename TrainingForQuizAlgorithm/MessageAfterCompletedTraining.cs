using System.Collections.Generic;
using Entitas.Unity;
using TMPro;
using UnityEngine;

namespace LibraryModule
{
    public class MessageAfterCompletedTraining : MonoBehaviour, IEnabledListener, IScrollWindowCompleteSpecialTrainingDisplayListener
    {
        [SerializeField] private CloseTrainingCompleteWindow _closeButton;
        [SerializeField] private TextMeshProUGUI collectionLabelTxt;
        
        [SerializeField] private TextMeshProUGUI completeLabelTxt;
        [SerializeField] private string completeText;
        
        public void Ctor(LibraryContext libraryContext)
        {
            var messageWindowEntity = gameObject.Link(libraryContext.CreateEntity()).entity as LibraryEntity;
            messageWindowEntity!.OnDestroyEntity += entty => gameObject.Unlink();

            messageWindowEntity.isCompletedMessageAfterTraining = true;
            messageWindowEntity.AddEnabled(false);
            
            messageWindowEntity.AddEnabledListener(this);
            messageWindowEntity.AddScrollWindowCompleteSpecialTrainingDisplayListener(this);

            _closeButton.OnClick = () => gameObject.SetActive(false);
        }

        public void OnEnabled(LibraryEntity entity, bool value)
        {
            gameObject.SetActive(value);
        }

        public void OnScrollWindowCompleteSpecialTrainingDisplay(LibraryEntity entity, int failedTasksCount, int totalTasksCount, string scrollSubject, string scrollGrade, string endingOfTheWord)
        {
            gameObject.SetActive(true);
            collectionLabelTxt.SetText(scrollSubject);
            completeLabelTxt.SetText(completeText.Replace("#1", $"{failedTasksCount}").Replace("#2", $"{totalTasksCount}").Replace("#3", $"{endingOfTheWord}"));
        }

        public void OnScrollWindowCompleteSpecialTrainingDisplay(LibraryEntity entity, int failedTasksCount, int totalTasksCount, List<string> wordsSucceed, string scrollSubject, string scrollGrade, string endingOfTheWord)
        {
            gameObject.SetActive(true);
            collectionLabelTxt.SetText(scrollSubject);
            completeLabelTxt.SetText(completeText.Replace("#1", $"{failedTasksCount}").Replace("#2", $"{totalTasksCount}").Replace("#3", $"{endingOfTheWord}").Replace("#4", $"{wordsSucceed}"));
        }
    }
}