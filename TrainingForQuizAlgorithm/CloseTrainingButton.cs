using UnityEngine;
using UnityEngine.EventSystems;

namespace LibraryModule
{
    public class CloseTrainingButton : MonoBehaviour, IPointerClickHandler
    {
        public void OnPointerClick(PointerEventData eventData)
        {
            LibraryEntity _buttonEnt = Contexts.sharedInstance.library.CreateEntity();
            _buttonEnt.requestCloseButtonAction = true;
        }
    }
}