using System.Diagnostics;
using System.ComponentModel;
using System.Collections.ObjectModel;

using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;

using SimpleData;

namespace NavigationView1
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

            if (MainWindow.Context.AutoPage)
            {
                MainWindow.RootFrame.GoBack();
            }
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
