// Copyright (c) Files Community. Licensed under the MIT License.

using Microsoft.UI.Xaml;

namespace Files.App.Controls
{
	/// <summary>
	/// Merged early in App.xaml (before Generic.xaml): merges the look's resource files (see <see cref="Look.Files"/>),
	/// so every style parsed after it finds the "Look*" keys for the active look.
	/// </summary>
	public sealed class LookResources : ResourceDictionary
	{
		public LookResources()
		{
			foreach (var file in Look.Files)
				MergedDictionaries.Add(new ResourceDictionary { Source = new Uri($"ms-appx:///Files.App.Controls/Look/{file}.xaml") });
		}
	}
}
