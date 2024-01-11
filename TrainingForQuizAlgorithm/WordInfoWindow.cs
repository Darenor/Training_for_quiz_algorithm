using Entitas.Unity;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

namespace LibraryModule
{
    public class WordInfoWindow : MonoBehaviour, IWordInfoNativeAndTargetWordListener, IEnabledListener,
        IObjectsBlockUnusedInfoListener
    {
        [SerializeField] private TextMeshProUGUI _infoTMP;
        [SerializeField] private string _infoText;
        [SerializeField] private GameObject _blockObject;
        
        public void Ctor(LibraryContext libraryContext)
        {
            var wordInfoEntity = gameObject.Link(libraryContext.CreateEntity()).entity as LibraryEntity;
            wordInfoEntity!.OnDestroyEntity += entty => gameObject.Unlink();

            wordInfoEntity.isWordInfoWindow = true;
            wordInfoEntity.AddWordInfoNativeAndTargetWord("native","target");
            wordInfoEntity.AddEnabled(false);
            wordInfoEntity.AddObjectsBlockUnusedInfo(false);
            wordInfoEntity.AddWordIdsForInfoWindow(null);
            wordInfoEntity.AddSpecialTrainingProgress(0);
            
            wordInfoEntity.AddWordInfoNativeAndTargetWordListener(this);
            wordInfoEntity.AddEnabledListener(this);
            wordInfoEntity.AddObjectsBlockUnusedInfoListener(this);
        }

        public void OnWordInfoNativeAndTargetWord(LibraryEntity entity, string native, string target)
        {
            _infoTMP.SetText(_infoText.Replace("#1", $"{native}").Replace("#2", $"{target}")); 
        }

        public void OnEnabled(LibraryEntity entity, bool value)
        {
            gameObject.SetActive(value);
        }

        public void OnObjectsBlockUnusedInfo(LibraryEntity entity, bool value)
        {
            _blockObject.SetActive(!value);
        }
    }
}