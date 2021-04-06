using System;
using System.Diagnostics;
using System.ComponentModel;

using Windows.UI.Xaml.Controls;

using SimpleData;

namespace ListView1
{
    // MP! Navigation Frame testing tool by Noemata

    public sealed partial class MainPage : Page, INotifyPropertyChanged
    {
        public static Frame RootFrame { get; private set; }
        public const int LineCount = 8000;
        public const int InMB = 1024 * 1024;

        public static ulong oldMin = 0, memMin = 0, memMax = 0;
        public static CircularBuffer<ulong> memCheck = new CircularBuffer<ulong>(checkCount);
        public const int settleCount = 14;
        public const int checkCount = 15;

        private bool _autoPage = false;
        public bool AutoPage
        {
            get => _autoPage;
            set { _autoPage = value; RaisePropertyChanged(nameof(AutoPage)); }
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

        public static MainPage Context;

        // MP! Automated paging ideas by Noemata.  The ideas here can be used to quickly nail down Navigation related memory usage issues.

        public MainPage()
        {
            this.InitializeComponent();
            RootFrame = rootFrame;
            RootFrame.Navigate(typeof(Page1));
            DataContext = Context = this;
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

                    if (memMin >=oldMin)
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
