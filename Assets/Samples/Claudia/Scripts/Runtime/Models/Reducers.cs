using System.Collections.Generic;
using Claudia;
using Unity.AppUI.Redux;

namespace ClaudiaApp.Models
{
    public class Reducers
    {
        public static AppState SetTemperatureValueReducer(AppState state, Action<float> action)
        {
            return state with { temperatureValue = action.payload };
        }

        public static AppState SetInputMessageReducer(AppState state, Action<string> action)
        {
            return state with { inputMessage = action.payload };
        }

        public static AppState SetSystemStringReducer(AppState state, Action<string> action)
        {
            return state with { systemString = action.payload };
        }

        public static AppState SetChatMessage(AppState state, Action<List<Message>> action)
        {
            return state with { ChatMessages = action.payload };
        }
    }
}