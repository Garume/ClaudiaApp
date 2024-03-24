using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Claudia;
using ClaudiaApp.Models;
using ClaudiaApp.Services;
using Unity.AppUI.MVVM;
using Unity.AppUI.Redux;
using UnityEngine;

namespace ClaudiaApp.ViewModels
{
    public class MainViewModel : ObservableObject, IMainViewModel
    {
        private readonly ILocalStorageService _localStorageService;
        private readonly IStoreService _storeService;
        private readonly Unsubscriber _unSubscriber;
        private List<Message> _chatMessage;

        private bool _running;

        public MainViewModel(IStoreService storeService, ILocalStorageService localService)
        {
            _storeService = storeService;
            _localStorageService = localService;

            SetTemperatureCommand = new RelayCommand<float>(SetTemperature);
            SetSystemMessageCommand = new RelayCommand<string>(SetSystemMessage);
            SetInputMessageCommand = new RelayCommand<string>(SetInputMessage);
            SendMessageCommand = new AsyncRelayCommand(SendMessage);

            var initialState = localService.GetValue(Actions.SliceName, new AppState());
            _storeService.Store.CreateSlice(Actions.SliceName, initialState, builder =>
            {
                builder
                    .Add<float>(Actions.SetTemperature, Reducers.SetTemperatureValueReducer)
                    .Add<List<Message>>(Actions.SetChatMessage, Reducers.SetChatMessage)
                    .Add<string>(Actions.SetInputMessage, Reducers.SetInputMessageReducer)
                    .Add<string>(Actions.SetSystemString, Reducers.SetSystemStringReducer);
            });

            _chatMessage = initialState.ChatMessages;

            _unSubscriber = storeService.Store.Subscribe<AppState>(Actions.SliceName, OnStateChanged);
            App.shuttingDown += OnShuttingDown;
        }

        public List<Message> ChatMessage
        {
            get => _chatMessage;
            set
            {
                _chatMessage = value;
                OnPropertyChanged();
            }
        }

        public RelayCommand<float> SetTemperatureCommand { get; }
        public RelayCommand<string> SetSystemMessageCommand { get; }
        public RelayCommand<string> SetInputMessageCommand { get; }
        public AsyncRelayCommand SendMessageCommand { get; }

        private void OnStateChanged(AppState state)
        {
            ChatMessage = state.ChatMessages;
        }

        private async Task SendMessage(CancellationToken token)
        {
            if (_running) return;

            _running = true;

            try
            {
                var state = _storeService.Store.GetState<AppState>(Actions.SliceName);
                var anthropic = state.Anthropic;
                ChatMessage.Add(new Message { Role = Roles.User, Content = state.inputMessage });
                var stream = anthropic.Messages.CreateStreamAsync(new MessageRequest
                {
                    Model = Claudia.Models.Claude3Opus,
                    MaxTokens = 1024,
                    Temperature = state.temperatureValue,
                    System = string.IsNullOrEmpty(state.systemString) ? null : state.systemString,
                    Messages = state.ChatMessages.ToArray()
                }, cancellationToken: token);

                var currentMessage = new Message
                {
                    Role = Roles.Assistant,
                    Content = ""
                };

                ChatMessage.Add(currentMessage);


                await foreach (var messageStreamEvent in stream)
                    if (messageStreamEvent is ContentBlockDelta content)
                    {
                        currentMessage.Content[0].Text += content.Delta.Text;
                        _storeService.Store.Dispatch(Actions.SetChatMessage, ChatMessage);
                    }
            }
            catch (Exception e)
            {
                Debug.Log(e);
            }
            finally
            {
                _running = false;
            }
        }

        private void SetInputMessage(string message)
        {
            _storeService.Store.Dispatch(Actions.SetInputMessage, message);
        }

        private void SetSystemMessage(string message)
        {
            _storeService.Store.Dispatch(Actions.SetSystemString, message);
        }

        private void SetTemperature(float temperature)
        {
            _storeService.Store.Dispatch(Actions.SetTemperature, temperature);
        }

        private void OnShuttingDown()
        {
            _localStorageService.SetValue(Actions.SliceName, _storeService.Store.GetState<AppState>(Actions.SliceName));
            App.shuttingDown -= OnShuttingDown;
            _unSubscriber?.Invoke();
        }
    }
}