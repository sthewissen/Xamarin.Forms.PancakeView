<img src="https://github.com/sthewissen/Xamarin.Forms.PancakeView/blob/master/images/pancake.png" width="150px" />

# Xamarin.Forms.PancakeView
An extended ContentView for Xamarin.Forms with rounded corners, borders, shadows and more!

[![Build status](https://sthewissen.visualstudio.com/PancakeView/_apis/build/status/PancakeView-Deployment-CI)](https://sthewissen.visualstudio.com/PancakeView/_build/latest?definitionId=26) ![](https://img.shields.io/nuget/vpre/Xamarin.Forms.PancakeView.svg)

## Why PancakeView?

In a lot of Xamarin.Forms UI work I do I have the need for a layout/view that I can use that supports gradients, borders, rounded corners and shadows. Since none of the existing Xamarin.Forms controls fit the bill I decided to roll my own. And why is it called a ```PancakeView``` you ask? Well, pancakes are also round, have shadows and have a glorious gradient color. What better fit can you think of?

## How to use it?

The project is up on NuGet at the following URL:

https://www.nuget.org/packages/Xamarin.Forms.PancakeView

You could also simply clone the repository and include the projects in the ```src``` folder in your Xamarin.Forms and Platform projects. It uses multi-targeting to resolve to the correct platform.

The first thing we need to do is tell our XAML page where it can find the PancakeView, which is done by adding the following attribute to our ContentPage:

```
<ContentPage xmlns="http://xamarin.com/schemas/2014/forms" xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"  xmlns:yummy="clr-namespace:Xamarin.Forms.PancakeView;assembly=Xamarin.Forms.PancakeView">
   ...
</ContentPage>
```

Next up, just smack a PancakeView onto that page and you're all set, simple as baking real pancakes!

```
<yummy:PancakeView BackgroundColor="#bc91d7" CornerRadius="60,0,0,60" IsClippedToBounds="true" HorizontalOptions="FillAndExpand" HeightRequest="150">
   <Image Source="unicorn.png" HorizontalOptions="FillAndExpand" VerticalOptions="FillAndExpand" Aspect="AspectFill" />
</yummy:PancakeView>
```

### What can I do with it?

| Property | What it does | Extra info |
| ------ | ------ | ------ |
| `BackgroundGradientAngle` | A value between 0-360 to define the angle of the background gradient. | |
| `BackgroundGradientStartColor` | The start color of the background gradient. | A ```Color``` object. |
| `BackgroundGradientEndColor` | The end color of the background gradient. | A ```Color``` object. |
| `BorderColor` | The color of the border. | A ```Color``` object. |
| `BorderIsDashed` | Whether or not the border needs to be dashed. | The length of the dash and spacing between them is currently not editable. |
| `BorderThickness` | The thickness of the border. | |
| `CornerRadius` | A `CornerRadius` object representing each individual corner's radius. | Uses the `CornerRadius` struct allowing you to specify individual corners. This does have some drawbacks on **Android**. |
| `HasShadow` | Whether or not to draw a shadow beneath the control. | For this to work on **Android** we need to clip the view. This means that individual corner radii will be lost. In this case the `TopLeft` value will be used for all corners. |
| `IsClippedToBounds` | Whether or not to clip to the view. | This is only implemented for Android since clipping has consequences for the rounded corners. Clipping using individual corner radii doesn't work on Android. In this case the TopLeft value will be used for all corners. |

## Example

<img src="https://github.com/sthewissen/Xamarin.Forms.PancakeView/blob/master/images/pancake.gif" width="400px" />
