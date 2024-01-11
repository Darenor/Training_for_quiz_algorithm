using System;
using System.Collections.Generic;
using Entitas.Unity;
using I2.Loc;
using TMPro;
using UnityEngine;

namespace LibraryModule
{
    public class CompleteSpecialTrainingWindow : MonoBehaviour, IEnabledListener, IScrollWindowCompleteSpecialTrainingDisplayListener
    {
        [SerializeField] private CloseTrainingCompleteWindow _closeButton;
        [SerializeField] private TextMeshProUGUI collectionLabelTxt;
        
        [SerializeField] private TextMeshProUGUI completeLabelTxt;
        [SerializeField] private string completeText;
        [SerializeField] private LocalizedString completeTextLocalize;
        
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

        public void OnScrollWindowCompleteSpecialTrainingDisplay(LibraryEntity entity, int failedTasksCount, int totalTasksCount, List<string> wordsSucceed, string scrollSubject, string scrollGrade, string endingOfTheWord)
        {
            // gameObject.SetActive(true);
            collectionLabelTxt.SetText(scrollSubject);

            var wordsLearnedSuccessfully = String.Join("<br>", wordsSucceed);
            var completeTextLocal= completeTextLocalize;
            completeLabelTxt.SetText(completeTextLocal.ToString().Replace("#4", $"{wordsLearnedSuccessfully}").Replace("#n", "\n"));
        }
    }
}