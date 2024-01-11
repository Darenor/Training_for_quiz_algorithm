using UnityEngine;
using UnityEngine.EventSystems;

namespace LibraryModule
{
    public class ContinueButtonOnIncorrectWindow : MonoBehaviour, IPointerClickHandler
    {
        public void OnPointerClick(PointerEventData eventData)
        {
            LibraryEntity _continueEnt = Contexts.sharedInstance.library.CreateEntity();
            _continueEnt.requestContinueAfterMistakeQuizTraining = true;
        }
    }
}