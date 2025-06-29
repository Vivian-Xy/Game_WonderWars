using System;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json; // Make sure Newtonsoft.Json is installed in your project

// Wrapper class for Unity's JsonUtility to parse a list of questions
[Serializable]
public class QuestionListWrapper<T>
{
    public List<T> questions;
}

public static class JsonUtilityWrapper
{
    /// <summary>
    /// Converts a raw JSON array (e.g. [ {...}, {...} ]) into a List<T> using Newtonsoft.Json.
    /// </summary>
    /// <typeparam name="T">The type of the question data (e.g. QuestionData)</typeparam>
    /// <param name="rawJson">The raw JSON array string</param>
    /// <returns>List of questions of type T</returns>
    public static List<T> FromJson<T>(string rawJson)
    {
        if (string.IsNullOrEmpty(rawJson))
        {
            Debug.LogError("JsonUtilityWrapper: Input JSON is null or empty.");
            return new List<T>();
        }

        try
        {
            // Directly parse the JSON array into a List<T>
            return JsonConvert.DeserializeObject<List<T>>(rawJson);
        }
        catch (Exception ex)
        {
            Debug.LogError("JsonUtilityWrapper: Failed to parse JSON with Newtonsoft.Json. " + ex.Message);
            return new List<T>();
        }
    }
}