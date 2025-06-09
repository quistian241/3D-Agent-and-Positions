using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;
using Proyecto26;
using System.Threading.Tasks;
using TMPro;

public class aiTest : MonoBehaviour
{

    [System.Serializable]
    public class Text {
        public string text;
    }

    [System.Serializable] 
    public class Part {
        public List<Text> parts;
    }

    [System.Serializable]
    public class Request {
        public List<Part> contents;
    }

    public TextMeshProUGUI display;
    private string API_KEY = "API_KEY"; // Please REMOVE this API key before pushing to GitHub

    void callGemini(string prompt)
    {
        string modelPath = "https://generativelanguage.googleapis.com/v1beta/models/gemini-2.0-flash:generateContent?key=" + API_KEY;

        Text text = new Text();
        text.text = prompt;

        Part part = new Part();
        part.parts = new List<Text>();
        part.parts.Add(text);

        Request request = new Request();
        request.contents = new List<Part>();
        request.contents.Add(part);

        RestClient.Post(modelPath, request).Then(response =>
        {
            Match match = Regex.Match(response.Text, "(?<=\"text\": \")(.*?)(?=\")");
            display.text = match.Value.Replace("\\n","\n");
        }).Catch(exception =>
        {
            display.text = exception.Message;
        });
    }

    // Start is called before the first frame update
    async Task Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
