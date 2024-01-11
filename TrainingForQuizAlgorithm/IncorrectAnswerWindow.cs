using Entitas.Unity;
using I2.Loc;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

namespace LibraryModule
{
    public class IncorrectAnswerWindow : MonoBehaviour, IWordInfoNativeAndTargetWordListener, IIncorrectAnswerNativeAndTargetWordListener,
        IEnabledListener
    {
        [SerializeField] private TextMeshProUGUI _correctInfoTMP;
        [SerializeField] private string _correctInfoText;
        [SerializeField] private Localize _correctInfoLZ;
        [SerializeField] private TextMeshProUGUI _incorrectInforTMP;
        [SerializeField] private string _incorrectInfoText;
        [SerializeField] private Localize _incorrectInfoLZ;

        public void Ctor(LibraryContext libraryContext)
        {
            var incorrectInfoEntity = gameObject.Link(libraryContext.CreateEntity()).entity as LibraryEntity;
            incorrectInfoEntity!.OnDestroyEntity += entty => gameObject.Unlink();

            incorrectInfoEntity.isIncorrectAnswerWindow = true;
            incorrectInfoEntity.AddWordInfoNativeAndTargetWord("native","target");
            incorrectInfoEntity.AddEnabled(false);
            
            incorrectInfoEntity.AddWordInfoNativeAndTargetWordListener(this);
            incorrectInfoEntity.AddIncorrectAnswerNativeAndTargetWordListener(this);
            incorrectInfoEntity.AddEnabledListener(this);
        }

        public void OnWordInfoNativeAndTargetWord(LibraryEntity entity, string native, string target)
        {
            _correctInfoText = LocalizationManager.GetTranslation(_correctInfoLZ.mTerm);
            // Debug.LogError("e IncorrectAnswerWindow + repair"); 
            _correctInfoTMP.SetText(_correctInfoText.Replace("#1", $"{native}").Replace("#2", $"{target}")); 
        }

        public void OnIncorrectAnswerNativeAndTargetWord(LibraryEntity entity, string nativeIncorrect, string targetIncorrect)
        {
            _incorrectInfoText = LocalizationManager.GetTranslation(_incorrectInfoLZ.mTerm);
            _incorrectInforTMP.SetText(_incorrectInfoText.Replace("#3", $"{nativeIncorrect}").Replace("#4", $"{targetIncorrect}"));
        }


        public void OnEnabled(LibraryEntity entity, bool value)
        {
            gameObject.SetActive(value);
        }
    }
}