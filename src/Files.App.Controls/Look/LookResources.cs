// Copyright (c) Files Community. Licensed under the MIT License.

using Microsoft.UI.Xaml;

namespace Files.App.Controls
{
	/// <summary>
	/// Merged early in App.xaml (before Generic.xaml): loads HudLook.xaml or MinimalLook.xaml,
	/// so every style parsed after it finds the "Look*" keys for the active look.
	/// </summary>
	public sealed class LookResources : ResourceDictionary
	{
		public LookResources()
		{
			Source = new Uri($"ms-appx:///Files.App.Controls/Look/{(Look.IsHud ? "Hud" : "Minimal")}Look.xaml");
		}
	}
}
