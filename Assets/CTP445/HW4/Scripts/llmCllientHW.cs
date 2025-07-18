using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;  // Newtonsoft.Json 네임스페이스 추가

public class llmClientHW : MonoBehaviour
{
    [SerializeField][TextArea] private string APIkey;
    [SerializeField] private string apiUrl = "https://api.openai.com/v1/chat/completions";
    [SerializeField] private string model = "gpt-4"; 
    
    public void SendPrompt(string systemPrompt, string userPrompt, System.Action<string> onResponse)
    {
        //TODO-7: 시스템 프롬프트와 씬 정보를 합쳐서 LLM에게 사전 정보를 주자.
        //HINT: string GenerateSceneJSON()
        StartCoroutine(SendRequest(systemPrompt, userPrompt, onResponse));
    }

    private IEnumerator SendRequest(string systemPrompt, string userPrompt, System.Action<string> onResponse)
    {
        var requestData = new
        {
            model = model,
            messages = new[]
            {
                new { role = "system", content = systemPrompt },
                new { role = "user", content = userPrompt }
            }
        };

        // Newtonsoft.Json을 사용하여 JSON 직렬화
        string jsonData = JsonConvert.SerializeObject(requestData);

        using (var request = new UnityWebRequest(apiUrl, "POST"))
        {
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");
            request.SetRequestHeader("Authorization", $"Bearer {apiKey}");

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("OpenAI Response: " + request.downloadHandler.text);

                // JSON 응답을 파싱
                var json = request.downloadHandler.text;
                var response = JsonConvert.DeserializeObject<OpenAIResponse>(json);
                string content = response.choices[0].message.content.Trim();
                onResponse?.Invoke(content);
            }
            else
            {
                Debug.LogError("OpenAI Error: " + request.error);
                onResponse?.Invoke(null);
            }
        }
    }
    
    string GenerateSceneJSON()
    {
    var sceneData = new SceneData();
    foreach (GameObject obj in FindObjectsOfType<GameObject>()) {
        if (obj.activeInHierarchy) {
            var renderer = obj.GetComponent<Renderer>();
            if (renderer != null) {
                sceneData.objects.Add(new SceneObject {
                    name = obj.name,
                    type = "Mesh",
                    material = renderer.sharedMaterial.name,
                    color = ColorUtility.ToHtmlStringRGB(renderer.material.color),
                    position = new float[] { obj.transform.position.x, obj.transform.position.y, obj.transform.position.z }
                });
            }
        }
    }
    return JsonConvert.SerializeObject(sceneData);
		}
		
		[System.Serializable]
		public class SceneData
		{
		    public List<SceneObject> objects = new List<SceneObject>();
		}

    [System.Serializable]
    private class OpenAIResponse
    {
        public Choice[] choices;
    }

    [System.Serializable]
    private class Choice
    {
        public Message message;
    }

    [System.Serializable]
    private class Message
    {
        public string role;
        public string content;
    }
}
