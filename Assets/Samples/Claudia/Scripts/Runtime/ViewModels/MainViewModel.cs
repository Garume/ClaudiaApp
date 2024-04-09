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

            _chatMessage = storeService.Store.GetState<AppState>(Actions.SliceName).ChatMessages;

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
                await _storeService.Store.DispatchAsync(Actions.RequestMessage, token);
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