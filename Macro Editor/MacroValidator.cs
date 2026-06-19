using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;


namespace MacroEditor
{
    public enum ValidationSeverity { Error, Warning }

    public class ValidationResult
    {
        public int BookIndex { get; set; }
        public int RowIndex { get; set; }
        public int MacroIndex { get; set; }
        public int LineIndex { get; set; }  // 0 = title, 1-6 = lines
        public ValidationSeverity Severity { get; set; }
        public int IType { get; set; }  // category index for backward compat with Assessment display
        public string Message { get; set; }
        public string Content { get; set; }
    }

    public class MacroValidator
    {
        private AutoTranslateEncoder atEncoder;

        public MacroValidator(AutoTranslateEncoder encoder)
        {
            this.atEncoder = encoder;
        }

        /// <summary>
        /// Validates all macros and returns errors and warnings.
        /// </summary>
        public List<ValidationResult> ValidateAll(List<MacroBook> books, int debuglimit)
        {
            var results = new List<ValidationResult>();

            checked
            {
                for (int i = 0; i <= debuglimit; i++)
                {
                    int num2 = 0;
                    do
                    {
                        int num3 = 0;
                        do
                        {
                            // Check title length > 8
                            bool flag = books[i].Rows[num2].Macros[num3][0].Length > 8;
                            if (flag)
                            {
                                results.Add(new ValidationResult
                                {
                                    BookIndex = i,
                                    RowIndex = num2,
                                    MacroIndex = num3,
                                    LineIndex = 0,
                                    Severity = ValidationSeverity.Error,
                                    IType = 1,
                                    Message = "8 characters max.",
                                    Content = books[i].Rows[num2].Macros[num3][0]
                                });
                            }

                            // Check lines 1-6
                            int num4 = 1;
                            do
                            {
                                string text = books[i].Rows[num2].Macros[num3][num4];
                                string text2 = this.atEncoder.Encode(text);
                                Match match = new Regex("[^\\x00-\\x7f]").Match(text);

                                bool flag2 = text.Length == 0;
                                if (!flag2)
                                {
                                    // Line length after AT encoding > 60
                                    bool flag3 = text2.Length > 60;
                                    if (flag3)
                                    {
                                        results.Add(new ValidationResult
                                        {
                                            BookIndex = i,
                                            RowIndex = num2,
                                            MacroIndex = num3,
                                            LineIndex = num4,
                                            Severity = ValidationSeverity.Error,
                                            IType = 2,
                                            Message = "After auto-translate, the line is " + text2.Length.ToString() + " characters.",
                                            Content = text
                                        });
                                    }
                                    else
                                    {
                                        // Non-ASCII characters
                                        bool flag4 = match.Length != 0;
                                        if (flag4)
                                        {
                                            results.Add(new ValidationResult
                                            {
                                                BookIndex = i,
                                                RowIndex = num2,
                                                MacroIndex = num3,
                                                LineIndex = num4,
                                                Severity = ValidationSeverity.Warning,
                                                IType = 3,
                                                Message = "Invalid characters",
                                                Content = text
                                            });
                                        }
                                        else
                                        {
                                            // Starts with "//"
                                            bool flag5 = text.StartsWith("//");
                                            if (flag5)
                                            {
                                                results.Add(new ValidationResult
                                                {
                                                    BookIndex = i,
                                                    RowIndex = num2,
                                                    MacroIndex = num3,
                                                    LineIndex = num4,
                                                    Severity = ValidationSeverity.Warning,
                                                    IType = 4,
                                                    Message = "Starts with '//'",
                                                    Content = text
                                                });
                                            }
                                            else
                                            {
                                                // Doesn't start with "/"
                                                bool flag6 = !text.StartsWith("/");
                                                if (flag6)
                                                {
                                                    results.Add(new ValidationResult
                                                    {
                                                        BookIndex = i,
                                                        RowIndex = num2,
                                                        MacroIndex = num3,
                                                        LineIndex = num4,
                                                        Severity = ValidationSeverity.Warning,
                                                        IType = 5,
                                                        Message = "Doesn't start with '/'",
                                                        Content = text
                                                    });
                                                }
                                                else
                                                {
                                                    // Starts with "/wait"
                                                    bool flag7 = text.StartsWith("/wait");
                                                    if (flag7)
                                                    {
                                                        results.Add(new ValidationResult
                                                        {
                                                            BookIndex = i,
                                                            RowIndex = num2,
                                                            MacroIndex = num3,
                                                            LineIndex = num4,
                                                            Severity = ValidationSeverity.Warning,
                                                            IType = 6,
                                                            Message = "Old Style /wait",
                                                            Content = text
                                                        });
                                                    }
                                                    else
                                                    {
                                                        // Contains "|UnknownItem>"
                                                        bool flag8 = text.Contains("|UnknownItem>");
                                                        if (flag8)
                                                        {
                                                            results.Add(new ValidationResult
                                                            {
                                                                BookIndex = i,
                                                                RowIndex = num2,
                                                                MacroIndex = num3,
                                                                LineIndex = num4,
                                                                Severity = ValidationSeverity.Warning,
                                                                IType = 7,
                                                                Message = "Item Auto-Translate",
                                                                Content = text
                                                            });
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                                num4++;
                            }
                            while (num4 <= 6);
                            num3++;
                        }
                        while (num3 <= 19);
                        num2++;
                    }
                    while (num2 <= 9);
                }
            }

            return results;
        }
    }
}
