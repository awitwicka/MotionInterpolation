using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;


using Microsoft.Xna.Framework;
using Windows.UI.Core;
using Windows.UI.Xaml.Controls;
using Windows.UI.ViewManagement;
using Windows.ApplicationModel.Core;
// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace MotionInterpolation
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class GamePage : Page
    {
        readonly Game1 _game1;

        public GamePage()
        {
            this.InitializeComponent();
            // Create the game.
            var launchArguments = string.Empty;
            _game1 = MonoGame.Framework.XamlGame<Game1>.Create(launchArguments, Window.Current.CoreWindow, swapChainPanel1);
        }


        //private async void NewWindow()
        //{
        //    CoreApplicationView newCoreView = CoreApplication.CreateNewView();

        //    ApplicationView newAppView = null;
        //    int mainViewId = ApplicationView.GetApplicationViewIdForWindow(CoreApplication.MainView.CoreWindow);

        //    await newCoreView.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,() =>
        //      {
        //          newAppView = ApplicationView.GetForCurrentView();
        //          //Window.Current.Content = new SubWindowUserControl();
        //          Window.Current.Activate();
        //      });

        //    await ApplicationViewSwitcher.TryShowAsStandaloneAsync(
        //      newAppView.Id,
        //      ViewSizePreference.UseHalf,
        //      mainViewId,
        //      ViewSizePreference.UseHalf);
        //}

        private void StartPauseAnimationButton_Checked(object sender, RoutedEventArgs e)
        {
            var button = (AppBarToggleButton)sender;
            button.Content = new SymbolIcon(Symbol.Pause);
            _game1.IsAnimated = true;
        }

        private void StartPauseAnimationButton_Unchecked(object sender, RoutedEventArgs e)
        {
            var button = (AppBarToggleButton)sender;
            button.Content = new SymbolIcon(Symbol.Play);
            _game1.IsAnimated = false;
        }
    }
}
