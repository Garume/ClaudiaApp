using System.Collections.Generic;
using Claudia;
using ClaudiaApp.Models;
using NUnit.Framework;
using Unity.AppUI.Redux;

namespace ClaudiaApp.Tests.Models
{
    [TestFixture]
    public class ReducersTest
    {
        [Test]
        public void ReducersTest_SetTemperatureValueReducer()
        {
            // Arrange
            var state = new AppState();
            var action = Store.CreateAction<float>(Actions.SetTemperature);
            var temperature = 0.5f;

            // Act
            var newState = Reducers.SetTemperatureValueReducer(state, action.Invoke(temperature));

            // Assert
            Assert.That(newState.temperatureValue, Is.EqualTo(temperature));
        }

        [Test]
        public void ReducersTest_SetInputMessageReducer()
        {
            // Arrange
            var state = new AppState();
            var action = Store.CreateAction<string>(Actions.SetInputMessage);
            var inputMessage = "Input message";

            // Act
            var newState = Reducers.SetInputMessageReducer(state, action.Invoke(inputMessage));

            // Assert
            Assert.That(newState.inputMessage, Is.EqualTo(inputMessage));
        }

        [Test]
        public void ReducersTest_SetSystemStringReducer()
        {
            // Arrange
            var state = new AppState();
            var action = Store.CreateAction<string>(Actions.SetSystemString);
            var systemString = "System message";

            // Act
            var newState = Reducers.SetSystemStringReducer(state, action.Invoke(systemString));

            // Assert
            Assert.That(newState.systemString, Is.EqualTo(systemString));
        }

        [Test]
        public void ReducersTest_SetChatMessage()
        {
            // Arrange
            var state = new AppState();
            var action = Store.CreateAction<List<Message>>(Actions.SetChatMessage);
            var chatMessage = new List<Message> { new() { Content = "", Role = Roles.User } };

            // Act
            var newState = Reducers.SetChatMessage(state, action.Invoke(chatMessage));

            // Assert
            Assert.That(newState.ChatMessages, Is.EqualTo(chatMessage));
        }
    }
}