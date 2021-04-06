# Navigation Frame testing tool.

This tool is intented to ease the process of testing Navigation Frame interactions with other controls both for performance and memory utilization.

Description: 

!! The original run of this benchmark was flawed !!  Both UWP and WinUI take a performance hit when attached to Visual Studio 2019.

This project compares the performance of three XAML frameworks: WPF, UWP and WinUI (v 0.5 as of March 30, 2021)

Test cases were adjusted somewhat because Maui is not yet equivalent in all drawing operations across the different XAML frameworks.  The work done on the Windows Community Toolkit demonstrates that it is possible to do equivalent drawing operations in UWP and WinUI, but those approaches are not yet reflected in the code of Maui.

This is not an exaustive performance test, but it does touch on the core capabilities of the XAML frameworks being tested.

## License same as original

https://github.com/dotnet/Microsoft.Maui.Graphics/blob/main/LICENSE

## Credits and Ideas

https://github.com/dotnet/Microsoft.Maui.Graphics

## Caveats

A detailed analysis of whether this test is an exact 1:1 performance comparison has not been done.  It is assumed that Microsoft has done a good job of architecting the framework specific rendering code.

If you examine the code, you will see an almost equivalent test implementation for WPF, UWP and WinUI.  UWP and WinUI are in fact 1:1 in every respect.

## Notes

Preliminary results show that UWP has the fastest XAML rendering layer.  WPF has a very respectable showing.

WinUI is sluggish in comparison to WPF and UWP.  Presumably, WinUI still needs a lot of under the hood tuning.

Microsoft has been boasting about how comparable WinUI is to UWP.  WinUI needs to narrow the performance and feature gap when compared to UWP to gain wider adoption from existing UWP deveopers.

WPF developers looking for a fresh look and the prospect of future feature gains may want to start looking at WinUI despite its performance deficit.
