using System.Collections.Generic;
using Claudia;
using Unity.AppUI.Redux;

namespace ClaudiaApp.Models
{
    public static class Actions
    {
        public const string SliceName = "app";
        public const string SetTemperature = SliceName + "/SetTemparature";
        public const string SetInputMessage = SliceName + "/SetInputMessage";
        public const string SetSystemString = SliceName + "/SetSystemString";
        public const string SetChatMessage = SliceName + "/SetChatMessage";
        public const string RequestMessage = SliceName + "/RequestMessage";

        public static readonly ActionCreator<List<Message>> SetChatMessageAction =
            Store.CreateAction<List<Message>>(SetChatMessage);
    }
}