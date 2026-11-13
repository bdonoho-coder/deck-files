// Copyright (c) Files Community. Licensed under the MIT License.

using System.IO;

namespace Files.App.Controls
{
	/// <summary>
	/// Which look this build draws. Read once at start from %USERPROFILE%\Themes\current.ini, which the theme picker writes:
	/// a "look=" key wins ("hud", "minimal", "dossier", "tree"); otherwise themes named "hud-*" get the HUD look and every
	/// other theme gets the minimal look. The picker restarts Files when the look changes.
	/// </summary>
	public static class Look
	{
		public static readonly string Name;

		/// <summary>The picker's theme id, e.g. "hud-acid" or "nord" (empty when unknown).</summary>
		public static readonly string ThemeName;

		static Look()
		{
			(Name, ThemeName) = Read();
		}

		/// <summary>Every look except minimal builds on the HUD look.</summary>
		public static bool IsHud => Name != "minimal";

		public static bool IsDossier => Name == "dossier";

		/// <summary>Node Tree look: square node marks, dotted connectors, view keys on a right rail.</summary>
		public static bool IsTree => Name == "tree";

		/// <summary>Resource files merged for this look, in order (later files override earlier keys).</summary>
		public static string[] Files => Name switch
		{
			"minimal" => ["MinimalLook"],
			"dossier" => ["HudLook", "DossierLook"],
			"tree" => ["HudLook", "TreeLook"],
			_ => ["HudLook"],
		};

		/// <summary>Caps in the HUD look, unchanged otherwise. For x:Bind.</summary>
		public static string? Text(string? text)
			=> IsHud ? text?.ToUpperInvariant() : text;

		/// <summary>Code label such as "GAMES  //  12" in the HUD look, the plain name otherwise.</summary>
		public static string Label(string name, int count)
			=> IsHud ? $"{name.ToUpperInvariant()}  //  {count:00}" : name;

		private static (string Look, string Theme) Read()
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

				if (look is not ("hud" or "minimal" or "dossier" or "tree"))
					look = theme is not null && theme.StartsWith("hud-") ? "hud" : "minimal";

				return (look, theme ?? string.Empty);
			}
			catch (Exception)
			{
				return ("minimal", string.Empty);
			}
		}
	}
}
