// Copyright (c) Files Community. Licensed under the MIT License.

using Files.App.Controls;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System.IO;

namespace Files.App.UserControls
{
	/// <summary>
	/// Title bar readout: theme id, system drive use and a clock, plus the scan line animation in the HUD look.
	/// </summary>
	public sealed partial class HudReadout : UserControl
	{
		private readonly DispatcherTimer _timer = new() { Interval = TimeSpan.FromSeconds(1) };
		private int _ticks;

		public string ThemeLabel { get; } = Look.ThemeName.Length > 0 ? $"SYS // {Look.ThemeName.Replace("hud-", string.Empty).ToUpperInvariant()}" : "SYS // FILES";

		public string DriveLabel { get; private set; } = string.Empty;

		public string Clock { get; private set; } = string.Empty;

		public HudReadout()
		{
			InitializeComponent();
			Refresh(true);

			Loaded += (_, _) =>
			{
				_timer.Tick += Timer_Tick;
				_timer.Start();
				if (Look.IsHud)
					ScanStoryboard.Begin();
			};
			Unloaded += (_, _) =>
			{
				_timer.Stop();
				_timer.Tick -= Timer_Tick;
				ScanStoryboard.Stop();
			};
		}

		private void Timer_Tick(object? sender, object e)
			=> Refresh((_ticks++ % 60) == 0);

		private void Refresh(bool includeDrive)
		{
			// Seconds only in the HUD look; the minimal look keeps a quiet clock.
			Clock = DateTime.Now.ToString(Look.IsHud ? "HH:mm:ss" : "HH:mm");

			if (includeDrive)
			{
				try
				{
					var drive = new DriveInfo(Path.GetPathRoot(Environment.SystemDirectory) ?? "C:\\");
					var used = 100 - (int)Math.Round(100.0 * drive.AvailableFreeSpace / drive.TotalSize);
					DriveLabel = $"{drive.Name.TrimEnd('\\')} {used}%";
				}
				catch (Exception)
				{
					DriveLabel = string.Empty;
				}
			}

			Bindings.Update();
		}
	}
}
