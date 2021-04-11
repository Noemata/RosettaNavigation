using System;
using System.Diagnostics;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Collections.ObjectModel;

using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

using SimpleData;

namespace ListView2
{
    // MP! Automated paging ideas by Noemata. Simpler version.

    public sealed partial class Page1 : Page, INotifyPropertyChanged
    {
        private ObservableCollection<SampleItem> _items;
        public ObservableCollection<SampleItem> Items
        {
            get => _items;
            set { _items = value; RaisePropertyChanged(nameof(Items)); }
        }

        public Page1()
        {
            // Note: NavigationCacheMode.Disabled is the default
            //NavigationCacheMode = NavigationCacheMode.Enabled;
            InitializeComponent();
            DataContext = null;
            Loaded += OnLoaded;
            Trace.WriteLine("Page 1 not cached.");
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

                Items = await Do.CreateItems(MainPage.LineCount);
                DataContext = this;
            }

            MainPage.Context.CheckMem();

            if (MainPage.Context.AutoPage)
            {
                await Task.Delay(5);
                MainPage.RootFrame.Navigate(typeof(Page2));
            }
        }

        private void OnClick(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            if (!MainPage.Context.AutoPage)
                MainPage.RootFrame.Navigate(typeof(Page2));
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
