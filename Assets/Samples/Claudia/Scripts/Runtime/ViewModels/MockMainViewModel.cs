using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Claudia;
using Unity.AppUI.MVVM;
using UnityEngine;

namespace ClaudiaApp.ViewModels
{
    public class MockMainViewModel : IMainViewModel
    {
        public MockMainViewModel()
        {
            ChatMessage = new List<Message>();
            SetTemperatureCommand = new RelayCommand<float>(v => Debug.Log(v));
            SetSystemMessageCommand = new RelayCommand<string>(Debug.Log);
            SetInputMessageCommand = new RelayCommand<string>(Debug.Log);
            SendMessageCommand = new AsyncRelayCommand(async () =>
            {
                await Task.Delay(1000);
                ChatMessage.Add(new Message { Role = Roles.User, Content = "Message sent" });
                OnPropertyChanged(nameof(ChatMessage));
            });
        }

        public RelayCommand<float> SetTemperatureCommand { get; }
        public RelayCommand<string> SetSystemMessageCommand { get; }
        public RelayCommand<string> SetInputMessageCommand { get; }
        public AsyncRelayCommand SendMessageCommand { get; }

        public List<Message> ChatMessage { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}