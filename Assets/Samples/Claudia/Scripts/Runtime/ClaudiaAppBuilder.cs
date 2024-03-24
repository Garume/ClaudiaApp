using ClaudiaApp.Services;
using ClaudiaApp.View;
using ClaudiaApp.ViewModels;
using Unity.AppUI.MVVM;
using UnityEngine.UIElements;

namespace ClaudiaApp
{
    public class ClaudiaAppBuilder : UIToolkitAppBuilder<ClaudiaApp>
    {
        public VisualTreeAsset MainPageTemplate;

        internal static ClaudiaAppBuilder Instance { get; private set; }

        protected override void OnConfiguringApp(AppBuilder builder)
        {
            base.OnConfiguringApp(builder);
            Instance = this;

            builder.services.AddSingleton<ILocalStorageService, LocalStorageService>();
            builder.services.AddSingleton<IStoreService, StoreService>();

            builder.services.AddTransient<IMainViewModel, MainViewModel>();
            builder.services.AddTransient<MainPage>();
        }
    }
}