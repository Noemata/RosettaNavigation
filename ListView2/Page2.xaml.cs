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
        }

        ~Page2()
        {
            Trace.WriteLine("Page2 cleared.");
        }

        private async void OnLoaded(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            if (DataContext == null)
            {
                Trace.WriteLine("Loading Page 2 data ...");
                Items = await Do.CreateItems(MainPage.LineCount);
                DataContext = this;
            }

            if (MainPage.Context.AutoPage)
            {
                await Task.Delay(5);
                MainPage.RootFrame.GoBack();
            }
        }

        private void OnClick(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            if (!MainPage.Context.AutoPage)
                MainPage.RootFrame.GoBack();
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
