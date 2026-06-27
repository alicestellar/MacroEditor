using System;
using System.Collections.Generic;
using System.Text;
using System.Web.Script.Serialization;

namespace MacroEditor
{
	public enum MacroTextFormat { Human, Ai }
	public enum MacroTextScope { Book, Page, Row, All }

	/// <summary>One macro's exported content.</summary>
	public class MacroExport
	{
		public string Slot;       // "Ctrl1".."Ctrl10", "Alt1".."Alt10"
		public string Title;
		public string[] Lines;    // exactly 6
		public int RawLineCount = 6; // number of lines found before normalization (for validation)
	}

	public class PageExport
	{
		public int Number;        // 1-10
		public List<MacroExport> Macros = new List<MacroExport>();
	}

	public class BookExport
	{
		public int Number;        // 1-40
		public string Name = "";
		public List<PageExport> Pages = new List<PageExport>();
	}

	public class MacroExportModel
	{
		public string Character = "";
		public MacroTextScope Scope = MacroTextScope.Book;
		public List<BookExport> Books = new List<BookExport>();
	}

	public class MacroImportResult
	{
		public bool Success;
		public string ErrorMessage;
		public MacroExportModel Model;
		public List<string> Warnings = new List<string>();
	}

	public enum ImportProblemKind { InvalidSlot, BookOutOfRange, PageOutOfRange, TitleTooLong, LineCount }

	/// <summary>
	/// A single per-item problem found while validating a parsed model, with references
	/// back into the model so the repair workflow can fix or drop the offending item.
	/// </summary>
	public class ImportProblem
	{
		public ImportProblemKind Kind;
		public string Description;
		public BookExport Book;     // the containing book (never null)
		public PageExport Page;     // the containing page (null for book-range problems)
		public MacroExport Macro;   // the offending macro (null for book/page-range problems)
	}

	public static class MacroTextSerializer
	{
		public const string JsonBegin = "<<<MACRO_JSON_BEGIN>>>";
		public const string JsonEnd = "<<<MACRO_JSON_END>>>";

		// ===== Slot helpers =====

		public static string IndexToSlot(int macroIndex)
		{
			return (macroIndex < 10)
				? "Ctrl" + (macroIndex + 1)
				: "Alt" + (macroIndex - 9);
		}

		/// <summary>Returns macro index 0-19 for a slot string, or -1 if invalid.</summary>
		public static int SlotToIndex(string slot)
		{
			if (string.IsNullOrEmpty(slot)) return -1;
			slot = slot.Trim();
			try
			{
				if (slot.StartsWith("Ctrl", StringComparison.OrdinalIgnoreCase))
				{
					int n = int.Parse(slot.Substring(4));
					if (n >= 1 && n <= 10) return n - 1;
				}
				else if (slot.StartsWith("Alt", StringComparison.OrdinalIgnoreCase))
				{
					int n = int.Parse(slot.Substring(3));
					if (n >= 1 && n <= 10) return n + 9;
				}
			}
			catch { }
			return -1;
		}

		// ===== Export: Human =====

		public static string ExportHuman(MacroExportModel model)
		{
			var sb = new StringBuilder();
			sb.AppendLine("================================================================");
			sb.AppendLine(" FFXI MACRO EXPORT  (Human-Editable Format v1)");
			sb.AppendLine(" Character: " + model.Character);
			sb.AppendLine(" Scope: " + model.Scope.ToString().ToUpper());
			if (model.Books.Count > 0)
				sb.AppendLine(" Source: Book " + model.Books[0].Number + "  \"" + model.Books[0].Name + "\"");
			sb.AppendLine("================================================================");
			sb.AppendLine(" HOW TO EDIT WITHOUT BREAKING THIS FILE:");
			sb.AppendLine("  * Do NOT change lines starting with  =  #  -  or  [ .");
			sb.AppendLine("  * Keep the \"  N:\" prefixes (1-6) on each macro line. To make a");
			sb.AppendLine("    line empty, leave it blank after the colon.");
			sb.AppendLine("  * Titles are max 8 characters; extra characters are dropped on import.");
			sb.AppendLine("  * Tokens like {Name} are variables - leave them as-is unless you");
			sb.AppendLine("    intend to change them.");
			sb.AppendLine("  * Do NOT reorder, add, or remove books, pages, or slots.");
			sb.AppendLine("================================================================");
			sb.AppendLine();

			foreach (var book in model.Books)
			{
				sb.AppendLine("#### BOOK " + book.Number + ": \"" + book.Name + "\" ####");
				sb.AppendLine();
				foreach (var page in book.Pages)
				{
					sb.AppendLine("-- Page " + page.Number + " --");
					sb.AppendLine();
					foreach (var macro in page.Macros)
					{
						string slotLabel = macro.Slot.Replace("Ctrl", "Ctrl ").Replace("Alt", "Alt ");
						sb.AppendLine("[" + slotLabel + "]  Title: " + macro.Title);
						for (int i = 0; i < 6; i++)
						{
							string line = (macro.Lines != null && i < macro.Lines.Length) ? macro.Lines[i] : "";
							sb.AppendLine("  " + (i + 1) + ": " + line);
						}
						sb.AppendLine();
					}
				}
			}
			return sb.ToString();
		}

		// ===== Export: AI =====

		public static string ExportAi(MacroExportModel model)
		{
			var sb = new StringBuilder();
			sb.AppendLine("=== INSTRUCTIONS FOR THE AI EDITING THIS FILE - READ FIRST ===");
			sb.AppendLine("This file contains Final Fantasy XI macros exported from \"Macro Editor\".");
			sb.AppendLine();
			sb.AppendLine("STRUCTURE:");
			sb.AppendLine("  books -> pages (1-10) -> macros. Each macro has a \"slot\" (Ctrl1-Ctrl10,");
			sb.AppendLine("  Alt1-Alt10), a \"title\" (max 8 characters), and exactly 6 \"lines\"");
			sb.AppendLine("  (strings; use \"\" for an empty line).");
			sb.AppendLine();
			sb.AppendLine("RULES - FOLLOW EXACTLY OR THE FILE WILL FAIL TO IMPORT:");
			sb.AppendLine("  * Edit ONLY the \"title\" and \"lines\" string values.");
			sb.AppendLine("  * Do NOT add, remove, rename, or reorder any keys, slots, pages, or books.");
			sb.AppendLine("  * Keep titles to 8 characters or fewer.");
			sb.AppendLine("  * Keep exactly 6 entries in every \"lines\" array.");
			sb.AppendLine("  * Tokens like {Name} or {!Name} are variables. Leave them exactly as");
			sb.AppendLine("    written unless explicitly asked to change them.");
			sb.AppendLine("  * Output the ENTIRE file back, including this instruction block and the");
			sb.AppendLine("    BEGIN/END markers, with your edits applied. The text between the markers");
			sb.AppendLine("    must be valid JSON.");
			sb.AppendLine("=== END INSTRUCTIONS ===");
			sb.AppendLine();
			sb.AppendLine(JsonBegin);
			sb.AppendLine(BuildJson(model));
			sb.AppendLine(JsonEnd);
			return sb.ToString();
		}

		private static string BuildJson(MacroExportModel model)
		{
			var sb = new StringBuilder();
			sb.AppendLine("{");
			sb.AppendLine("  \"format\": \"macro-editor-ai-v1\",");
			sb.AppendLine("  \"character\": " + J(model.Character) + ",");
			sb.AppendLine("  \"scope\": " + J(model.Scope.ToString().ToLower()) + ",");
			sb.AppendLine("  \"books\": [");
			for (int bi = 0; bi < model.Books.Count; bi++)
			{
				var book = model.Books[bi];
				sb.AppendLine("    {");
				sb.AppendLine("      \"book\": " + book.Number + ",");
				sb.AppendLine("      \"name\": " + J(book.Name) + ",");
				sb.AppendLine("      \"pages\": [");
				for (int pi = 0; pi < book.Pages.Count; pi++)
				{
					var page = book.Pages[pi];
					sb.AppendLine("        {");
					sb.AppendLine("          \"page\": " + page.Number + ",");
					sb.AppendLine("          \"macros\": [");
					for (int mi = 0; mi < page.Macros.Count; mi++)
					{
						var macro = page.Macros[mi];
						var lineParts = new List<string>();
						for (int i = 0; i < 6; i++)
						{
							string line = (macro.Lines != null && i < macro.Lines.Length) ? macro.Lines[i] : "";
							lineParts.Add(J(line));
						}
						string comma = (mi < page.Macros.Count - 1) ? "," : "";
						sb.AppendLine("            { \"slot\": " + J(macro.Slot) + ", \"title\": " + J(macro.Title) +
							", \"lines\": [" + string.Join(", ", lineParts) + "] }" + comma);
					}
					sb.AppendLine("          ]");
					sb.AppendLine("        }" + (pi < book.Pages.Count - 1 ? "," : ""));
				}
				sb.AppendLine("      ]");
				sb.AppendLine("    }" + (bi < model.Books.Count - 1 ? "," : ""));
			}
			sb.AppendLine("  ]");
			sb.Append("}");
			return sb.ToString();
		}

		/// <summary>Serializes a string as a properly-escaped JSON string literal.</summary>
		private static string J(string s)
		{
			return new JavaScriptSerializer().Serialize(s ?? "");
		}

		// ===== Import (auto-detect) =====

		public static MacroImportResult Parse(string content)
		{
			var result = new MacroImportResult();
			if (string.IsNullOrEmpty(content))
			{
				result.Success = false;
				result.ErrorMessage = "The file is empty.";
				return result;
			}

			if (content.Contains(JsonBegin))
				return ParseAi(content, result);
			if (content.Contains("FFXI MACRO EXPORT") || content.Contains("#### BOOK"))
				return ParseHuman(content, result);

			result.Success = false;
			result.ErrorMessage = "Unrecognized macro text file (no AI markers or human-format header found).";
			return result;
		}

		private static MacroImportResult ParseAi(string content, MacroImportResult result)
		{
			int start = content.IndexOf(JsonBegin);
			int end = content.IndexOf(JsonEnd);
			if (start < 0 || end < 0 || end <= start)
			{
				result.Success = false;
				result.ErrorMessage = "Could not find the " + JsonBegin + " / " + JsonEnd + " markers around the JSON.";
				return result;
			}
			start += JsonBegin.Length;
			string json = content.Substring(start, end - start).Trim();

			object parsed;
			try
			{
				var ser = new JavaScriptSerializer();
				ser.MaxJsonLength = 64 * 1024 * 1024;
				parsed = ser.DeserializeObject(json);
			}
			catch (Exception ex)
			{
				result.Success = false;
				result.ErrorMessage = "The JSON between the markers is not valid: " + ex.Message;
				return result;
			}

			var root = parsed as Dictionary<string, object>;
			if (root == null)
			{
				result.Success = false;
				result.ErrorMessage = "The JSON root is not an object.";
				return result;
			}

			var model = new MacroExportModel();
			model.Character = GetString(root, "character", "");
			model.Scope = ParseScope(GetString(root, "scope", "book"));

			object booksObj;
			if (!root.TryGetValue("books", out booksObj) || !(booksObj is object[]))
			{
				result.Success = false;
				result.ErrorMessage = "The JSON has no \"books\" array.";
				return result;
			}

			foreach (object bo in (object[])booksObj)
			{
				var bd = bo as Dictionary<string, object>;
				if (bd == null) continue;
				var book = new BookExport();
				book.Number = GetInt(bd, "book", 0);
				book.Name = GetString(bd, "name", "");

				object pagesObj;
				if (bd.TryGetValue("pages", out pagesObj) && pagesObj is object[])
				{
					foreach (object po in (object[])pagesObj)
					{
						var pd = po as Dictionary<string, object>;
						if (pd == null) continue;
						var page = new PageExport();
						page.Number = GetInt(pd, "page", 0);

						object macrosObj;
						if (pd.TryGetValue("macros", out macrosObj) && macrosObj is object[])
						{
							foreach (object mo in (object[])macrosObj)
							{
								var md = mo as Dictionary<string, object>;
								if (md == null) continue;
								var macro = new MacroExport();
								macro.Slot = GetString(md, "slot", "");
								macro.Title = GetString(md, "title", "");
								var rawLines = GetStringArray(md, "lines");
								macro.RawLineCount = rawLines.Length;
								macro.Lines = NormalizeLines(rawLines, book.Number, page.Number, macro.Slot, result);
								page.Macros.Add(macro);
							}
						}
						book.Pages.Add(page);
					}
				}
				model.Books.Add(book);
			}

			result.Model = model;
			result.Success = true;
			return result;
		}

		private static MacroImportResult ParseHuman(string content, MacroImportResult result)
		{
			var model = new MacroExportModel();
			var lines = content.Replace("\r\n", "\n").Replace("\r", "\n").Split('\n');

			BookExport currentBook = null;
			PageExport currentPage = null;
			MacroExport currentMacro = null;

			foreach (string raw in lines)
			{
				string line = raw;
				string trimmed = line.Trim();

				// Header metadata
				if (trimmed.StartsWith("Character:"))
				{
					model.Character = trimmed.Substring("Character:".Length).Trim();
					continue;
				}
				if (trimmed.StartsWith("Scope:"))
				{
					model.Scope = ParseScope(trimmed.Substring("Scope:".Length).Trim());
					continue;
				}

				if (trimmed.StartsWith("#### BOOK"))
				{
					currentBook = new BookExport();
					// Format: #### BOOK n: "name" ####
					try
					{
						int colon = trimmed.IndexOf(':');
						string numPart = trimmed.Substring("#### BOOK".Length, colon - "#### BOOK".Length).Trim();
						currentBook.Number = int.Parse(numPart);
						int q1 = trimmed.IndexOf('"');
						int q2 = trimmed.LastIndexOf('"');
						if (q1 >= 0 && q2 > q1)
							currentBook.Name = trimmed.Substring(q1 + 1, q2 - q1 - 1);
					}
					catch { result.Warnings.Add("Could not fully parse book header: " + trimmed); }
					model.Books.Add(currentBook);
					currentPage = null;
					currentMacro = null;
					continue;
				}

				if (trimmed.StartsWith("-- Page"))
				{
					currentPage = new PageExport();
					try
					{
						string p = trimmed.Replace("-- Page", "").Replace("--", "").Trim();
						currentPage.Number = int.Parse(p);
					}
					catch { result.Warnings.Add("Could not parse page header: " + trimmed); }
					if (currentBook == null) { currentBook = new BookExport(); model.Books.Add(currentBook); }
					currentBook.Pages.Add(currentPage);
					currentMacro = null;
					continue;
				}

				if (trimmed.StartsWith("["))
				{
					// [Ctrl 1]  Title: xxx
					currentMacro = new MacroExport();
					currentMacro.Lines = new string[] { "", "", "", "", "", "" };
					try
					{
						int rb = trimmed.IndexOf(']');
						string slotRaw = trimmed.Substring(1, rb - 1).Replace(" ", "");
						currentMacro.Slot = slotRaw;
						int ti = trimmed.IndexOf("Title:");
						currentMacro.Title = (ti >= 0) ? trimmed.Substring(ti + "Title:".Length).Trim() : "";
					}
					catch { result.Warnings.Add("Could not parse macro header: " + trimmed); }
					if (currentPage == null) { currentPage = new PageExport(); if (currentBook != null) currentBook.Pages.Add(currentPage); }
					if (currentPage != null) currentPage.Macros.Add(currentMacro);
					continue;
				}

				// Macro line:  "  N: text"
				string lt = line.TrimStart();
				if (lt.Length >= 2 && char.IsDigit(lt[0]) && lt[1] == ':' && currentMacro != null)
				{
					int lineNum = lt[0] - '0';
					if (lineNum >= 1 && lineNum <= 6)
					{
						string text = lt.Substring(2);
						if (text.StartsWith(" ")) text = text.Substring(1);
						currentMacro.Lines[lineNum - 1] = text;
					}
				}
			}

			result.Model = model;
			result.Success = true;
			return result;
		}

		// ===== Helpers =====

		private static MacroTextScope ParseScope(string s)
		{
			switch ((s ?? "").Trim().ToLower())
			{
				case "page": return MacroTextScope.Page;
				case "row": return MacroTextScope.Row;
				case "all": return MacroTextScope.All;
				default: return MacroTextScope.Book;
			}
		}

		private static string[] NormalizeLines(string[] input, int book, int page, string slot, MacroImportResult result)
		{
			var lines = new string[6];
			for (int i = 0; i < 6; i++)
				lines[i] = (input != null && i < input.Length) ? (input[i] ?? "") : "";
			if (input != null && input.Length != 6)
				result.Warnings.Add(string.Format("Book {0} Page {1} {2}: expected 6 lines, found {3} (adjusted).",
					book, page, slot, input.Length));
			return lines;
		}

		private static string GetString(Dictionary<string, object> d, string key, string def)
		{
			object v;
			if (d.TryGetValue(key, out v) && v != null) return v.ToString();
			return def;
		}

		private static int GetInt(Dictionary<string, object> d, string key, int def)
		{
			object v;
			if (d.TryGetValue(key, out v) && v != null)
			{
				try { return Convert.ToInt32(v); } catch { }
			}
			return def;
		}

		private static string[] GetStringArray(Dictionary<string, object> d, string key)
		{
			object v;
			if (d.TryGetValue(key, out v) && v is object[])
			{
				var arr = (object[])v;
				var outArr = new string[arr.Length];
				for (int i = 0; i < arr.Length; i++)
					outArr[i] = (arr[i] != null) ? arr[i].ToString() : "";
				return outArr;
			}
			return new string[0];
		}

		// ===== Validation (for the import repair workflow) =====

		/// <summary>
		/// Finds per-item problems in a parsed model. <paramref name="bookCount"/> is the number of
		/// books currently loaded. <paramref name="checkBookRange"/> / <paramref name="checkPageRange"/>
		/// control whether book/page numbers matter (they do not for targeted "import over" operations).
		/// </summary>
		public static List<ImportProblem> Validate(MacroExportModel model, int bookCount, bool checkBookRange, bool checkPageRange)
		{
			var problems = new List<ImportProblem>();
			if (model == null) return problems;
			foreach (var book in model.Books)
			{
				if (checkBookRange && (book.Number < 1 || book.Number > bookCount))
				{
					problems.Add(new ImportProblem
					{
						Kind = ImportProblemKind.BookOutOfRange,
						Description = "Book number " + book.Number + " is out of range (valid 1-" + bookCount + ").",
						Book = book
					});
					continue; // don't drill into pages of an out-of-range book until it's fixed
				}
				foreach (var page in book.Pages)
				{
					if (checkPageRange && (page.Number < 1 || page.Number > 10))
					{
						problems.Add(new ImportProblem
						{
							Kind = ImportProblemKind.PageOutOfRange,
							Description = "Book " + book.Number + ": page number " + page.Number + " is out of range (valid 1-10).",
							Book = book,
							Page = page
						});
						continue;
					}
					foreach (var macro in page.Macros)
					{
						if (SlotToIndex(macro.Slot) < 0)
						{
							problems.Add(MakeMacroProblem(ImportProblemKind.InvalidSlot, book, page, macro,
								"Invalid slot \"" + macro.Slot + "\" (expected Ctrl1-Ctrl10 or Alt1-Alt10)."));
						}
						if (macro.Title != null && macro.Title.Length > 8)
						{
							problems.Add(MakeMacroProblem(ImportProblemKind.TitleTooLong, book, page, macro,
								"Title \"" + macro.Title + "\" is " + macro.Title.Length + " characters (max 8)."));
						}
						if (macro.RawLineCount != 6)
						{
							problems.Add(MakeMacroProblem(ImportProblemKind.LineCount, book, page, macro,
								"Expected exactly 6 lines, found " + macro.RawLineCount + "."));
						}
					}
				}
			}
			return problems;
		}

		private static ImportProblem MakeMacroProblem(ImportProblemKind kind, BookExport b, PageExport p, MacroExport m, string desc)
		{
			return new ImportProblem { Kind = kind, Book = b, Page = p, Macro = m, Description = desc };
		}

		// ===== Snippet (single macro) for the per-problem editor =====

		public static string MacroToSnippet(MacroExport m)
		{
			var sb = new StringBuilder();
			sb.AppendLine("slot: " + (m.Slot ?? ""));
			sb.AppendLine("title: " + (m.Title ?? ""));
			for (int i = 0; i < 6; i++)
			{
				string line = (m.Lines != null && i < m.Lines.Length) ? m.Lines[i] : "";
				sb.AppendLine((i + 1) + ": " + line);
			}
			return sb.ToString().TrimEnd('\r', '\n');
		}

		/// <summary>Parse an edited macro snippet back into a MacroExport. Sets RawLineCount to 6.</summary>
		public static MacroExport ParseMacroSnippet(string text)
		{
			var m = new MacroExport { Title = "", Slot = "", Lines = new string[] { "", "", "", "", "", "" }, RawLineCount = 6 };
			if (text == null) return m;
			var lines = text.Replace("\r\n", "\n").Replace("\r", "\n").Split('\n');
			foreach (string raw in lines)
			{
				string line = raw;
				string trimmed = line.TrimStart();
				if (trimmed.StartsWith("slot:", StringComparison.OrdinalIgnoreCase))
				{
					m.Slot = trimmed.Substring(5).Trim();
				}
				else if (trimmed.StartsWith("title:", StringComparison.OrdinalIgnoreCase))
				{
					m.Title = trimmed.Substring(6).TrimStart();
				}
				else if (trimmed.Length >= 2 && char.IsDigit(trimmed[0]) && trimmed[1] == ':')
				{
					int n = trimmed[0] - '0';
					if (n >= 1 && n <= 6)
					{
						string val = trimmed.Substring(2);
						if (val.StartsWith(" ")) val = val.Substring(1);
						m.Lines[n - 1] = val;
					}
				}
			}
			return m;
		}
	}
}
