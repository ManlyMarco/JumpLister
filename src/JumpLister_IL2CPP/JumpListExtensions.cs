using BepInEx;
using System.IO;
using HarmonyLib;
using Microsoft.WindowsAPICodePack.Shell;
using Microsoft.WindowsAPICodePack.Taskbar;

#pragma warning disable CA1416

namespace JumpLister
{
    internal static class JumpListExtensions
    {
        public static void AddFileShowInExplorer(this JumpListCustomCategory category, string displayName, string relativePath, bool ifExists = false, IconReference icon = default)
        {
            var fullPath = Path.Combine(Paths.GameRootPath, relativePath);

            if (ifExists && !File.Exists(fullPath))
                return;

            // Required or it won't open correctly
            fullPath = Path.GetFullPath(fullPath);

            AddCommand(category, displayName, "explorer.exe", $"/select,\"{fullPath}\"", null, icon);
        }

        public static void AddFileOpen(this JumpListCustomCategory category, string displayName, string relativePath, bool ifExists = false, IconReference icon = default)
        {
            var fullPath = Path.Combine(Paths.GameRootPath, relativePath);

            if (ifExists && !File.Exists(fullPath))
                return;

            fullPath = Path.GetFullPath(fullPath);

            AddCommand(category, displayName, fullPath, "", Path.GetDirectoryName(fullPath), icon);
        }

        public static void AddDirectoryOpen(this JumpListCustomCategory category, string displayName, string relativePath, bool ifExists = false, IconReference? icon = null)
        {
            var fullPath = Path.Combine(Paths.GameRootPath, relativePath);

            if (ifExists && !Directory.Exists(fullPath))
                return;

            // Required or it won't open correctly
            fullPath = Path.GetFullPath(fullPath);
            AddCommand(category, displayName, "explorer.exe", $"\"{fullPath}\"", fullPath, icon ?? new IconReference("shell32.dll", 3));
        }

        public static void AddCommand(this JumpListCustomCategory category, string displayName, string command, string arguments, string workDir = null, IconReference icon = default)
        {
            var capFolder = new JumpListLink(command, displayName);
            capFolder.Arguments = arguments;
            capFolder.WorkingDirectory = workDir ?? Path.GetDirectoryName(Path.GetFullPath(command));
            capFolder.IconReference = icon;

            category.AddJumpListItems(capFolder);
        }

        public static void ClearJumpList(this JumpList jumpList)
        {
            jumpList.ClearAllUserTasks();
            Traverse.Create(jumpList).Field("_customCategoriesCollection").Method("Clear").GetValue();
        }
    }
}
