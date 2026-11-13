// Copyright (c) Files Community. Licensed under the MIT License.

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Markup;
using Microsoft.UI.Xaml.Media;
using System.Globalization;

namespace Files.App.Controls
{
	/// <summary>
	/// Cyber look selected tab plate (cut corner, stepped top edge, long right cut, glow). Shown only through the LookCyberTab resource.
	/// </summary>
	public sealed partial class CyberTab : UserControl
	{
		public CyberTab()
		{
			InitializeComponent();
		}

		private void CyberTab_SizeChanged(object sender, SizeChangedEventArgs e)
		{
			var w = e.NewSize.Width;
			var h = e.NewSize.Height;
			if (w < 60 || h < 16)
				return;

			var stepX = w * 0.62;
			var outline = $"M0.5,{F(h)} V8 L8,0.5 H{F(stepX)} L{F(stepX + 6)},6.5 H{F(w - 24)} L{F(w - 0.5)},{F(h)} Z";
			Plate.Data = Geometry(outline);
			Line.Data = Geometry(outline);
			Glow.Data = Geometry(outline);
			Notch.Data = Geometry($"M14,0 H44 V3 H14 Z M{F(stepX + 10)},6.5 H{F(stepX + 30)} V9 H{F(stepX + 10)} Z");
		}

		private static string F(double v)
			=> v.ToString("0.##", CultureInfo.InvariantCulture);

		private static Geometry Geometry(string data)
			=> (Geometry)XamlBindingHelper.ConvertValue(typeof(Geometry), data);
	}
}
