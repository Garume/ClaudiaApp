using System;
using ClaudiaApp.ViewModels;
using Unity.AppUI.Editor;
using UnityEditor;
using UnityEngine.UIElements;

namespace ClaudiaApp.Tests.Editor
{
    public class MainPage : StoryBookPage
    {
        private const string MainPageUxmlGuid = "77a40617b8f8e6844a325bad240d9287";
        private const string MainPageUssGuid = "9432707a9a7941d48d41636a0df4e3bc";

        public MainPage()
        {
            m_Stories.Add(new StoryBookStory("Main", () =>
            {
                var element = new VisualElement();
                var tree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(
                    AssetDatabase.GUIDToAssetPath(MainPageUxmlGuid));
                tree.CloneTree(element);
                var root = element.Q<VisualElement>(className: "claudia-root");
                root.styleSheets.Add(
                    AssetDatabase.LoadAssetAtPath<StyleSheet>(AssetDatabase.GUIDToAssetPath(MainPageUssGuid)));

                return root;
            }));

            m_Stories.Add(new StoryBookStory("Main with DataBinding", () =>
            {
                var template = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(
                    AssetDatabase.GUIDToAssetPath(MainPageUxmlGuid));
                var style = AssetDatabase.LoadAssetAtPath<StyleSheet>(AssetDatabase.GUIDToAssetPath(MainPageUssGuid));
                var viewModel = new MockMainViewModel();
                var page = new View.MainPage(template, viewModel);
                var root = page.Q<VisualElement>(className: "claudia-root");
                root.styleSheets.Add(style);

                return root;
            }));
        }

        public override string displayName => "Claudia-MainPage";
        public override Type componentType => null;
    }
}