using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace LibraryModule
{
    public class CloseTrainingCompleteWindow : MonoBehaviour, IPointerClickHandler
    {
        public Action OnClick;

        public void OnPointerClick(PointerEventData eventData)
        {
            OnClick?.Invoke();
        }
    }
}