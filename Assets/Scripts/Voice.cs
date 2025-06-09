using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Meta.WitAi.Dictation;
using TMPro;
using Oculus.Voice.Dictation;
using UnityEngine.Android;

public class Voice : MonoBehaviour
{
    public AppDictationExperience dictationExperience;
    public TextMeshProUGUI displayText;
    // Start is called before the first frame update
    void Start()
    {
        if (!HasMicrophonePermission())
        {
            RequestMicrophonePermission();
        }
        dictationExperience.DictationEvents.onFullTranscription.AddListener(OnFullTranscription);
    }
    private void OnFullTranscription(string text)
    {
        if (displayText != null)
        {
            displayText.text = text;
        }
    }


    public void StartListening()
    {
        dictationExperience.Activate();
    }

    public void StopListening()
    {
        dictationExperience.Deactivate();
    }

    public void ToggleListening()
    {
        if (HasMicrophonePermission())
        {
            if (dictationExperience.Active)
            {
                StopListening();
            }
            else
            {
                StartListening();
            }
        }
        else
        {
            RequestMicrophonePermission();
        }
    }

    private bool HasMicrophonePermission()
    {
#if PLATFORM_ANDROID
        return Permission.HasUserAuthorizedPermission(Permission.Microphone);
#else
        return true;
#endif
    }

    private void RequestMicrophonePermission()
    {
#if PLATFORM_ANDROID
        Permission.RequestUserPermission(Permission.Microphone);
#endif
    }
}
