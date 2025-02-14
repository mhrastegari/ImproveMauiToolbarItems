using System.Diagnostics.CodeAnalysis;
using Microsoft.Maui.Controls.Compatibility.Platform.iOS;
using Microsoft.Maui.Controls.PlatformConfiguration.iOSSpecific;
using Microsoft.Maui.Handlers;
using UIKit;
using ContentView = Microsoft.Maui.Platform.ContentView;

//https://github.com/securefolderfs-community/SecureFolderFS/blob/master/src/Platforms/SecureFolderFS.Maui/Platforms/iOS/Handlers/ContentPageHandler.cs
namespace ImproveMauiToolbarItems.Handlers
{
    // Callsite only reachable on iOS 13 and above
    [SuppressMessage("Interoperability", "CA1416:Validate platform compatibility")]
    public sealed class ContentPageHandler : PageHandler
    {
        private ContentPage? ThisPage => VirtualView as ContentPage;

        protected override void ConnectHandler(ContentView platformView)
        {
            base.ConnectHandler(platformView);
            if (ThisPage is null) return;

            ThisPage.Loaded += ContentPage_Loaded;
            ThisPage.NavigatedTo += ContentPage_NavigatedTo;
            ThisPage.Appearing += ContentPage_Appearing;
            App.Instance.AppResumed += App_Resumed;
            Microsoft.Maui.Controls.Application.Current!.RequestedThemeChanged += OnThemeChanged;
        }

        protected override void DisconnectHandler(ContentView platformView)
        {
            ThisPage!.Loaded -= ContentPage_Loaded;
            ThisPage!.Appearing -= ContentPage_Appearing;
            App.Instance.AppResumed -= App_Resumed;
            Microsoft.Maui.Controls.Application.Current!.RequestedThemeChanged -= OnThemeChanged;

            base.DisconnectHandler(platformView);
        }

        private void ContentPage_Unloaded(object? sender, EventArgs e)
        {
            ThisPage!.NavigatedTo -= ContentPage_NavigatedTo;
            ThisPage!.Unloaded -= ContentPage_Unloaded;
            ThisPage.ToolbarItems.Clear();
        }

        private async void ContentPage_Loaded(object? sender, EventArgs e)
        {
            ThisPage!.NavigatedTo += ContentPage_NavigatedTo;
            ThisPage!.Unloaded += ContentPage_Unloaded;

            await Task.Delay(10);
            UpdateItems();
        }

        private void ContentPage_Appearing(object? sender, EventArgs e) => UpdateItems();

        private void ContentPage_NavigatedTo(object? sender, NavigatedToEventArgs e) => UpdateItems();

        private void App_Resumed(object? sender, EventArgs e) => UpdateItems();

        private void OnThemeChanged(object? sender, AppThemeChangedEventArgs e) => UpdateItems();

        private void UpdateItems()
        {
            if (ThisPage is null || this is not IPlatformViewHandler pvh) return;

            if (pvh.ViewController?.ParentViewController?.NavigationItem is { } navItem)
                UpdateToolbarItems(ThisPage, navItem);

            if (pvh.ViewController?.NavigationController?.NavigationBar is { } navigationBar)
                UpdateTitleMode(ThisPage, navigationBar);
        }

        private void UpdateTitleMode(ContentPage contentPage, UINavigationBar navigationBar)
        {
            var largeTitleMode = Microsoft.Maui.Controls.PlatformConfiguration.iOSSpecific.Page.GetLargeTitleDisplay(contentPage);
            navigationBar.PrefersLargeTitles = largeTitleMode == LargeTitleDisplayMode.Always;
        }

        private void UpdateToolbarItems(ContentPage contentPage, UINavigationItem navigationItem)
        {
            if (contentPage.ToolbarItems is null || contentPage.ToolbarItems.Count is 0) return;

            var rightBarItems = new List<UIBarButtonItem>();

            var secondaryItems = contentPage.ToolbarItems
                .Where(x => x.Order == ToolbarItemOrder.Secondary)
                .Select(CreateUIMenuElement)
                .ToArray();

            if (secondaryItems is not null || secondaryItems?.Length > 0)
            {
                var menu = UIMenu.Create(string.Empty, null, UIMenuIdentifier.Edit, UIMenuOptions.DisplayInline, secondaryItems);
                var menuButton = new UIBarButtonItem(UIImage.FromBundle("cupertino_ellipsis.png"), menu);
                rightBarItems.Add(menuButton);
            }

            foreach (var item in contentPage.ToolbarItems)
            {
                if (item.Order is not ToolbarItemOrder.Secondary)
                {
                    rightBarItems.Add(item.ToUIBarButtonItem());
                }
            }

            navigationItem.RightBarButtonItems = rightBarItems.ToArray();
        }

        private static UIMenuElement CreateUIMenuElement(ToolbarItem item)
        {
            var imagePath = item.IconImageSource?.ToString();
            var image = string.IsNullOrEmpty(imagePath) ? null : UIImage.FromBundle(imagePath);
            var action = UIAction.Create(item.Text, image, null, _ =>
            {
                item.Command?.Execute(item.CommandParameter);
                ((IMenuItemController)item).Activate();
            });
            return action;
        }
    }
}