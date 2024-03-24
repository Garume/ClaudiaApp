using System.Collections.Generic;
using System.ComponentModel;
using Claudia;
using Unity.AppUI.MVVM;

namespace ClaudiaApp.ViewModels
{
    public interface IMainViewModel : INotifyPropertyChanged
    {
        public RelayCommand<float> SetTemperatureCommand { get; }
        public RelayCommand<string> SetSystemMessageCommand { get; }
        public RelayCommand<string> SetInputMessageCommand { get; }
        public AsyncRelayCommand SendMessageCommand { get; }

        public List<Message> ChatMessage { get; set; }
    }
}