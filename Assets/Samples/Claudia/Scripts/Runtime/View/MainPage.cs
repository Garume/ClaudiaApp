using System.ComponentModel;
using System.Linq;
using ClaudiaApp.ViewModels;
using Unity.AppUI.UI;
using UnityEngine.UIElements;
using Button = Unity.AppUI.UI.Button;

namespace ClaudiaApp.View
{
    public class MainPage : VisualElement
    {
        private readonly IMainViewModel _model;
        private readonly VisualTreeAsset _template;
        private TextArea _inputMessage;
        private TextArea _outputMessage;
        private Button _sendButton;

        private SliderFloat _slider;

        private TextArea _systemMessage;

        public MainPage(IMainViewModel model)
        {
            _model = model;
            InitializeComponent();
            _model.PropertyChanged += OnPropertyChanged;
        }

        public MainPage(VisualTreeAsset template, IMainViewModel model)
        {
            _template = template;
            _model = model;
            InitializeComponent();
            _model.PropertyChanged += OnPropertyChanged;
        }

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(_model.ChatMessage))
            {
                var texts = _model.ChatMessage.Select(x => x.Content[0].Text);
                _outputMessage.value = string.Join("\n", texts);
            }
        }

        private void InitializeComponent()
        {
            if (_template == null)
            {
                var template = ClaudiaAppBuilder.Instance.MainPageTemplate;
                template.CloneTree(this);
            }
            else
            {
                _template.CloneTree(this);
            }

            _slider = this.Q<SliderFloat>();
            _systemMessage = this.Q<TextArea>(className: "claudia-system-text-area");
            _inputMessage = this.Q<TextArea>(className: "claudia-form-text-area");
            _outputMessage = this.Q<TextArea>(className: "claudia-text-area");
            _sendButton = this.Q<Button>(className: "claudia-form-button");


            _slider.RegisterValueChangingCallback(OnSliderValueChanged);
            _systemMessage.RegisterValueChangingCallback(OnSystemMessageChanged);
            _inputMessage.RegisterValueChangingCallback(OnInputMessageChanged);
            _sendButton.clicked += OnSendButtonClicked;
        }

        private void OnInputMessageChanged(ChangingEvent<string> evt)
        {
            if (!string.Equals(evt.newValue, evt.previousValue)) _model.SetInputMessageCommand.Execute(evt.newValue);
        }

        private void OnSystemMessageChanged(ChangingEvent<string> evt)
        {
            if (!string.Equals(evt.newValue, evt.previousValue)) _model.SetSystemMessageCommand.Execute(evt.newValue);
        }

        private void OnSliderValueChanged(ChangingEvent<float> evt)
        {
            if (!evt.newValue.Equals(evt.previousValue)) _model.SetTemperatureCommand.Execute(evt.newValue);
        }

        private void OnSendButtonClicked()
        {
            _model.SendMessageCommand.Execute();
        }
    }
}