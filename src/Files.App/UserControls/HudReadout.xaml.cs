// Copyright (c) Files Community. Licensed under the MIT License.

using Files.App.Controls;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System.IO;

namespace Files.App.UserControls
{
	/// <summary>
	/// Title bar readout: theme id, system drive use and a clock.
	/// The retro look (Acid Signal) adds a short glitch flicker on the theme id every few seconds.
	/// </summary>
	public sealed partial class HudReadout : UserControl
	{
		private const string GlitchChars = "#%&/<>|=+*_";
		private readonly DispatcherTimer _timer = new() { Interval = TimeSpan.FromSeconds(1) };
		private readonly Random _random = new();
		private readonly string _themeLabel = Look.ThemeName.Length > 0 ? $"SYS // {Look.ThemeName.Replace("hud-", string.Empty).ToUpperInvariant()}" : "SYS // FILES";
		private int _ticks;

		public string ThemeLabel { get; private set; }

		public string DriveLabel { get; private set; } = string.Empty;

		public string Clock { get; private set; } = string.Empty;

		public HudReadout()
		{
			ThemeLabel = _themeLabel;
			InitializeComponent();
			Refresh(true);

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

		private void Timer_Tick(object? sender, object e)
		{
			Refresh((_ticks++ % 60) == 0);
			if (Look.IsRetro && _ticks % 7 == 3)
				_ = GlitchAsync();
		}

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

		// Three quick frames: a few letters swapped for symbols and the label nudged sideways, then back to normal.
		private async Task GlitchAsync()
		{
			for (var frame = 0; frame < 3; frame++)
			{
				var chars = _themeLabel.ToCharArray();
				for (var i = 0; i < chars.Length; i++)
					if (chars[i] != ' ' && _random.Next(4) == 0)
						chars[i] = GlitchChars[_random.Next(GlitchChars.Length)];
				ThemeLabel = new string(chars);
				GlitchShift.X = _random.Next(-3, 4);
				Bindings.Update();
				await Task.Delay(55);
			}

			ThemeLabel = _themeLabel;
			GlitchShift.X = 0;
			Bindings.Update();
		}
	}
}
