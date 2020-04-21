<img src="https://github.com/sthewissen/Xamarin.Forms.PancakeView/blob/master/images/icon.png" width="150px" />

# Xamarin.Forms.PancakeView
An extended ContentView for Xamarin.Forms with rounded corners, borders, shadows and more!

[![Build Status](https://sthewissen.visualstudio.com/PancakeView/_apis/build/status/CI%20PancakeView%20YAML?branchName=master)](https://sthewissen.visualstudio.com/PancakeView/_build/latest?definitionId=40&branchName=master) [![](https://img.shields.io/nuget/vpre/Xamarin.Forms.PancakeView.svg)](https://nuget.org/packages/Xamarin.Forms.PancakeView)

## Why PancakeView?

In a lot of Xamarin.Forms UI work I do I have the need for a layout/view that I can use that supports gradients, borders, rounded corners and shadows. Since none of the existing Xamarin.Forms controls fit the bill I decided to roll my own. And why is it called a ```PancakeView``` you ask? Well, pancakes are also round, have shadows and have a glorious gradient color. What better fit can you think of?

<img src="https://github.com/sthewissen/Xamarin.Forms.PancakeView/blob/master/images/pancake.gif" width="400px" />

## How to use it?

We have [a great wiki article](https://github.com/sthewissen/Xamarin.Forms.PancakeView/wiki/Setup) explaining exactly that!


## Platform support
At the time of this writing PancakeView has full support for iOS and Android, but only partial support for UWP and WPF. The matrix below shows which features are and aren't supported on UWP and WPF. Feel like implementing one of them? I'm taking PRs! ☺️

| Property | iOS | Android | UWP | WPF |
| ------ | ------ | ------ | ------ | ------ |
| `BackgroundGradientAngle` | ✅ | ✅ | ✅ | ✅ |
| `BackgroundGradientStartColor` | ✅ | ✅ | ✅ | ✅ |
| `BackgroundGradientEndColor` | ✅ | ✅ | ✅ | ✅ |
| `BackgroundGradientStops` | ✅ | ✅ | ✅ | ✅ |
| `BorderColor` | ✅ | ✅ | ✅ | ❌ |
| `BorderGradientAngle` | ✅ | ✅ | ✅ | ❌ |
| `BorderGradientStartColor` | ✅ | ✅ | ✅ | ❌ |
| `BorderGradientEndColor` | ✅ | ✅ | ✅ | ❌ |
| `BorderGradientStops` | ✅ | ✅ | ✅ | ❌ |
| `BorderIsDashed` | ✅ | ✅ | ❌ | ❌ |
| `BorderDashPattern` | ✅ | ✅ | ❌ | ❌ |
| `BorderThickness` | ✅ | ✅ | ✅ | ❌ |
| `CornerRadius` | ✅ | ✅ | ✅ | ❌ |
| `HasShadow` | ✅ | ✅ | ⚠️<sup>*</sup> | ❌ |
| `Elevation` | ✅ | ✅ | ⚠️<sup>*</sup> | ❌ |
| `OffsetAngle` | ✅ | ✅ | ❌ | ❌ |
| `Sides` | ✅ | ✅ | ❌| ❌ |

<sup>*</sup> Limited shadow support available.

## Property reference

| Property | What it does | Extra info |
| ------ | ------ | ------ |
| `BackgroundGradientAngle` | A value between 0-360 to define the angle of the background gradient. | |
| `BackgroundGradientStartColor` | The start color of the background gradient. | A ```Color``` object. |
| `BackgroundGradientEndColor` | The end color of the background gradient. | A ```Color``` object. |
| `BackgroundGradientStops` | A list of `GradientStop` objects that define a multi color gradient. | `Offset` is a value between 0-1 defining the location within the gradient. |
| ~~`BorderDrawingStyle`~~ | ~~Whether to draw the border on the inside or outside of the control.~~ | Has been removed for now due to the addition of new shapes. |
| `BorderColor` | The color of the border. | A ```Color``` object. |
| `BorderGradientAngle` | A value between 0-360 to define the angle of the border gradient. | |
| `BorderGradientStartColor` | The start color of the border gradient. | A ```Color``` object. |
| `BorderGradientEndColor` | The end color of the border gradient. | A ```Color``` object. |
| `BorderGradientStops` | A list of `GradientStop` objects that define a multi color gradient. | `Offset` is a value between 0-1 defining the location within the gradient. |
| `BorderIsDashed` | Whether or not the border needs to be dashed. | The length of the dash and spacing between them is currently not editable. |
| `BorderThickness` | The thickness of the border. | |
| `CornerRadius` | A `CornerRadius` object representing each individual corner's radius. | Uses the `CornerRadius` struct allowing you to specify individual corners. This does have some drawbacks. |
| `HasShadow` | Whether or not to draw a shadow beneath the control. | For this to work we need to clip the view. This means that individual corner radii will be lost. In this case the `TopLeft` value will be used for all corners. |
| `Elevation` | The Material Design elevation desired. | For this to work we need to clip the view. This means that individual corner radii will be lost. In this case the `TopLeft` value will be used for all corners. |
| `OffsetAngle` | The rotation of the `PancakeView` when `Sides` is not its default value of 4. |  |
| `Sides` | The amount of sides to the shape. | Changes the `PancakeView` from being 4-sided to what you provide here. |
