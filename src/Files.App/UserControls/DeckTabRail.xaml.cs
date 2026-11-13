// Copyright (c) Files Community. Licensed under the MIT License.

using Microsoft.UI.Xaml.Controls;
using Windows.ApplicationModel;

namespace Files.App.UserControls
{
	/// <summary>
	/// Console Deck look: the open tabs stacked on a left rail. Selection goes through App.AppModel.TabStripSelectedIndex, the same index the top tab strip binds to.
	/// </summary>
	public sealed partial class DeckTabRail : UserControl
	{
		private readonly ICommandManager Commands = Ioc.Default.GetRequiredService<ICommandManager>();

		public string Footer { get; }

		public DeckTabRail()
		{
			InitializeComponent();
			var version = Package.Current.Id.Version;
			Footer = $"DECK // V{version.Major}.{version.Minor}";
		}

		/// <summary>Three-letter code for a tab header, e.g. "Downloads" -> "DOW".</summary>
		public static string Code(string? header)
		{
			var code = new string((header ?? string.Empty).Where(char.IsLetterOrDigit).Take(3).ToArray()).ToUpperInvariant();
			return code.Length > 0 ? code : "TAB";
		}

		// -1 shows up while the tab list changes; the top strip's own handling keeps the index valid then.
		private void Rail_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (Rail.SelectedIndex >= 0 && Rail.SelectedIndex != App.AppModel.TabStripSelectedIndex)
				App.AppModel.TabStripSelectedIndex = Rail.SelectedIndex;
		}
	}
}
