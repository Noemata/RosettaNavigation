using System.Diagnostics;
using System.ComponentModel;
using System.Collections.ObjectModel;

using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;

using SimpleData;

namespace ListView1
{
    // MP! Automated paging ideas by Noemata.

    public sealed partial class Page2 : Page, INotifyPropertyChanged
    {
        private ObservableCollection<SampleItem> _items;
        public ObservableCollection<SampleItem> Items
        {
            get => _items;
            set { _items = value; RaisePropertyChanged(nameof(Items)); }
        }

        public Page2()
        {
            // Note: NavigationCacheMode.Disabled is the default
            //NavigationCacheMode = NavigationCacheMode.Enabled;
            InitializeComponent();
            DataContext = null;
            Loaded += OnLoaded;
            Trace.WriteLine("Page 2 not cached.");
            RegisterRendering();
        }

        int redrawCycle = 0;
        private void OnRendering(object sender, object e)
        {
            redrawCycle++;
            // When NavigationCacheMode.Disabled, we need to give the UI time to render and respond to input.  Is there a better/faster way to do this?
            if (MainWindow.Context.AutoPage && redrawCycle == 4)
            {
                UnRegisterRendering();
                MainWindow.RootFrame.GoBack();
            }

            // Stop rendering if UI is going idle.
            if (!MainWindow.Context.AutoPage && redrawCycle > 4)
                UnRegisterRendering();
        }

        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            UnRegisterRendering();
        }

        ~Page2()
        {
            Trace.WriteLine("Page2 cleared.");
        }

        private async void OnLoaded(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            if (DataContext == null)
            {
                Trace.WriteLine("Loading Page 2 data ...");
                Items = await Do.CreateItems(MainWindow.LineCount);
                DataContext = this;
            }

            if (MainWindow.Context.AutoPage && NavigationCacheMode != NavigationCacheMode.Disabled)
                MainWindow.RootFrame.GoBack();
        }

        private void OnClick(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            // Need to prevent the OnRender page change from becoming additive.  Required when rendering is active.
            if (redrawCycle > 4 || NavigationCacheMode == NavigationCacheMode.Enabled)
                MainWindow.RootFrame.GoBack();
        }

        void RegisterRendering()
        {
            if (NavigationCacheMode == NavigationCacheMode.Disabled)
                Microsoft.UI.Xaml.Media.CompositionTarget.Rendering += OnRendering;
        }

        void UnRegisterRendering()
        {
            if (NavigationCacheMode == NavigationCacheMode.Disabled)
                Microsoft.UI.Xaml.Media.CompositionTarget.Rendering -= OnRendering;
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
