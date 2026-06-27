using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace MacroEditor
{
    // Unit 15: Partial load + fix-each-problem import workflow.
    // Single-file imports run a two-stage repair (Stage 1 = file syntax, Stage 2 = per-macro
    // problems) and load nothing until every problem is fixed or skipped. Bulk folder import
    // applies clean files as it goes and reports problem files for individual repair.
    public partial class MainForm
    {
        /// <summary>Result of the prompt+parse+repair pipeline for a single file.</summary>
        private class RepairOutcome
        {
            public MacroExportModel Model;
            public string SourcePath;
            public MacroTextFormat Format;
            public bool Cancelled;
            public bool StageOneEdited;
            public int FixCount;
            public int SkipCount;
            public bool Repaired { get { return StageOneEdited || FixCount > 0 || SkipCount > 0; } }
        }

        // ===== File open helper =====

        private bool PromptForImportFile(string title, out string path, out string content)
        {
            path = null;
            content = null;
            this.OpenDialog.InitialDirectory = this.macropath;
            this.OpenDialog.Filter = "Text Files|*.txt|All Files|*.*";
            this.OpenDialog.FileName = "";
            this.OpenDialog.Multiselect = false;
            this.OpenDialog.Title = title;
            bool ok = this.OpenDialog.ShowDialog() != DialogResult.Cancel;
            this.OpenDialog.Title = "Find Macro Files";
            this.OpenDialog.Filter = "Macro Title Files|mcr.ttl";
            if (!ok) return false;
            path = this.OpenDialog.FileName;
            try { content = System.IO.File.ReadAllText(path); }
            catch (Exception ex)
            {
                MessageBox.Show("Could not read the file:\n" + ex.Message, "Import", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            return true;
        }

        // ===== Repair pipeline =====

        private RepairOutcome PromptParseAndRepair(string title, bool checkBookRange, bool checkPageRange)
        {
            var outcome = new RepairOutcome();
            string path, content;
            if (!PromptForImportFile(title, out path, out content)) { outcome.Cancelled = true; return outcome; }
            outcome.SourcePath = path;

            // ----- Stage 1: file-level syntax -----
            MacroImportResult result = MacroTextSerializer.Parse(content);
            while (!result.Success)
            {
                using (var dlg = new ImportRepairForm(
                    "Repair Import - File Problem",
                    "This file could not be read:\n" + result.ErrorMessage +
                    "\n\nEdit the text below to fix it, then click Re-check.",
                    content, false))
                {
                    dlg.ShowDialog(this);
                    if (dlg.Result == RepairChoice.Cancel) { outcome.Cancelled = true; return outcome; }
                    if (dlg.EditedText != content) outcome.StageOneEdited = true;
                    content = dlg.EditedText;
                }
                result = MacroTextSerializer.Parse(content);
            }

            outcome.Format = content.Contains(MacroTextSerializer.JsonBegin) ? MacroTextFormat.Ai : MacroTextFormat.Human;
            outcome.Model = result.Model;
            this._importParseWarnings = result.Warnings ?? new List<string>();

            // ----- Stage 2: per-item problems -----
            bool skipAll = false;
            while (true)
            {
                List<ImportProblem> problems = MacroTextSerializer.Validate(outcome.Model, this.Books.Count, checkBookRange, checkPageRange);
                if (problems.Count == 0) break;

                ImportProblem p = problems[0];

                if (skipAll)
                {
                    DropProblemTarget(outcome.Model, p);
                    outcome.SkipCount++;
                    continue;
                }

                // Combine all problem descriptions that share this target (e.g. a macro with both a bad slot and a long title).
                string combinedDesc = string.Join("\n", problems.Where(q => SameTarget(q, p)).Select(q => " - " + q.Description).ToArray());
                bool macroLevel = p.Macro != null;
                string snippet;
                string caption;
                if (macroLevel) { snippet = MacroTextSerializer.MacroToSnippet(p.Macro); caption = "Repair Import - Macro Problem"; }
                else if (p.Kind == ImportProblemKind.BookOutOfRange) { snippet = "book: " + p.Book.Number; caption = "Repair Import - Book Number"; }
                else { snippet = "page: " + p.Page.Number; caption = "Repair Import - Page Number"; }

                using (var dlg = new ImportRepairForm(caption, combinedDesc + "\n\nFix it below, or Skip to leave it out of the import.", snippet, true))
                {
                    dlg.ShowDialog(this);
                    switch (dlg.Result)
                    {
                        case RepairChoice.Cancel:
                            outcome.Cancelled = true;
                            return outcome;
                        case RepairChoice.SkipAll:
                            skipAll = true; // current item handled on next loop iteration
                            continue;
                        case RepairChoice.Skip:
                            DropProblemTarget(outcome.Model, p);
                            outcome.SkipCount++;
                            continue;
                        case RepairChoice.Fix:
                            ApplyFix(p, macroLevel, dlg.EditedText);
                            outcome.FixCount++;
                            continue;
                    }
                }
            }

            return outcome;
        }

        private static bool SameTarget(ImportProblem a, ImportProblem b)
        {
            if (a.Macro != null || b.Macro != null) return ReferenceEquals(a.Macro, b.Macro);
            if (a.Kind == ImportProblemKind.PageOutOfRange || b.Kind == ImportProblemKind.PageOutOfRange) return ReferenceEquals(a.Page, b.Page);
            return ReferenceEquals(a.Book, b.Book);
        }

        private void DropProblemTarget(MacroExportModel model, ImportProblem p)
        {
            if (p.Macro != null) { if (p.Page != null) p.Page.Macros.Remove(p.Macro); }
            else if (p.Kind == ImportProblemKind.PageOutOfRange) { if (p.Book != null) p.Book.Pages.Remove(p.Page); }
            else { model.Books.Remove(p.Book); }
        }

        private void ApplyFix(ImportProblem p, bool macroLevel, string editedText)
        {
            if (macroLevel)
            {
                MacroExport edited = MacroTextSerializer.ParseMacroSnippet(editedText);
                p.Macro.Slot = edited.Slot;
                p.Macro.Title = edited.Title;
                p.Macro.Lines = edited.Lines;
                p.Macro.RawLineCount = 6;
            }
            else if (p.Kind == ImportProblemKind.BookOutOfRange)
            {
                int n;
                if (TryParseLabeledInt(editedText, "book", out n)) p.Book.Number = n;
            }
            else // PageOutOfRange
            {
                int n;
                if (TryParseLabeledInt(editedText, "page", out n)) p.Page.Number = n;
            }
        }

        private static bool TryParseLabeledInt(string text, string label, out int value)
        {
            value = 0;
            if (string.IsNullOrEmpty(text)) return false;
            string s = text.Trim();
            int colon = s.IndexOf(':');
            if (colon >= 0) s = s.Substring(colon + 1);
            s = s.Trim();
            return int.TryParse(s, out value);
        }

        // ===== Save corrected file back =====

        private void OfferSaveBack(RepairOutcome outcome)
        {
            if (outcome == null || !outcome.Repaired || string.IsNullOrEmpty(outcome.SourcePath)) return;
            var msg = new StringBuilder();
            msg.AppendLine("This file had problems that were repaired during import.");
            if (outcome.SkipCount > 0)
                msg.AppendLine("NOTE: " + outcome.SkipCount + " skipped item(s) will be OMITTED from the saved file.");
            msg.AppendLine();
            msg.AppendLine("Save the corrected version back to:");
            msg.AppendLine(outcome.SourcePath + " ?");
            if (MessageBox.Show(msg.ToString(), "Save Corrected File", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                return;
            try
            {
                string corrected = (outcome.Format == MacroTextFormat.Ai)
                    ? MacroTextSerializer.ExportAi(outcome.Model)
                    : MacroTextSerializer.ExportHuman(outcome.Model);
                System.IO.File.WriteAllText(outcome.SourcePath, corrected);
                this.UpdateStatusBar("Saved corrected file", System.IO.Path.GetFileName(outcome.SourcePath));
            }
            catch (Exception ex)
            {
                MessageBox.Show("Could not save the corrected file:\n" + ex.Message, "Save Corrected File", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
