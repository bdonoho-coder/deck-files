// Copyright (c) Files Community. Licensed under the MIT License.

using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Shapes;

namespace Files.App.UserControls
{
	/// <summary>
	/// Retro look (Acid Signal): CRT scan lines over the window. Not hit-testable, so everything under it still works.
	/// </summary>
	public sealed partial class CrtOverlay : UserControl
	{
		private const int Step = 4;
		private static readonly SolidColorBrush LineBrush = new(Colors.Black);

		public CrtOverlay()
		{
			InitializeComponent();
		}

		// Reuses the rectangles it already has; only adds or removes lines when the height changes.
		private void CrtOverlay_SizeChanged(object sender, SizeChangedEventArgs e)
		{
			var width = e.NewSize.Width;
			var wanted = (int)(e.NewSize.Height / Step) + 1;
			var children = Lines.Children;

			while (children.Count > wanted)
				children.RemoveAt(children.Count - 1);

			for (var i = 0; i < wanted; i++)
			{
				if (i < children.Count)
				{
					((Rectangle)children[i]).Width = width;
					continue;
				}

				var line = new Rectangle { Width = width, Height = 1, Fill = LineBrush };
				Canvas.SetTop(line, i * Step);
				children.Add(line);
			}
		}
	}
}
