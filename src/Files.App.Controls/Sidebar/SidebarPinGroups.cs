// Copyright (c) Files Community. Licensed under the MIT License.

using System.IO;

namespace Files.App.Controls
{
	/// <summary>
	/// Reads %USERPROFILE%\Themes\pin-groups.txt ("GROUP | path | optional name" per line), which groups the pinned
	/// folders in the side panel. The first pin of a group gets a HUD group label; the optional name replaces the shown name.
	/// </summary>
	internal static class SidebarPinGroups
	{
		internal sealed record Entry(string Group, int Count, bool IsFirst, string? DisplayName);

		private static readonly Dictionary<string, Entry> entries = Load();

		public static Entry? Find(string? path)
			=> path is not null && entries.TryGetValue(path.TrimEnd('\\'), out var entry) ? entry : null;

		private static Dictionary<string, Entry> Load()
		{
			var result = new Dictionary<string, Entry>(StringComparer.OrdinalIgnoreCase);
			try
			{
				var file = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Themes", "pin-groups.txt");
				if (!File.Exists(file))
					return result;

				var rows = File.ReadAllLines(file)
					.Select(line => line.Trim())
					.Where(line => line.Length > 0 && !line.StartsWith('#'))
					.Select(line => line.Split('|', StringSplitOptions.TrimEntries))
					.Where(parts => parts.Length >= 2 && parts[0].Length > 0 && parts[1].Length > 0)
					.ToList();

				var counts = rows.GroupBy(parts => parts[0], StringComparer.OrdinalIgnoreCase)
					.ToDictionary(group => group.Key, group => group.Count(), StringComparer.OrdinalIgnoreCase);
				var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
				foreach (var parts in rows)
				{
					var group = parts[0].ToUpperInvariant();
					var name = parts.Length > 2 && parts[2].Length > 0 ? parts[2] : null;
					result.TryAdd(parts[1].TrimEnd('\\'), new Entry(group, counts[parts[0]], seen.Add(group), name));
				}
			}
			catch (Exception)
			{
				// A broken file only means no group labels.
			}
			return result;
		}
	}
}
