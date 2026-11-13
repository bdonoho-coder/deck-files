// Copyright (c) Files Community. Licensed under the MIT License.

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Markup;
using Microsoft.UI.Xaml.Media;
using System.Text;

namespace Files.App.UserControls
{
	/// <summary>
	/// Cyber look filler: 2 px dots every 24 px behind the file list. Rebuilt only when the size grows past the last grid.
	/// </summary>
	public sealed partial class CyberDots : UserControl
	{
		private const int Step = 24;
		private int _columns;
		private int _rows;

		public CyberDots()
		{
			InitializeComponent();
		}

		private void CyberDots_SizeChanged(object sender, SizeChangedEventArgs e)
		{
			var columns = (int)(e.NewSize.Width / Step) + 1;
			var rows = (int)(e.NewSize.Height / Step) + 1;
			if (columns <= _columns && rows <= _rows)
				return;

			_columns = columns;
			_rows = rows;
			var sb = new StringBuilder(columns * rows * 16);
			for (var r = 0; r < rows; r++)
				for (var c = 0; c < columns; c++)
					sb.Append('M').Append(c * Step + 11).Append(',').Append(r * Step + 11).Append(" h2 v2 h-2 z ");

			Dots.Data = (Geometry)XamlBindingHelper.ConvertValue(typeof(Geometry), sb.ToString());
		}
	}
}
