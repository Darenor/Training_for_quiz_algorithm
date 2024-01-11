using System;
using System.Collections.Generic;
using DG.Tweening;
using Entitas.Unity;
using I2.Loc;
using TMPro;
using UnityEngine;

namespace LibraryModule
{
    public class FailedSpecialTrainingWindow : MonoBehaviour, IEnabledListener, IScrollWindowFailedSpecialTrainingDisplayListener
    {
        [SerializeField] private CloseTrainingCompleteWindow _closeButton;
        [SerializeField] private CloseTrainingCompleteWindow _resetButton;
        
        [SerializeField] private TextMeshProUGUI failedLabelTxt;
        [SerializeField] private string failedText;
        [SerializeField] private TextMeshProUGUI collectionLabelTxt;
        [SerializeField] private LocalizedString failedTextLocalize;
        private Sequence _sequence;  
        
        public void Ctor(LibraryContext libraryContext)
        {
            var messageWindowEntity = gameObject.Link(libraryContext.CreateEntity()).entity as LibraryEntity;
            messageWindowEntity!.OnDestroyEntity += entty => gameObject.Unlink();

            messageWindowEntity.isFailedMessageAfterTraining = true;
            messageWindowEntity.AddEnabled(false);
            
            messageWindowEntity.AddEnabledListener(this);
            messageWindowEntity.AddScrollWindowFailedSpecialTrainingDisplayListener(this);

            _closeButton.OnClick = () => gameObject.SetActive(false);
            _resetButton.OnClick = SequenceStart;
        }

        private void SequenceStart()
        {
            // Contexts.sharedInstance.scroll.CreateEntity().
            Contexts.sharedInstance.scroll.CreateEntity().requestRestartTrainingWithWholeScrollLanguage = true;
            gameObject.SetActive(false);
        }

        public void OnEnabled(LibraryEntity entity, bool value)
        {
            gameObject.SetActive(value);
        }
        
        public void OnScrollWindowFailedSpecialTrainingDisplay(LibraryEntity entity, int failedTasksCount, int totalTasksCount, List<string> wordsFailed, string scrollSubject, string endingOfTheWord)
        {
            // gameObject.SetActive(true);
            collectionLabelTxt.SetText(scrollSubject);
            var stringOfWordsFailed = String.Join("<br>", wordsFailed);
            var failedTextLocal = failedTextLocalize.ToString();
            
            failedLabelTxt.SetText(failedTextLocal.Replace("#1",$"{failedTasksCount}").Replace("#2", $"{totalTasksCount}").Replace("#4", $"{stringOfWordsFailed}").Replace("#n", "\n"));
        }
    }
}