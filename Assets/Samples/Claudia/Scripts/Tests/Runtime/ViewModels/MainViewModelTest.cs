using System.Collections;
using System.Threading.Tasks;
using ClaudiaApp.Models;
using ClaudiaApp.Services;
using ClaudiaApp.ViewModels;
using NUnit.Framework;
using UnityEngine.TestTools;

namespace ClaudiaApp.Tests.ViewModels
{
    [TestFixture]
    public class MainViewModelTest
    {
        [SetUp]
        public void SetUp()
        {
            _localStorageService = new MockStorageService();
            _storeService = new StoreService(_localStorageService);
            _viewModel = new MainViewModel(_storeService, _localStorageService);
        }

        private IStoreService _storeService;
        private ILocalStorageService _localStorageService;
        private IMainViewModel _viewModel;

        [Test]
        public void MainViewModelTest_SetTemperatureCommand()
        {
            // Arrange
            var temperature = 25.0f;

            // Act
            _viewModel.SetTemperatureCommand.Execute(temperature);

            // Assert
            var state = _storeService.Store.GetState<AppState>(Actions.SliceName);
            Assert.AreEqual(temperature, state.temperatureValue);
        }

        [Test]
        public void MainViewModelTest_SetSystemMessageCommand()
        {
            // Arrange
            var systemMessage = "System message";

            // Act
            _viewModel.SetSystemMessageCommand.Execute(systemMessage);

            // Assert
            var state = _storeService.Store.GetState<AppState>(Actions.SliceName);
            Assert.AreEqual(systemMessage, state.systemString);
        }

        [Test]
        public void MainViewModelTest_SetInputMessageCommand()
        {
            // Arrange
            var inputMessage = "Input message";

            // Act
            _viewModel.SetInputMessageCommand.Execute(inputMessage);

            // Assert
            var state = _storeService.Store.GetState<AppState>(Actions.SliceName);
            Assert.AreEqual(inputMessage, state.inputMessage);
        }

        [UnityTest]
        public IEnumerator MainViewModelTest_SendMessageCommand()
        {
            // Arrange
            var inputMessage = "Hello";
            _viewModel.SetInputMessageCommand.Execute(inputMessage);

            // Act
            var task = Task.Run(() => _viewModel.SendMessageCommand.ExecuteAsync(null));

            while (!task.IsCompleted) yield return null;

            // Assert
            var state = _storeService.Store.GetState<AppState>(Actions.SliceName);
            Assert.AreEqual(2, state.ChatMessages.Count);
            Assert.That(state.ChatMessages[1].Content[0].Text, Is.Not.Null);
        }

        [UnityTest]
        public IEnumerator MainViewModelTest_SendMessageCommand_WhenRunning()
        {
            // Arrange
            var inputMessage = "Hello";
            _viewModel.SetInputMessageCommand.Execute(inputMessage);

            // Act
            var task = Task.Run(() => _viewModel.SendMessageCommand.ExecuteAsync(null));
            var task2 = Task.Run(() => _viewModel.SendMessageCommand.ExecuteAsync(null));

            while (!task.IsCompleted || !task2.IsCompleted) yield return null;

            // Assert
            var state = _storeService.Store.GetState<AppState>(Actions.SliceName);
            Assert.AreEqual(2, state.ChatMessages.Count);
            Assert.That(state.ChatMessages[1].Content[0].Text, Is.Not.Null);
        }
    }
}