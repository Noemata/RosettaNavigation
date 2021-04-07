using System;
using System.Diagnostics;
using System.ComponentModel;
using System.Collections.ObjectModel;

using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

using SimpleData;

namespace TabView1
{
    // MP! Automated paging ideas by Noemata.

    public sealed partial class Page1 : Page, INotifyPropertyChanged
    {
        private ObservableCollection<SampleItem> _items1;
        public ObservableCollection<SampleItem> Items1
        {
            get => _items1;
            set { _items1 = value; RaisePropertyChanged(nameof(Items1)); }
        }

        private ObservableCollection<SampleItem> _items2;
        public ObservableCollection<SampleItem> Items2
        {
            get => _items2;
            set { _items2 = value; RaisePropertyChanged(nameof(Items2)); }
        }

        private ObservableCollection<SampleItem> _items3;
        public ObservableCollection<SampleItem> Items3
        {
            get => _items3;
            set { _items3 = value; RaisePropertyChanged(nameof(Items3)); }
        }

        public Page1()
        {
            // Note: NavigationCacheMode.Disabled is the default
            //NavigationCacheMode = NavigationCacheMode.Enabled;
            InitializeComponent();
            DataContext = null;
            Loaded += OnLoaded;
            Trace.WriteLine("Page 1 not cached.");

            RegisterRendering();
        }

        int redrawCycle = 0;
        private void OnRendering(object sender, object e)
        {
            redrawCycle++;
            // When NavigationCacheMode.Disabled, we need to give the UI time to render and respond to input.  Is there a better/faster way to do this?
            if (MainPage.Context.AutoPage && redrawCycle == 4)
            {
                UnRegisterRendering();
                MainPage.RootFrame.Navigate(typeof(Page2));
            }

            // Stop rendering if UI is going idle.
            if (!MainPage.Context.AutoPage && redrawCycle > 4)
                UnRegisterRendering();
        }

        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            UnRegisterRendering();
        }

        ~Page1()
        {
            Trace.WriteLine("Page1 cleared.");
        }

        private async void OnLoaded(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            if (DataContext == null)
            {
                Trace.WriteLine("Loading Page 1 data ...");

                Items1 = await Do.CreateItems(MainPage.LineCount);
                Items2 = await Do.CreateItems(MainPage.LineCount);
                Items3 = await Do.CreateItems(MainPage.LineCount);
                DataContext = this;
            }

            MainPage.Context.CheckMem();

            if (MainPage.Context.AutoPage && NavigationCacheMode != NavigationCacheMode.Disabled)
                MainPage.RootFrame.Navigate(typeof(Page2));
        }

        private void OnClick(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            // Need to prevent the OnRender page change from becoming additive.  Required when rendering is active.
            if (redrawCycle > 4 || NavigationCacheMode == NavigationCacheMode.Enabled)
                MainPage.RootFrame.Navigate(typeof(Page2));
        }

        void RegisterRendering()
        {
            if (NavigationCacheMode == NavigationCacheMode.Disabled)
                Windows.UI.Xaml.Media.CompositionTarget.Rendering += OnRendering;
        }

        void UnRegisterRendering()
        {
            if (NavigationCacheMode == NavigationCacheMode.Disabled)
                Windows.UI.Xaml.Media.CompositionTarget.Rendering -= OnRendering;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void RaisePropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}


