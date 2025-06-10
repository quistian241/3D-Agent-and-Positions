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
    private string API_KEY = "AIzaSyDit56QeYkewdGxLasyABDd_XgT6tajFr0"; // Please REMOVE this API key before pushing to GitHub

    public void callGemini(string prompt)
    {
        string modelPath = "https://generativelanguage.googleapis.com/v1beta/models/gemini-2.0-flash:generateContent?key=" + API_KEY;
        string startPrompt = "Behave like an AI Agent named Amy. Respond in a friendly and helpful tone. Keep your responses less than 50 words.";

        Text text = new Text();
        text.text = startPrompt + prompt;

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
    void Start()
    {
        string startPrompt = "Behave like an AI Agent named Amy. Respond in a friendly and helpful tone. Keep your responses concise. If you understand this instruction then respond with 'Hi, my name is Amy'.";
        callGemini(startPrompt);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
