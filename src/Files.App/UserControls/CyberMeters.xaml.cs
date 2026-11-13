// Copyright (c) Files Community. Licensed under the MIT License.

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System.IO;

namespace Files.App.UserControls
{
	/// <summary>
	/// Cyber look filler under the side panel: fixed drives as dot-matrix meters, refreshed once a minute.
	/// </summary>
	public sealed partial class CyberMeters : UserControl
	{
		private readonly DispatcherTimer _timer = new() { Interval = TimeSpan.FromSeconds(60) };

		public ObservableCollection<DriveMeter> Meters { get; } = [];

		public string Title { get; } = $"{Strings.Drives.GetLocalizedResource().ToUpperInvariant()}  //  DOT";

		public CyberMeters()
		{
			InitializeComponent();
			Refresh();
			Loaded += (_, _) =>
			{
				_timer.Tick += (_, _) => Refresh();
				_timer.Start();
			};
			Unloaded += (_, _) => _timer.Stop();
		}

		// Up to three fixed drives; existing meters are updated in place so the dot tracks keep their width.
		private void Refresh()
		{
			try
			{
				var drives = DriveInfo.GetDrives().Where(d => d.DriveType == System.IO.DriveType.Fixed && d.IsReady).Take(3).ToList();
				while (Meters.Count > drives.Count)
					Meters.RemoveAt(Meters.Count - 1);
				for (var i = 0; i < drives.Count; i++)
				{
					var used = 100 - (int)Math.Round(100.0 * drives[i].AvailableFreeSpace / drives[i].TotalSize);
					if (i < Meters.Count)
						Meters[i].Update(drives[i].Name.TrimEnd('\\'), used);
					else
						Meters.Add(new DriveMeter(drives[i].Name.TrimEnd('\\'), used));
				}
			}
			catch (Exception)
			{
				// A drive that disappears mid-scan just drops off until the next refresh.
			}
		}

		// The dashed line needs its length in pixels: 10 px per dot (6 dash + 4 gap at thickness 6), whole dots only.
		private void Track_SizeChanged(object sender, SizeChangedEventArgs e)
		{
			if (sender is FrameworkElement { DataContext: DriveMeter meter })
				meter.TrackWidth = Math.Floor(e.NewSize.Width / 10) * 10 - 4;
		}
	}

	public sealed class DriveMeter(string name, int percent) : ObservableObject
	{
		private string _name = name;
		private int _percent = percent;
		private double _trackWidth;

		public string Name
		{
			get => _name;
			private set => SetProperty(ref _name, value);
		}

		public string Percent => $"{_percent}%";

		public double TrackWidth
		{
			get => _trackWidth;
			set
			{
				if (SetProperty(ref _trackWidth, value))
					OnPropertyChanged(nameof(LitWidth));
			}
		}

		/// <summary>Lit dots: the used share of the track, rounded to whole dots.</summary>
		public double LitWidth => Math.Max(0, Math.Round(TrackWidth / 10 * _percent / 100) * 10 - 4);

		public void Update(string name, int percent)
		{
			Name = name;
			_percent = percent;
			OnPropertyChanged(nameof(Percent));
			OnPropertyChanged(nameof(LitWidth));
		}
	}
}
