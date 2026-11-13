// Copyright (c) Files Community. Licensed under the MIT License.

using Files.App.Controls;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Markup;
using Microsoft.UI.Xaml.Media;
using Windows.ApplicationModel;

namespace Files.App.UserControls
{
	/// <summary>
	/// Cyber look: the main places as big drawn icons on a left rail. A click opens the place in the active pane; the open place is lit.
	/// </summary>
	public sealed partial class CyberRail : UserControl
	{
		private readonly IContentPageContext _context = Ioc.Default.GetRequiredService<IContentPageContext>();

		public List<RailPlace> Places { get; } =
		[
			new("HOME", Strings.Home.GetLocalizedResource(), null,
				"M12,2 L21,7 V17 L12,22 L3,17 V7 Z M12,8 V16 M8,10 L12,12 L16,10"),
			new("DESK", Strings.Desktop.GetLocalizedResource(), Constants.UserEnvironmentPaths.DesktopPath,
				"M3,4 H21 V15 H3 Z M9,20 H15 M12,15 V20"),
			new("DOWN", Strings.Downloads.GetLocalizedResource(), Constants.UserEnvironmentPaths.DownloadsPath,
				"M12,3 V14 M7,10 L12,15 L17,10 M4,17 H20 V21 H4 Z"),
			new("DOCS", Strings.Documents.GetLocalizedResource(), Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
				"M5,2 H15 L19,6 V22 H5 Z M15,2 V6 H19 M8,11 H16 M8,15 H16 M8,19 H12"),
			new("PICS", Strings.Pictures.GetLocalizedResource(), Environment.GetFolderPath(Environment.SpecialFolder.MyPictures),
				"M3,4 H21 V20 H3 Z M3,16 L9,10 L14,15 L17,12 L21,16 M16,8 L16.01,8"),
			new("DRV", Strings.ThisPC.GetLocalizedResource(), Constants.UserEnvironmentPaths.MyComputerPath,
				"M3,5 H21 V11 H3 Z M3,13 H21 V19 H3 Z M6,8 L6.01,8 M6,16 L6.01,16 M10,8 H18 M10,16 H18"),
			new("BIN", Strings.RecycleBin.GetLocalizedResource(), Constants.UserEnvironmentPaths.RecycleBinPath,
				"M4,7 H20 M9,7 V4 H15 V7 M6,7 L7,21 H17 L18,7 M10,11 V17 M14,11 V17"),
		];

		public string Footer { get; }

		public CyberRail()
		{
			foreach (var place in Places)
				place.IsOn = Visibility.Collapsed;
			InitializeComponent();
			var version = Package.Current.Id.Version;
			Footer = $"PART 01 // V{version.Major}.{version.Minor}";

			Loaded += (_, _) =>
			{
				_context.PropertyChanged += Context_PropertyChanged;
				UpdateLit();
			};
			Unloaded += (_, _) => _context.PropertyChanged -= Context_PropertyChanged;
		}

		/// <summary>Dark ink on the lit accent key, normal text otherwise. For x:Bind.</summary>
		public static Brush Ink(Visibility lit)
			=> (Brush)Application.Current.Resources[lit == Visibility.Visible ? "LookTabSelectedForeground" : "TextFillColorPrimaryBrush"];

		private void Context_PropertyChanged(object? sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName is nameof(IContentPageContext.Folder) or nameof(IContentPageContext.PageType) or nameof(IContentPageContext.ShellPage))
				UpdateLit();
		}

		private void UpdateLit()
		{
			var isHome = _context.PageType == ContentPageTypes.Home;
			var current = _context.ShellPage?.ShellViewModel?.WorkingDirectory?.TrimEnd('\\');
			foreach (var place in Places)
			{
				var on = place.Path is null
					? isHome
					: !isHome && current is not null && string.Equals(current, place.Path.TrimEnd('\\'), StringComparison.OrdinalIgnoreCase);
				place.IsOn = on ? Visibility.Visible : Visibility.Collapsed;
			}
		}

		private void Place_Click(object sender, RoutedEventArgs e)
		{
			if (sender is not Button { Tag: RailPlace place } || _context.ShellPage is not IShellPage shell)
				return;

			if (place.Path is null)
				shell.NavigateHome();
			else
				shell.NavigateToPath(place.Path);
		}
	}

	public sealed partial class RailPlace(string code, string name, string? path, string icon) : ObservableObject
	{
		public string Code => code;

		public string Name => name;

		/// <summary>Null = the Home page.</summary>
		public string? Path => path;

		public Geometry Icon { get; } = (Geometry)XamlBindingHelper.ConvertValue(typeof(Geometry), icon);

		[ObservableProperty]
		public partial Visibility IsOn { get; set; }
	}
}
