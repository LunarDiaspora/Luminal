using ImGuiNET;
using Luminal.Console;
using Luminal.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Luminal.UI
{
    public enum FileBrowserStyle
    {
        Classic,
        Table
    }

    public class FileBrowserPanel : BasePanel
    {
        public delegate void OnFileSelected(FSFile file);
        public event OnFileSelected Selected;

        public string Root;
        public FSDirectory Dir;

        FSDirectory originalDir;

        FSDirectory _select;
        public FSDirectory SelectedDir
        {
            get => _select;
            set
            {
                _select = value;
                dirBox = value.AbsolutePath;
            }
        }

        bool stillOpen = true;

        public List<FileFilter> Filters;

        int filterIndex = 0;

        string fileBox = "";
        string dirBox = "";

        bool openModal = false;

        public bool CloseOnSelect = true;

        [ConVar("ui_filebrowser_style")]
        public static FileBrowserStyle Style = FileBrowserStyle.Classic;

        public FileBrowserPanel(string root = null, string filterPattern = "All files,*") : base()
        {
            Root = root ?? Filesystem.DataRoot;

            Filters = FileFilter.Parse(filterPattern);

            Dir = new FSDirectory(Directory.GetDirectoryRoot(root));

            originalDir = new FSDirectory(root);

            ExpandAll(originalDir);

            SelectedDir = new FSDirectory(root);
        }

        public override void Render()
        {
            var vpc = ImGui.GetMainViewport().GetCenter();

            ImGui.SetNextWindowPos(vpc, ImGuiCond.Appearing, new(0.5f, 0.5f));
            ImGui.SetNextWindowSize(new(Viewport.Width / 2f, Viewport.Height / 2f), ImGuiCond.Appearing);
            if (ImGui.Begin($"Select a file##filebrowser {ID}", ref stillOpen, ImGuiWindowFlags.NoCollapse))
            {
                var reservedHeight = ImGui.GetStyle().ItemSpacing.Y + (ImGui.GetFrameHeightWithSpacing() * 0.85f);

                var the = ImGui.GetWindowContentRegionMax().X;

                ImGui.PushStyleVar(ImGuiStyleVar.ChildBorderSize, 1.0f);
                ImGui.PushStyleVar(ImGuiStyleVar.ChildRounding, 5.0f);

                var topbarHeight = ImGui.GetFrameHeightWithSpacing() - ImGui.GetStyle().FramePadding.Y;

                if (ImGui.BeginChild("top bar", new(-1, topbarHeight), false))
                {
                    if (ImGui.ArrowButton("up one level", ImGuiDir.Up))
                    {
                        Root = Path.GetFullPath(Path.Combine(Root, ".."));
                        SelectedDir = new FSDirectory(Root);
                    }

                    ImGui.SameLine();
                    ImGui.SetNextItemWidth(-1f);
                    ImGui.InputTextWithHint("", "Directory path", ref dirBox, 65536);

                    ImGui.EndChild();
                }

                float ratio = 0.3f;

                if (ImGui.BeginChild("left side panel", new(the * ratio, -reservedHeight), true))
                {
                    DoDirectory(Dir, true);

                    ImGui.EndChild();
                }

                ImGui.SameLine();
                var sv = ImGui.GetStyle().FramePadding.X;
                var wp = ImGui.GetStyle().WindowPadding.X;
                var w = (the * (1 - ratio)) - (sv + wp);

                if (ImGui.BeginChild("right side panel", new(w, -reservedHeight), true))
                {
                    var matches = SelectedDir.Files.Where(f => Filters[filterIndex].Matches(f.Name));

                    if (matches.Count() == 0)
                    {
                        ImGui.Text("There are no files in this directory that match the filter.");
                        ImGui.TextDisabled(SelectedDir.AbsolutePath);
                    }
                    else
                    {
                        DoStyles(matches);
                    }

                    ImGui.EndChild();
                }

                var selectLabel = "Select file";

                var txt = ImGui.CalcTextSize(selectLabel).X;

                var fp = ImGui.GetStyle().FramePadding.X;

                var cr = ImGui.GetContentRegionMax().X;

                var btnX = (cr - (fp + txt)) - (fp * 2);

                ImGui.SetCursorPosX(btnX);
                ImGui.PushID("select button");

                var close = ImGui.Button(selectLabel);

                //var longest = Filters.Max(j => j.ToString().Length);
                var longestFil = Filters[filterIndex];

                var ftw = ImGui.CalcTextSize(longestFil.ToString()).X;

                ImGui.SameLine();
                var cw = (ftw + (fp * 6));

                ImGui.SetCursorPosX((btnX - cw) - fp);
                ImGui.SetNextItemWidth(cw);
                ImGui.PushID("filter combobox");
                ImGui.Combo("", ref filterIndex, Filters.Select(h => h.ToString()).ToArray(), Filters.Count);

                ImGui.SetNextItemWidth((btnX - cw) - (fp * 3));

                ImGui.SameLine();
                ImGui.SetCursorPosX(fp);
                ImGui.PushID("file path box");
                
                var close2 =
                    ImGui.InputTextWithHint("", "File path...", ref fileBox, 65535, ImGuiInputTextFlags.EnterReturnsTrue);

                if (close || close2)
                {
                    // File has been selected.
                    if (!File.Exists(fileBox))
                    {
                        openModal = true;
                    } else
                    {
                        // The file *does* exist.
                        var f = new FSFile(fileBox);
                        Selected?.Invoke(f);

                        if (CloseOnSelect)
                            Close();
                    }
                }

                ImGui.PopStyleVar();

                var open = true;

                ImGui.SetNextWindowPos(vpc, ImGuiCond.Appearing, new(0.5f, 0.5f));
                if (ImGui.BeginPopupModal("Invalid file##fileselect bad file", ref open, ImGuiWindowFlags.AlwaysAutoResize))
                {
                    ImGui.Text("The file you have specified does not exist at this location.");
                    ImGui.Text("Please check that your path is correct.");

                    ImGui.Separator();

                    if (ImGui.Button("OK", new(150, 25)))
                        ImGui.CloseCurrentPopup();

                    ImGui.EndPopup();
                }

                if (openModal)
                {
                    openModal = false;
                    ImGui.OpenPopup("Invalid file##fileselect bad file");
                }

                ImGui.End();
            }

            if (!stillOpen)
                Close();
        }

        Dictionary<string, bool> Expanded = new();

        void ExpandAll(FSDirectory dir)
        {
            Expanded[dir.AbsolutePath] = true;

            foreach (var c in PathHelper.GetAllSteps(dir.AbsolutePath))
            {
                Expanded[c] = true;
            }

            //foreach (var c in dir.Children)
            //    ExpandAll(c);
        }

        void DoDirectory(FSDirectory dir, bool first = false)
        {
            if (dir.Children.Count == 0)
            {
                // end of the line
                ImGui.TreePush();

                ImGui.Selectable($"{dir.Name}##{dir.AbsolutePath}");

                if (ImGui.IsItemClicked())
                    SelectedDir = dir;

                ImGui.TreePop();
                return;
            }

            if (first && !Expanded.ContainsKey(dir.AbsolutePath))
            {
                Expanded[dir.AbsolutePath] = true;
            }

            bool open = first || Expanded.ContainsKey(dir.AbsolutePath) ? Expanded[dir.AbsolutePath] : false;

            ImGui.SetNextItemOpen(open);

            var n = dir.Name;
            if (n.Length == 0)
            {
                n = dir.AbsolutePath;
            }

            if (ImGui.TreeNodeEx($"{n}##{dir.AbsolutePath}",
                ImGuiTreeNodeFlags.OpenOnDoubleClick |
                ImGuiTreeNodeFlags.NoTreePushOnOpen))
            {
                ImGui.TreePush();

                Expanded[dir.AbsolutePath] = true;

                if (ImGui.IsItemClicked())
                {
                    Root = dir.AbsolutePath;
                    SelectedDir = dir;
                }

                foreach (var j in dir.Children)
                {
                    DoDirectory(j, false); // recursion is fun innit
                }

                ImGui.TreePop();
            } else
            {
                Expanded[dir.AbsolutePath] = false;
            }
        }

        void DoStyles(IEnumerable<FSFile> matches)
        {
            if (Style == FileBrowserStyle.Table)
            {
                var tabflag = ImGuiTableFlags.Resizable | ImGuiTableFlags.ScrollY
                    | ImGuiTableFlags.Sortable | ImGuiTableFlags.Reorderable;

                if (ImGui.BeginTable("right side panel's table", 3, tabflag, new(-1, -1)))
                {
                    ImGui.TableSetupColumn("Filename", ImGuiTableColumnFlags.DefaultSort,
                        0.8f);
                    ImGui.TableSetupColumn("File type", ImGuiTableColumnFlags.None,
                        0.2f);
                    ImGui.TableSetupColumn("File size", ImGuiTableColumnFlags.None,
                        0.1f);
                    ImGui.TableSetupScrollFreeze(0, 1);
                    ImGui.TableHeadersRow();

                    unsafe
                    {
                        ImGuiListClipper* cl = ImGuiNative.ImGuiListClipper_ImGuiListClipper();
                        ImGuiNative.ImGuiListClipper_Begin(cl, matches.Count(), 0.0f);
                        while (ImGuiNative.ImGuiListClipper_Step(cl) != 0)
                        {
                            for (int row = cl->DisplayStart; row < cl->DisplayEnd; row++)
                            {
                                var f = matches.ElementAt(row);
                                ImGui.PushID(f.AbsolutePath);

                                ImGui.BeginGroup();
                                ImGui.TableNextRow();
                                ImGui.TableNextColumn();
                                ImGui.Text(f.FileName);

                                ImGui.TableNextColumn();
                                var type = WinAPIUtilities.GetFileType(f.AbsolutePath);
                                ImGui.Text(type);

                                ImGui.TableNextColumn();
                                ImGui.Text(f.HumanFileSize);
                                ImGui.EndGroup();

                                if (ImGui.IsItemClicked())
                                {
                                    fileBox = f.AbsolutePath;
                                }

                                ImGui.PopID();
                            }
                        }
                    }


                    ImGui.EndTable();
                }
            }
            else if (Style == FileBrowserStyle.Classic)
            {
                ImGui.PushStyleVar(ImGuiStyleVar.ItemSpacing, new Vector2(5.0f, 5.0f));

                foreach (var i in matches.OrderBy(x => x.FileName))
                {
                    ImGui.Dummy(new Vector2(0.0f, 1f));

                    var text = $"{WinAPIUtilities.GetFileType(i.Path)}";

                    ImGui.BeginGroup();
                    var icon = WinAPIUtilities.GetIcon(i.Path);
                    ImGui.Image(new IntPtr(icon.GLObject), new(30, 30));

                    var the = ImGui.GetContentRegionMax();

                    ImGui.SameLine();
                    if (ImGui.Selectable(i.FileName, false, ImGuiSelectableFlags.None, new(the.X, 30)))
                    {
                        fileBox = i.AbsolutePath;
                    }

                    var fp = ImGui.GetStyle().FramePadding.X;
                    var ts = ImGui.CalcTextSize(text).X;
                    var cr = ImGui.GetContentRegionMax().X;
                    ImGui.SameLine();
                    ImGui.SetCursorPosX(cr - (ts + (fp * 2)));
                    ImGui.TextDisabled(text);

                    ImGui.SameLine(0.0f, 0.0f);
                    var cp = ImGui.GetCursorPosY();
                    ImGui.SetCursorPosX(30 + (fp*1.5f));
                    ImGui.SetCursorPosY(cp + 4.0f);

                    ImGui.TextDisabled("\n"+i.HumanFileSize);
                    ImGui.EndGroup();

                    if (ImGui.IsItemClicked())
                    {
                        fileBox = i.AbsolutePath;
                    }

                    ImGui.Separator();
                }

                ImGui.PopStyleVar();
            }
        }
    }
}
