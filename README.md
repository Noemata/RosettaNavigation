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

Visual Studio 2019 appears to contribute a memory leak artifact to the tests.  Correct results may be obtained by executing outside of VS2019.  VS2019 also contributes a negative performance artifact.

## Findings

(As of April 2021)

**UWP**

> **CanvasUwp1** *slightly sluggish depending on controls used, does not leak with either NavigationCacheMode Enabled or Disabled.  
> **ListBoxUwp1** performs well and does not leak with either NavigationCacheMode Enabled or Disabled.  
> **ListViewUwp1** performs well and does not leak with either NavigationCacheMode Enabled or Disabled.  
> **GridViewUwp1** performs well and does not leak with either NavigationCacheMode Enabled or Disabled.  
> **DataGridUwp1** performs well and does not leak with NavigationCacheMode set to Enabled, does not leak but is sluggish when Disabled.  
> **TabViewUwp1** leaks memory badly and is unusable as is.  
> **WebViewUwp1** performs well, has a small leak when NavigationCacheMode set to Disabled.  
> **NavigationViewUwp1** sluggish when throttled, does not leak with either NavigationCacheMode Enabled or Disabled.  
> **TreeViewUwp1** performs well and does not leak with either NavigationCacheMode Enabled or Disabled.  

*TextBox is causing slower performance due to redraw issue.

**WinUI**

> **CanvasWinUI** sluggish depending on controls used, becomes progressively slower due to a memory leak with NavigationCacheMode Enabled or Disabled.  
> **ListBoxWinUI1** becomes progressively slower due to a memory leak with NavigationCacheMode Enabled or Disabled.  
> **ListViewWinUI1** becomes progressively slower due to a memory leak with NavigationCacheMode Enabled or Disabled.  
> **GridViewWinUI1** becomes progressively slower due to a memory leak with NavigationCacheMode Enabled or Disabled.  
> **DataGridWinUI1** cannot be completed at this time because DataGrid has not been updated to the current release of WinUI.  
> **TabViewWinUI1** leaks memory badly and is even worse than Uwp variant.  
> **WebViewWinUI1** performs very well, does not leak when NavigationCacheMode set to Enabled, small leak when Disabled.  !!Warning !! flicker may induce seizure for those with photosensitive epilepsy.  
> **NavigationViewWinUI1** Crashes on same code as UWP version.  
> **TreeViewWinUI1** *Crashes.  

*The WinUI crash happens inside of JsonHelper.cs in the static method call shown below:

```csharp
        private static TreeViewNode GetChild(KeyValuePair<string, JToken> pair)
        {
            if (pair.Value == null)
                return null;

            // MP! WinUI crashes here??
            TreeViewNode child = new TreeViewNode()
            {
                Content = pair, IsExpanded = true
            };

            return child;
        }
```
