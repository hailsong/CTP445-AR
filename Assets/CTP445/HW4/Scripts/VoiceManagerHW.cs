using UnityEngine;
using UnityEngine.Events;
using TMPro;
using Oculus.Voice;
using System.Reflection;
using Meta.WitAi.CallbackHandlers;

public class VoiceManagerHW : MonoBehaviour
{
    [Header("Wit Configuration")]
    [SerializeField] private AppVoiceExperience appVoiceExperience;
    [SerializeField] private WitResponseMatcher responseMatcher;
    [SerializeField] private TextMeshProUGUI transcriptionText;
    
    [Header("Voice Events")]
    // wake word 실행 사운드 재생 이벤트
    [SerializeField] private UnityEvent wakeWordDetected; 
    // 문장 마무리 시 사운드 재생 이벤트
    [SerializeField] private UnityEvent<string> completeTranscription;
    //HINT - Brownie Point 2: wake word 활성 시각 이펙트 이벤트를 만들어보자

		[Header("LLM")]
    [SerializeField] private llmClient llmClient;
    //TODO-6: 
    [SerializeField][TextArea] private string systemPrompt;
    [SerializeField] private TextMeshProUGUI llmText;

    private bool _voiceCommandReady;

    private void Awake()
    {
        appVoiceExperience.VoiceEvents.OnRequestCompleted.AddListener(ReactivateVoice);
        appVoiceExperience.VoiceEvents.OnPartialTranscription.AddListener(OnPartialTranscription);
        appVoiceExperience.VoiceEvents.OnFullTranscription.AddListener(OnFullTranscription);

        var eventField = typeof(WitResponseMatcher).GetField("onMultiValueEvent", BindingFlags.NonPublic | BindingFlags.Instance);
        if (eventField != null && eventField.GetValue(responseMatcher) is MultiValueEvent onMultiValueEvent)
        {
            // onMultiValueEvent 는 wake word를 판별하기 위한 수단 entity와 intent를 비교하여서 같을 경우 호출
            onMultiValueEvent.AddListener(WakeWordDetected);
        }

        appVoiceExperience.Activate();
    }
    
    private void Update()
    {
	      //HINT - Brownie Point 1: .AddListener(WakeWordDetected)를 활용하여 손동작/버튼을 통해 음성명령을 활성화 하자 
    }

    private void OnDestroy()
    {
        appVoiceExperience.VoiceEvents.OnRequestCompleted.RemoveListener(ReactivateVoice);
        appVoiceExperience.VoiceEvents.OnPartialTranscription.RemoveListener(OnPartialTranscription);
        appVoiceExperience.VoiceEvents.OnFullTranscription.RemoveListener(OnFullTranscription);

        var eventField = typeof(WitResponseMatcher).GetField("onMultiValueEvent", BindingFlags.NonPublic | BindingFlags.Instance);
        if (eventField != null && eventField.GetValue(responseMatcher) is MultiValueEvent onMultiValueEvent)
        {
            //TODO-0: WakeWordDetected 함수 이벤트를 끄자
        }
    }
    
    private void ReactivateVoice() => appVoiceExperience.Activate();

    private void WakeWordDetected(string[] arg0)
    {
        //TODO-1: OnPartialTranscription, OnFullTranscription 함수를 실행할 수 있는 조건을 만들자
        //HINT: 두 함수 내부 구조 참고
        
        wakeWordDetected?.Invoke();
        
        //HINT - Brownie Point 2: wake word 활성 시각 이펙트
    }

    private void OnPartialTranscription(string transcription)
    {
        if (!_voiceCommandReady) return;
        
        //TODO-2: TextMeshProUGUI transcriptionText에 명령발화(transcription)를 실시간으로 할당하자
    }

    private void OnFullTranscription(string transcription)
    {
        if (!_voiceCommandReady) return;
        _voiceCommandReady = false;
        
        //TODO-3: transcription이 들어왔을 때 completeTranscription 사운드 재생 이벤트를 실행하자
        //HINT: UnityEvent<string> 문자열 이벤트 구조 및 invoke 방식 고려.
        
        //TODO-4: llmClient 스크립트 안에 함수를 활용하여 LLM에게 명령발화랑 시스템프롬프트를 보내고, OnLLMResponse를 실행시키자.
        //HINT: llmClient.cs 구조 참고.
    }
    
    // LLM이 응답을 주었을 때 실행하는 이벤트 함수
    private void OnLLMResponse(string llmResponse)
    {
        Debug.Log("LLM Answer: " + llmResponse);

        //TODO-5: TextMeshProUGUI llmText에 LLM이 보낸 내용을 실시간으로 할당하자
    }
}