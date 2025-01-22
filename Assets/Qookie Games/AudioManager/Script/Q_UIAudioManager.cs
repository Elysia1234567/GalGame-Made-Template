using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace QAudioManager
{
    public class Q_UIAudioManager : MonoBehaviour
    {
        public Sprite musicOffSprite, soundOffSprite, soundOnSprite, musicOnSprite;
        public Image soundImage, musicImage;

        private void Start()
        {
            //Automatically plays the music at the start of the game
            if (PlayerPrefs.GetInt("music") == 1)
            {
                FindObjectOfType<AudioManager>().Play("music");
            }
            statusToggle();
        }

        public void toggleMusic()
        {
            if (PlayerPrefs.GetInt("music") == 0)
            {
                PlayerPrefs.SetInt("music", 1);
                FindObjectOfType<AudioManager>().Play("music");
            }
            else if (PlayerPrefs.GetInt("music") == 1)
            {
                PlayerPrefs.SetInt("music", 0);
                FindObjectOfType<AudioManager>().Stop("music");
            }
            clickSound();
            statusToggle();
        }

        public void toggleSound()
        {
            if (PlayerPrefs.GetInt("sound") == 0)
            {
                PlayerPrefs.SetInt("sound", 1);
            }
            else if (PlayerPrefs.GetInt("sound") == 1)
            {
                PlayerPrefs.SetInt("sound", 0);
            }
            clickSound();
            statusToggle();
        }

        public void clickSound()
        {
            FindObjectOfType<AudioManager>().Play("click");
        }

        public void statusToggle()
        {
            if (PlayerPrefs.GetInt("sound") == 0)
            {
                soundImage.sprite = soundOffSprite;
            }
            else if (PlayerPrefs.GetInt("sound") == 1)
            {
                soundImage.sprite = soundOnSprite;
            }
            if (PlayerPrefs.GetInt("music") == 0)
            {
                musicImage.sprite = musicOffSprite;
            }
            else if (PlayerPrefs.GetInt("music") == 1)
            {
                musicImage.sprite = musicOnSprite;
            }
        }
    }
}
