# Navigation Frame testing tool.

This tool is intended to ease the process of testing Navigation Frame interactions with other controls in order to evaluate performance and memory utilization.

## Notes

To test Navigation Frame handling for NavigationCacheMode.Enabled scenarios, uncomment the NavigationCacheMode code shown below in Page1 and Page2.

```csharp
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
```

Visual Studio 2019 appears to contribute a memory leak artifact to the tests.  Correct results may be obtained by executing outside of VS2019.

## Findings

(As of April 2021)

ListViewUwp1 performs well and does not leak with either NavigationCacheMode Enabled or Disabled.  
ListViewWinUI1 becomes progressively slower due to a memory leak with NavigationCacheMode Enabled or Disabled.  
DataGridUwp1 performs well and does not leak with NavigationCacheMode set to Enabled, does not leak but is sluggish when Enabled.  
DataGridWinUI1 cannot be completed at this time because DataGrid has not been updated to the current release of WinUI.  
TabViewUwp1 leaks memory badly and is unusable as is.


