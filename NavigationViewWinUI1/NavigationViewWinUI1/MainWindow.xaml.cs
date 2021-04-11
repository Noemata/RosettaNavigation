using System;
using System.Linq;
using System.Diagnostics;
using System.ComponentModel;
using System.Collections.Generic;

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;

using SimpleData;

namespace NavigationView1
{
    // MP! Navigation Frame testing tool by Noemata

    public sealed partial class MainWindow : Window, INotifyPropertyChanged
    {
        public static Frame RootFrame { get; private set; }
        public const int LineCount = 8000;
        public const int InMB = 1024 * 1024;

        public static ulong oldMin = 0, memMin = 0, memMax = 0;
        public static CircularBuffer<ulong> memCheck = new CircularBuffer<ulong>(checkCount);
        public const int settleCount = 14;
        public const int checkCount = 15;

        bool pageToggle = false;

        private bool _autoPage = false;
        public bool AutoPage
        {
            get => _autoPage;
            set
            {
                _autoPage = value;
                RaisePropertyChanged(nameof(AutoPage));
                if (_autoPage)
                {
                    if (pageToggle)
                    {
                        if (TryGoBack())
                            pageToggle = !pageToggle;
                        else
                        {
                            pageToggle = false;
                            RootFrame.Navigate(typeof(Page2));
                        }
                    }
                    else
                    {
                        RootFrame.Navigate(typeof(Page2));
                        pageToggle = !pageToggle;
                    }

                }
            }
        }

        private string _minimumUsage;
        public string MinimumUsage
        {
            get => _minimumUsage;
            set { _minimumUsage = value; RaisePropertyChanged(nameof(MinimumUsage)); }
        }

        private string _memoryUsage;
        public string MemoryUsage
        {
            get => _memoryUsage;
            set { _memoryUsage = value; RaisePropertyChanged(nameof(MemoryUsage)); }
        }

        private readonly List<(string Nav, Type Page)> _pages = new List<(string Nav, Type Page)>
        {
            ("1", typeof(Page1)),
            ("2", typeof(Page2)),
        };

        public static MainWindow Context;

        // MP! Automated paging ideas by Noemata.  The ideas here can be used to quickly nail down Navigation related memory usage issues.

        public MainWindow()
        {
            this.InitializeComponent();
            RootFrame = rootFrame;
            Context = this;
            RootFrame.Navigated += OnNavigated;
            RootFrame.Navigate(typeof(Page1));
        }

        private void OnNavigated(object sender, NavigationEventArgs e)
        {
            NavView.IsBackEnabled = RootFrame.CanGoBack;

            var item = _pages.FirstOrDefault(p => p.Page == e.SourcePageType);
            NavView.SelectedItem = NavView.MenuItems[_pages.IndexOf((item.Nav, e.SourcePageType))];
            Trace.WriteLine($"Navigation stack depth: {RootFrame.BackStackDepth}");
        }

        private void OnItemInvoked(NavigationView sender, NavigationViewItemInvokedEventArgs args)
        {
            if (args.IsSettingsInvoked == true)
                DoNavigate("settings", args.RecommendedNavigationTransitionInfo);
            else
            {
                string nav = args.InvokedItem as string;
                DoNavigate(nav, args.RecommendedNavigationTransitionInfo);
            }
        }

        private void DoNavigate(string nav, Microsoft.UI.Xaml.Media.Animation.NavigationTransitionInfo transitionInfo)
        {
            Type _page = null;
            if (nav == "settings")
            {
                ; // Do nothing.
            }
            else
            {
                var item = _pages.FirstOrDefault(p => p.Nav.Equals(nav));
                _page = item.Page;
            }

            // Get the page type before navigation so you can prevent duplicate
            // entries in the backstack.
            var preNavPageType = RootFrame.CurrentSourcePageType;

            // Only navigate if the selected page isn't currently loaded.
            if (!(_page is null) && !Type.Equals(preNavPageType, _page))
            {
                if (!AutoPage)
                {
                    if (_page.Name.Equals("Page2"))
                        pageToggle = true;
                    else
                        pageToggle = false;
                }
                RootFrame.Navigate(_page, null, transitionInfo);
            }
        }

        private void OnBackRequested(NavigationView sender, NavigationViewBackRequestedEventArgs args)
        {
            TryGoBack();
        }

        private bool TryGoBack()
        {
            if (!RootFrame.CanGoBack)
                return false;

            // Don't go back if the nav pane is overlayed.
            if (NavView.IsPaneOpen &&
                (NavView.DisplayMode == NavigationViewDisplayMode.Compact ||
                 NavView.DisplayMode == NavigationViewDisplayMode.Minimal))
                return false;

            Trace.WriteLine($"Navigation stack depth: {RootFrame.BackStackDepth}");

            RootFrame.GoBack();

            return true;
        }

        static int instance = 0;
        public void CheckMem()
        {
            long managedMem = GC.GetTotalMemory(true);
            ulong totalMem = Windows.System.MemoryManager.AppMemoryUsage;
            MemoryUsage = $"Managed Memory: {managedMem / InMB}MB | Total Memory: {totalMem / InMB}MB";
            Trace.WriteLine(MemoryUsage);
            GC.Collect();

            memCheck.PushBack(totalMem);

            // Let the GC settle before accumulating results.
            if (instance == 0 && memCheck.Size == settleCount)
            {
                for (int i = 0; i < settleCount; i++)
                    memCheck.PopBack();

                instance++;
            }

            // Instance boundary happens when accumulation buffer capacity is reached.
            if (instance > 0 && memCheck.Size == memCheck.Capacity)
            {
                ulong min = 0, max = 0;

                for (int i = 0; i < memCheck.Capacity; i++)
                {
                    var val = memCheck.Back();
                    memCheck.PopBack();
                    if (min == 0)
                        min = max = val;
                    else
                    {
                        if (val <= min) min = val;
                        if (val >= max) max = val;
                    }
                }

                if (memMin == 0)
                {
                    memMin = min;
                    oldMin = memMin;
                    memMax = max;
                    MinimumUsage = $"Pass [{instance}] - Min / Max Usage: {oldMin / InMB}MB / {max / InMB}MB";
                }
                else
                {
                    memMin = min;

                    if (memMin >= oldMin)
                        MinimumUsage = $"Pass [{instance}] - Miniumum Exceeded: old {oldMin / InMB}MB / same or higher {memMin / InMB}MB";
                    else
                    {
                        MinimumUsage = $"Pass  [{instance}] - Miniumum Dropped: old {oldMin / InMB}MB / lower {memMin / InMB}MB";
                        oldMin = memMin;
                    }
                }

                instance++;
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
