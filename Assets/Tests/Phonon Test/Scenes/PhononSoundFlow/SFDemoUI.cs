using UnityEngine;
using UnityEngine.UI;

public class SFDemoUI : MonoBehaviour 
{

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            var sfSources = GameObject.FindObjectsOfType<Phonon.PhononSoundFlowSource>();

            var offImage = GameObject.Find("Image (off)").GetComponent<Image>();
            var offText = GameObject.Find("Text (off)").GetComponent<Text>();
            var onImage = GameObject.Find("Image (on)").GetComponent<Image>();
            var onText = GameObject.Find("Text (on)").GetComponent<Text>();

            if (PhononEnabled)
            {
                foreach (var sfSource in sfSources)
                {
                    var audioSource = sfSource.GetComponent<AudioSource>();
                    audioSource.spatialBlend = 1.0f;
                    sfSource.enabled = false;
                }

                onImage.enabled = false;
                onText.enabled = false;
                offImage.enabled = true;
                offText.enabled = true;
            }
            else
            {
                foreach (var sfSource in sfSources)
                {
                    var audioSource = sfSource.GetComponent<AudioSource>();
                    audioSource.spatialBlend = 0.0f;
                    sfSource.enabled = true;
                }

                offImage.enabled = false;
                offText.enabled = false;
                onImage.enabled = true;
                onText.enabled = true;
            }

            PhononEnabled = !PhononEnabled;
        }
    }

    bool PhononEnabled = true;

}
