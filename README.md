# Navigation Frame testing tool.

This tool is intented to ease the process of testing Navigation Frame interactions with other controls in order to evaluate performance and memory utilization.

## Notes

To test Navigation Frame handling for NavigationCacheMode.Enabled scenarios, uncomment the NavigationCacheMode code shown below in Page1 and Page2.

```
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