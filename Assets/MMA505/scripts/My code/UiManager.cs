using System.Collections.Generic;
using TMPro;
using UnityEngine;


namespace MMA505
{
    public class UiManager : MonoBehaviour
    {

        [SerializeField] private TextMeshProUGUI _text1;
        [SerializeField] private TextMeshProUGUI _text2;

        #region Initialisation & Destruction

        private void Awake()
        {
            GameManager.UpdateUi += UpdateUi;
            PlayerCamControler.OnPhotoTake += UpdateLastPhoto;
        }

        private void OnDestroy()
        {
            GameManager.UpdateUi -= UpdateUi;
            PlayerCamControler.OnPhotoTake -= UpdateLastPhoto;

        }

        #endregion

        public void UpdateUi(List<Photo> photos)
        {
            string text = "";
            int photoInx = 0;
            foreach (Photo photo in photos)
            {
                photoInx++;
                if (photo.GetFoundPhoto() != null)
                {
                    text += $"Photo{photoInx} Complete: {photo.GetFoundPhoto()}";
                }
                else
                {
                    text += $"Photo{photoInx} incomplete:\nGet a {photo}";
                }
            }

            _text1.text = text;

        }

        public void UpdateLastPhoto(Photo photo)
        {
            string text = $"Last photo \nSubject: {photo.GetObject().name}\nDistences from camra: {photo.GetDist()}\nThey where are {photo.GetState()}";
            _text2.text = text;
        }
    }
}
