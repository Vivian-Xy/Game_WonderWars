using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
using System.Text;

public class BackendManager : MonoBehaviour
{
    public string serverURL = "http://localhost:5000/api"; // Replace with your server address

    [System.Serializable]
    public class Question
    {
        public string id; // Add this line to expose the question ID
        public string questionText;
        public List<string> options;
        public string correctAnswer;
        public string monumentUnlocked;
        public string category;
        public string difficulty;
    }

    [System.Serializable]
    public class QuestionList
    {
        public List<Question> questions;
    }

    [System.Serializable]
    public class UserLogin
    {
        public string email;
        public string password;
    }

    [System.Serializable]
    public class UserRegister
    {
        public string username;
        public string email;
        public string password;
        public string avatar;
    }

    [System.Serializable]
    public class ProgressUpdate
    {
        public string completedQuestionId;
        public string unlockedMonumentId;
        public int score;
    }

    // 🚀 REGISTER
    public IEnumerator RegisterUser(string username, string email, string password, string avatar)
    {
        UserRegister newUser = new UserRegister
        {
            username = username,
            email = email,
            password = password,
            avatar = avatar
        };

        string json = JsonUtility.ToJson(newUser);
        UnityWebRequest req = new UnityWebRequest($"{serverURL}/users/register", "POST");
        byte[] bodyRaw = Encoding.UTF8.GetBytes(json);

        req.uploadHandler = new UploadHandlerRaw(bodyRaw);
        req.downloadHandler = new DownloadHandlerBuffer();
        req.SetRequestHeader("Content-Type", "application/json");

        yield return req.SendWebRequest();

        if (req.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("✅ Registered user: " + req.downloadHandler.text);
        }
        else
        {
            Debug.LogError("❌ Register failed: " + req.error);
        }
    }

    // 🔐 LOGIN
    public IEnumerator LoginUser(string email, string password)
    {
        UserLogin loginData = new UserLogin { email = email, password = password };
        string json = JsonUtility.ToJson(loginData);

        UnityWebRequest req = new UnityWebRequest($"{serverURL}/users/login", "POST");
        byte[] bodyRaw = Encoding.UTF8.GetBytes(json);

        req.uploadHandler = new UploadHandlerRaw(bodyRaw);
        req.downloadHandler = new DownloadHandlerBuffer();
        req.SetRequestHeader("Content-Type", "application/json");

        yield return req.SendWebRequest();

        if (req.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("✅ Logged in: " + req.downloadHandler.text);
            // You can parse the token & user here if needed
        }
        else
        {
            Debug.LogError("❌ Login failed: " + req.error);
        }
    }

    // 🔐 LOGIN with callback
    public IEnumerator LoginUser(string email, string password, System.Action<bool, string> callback)
    {
        // 1. Build payload
        var payload = new { email, password };
        string json = JsonUtility.ToJson(payload);

        // 2. Create and send request
        using var req = new UnityWebRequest($"{serverURL}/users/login", "POST");
        byte[] body = Encoding.UTF8.GetBytes(json);
        req.uploadHandler = new UploadHandlerRaw(body);
        req.downloadHandler = new DownloadHandlerBuffer();
        req.SetRequestHeader("Content-Type", "application/json");
        yield return req.SendWebRequest();

        // 3. Invoke callback with status + response text
        bool success = req.result == UnityWebRequest.Result.Success;
        callback?.Invoke(success, req.downloadHandler.text);
    }

    // 🚀 REGISTER with callback
    public IEnumerator RegisterUser(
        string username,
        string email,
        string password,
        string avatar,
        System.Action<bool, string> callback
    )
    {
        var payload = new { username, email, password, avatar };
        string json = JsonUtility.ToJson(payload);

        using var req = new UnityWebRequest($"{serverURL}/users/register", "POST");
        byte[] body = Encoding.UTF8.GetBytes(json);
        req.uploadHandler   = new UploadHandlerRaw(body);
        req.downloadHandler = new DownloadHandlerBuffer();
        req.SetRequestHeader("Content-Type", "application/json");
        yield return req.SendWebRequest();

        bool success = req.result == UnityWebRequest.Result.Success;
        callback?.Invoke(success, req.downloadHandler.text);
    }

    // ❓ GET TRIVIA QUESTIONS
    public IEnumerator GetQuestions()
    {
        UnityWebRequest req = UnityWebRequest.Get($"{serverURL}/questions");

        yield return req.SendWebRequest();

        if (req.result == UnityWebRequest.Result.Success)
        {
            string json = "{\"questions\":" + req.downloadHandler.text + "}";
            QuestionList qList = JsonUtility.FromJson<QuestionList>(json);
            Debug.Log("✅ Questions Loaded: " + qList.questions.Count);
            // Store or display questions in UI
        }
        else
        {
            Debug.LogError("❌ Failed to load questions: " + req.error);
        }
    }

    // Fetch all questions
    public IEnumerator GetQuestions(System.Action<List<Question>> callback)
    {
        using var req = UnityWebRequest.Get($"{serverURL}/questions");
        yield return req.SendWebRequest();

        if (req.result == UnityWebRequest.Result.Success)
        {
            // wrap array into object for JsonUtility
            string json = "{\"questions\":" + req.downloadHandler.text + "}";
            var list = JsonUtility.FromJson<QuestionList>(json).questions;
            callback?.Invoke(list);
        }
        else
        {
            Debug.LogError("GetQuestions failed: " + req.error);
            callback?.Invoke(new List<Question>());
        }
    }

    // 🧠 SEND PROGRESS UPDATE
    public IEnumerator UpdateProgress(string userId, string questionId, string monumentId, int score)
    {
        ProgressUpdate progress = new ProgressUpdate
        {
            completedQuestionId = questionId,
            unlockedMonumentId = monumentId,
            score = score
        };

        string json = JsonUtility.ToJson(progress);
        UnityWebRequest req = UnityWebRequest.Put($"{serverURL}/users/{userId}/progress", json);
        byte[] bodyRaw = Encoding.UTF8.GetBytes(json);

        req.uploadHandler = new UploadHandlerRaw(bodyRaw);
        req.downloadHandler = new DownloadHandlerBuffer();
        req.SetRequestHeader("Content-Type", "application/json");

        yield return req.SendWebRequest();

        if (req.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("✅ Progress updated: " + req.downloadHandler.text);
        }
        else
        {
            Debug.LogError("❌ Update failed: " + req.error);
        }
    }

    // Update progress
    public IEnumerator UpdateProgress(
        string userId,
        string questionId,
        string monumentId,
        int score,
        System.Action<bool, string> callback
    )
    {
        var payload = new { completedQuestionId = questionId, unlockedMonumentId = monumentId, score };
        string json = JsonUtility.ToJson(payload);

        using var req = new UnityWebRequest($"{serverURL}/users/{userId}/progress", "PUT");
        byte[] body = Encoding.UTF8.GetBytes(json);
        req.uploadHandler   = new UploadHandlerRaw(body);
        req.downloadHandler = new DownloadHandlerBuffer();
        req.SetRequestHeader("Content-Type", "application/json");
        yield return req.SendWebRequest();

        bool ok = req.result == UnityWebRequest.Result.Success;
        callback?.Invoke(ok, req.downloadHandler.text);
    }

    // Add this coroutine to fetch user data
    public IEnumerator GetUserData(string userId, string authToken, System.Action<bool, string> callback)
    {
        string url = $"https://your.api.endpoint/users/{userId}";
        UnityWebRequest request = UnityWebRequest.Get(url);
        request.SetRequestHeader("Authorization", $"Bearer {authToken}");

        yield return request.SendWebRequest();

#if UNITY_2020_1_OR_NEWER
        if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
#else
        if (request.isNetworkError || request.isHttpError)
#endif
        {
            Debug.LogError("GetUserData error: " + request.error);
            callback?.Invoke(false, null);
        }
        else
        {
            callback?.Invoke(true, request.downloadHandler.text);
        }
    }
}