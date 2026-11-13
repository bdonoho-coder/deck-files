// Copyright (c) Files Community. Licensed under the MIT License.

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace Files.App.Controls
{
	/// <summary>
	/// Console Deck pane header (NAV / DIR / INFO). Hidden in every other look through the LookPane resource.
	/// </summary>
	public sealed partial class PaneHeader : UserControl
	{
		public string? Title
		{
			get => (string?)GetValue(TitleProperty);
			set => SetValue(TitleProperty, value);
		}
		public static readonly DependencyProperty TitleProperty =
			DependencyProperty.Register(nameof(Title), typeof(string), typeof(PaneHeader), new PropertyMetadata(null));

		/// <summary>Secondary accent label after the tab, e.g. the open folder's name.</summary>
		public string? Code
		{
			get => (string?)GetValue(CodeProperty);
			set => SetValue(CodeProperty, value);
		}
		public static readonly DependencyProperty CodeProperty =
			DependencyProperty.Register(nameof(Code), typeof(string), typeof(PaneHeader), new PropertyMetadata(null));

		/// <summary>Content at the right end of the strip, e.g. a close button.</summary>
		public object? Trailing
		{
			get => GetValue(TrailingProperty);
			set => SetValue(TrailingProperty, value);
		}
		public static readonly DependencyProperty TrailingProperty =
			DependencyProperty.Register(nameof(Trailing), typeof(object), typeof(PaneHeader), new PropertyMetadata(null));

		public PaneHeader()
		{
			InitializeComponent();
		}
	}
}
