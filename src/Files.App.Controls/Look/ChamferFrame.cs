// Copyright (c) Files Community. Licensed under the MIT License.

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Markup;
using Microsoft.UI.Xaml.Media;
using System.Globalization;

namespace Files.App.Controls
{
	/// <summary>
	/// Cyber look pane frame: an open HUD-kit outline (corner cuts, a stepped top edge, gaps ending in nodes, glow). Hidden in every other look through the LookChamfer resource.
	/// </summary>
	public sealed partial class ChamferFrame : UserControl
	{
		private const double Cut = 14;   // corner cut size
		private const double Step = 6;   // height of the 45 degree jog on the top edge
		private const double Node = 5;   // side of the square nodes

		public ChamferFrame()
		{
			InitializeComponent();
		}

		private void ChamferFrame_SizeChanged(object sender, SizeChangedEventArgs e)
		{
			var w = e.NewSize.Width;
			var h = e.NewSize.Height;
			if (w < 4 * Cut || h < 4 * Cut)
				return;

			var outline = Outline(w, h);
			Line.Data = Geometry(outline);
			Glow.Data = Geometry(outline);
			Nodes.Data = Geometry(NodeSquares(w, h));
		}

		// Three open strokes: top edge with a step into the upper right edge; lower right edge, bottom and lower left edge; upper left edge into the top-left cut.
		private static string Outline(double w, double h)
		{
			var stepX = w * 0.58;
			return string.Join(' ',
				$"M{F(Cut)},0.5 H{F(stepX)} L{F(stepX + Step)},{F(Step + 0.5)} H{F(w - Cut)} L{F(w - 0.5)},{F(Cut)} V{F(h * 0.40)}",
				$"M{F(w - 0.5)},{F(h * 0.58)} V{F(h - Cut)} L{F(w - Cut)},{F(h - 0.5)} H{F(Cut)} L0.5,{F(h - Cut)} V{F(h * 0.56)}",
				$"M0.5,{F(h * 0.38)} V{F(Cut)} L{F(Cut)},0.5");
		}

		// Square nodes where the strokes stop, plus a notch block on the top edge before the step.
		private static string NodeSquares(double w, double h)
		{
			var half = Node / 2;
			return string.Join(' ',
				Square(w - 0.5, h * 0.40, half),
				Square(w - 0.5, h * 0.58, half),
				Square(0.5, h * 0.38, half),
				Square(0.5, h * 0.56, half),
				$"M{F(w * 0.58 - 40)},0 H{F(w * 0.58 - 12)} V3 H{F(w * 0.58 - 40)} Z");
		}

		private static string Square(double cx, double cy, double half)
			=> $"M{F(cx - half)},{F(cy - half)} H{F(cx + half)} V{F(cy + half)} H{F(cx - half)} Z";

		private static string F(double v)
			=> v.ToString("0.##", CultureInfo.InvariantCulture);

		private static Geometry Geometry(string data)
			=> (Geometry)XamlBindingHelper.ConvertValue(typeof(Geometry), data);
	}
}
