using Entitas.Unity;
using UnityEngine;
using UnityEngine.EventSystems;

namespace LibraryModule
{
    public class ContinueButton : MonoBehaviour, IPointerClickHandler
    {
        public void OnPointerClick(PointerEventData eventData)
        {
            LibraryEntity _continueEnt = Contexts.sharedInstance.library.CreateEntity();
            _continueEnt.requestTaskCreated = true;
            _continueEnt.ReplaceIndexTasks(13);
            _continueEnt.requestContinueButtonAction = true;
            // _continueEnt.requestStartQuizTraining = true;
            

        }
    }
}