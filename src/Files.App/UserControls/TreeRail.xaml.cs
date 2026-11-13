// Copyright (c) Files Community. Licensed under the MIT License.

using Files.App.Controls;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System.IO;
using Windows.ApplicationModel;

namespace Files.App.UserControls
{
	/// <summary>
	/// Node Tree look: right rail with X1-X4 view keys (layouts + preview pane), fixed-drive meters and a vertical version label.
	/// </summary>
	public sealed partial class TreeRail : UserControl
	{
		private readonly ICommandManager Commands = Ioc.Default.GetRequiredService<ICommandManager>();
		private readonly DispatcherTimer _timer = new() { Interval = TimeSpan.FromSeconds(60) };

		public ObservableCollection<RailMeter> Meters { get; } = [];

		public string Footer { get; }

		// The command's own label ("Toggle the info pane") is too long for a 56 px key.
		public string PreviewLabel { get; } = Look.Text(Strings.Preview.GetLocalizedResource()) ?? string.Empty;

		public TreeRail()
		{
			InitializeComponent();
			var version = Package.Current.Id.Version;
			Footer = $"TREE // V{version.Major}.{version.Minor}";
			Refresh();

			Loaded += (_, _) =>
			{
				_timer.Tick += Timer_Tick;
				_timer.Start();
			};
			Unloaded += (_, _) =>
			{
				_timer.Stop();
				_timer.Tick -= Timer_Tick;
			};
		}

		private void Timer_Tick(object? sender, object e) => Refresh();

		// Up to three fixed drives, used space in percent.
		private void Refresh()
		{
			Meters.Clear();
			try
			{
				foreach (var drive in DriveInfo.GetDrives().Where(d => d.DriveType == System.IO.DriveType.Fixed && d.IsReady).Take(3))
				{
					var used = 100 - (int)Math.Round(100.0 * drive.AvailableFreeSpace / drive.TotalSize);
					Meters.Add(new RailMeter(drive.Name.TrimEnd('\\'), used));
				}
			}
			catch (Exception)
			{
				// A drive that disappears mid-scan just drops off the rail until the next refresh.
			}
		}
	}

	public sealed class RailMeter(string name, int percent)
	{
		public string Label => $"{name} {percent}%";

		/// <summary>Accent bar length inside the 56 px meter.</summary>
		public double BarWidth => 56.0 * percent / 100;
	}
}
