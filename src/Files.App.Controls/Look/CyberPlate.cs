// Copyright (c) Files Community. Licensed under the MIT License.

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Markup;
using Microsoft.UI.Xaml.Media;
using System.Globalization;

namespace Files.App.Controls
{
	/// <summary>
	/// Cyber look selection plate (cut corners, glow) behind the selected side panel row. Shown only through the LookCyberSelect resource.
	/// </summary>
	public sealed partial class CyberPlate : UserControl
	{
		public CyberPlate()
		{
			InitializeComponent();
		}

		private void CyberPlate_SizeChanged(object sender, SizeChangedEventArgs e)
		{
			var w = e.NewSize.Width;
			var h = e.NewSize.Height;
			if (w < 24 || h < 12)
				return;

			const double c = 6;
			var outline = $"M{F(c)},0.5 H{F(w - 0.5)} V{F(h - c)} L{F(w - c)},{F(h - 0.5)} H0.5 V{F(c)} Z";
			Plate.Data = Geometry(outline);
			Line.Data = Geometry(outline);
			Glow.Data = Geometry(outline);
			Notch.Data = Geometry($"M{F(w - 34)},0 H{F(w - 10)} V2.5 H{F(w - 34)} Z");
		}

		private static string F(double v)
			=> v.ToString("0.##", CultureInfo.InvariantCulture);

		private static Geometry Geometry(string data)
			=> (Geometry)XamlBindingHelper.ConvertValue(typeof(Geometry), data);
	}
}
