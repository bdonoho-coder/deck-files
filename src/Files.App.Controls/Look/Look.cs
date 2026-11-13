// Copyright (c) Files Community. Licensed under the MIT License.

using System.IO;

namespace Files.App.Controls
{
	/// <summary>
	/// Which look this build draws. Read once at start from %USERPROFILE%\Themes\current.ini, which the theme picker writes:
	/// a "look=" key wins ("hud", "minimal", "dossier", "tree", "deck", "cyber"); otherwise every theme gets the Cyber look.
	/// The picker restarts Files when the look changes.
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

		/// <summary>Console Deck look: titled panes (NAV / DIR / INFO), key-cap toolbar, tabs stacked on a left rail.</summary>
		public static bool IsDeck => Name == "deck";

		/// <summary>Cyber Revamp look: big-icon places rail, console panes, file plates, command bar, ID card, filler graphics.</summary>
		public static bool IsCyber => Name == "cyber";

		/// <summary>Retro-cyberpunk extras (CRT scan lines, glitch readout, dashed circuit lines) for the Acid Signal theme, on top of any HUD-based look.</summary>
		public static bool IsRetro => IsHud && ThemeName == "hud-acid";

		/// <summary>Resource files merged for this look, in order (later files override earlier keys).</summary>
		public static string[] Files
		{
			get
			{
				string[] files = Name switch
				{
					"minimal" => ["MinimalLook"],
					"dossier" => ["HudLook", "DossierLook"],
					"tree" => ["HudLook", "TreeLook"],
					"deck" => ["HudLook", "DeckLook"],
					"cyber" => ["HudLook", "CyberLook"],
					_ => ["HudLook"],
				};
				return IsRetro ? [.. files, "AcidLook"] : files;
			}
		}

		/// <summary>Caps in the HUD look, unchanged otherwise. For x:Bind.</summary>
		public static string? Text(string? text)
			=> IsHud ? text?.ToUpperInvariant() : text;

		/// <summary>Cyber look status bar readout: the item count in hex plus a checksum of the text, e.g. "N 0x001F  //  CRC 3A7C".</summary>
		public static string Hex(string? text)
		{
			if (string.IsNullOrEmpty(text))
				return string.Empty;
			var n = 0;
			foreach (var ch in text)
			{
				if (ch < '0' || ch > '9')
					break;
				n = n * 10 + (ch - '0');
			}
			var crc = 0xFFFF;
			foreach (var ch in text)
			{
				crc ^= ch << 8;
				for (var i = 0; i < 8; i++)
					crc = (crc & 0x8000) != 0 ? ((crc << 1) ^ 0x1021) & 0xFFFF : (crc << 1) & 0xFFFF;
			}
			return $"N 0x{n:X4}  //  CRC {crc:X4}";
		}

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

				// No explicit look (the picker's AUTO): the Cyber look for every theme, in that theme's colors.
				if (look is not ("hud" or "minimal" or "dossier" or "tree" or "deck" or "cyber"))
					look = "cyber";

				return (look, theme ?? string.Empty);
			}
			catch (Exception)
			{
				return ("minimal", string.Empty);
			}
		}
	}
}
