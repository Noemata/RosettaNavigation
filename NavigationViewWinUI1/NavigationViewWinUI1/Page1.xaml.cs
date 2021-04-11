using System;
using System.Diagnostics;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Collections.ObjectModel;

using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;

using SimpleData;

namespace NavigationView1
{
    // MP! Automated paging ideas by Noemata.

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

        private async void OnLoaded(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            if (DataContext == null)
            {
                Trace.WriteLine("Loading Page 1 data ...");

                Items = await Do.CreateItems(MainWindow.LineCount);

                DataContext = this;
            }

            MainWindow.Context.CheckMem();

            if (MainWindow.Context.AutoPage)
            {
                await Task.Delay(5);
                MainWindow.RootFrame.Navigate(typeof(Page2));
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
