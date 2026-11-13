// Copyright (c) Files Community. Licensed under the MIT License.

using System.IO;

namespace Files.App.Controls
{
	/// <summary>
	/// Which look this build draws. Read once at start from %USERPROFILE%\Themes\current.ini, which the theme picker writes:
	/// a "look=" key wins; otherwise themes named "hud-*" get the HUD look and every other theme gets the minimal look.
	/// The picker restarts Files when a theme switch changes the look.
	/// </summary>
	public static class Look
	{
		public static readonly string Name = Read();

		public static bool IsHud => Name == "hud";

		/// <summary>Caps in the HUD look, unchanged otherwise. For x:Bind.</summary>
		public static string? Text(string? text)
			=> IsHud ? text?.ToUpperInvariant() : text;

		/// <summary>Code label such as "GAMES  //  12" in the HUD look, the plain name otherwise.</summary>
		public static string Label(string name, int count)
			=> IsHud ? $"{name.ToUpperInvariant()}  //  {count:00}" : name;

		private static string Read()
		{
			try
			{
				var file = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Themes", "current.ini");
				string? look = null, theme = null;
				foreach (var raw in File.ReadAllLines(file))
				{
					var line = raw.Trim();
					if (line.StartsWith("look=", StringComparison.OrdinalIgnoreCase))
						look = line[5..].Trim().ToLowerInvariant();
					else if (line.StartsWith("current=", StringComparison.OrdinalIgnoreCase))
						theme = line[8..].Trim().ToLowerInvariant();
				}

				if (look is "hud" or "minimal")
					return look;

				return theme is not null && theme.StartsWith("hud-") ? "hud" : "minimal";
			}
			catch (Exception)
			{
				return "minimal";
			}
		}
	}
}
