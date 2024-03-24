using System;
using System.Collections.Generic;
using Claudia;
using UnityEngine;

namespace ClaudiaApp.Models
{
    [Serializable]
    public record AppState
    {
        [SerializeField] public float temperatureValue;
        [SerializeField] public string inputMessage;
        [SerializeField] public string systemString;
        [SerializeField] public string apiKey;

        [SerializeField] public Anthropic Anthropic = new()
        {
            ApiKey = "Please use your own API key.",
        };

        [SerializeField] public List<Message> ChatMessages = new();
    }
}