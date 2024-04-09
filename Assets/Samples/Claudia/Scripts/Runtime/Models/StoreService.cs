using System.Collections.Generic;
using Claudia;
using ClaudiaApp.Models;
using R3;
using Unity.AppUI.Redux;
using UnityReduxMiddleware;
using UnityReduxMiddleware.Epic;
using UnityReduxMiddleware.Middlewares;
using MessageRequest = Claudia.MessageRequest;

namespace ClaudiaApp.Services
{
    public class StoreService : IStoreService
    {
        public StoreService(ILocalStorageService localService)
        {
            Store = new MiddlewareStore();
            var epicMiddleware = EpicMiddleware.Default<AppState>();

            Store.AddMiddleware(ExceptionMiddleware.Create());

            var initialState = localService.GetValue(Actions.SliceName, new AppState());
            Store.CreateSlice(Actions.SliceName, initialState, builder =>
            {
                builder
                    .Add<float>(Actions.SetTemperature, Reducers.SetTemperatureValueReducer)
                    .Add<List<Message>>(Actions.SetChatMessage, Reducers.SetChatMessage)
                    .Add<string>(Actions.SetInputMessage, Reducers.SetInputMessageReducer)
                    .Add<string>(Actions.SetSystemString, Reducers.SetSystemStringReducer);
            });

            // Store.AddMiddleware(epicMiddleware.Create());
            Store.AddMiddleware(RequestMessageMiddleware());
            // epicMiddleware.Run(RootEpic());
        }

        public MiddlewareStore Store { get; }

        public Epic<AppState> RequestMessageEpic()
        {
            Message currentMessage = null;
            List<Message> messages = null;

            return (action, state) => action
                .OfAction(Actions.RequestMessage)
                .SelectMany((_, _) =>
                {
                    var stateValue = state.CurrentValue;
                    var anthropic = stateValue.Anthropic;
                    messages = stateValue.ChatMessages;
                    messages.Add(new Message { Role = Roles.User, Content = stateValue.inputMessage });
                    var response = anthropic.Messages.CreateStreamAsync(
                        new MessageRequest
                        {
                            Model = Claudia.Models.Claude3Opus,
                            MaxTokens = 1024,
                            Temperature = stateValue.temperatureValue,
                            System = string.IsNullOrEmpty(stateValue.systemString) ? null : stateValue.systemString,
                            Messages = stateValue.ChatMessages.ToArray()
                        });

                    currentMessage = new Message
                    {
                        Role = Roles.Assistant,
                        Content = ""
                    };
                    messages.Add(currentMessage);

                    return response.ToObservable();
                })
                .Where(x => x is ContentBlockDelta)
                .Select(x =>
                {
                    currentMessage.Content[0].Text += ((ContentBlockDelta)x).Delta.Text;
                    return Actions.SetChatMessageAction.Invoke(messages) as Action;
                });
        }

        public MiddlewareDelegate RequestMessageMiddleware()
        {
            return store => next => async (action, token) =>
            {
                if (action.type == Actions.RequestMessage)
                {
                    var state = store.GetState<AppState>(Actions.SliceName);
                    var anthropic = state.Anthropic;
                    var messages = state.ChatMessages;
                    messages.Add(new Message { Role = Roles.User, Content = state.inputMessage });
                    var stream = anthropic.Messages.CreateStreamAsync(
                        new MessageRequest
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
                    messages.Add(currentMessage);

                    await foreach (var messageStreamEvent in stream)
                        if (messageStreamEvent is ContentBlockDelta content)
                        {
                            currentMessage.Content[0].Text += content.Delta.Text;
                            await next(Actions.SetChatMessageAction.Invoke(messages), token);
                        }
                }
                else
                {
                    await next(action, token);
                }
            };
        }
    }
}