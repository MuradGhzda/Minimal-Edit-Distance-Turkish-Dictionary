using System.Text;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Globalization;
using System.Text;
using System.Windows.Forms;

namespace MinimalEditDistanceUI
{
    public class Form1 : Form
    {
        // === MED costs ===
        private const int INS_COST = 1;
        private const int DEL_COST = 1;
        private const int SUB_COST = 1;

        // === UI controls for Part 1 ===
        private TextBox txtInput;
        private Button btnFind;
        private Button btnLoadFile;
        private ListBox lstSuggestions;
        private Label lblStatus;
        private Label lblInfo;
        private Label lblPart1Time;

        // === UI controls for Part 2 ===
        private TabControl tabControl;
        private TabPage tabPart1;
        private TabPage tabPart2;
        private TextBox txtSource;
        private TextBox txtTarget;
        private Button btnCalculateMED;
        private Label lblMEDResult;
        private DataGridView dgvMatrix;
        private ListBox lstOperations;
        private Label lblPart2Time;

        // === Export Button ===
        private Button btnExportResults;

        // === Data ===
        private List<string> vocabulary = new List<string>();
        private string currentVocabPath = null;

        // === Results storage for report ===
        private StringBuilder reportResults = new StringBuilder();

       public Form1()
{
    InitializeComponent();
    lblStatus.Text = "No vocabulary loaded. Please use 'Load Vocabulary...' button.";
    InitializeReport();
}




        private void InitializeComponent()
        {
            // === Tab Control ===
            this.tabControl = new TabControl();
            this.tabPart1 = new TabPage("Part 1: Word Suggestions");
            this.tabPart2 = new TabPage("Part 2: MED Transformation");

            // === FORM ===
            this.Text = "Minimal Edit Distance - Assignment 2";
            this.Width = 1200;
            this.Height = 780;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Font = new Font("Segoe UI", 10);

            // === Setup Tab Control ===
            this.tabControl.Left = 10;
            this.tabControl.Top = 10;
            this.tabControl.Width = 1160;
            this.tabControl.Height = 690;

            InitializePart1();
            InitializePart2();

            this.tabControl.TabPages.Add(this.tabPart1);
            this.tabControl.TabPages.Add(this.tabPart2);
            this.Controls.Add(this.tabControl);

            // === Export Button ===
            this.btnExportResults = new Button();
            this.btnExportResults.Left = 10;
            this.btnExportResults.Top = 710;
            this.btnExportResults.Width = 200;
            this.btnExportResults.Height = 35;
            this.btnExportResults.Text = "Export Results to File";
            this.btnExportResults.BackColor = Color.LightBlue;
            this.btnExportResults.Click += BtnExportResults_Click;
            this.Controls.Add(this.btnExportResults);
        }

        private void InitializePart1()
        {
            this.txtInput = new TextBox();
            this.btnFind = new Button();
            this.btnLoadFile = new Button();
            this.lstSuggestions = new ListBox();
            this.lblStatus = new Label();
            this.lblInfo = new Label();
            this.lblPart1Time = new Label();

            // === INPUT TEXTBOX ===
            this.txtInput.Left = 20;
            this.txtInput.Top = 20;
            this.txtInput.Width = 400;
            this.txtInput.Height = 30;
            this.txtInput.Font = new Font("Segoe UI", 12);

            // === FIND BUTTON ===
            this.btnFind.Left = 440;
            this.btnFind.Top = 18;
            this.btnFind.Width = 180;
            this.btnFind.Height = 35;
            this.btnFind.Text = "Find Suggestions";
            this.btnFind.Click += BtnFind_Click;

            // === LOAD FILE BUTTON ===
            this.btnLoadFile.Left = 640;
            this.btnLoadFile.Top = 18;
            this.btnLoadFile.Width = 200;
            this.btnLoadFile.Height = 35;
            this.btnLoadFile.Text = "Load Vocabulary...";
            this.btnLoadFile.Click += BtnLoadFile_Click;

            // === STATUS LABEL ===
            this.lblStatus.Left = 20;
            this.lblStatus.Top = 65;
            this.lblStatus.Width = 1000;
            this.lblStatus.Height = 25;

            // === SUGGESTION LIST ===
            this.lstSuggestions.Left = 20;
            this.lstSuggestions.Top = 100;
            this.lstSuggestions.Width = 1100;
            this.lstSuggestions.Height = 420;
            this.lstSuggestions.Font = new Font("Consolas", 11);

            // === INFO LABEL ===
            this.lblInfo.Left = 20;
            this.lblInfo.Top = 535;
            this.lblInfo.Width = 1000;
            this.lblInfo.Height = 25;

            // === TIME LABEL ===
            this.lblPart1Time.Left = 20;
            this.lblPart1Time.Top = 570;
            this.lblPart1Time.Width = 1000;
            this.lblPart1Time.Height = 25;
            this.lblPart1Time.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            this.lblPart1Time.ForeColor = Color.DarkGreen;

            // === Add controls to Part 1 Tab ===
            this.tabPart1.Controls.Add(this.txtInput);
            this.tabPart1.Controls.Add(this.btnFind);
            this.tabPart1.Controls.Add(this.btnLoadFile);
            this.tabPart1.Controls.Add(this.lstSuggestions);
            this.tabPart1.Controls.Add(this.lblStatus);
            this.tabPart1.Controls.Add(this.lblInfo);
            this.tabPart1.Controls.Add(this.lblPart1Time);
        }

        private void InitializePart2()
        {
            this.txtSource = new TextBox();
            this.txtTarget = new TextBox();
            this.btnCalculateMED = new Button();
            this.lblMEDResult = new Label();
            this.dgvMatrix = new DataGridView();
            this.lstOperations = new ListBox();
            this.lblPart2Time = new Label();

            Label lblSource = new Label();
            Label lblTarget = new Label();
            Label lblMatrixTitle = new Label();
            Label lblOperationsTitle = new Label();

            // === SOURCE WORD ===
            lblSource.Left = 20;
            lblSource.Top = 20;
            lblSource.Width = 120;
            lblSource.Text = "Source Word:";
            lblSource.Font = new Font("Segoe UI", 11, FontStyle.Bold);

            this.txtSource.Left = 150;
            this.txtSource.Top = 18;
            this.txtSource.Width = 250;
            this.txtSource.Height = 30;
            this.txtSource.Font = new Font("Segoe UI", 12);

            // === TARGET WORD ===
            lblTarget.Left = 430;
            lblTarget.Top = 20;
            lblTarget.Width = 120;
            lblTarget.Text = "Target Word:";
            lblTarget.Font = new Font("Segoe UI", 11, FontStyle.Bold);

            this.txtTarget.Left = 560;
            this.txtTarget.Top = 18;
            this.txtTarget.Width = 250;
            this.txtTarget.Height = 30;
            this.txtTarget.Font = new Font("Segoe UI", 12);

            // === CALCULATE BUTTON ===
            this.btnCalculateMED.Left = 840;
            this.btnCalculateMED.Top = 15;
            this.btnCalculateMED.Width = 180;
            this.btnCalculateMED.Height = 38;
            this.btnCalculateMED.Text = "Calculate MED";
            this.btnCalculateMED.Click += BtnCalculateMED_Click;

            // === RESULT LABEL ===
            this.lblMEDResult.Left = 20;
            this.lblMEDResult.Top = 65;
            this.lblMEDResult.Width = 1000;
            this.lblMEDResult.Height = 30;
            this.lblMEDResult.Font = new Font("Segoe UI", 12, FontStyle.Bold);
            this.lblMEDResult.ForeColor = Color.DarkBlue;

            // === TIME LABEL ===
            this.lblPart2Time.Left = 20;
            this.lblPart2Time.Top = 95;
            this.lblPart2Time.Width = 1000;
            this.lblPart2Time.Height = 25;
            this.lblPart2Time.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            this.lblPart2Time.ForeColor = Color.DarkGreen;

            // === MATRIX TITLE ===
            lblMatrixTitle.Left = 20;
            lblMatrixTitle.Top = 130;
            lblMatrixTitle.Width = 200;
            lblMatrixTitle.Height = 25;
            lblMatrixTitle.Text = "DP Matrix (with steps):";
            lblMatrixTitle.Font = new Font("Segoe UI", 10, FontStyle.Bold);

            // === DP MATRIX GRID ===
            this.dgvMatrix.Left = 20;
            this.dgvMatrix.Top = 160;
            this.dgvMatrix.Width = 700;
            this.dgvMatrix.Height = 420;
            this.dgvMatrix.AllowUserToAddRows = false;
            this.dgvMatrix.AllowUserToDeleteRows = false;
            this.dgvMatrix.ReadOnly = true;
            this.dgvMatrix.RowHeadersWidth = 80;
            this.dgvMatrix.DefaultCellStyle.Font = new Font("Consolas", 10);
            this.dgvMatrix.ColumnHeadersDefaultCellStyle.Font = new Font("Consolas", 10, FontStyle.Bold);

            // === OPERATIONS TITLE ===
            lblOperationsTitle.Left = 740;
            lblOperationsTitle.Top = 130;
            lblOperationsTitle.Width = 250;
            lblOperationsTitle.Height = 25;
            lblOperationsTitle.Text = "Operations (in order):";
            lblOperationsTitle.Font = new Font("Segoe UI", 10, FontStyle.Bold);

            // === OPERATIONS LIST ===
            this.lstOperations.Left = 740;
            this.lstOperations.Top = 160;
            this.lstOperations.Width = 380;
            this.lstOperations.Height = 420;
            this.lstOperations.Font = new Font("Consolas", 10);

            // === Add controls to Part 2 Tab ===
            this.tabPart2.Controls.Add(lblSource);
            this.tabPart2.Controls.Add(this.txtSource);
            this.tabPart2.Controls.Add(lblTarget);
            this.tabPart2.Controls.Add(this.txtTarget);
            this.tabPart2.Controls.Add(this.btnCalculateMED);
            this.tabPart2.Controls.Add(this.lblMEDResult);
            this.tabPart2.Controls.Add(this.lblPart2Time);
            this.tabPart2.Controls.Add(lblMatrixTitle);
            this.tabPart2.Controls.Add(this.dgvMatrix);
            this.tabPart2.Controls.Add(lblOperationsTitle);
            this.tabPart2.Controls.Add(this.lstOperations);
        }

        private void InitializeReport()
        {
            reportResults.AppendLine("=".PadRight(80, '='));
            reportResults.AppendLine("CME4408 - INTRODUCTION TO NATURAL LANGUAGE PROCESSING");
            reportResults.AppendLine("Assignment 2 - Test Results");
            reportResults.AppendLine("=".PadRight(80, '='));
            reportResults.AppendLine();
        }

        // ======================
        //  Load vocabulary file
        // ======================
        private void BtnLoadFile_Click(object sender, EventArgs e)
        {
            using (var ofd = new OpenFileDialog())
            {
                ofd.Title = "Select Turkish vocabulary file";
                ofd.Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*";

                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    currentVocabPath = ofd.FileName;
                    LoadVocabulary(currentVocabPath);
                }
            }
        }

        private void LoadVocabulary(string vocabFile)
{
    try
    {
        if (!File.Exists(vocabFile))
        {
            lblStatus.Text = $"File not found: {vocabFile}";
            MessageBox.Show($"Dictionary file '{vocabFile}' not found.",
                "File not found", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return;
        }

        string baseDir = Path.GetDirectoryName(vocabFile);
        string utf8Path = Path.Combine(baseDir, "vocabulary_tr_utf8.txt");

        if (!File.Exists(utf8Path))
        {
            var ansi1254 = Encoding.GetEncoding(1254);
            var utf8NoBom = new UTF8Encoding(false);

            var ansiLines = File.ReadAllLines(vocabFile, ansi1254);
            File.WriteAllLines(utf8Path, ansiLines, utf8NoBom);
        }

        vocabulary = File.ReadAllLines(utf8Path, Encoding.UTF8)
                         .Select(line => line.Trim())
                         .Where(line => !string.IsNullOrWhiteSpace(line))
                         .Distinct(StringComparer.CurrentCultureIgnoreCase)
                         .ToList();

        currentVocabPath = utf8Path;
        lblStatus.Text = $"Vocabulary loaded: {vocabulary.Count} words. ({Path.GetFileName(utf8Path)})";
    }
    catch (Exception ex)
    {
        lblStatus.Text = "Error loading vocabulary.";
        MessageBox.Show("Error loading vocabulary: " + ex.Message,
            "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
    }
}



        // ======================
        //  PART 1: Button click
        // ======================
        private void BtnFind_Click(object sender, EventArgs e)
        {
            string input = txtInput.Text.Trim();

            if (string.IsNullOrEmpty(input))
            {
                MessageBox.Show("Please enter a word.", "Warning",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (vocabulary == null || vocabulary.Count == 0)
            {
                MessageBox.Show("Please load a vocabulary file first.", "Warning",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Start timing
            Stopwatch stopwatch = Stopwatch.StartNew();

            var nearest = GetNearestWords(input, vocabulary, 5);
            
            stopwatch.Stop();
            double elapsedMs = stopwatch.Elapsed.TotalMilliseconds;

            // Display results
            lstSuggestions.Items.Clear();

            foreach (var (word, distance) in nearest)
            {
                lstSuggestions.Items.Add($"{word}    (MED = {distance})");
            }

            bool exists = vocabulary.Contains(input, StringComparer.CurrentCultureIgnoreCase);
            lblInfo.Text = exists
                ? $"'{input}' is in the dictionary.(MED = 0)"
                : $"'{input}' is NOT in the dictionary. Showing nearest words.";

            lblPart1Time.Text = $"Running Time: {elapsedMs:F2} ms ({elapsedMs / 1000:F4} seconds)";

            // Add to report
            reportResults.AppendLine($"PART 1 - Word: '{input}'");
            reportResults.AppendLine($"Running Time: {elapsedMs:F2} ms");
            reportResults.AppendLine("Top 5 Nearest Words:");
            int rank = 1;
            foreach (var (word, distance) in nearest)
            {
                reportResults.AppendLine($"  {rank}. {word} (MED = {distance})");
                rank++;
            }
            reportResults.AppendLine();
        }

        // ======================
        //  PART 2: Button click
        // ======================
        private void BtnCalculateMED_Click(object sender, EventArgs e)
        {
            string source = txtSource.Text.Trim();
            string target = txtTarget.Text.Trim();

            if (string.IsNullOrEmpty(source) || string.IsNullOrEmpty(target))
            {
                MessageBox.Show("Please enter both source and target words.", "Warning",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            Stopwatch stopwatch = Stopwatch.StartNew();

            var (distance, operations, matrix) = FindMinimumEditDistanceWithMatrix(source, target);

            stopwatch.Stop();
            double elapsedMs = stopwatch.Elapsed.TotalMilliseconds;

            lblMEDResult.Text = $"Minimum Edit Distance: {distance}  |  Source: '{source}'  →  Target: '{target}'";
            lblPart2Time.Text = $"Running Time: {elapsedMs:F2} ms ({elapsedMs / 1000:F6} seconds)";

            DisplayMatrix(matrix, source, target);
            DisplayOperations(operations);

            // Add to report
            reportResults.AppendLine($"PART 2 - Transformation");
            reportResults.AppendLine($"Source: '{source}' → Target: '{target}'");
            reportResults.AppendLine($"Minimum Edit Distance: {distance}");
            reportResults.AppendLine($"Running Time: {elapsedMs:F2} ms");
            reportResults.AppendLine("Operations:");
            int step = 1;
            foreach (var (op, c1, c2) in operations)
            {
                string description = "";
                if (op == "SUBSTITUTE")
                    description = $"  {step}. SUBSTITUTE '{c1}' → '{c2}'";
                else if (op == "INSERT")
                    description = $"  {step}. INSERT '{c1}'";
                else if (op == "DELETE")
                    description = $"  {step}. DELETE '{c1}'";
                
                reportResults.AppendLine(description);
                step++;
            }
            reportResults.AppendLine();
            reportResults.AppendLine("DP Matrix:");
            reportResults.AppendLine(MatrixToString(matrix, source, target));
            reportResults.AppendLine();
        }

        // ======================
        //  Export Results
        // ======================
        private void BtnExportResults_Click(object sender, EventArgs e)
        {
            using (var sfd = new SaveFileDialog())
            {
                sfd.Title = "Save Test Results";
                sfd.Filter = "Text files (*.txt)|*.txt";
                sfd.FileName = "Assignment2_TestResults.txt";

                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        File.WriteAllText(sfd.FileName, reportResults.ToString(), Encoding.UTF8);
                        MessageBox.Show($"Results exported successfully to:\n{sfd.FileName}",
                                      "Export Successful",
                                      MessageBoxButtons.OK,
                                      MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error exporting results: {ex.Message}",
                                      "Export Error",
                                      MessageBoxButtons.OK,
                                      MessageBoxIcon.Error);
                    }
                }
            }
        }

        // ========================================
        //  Get k nearest words using MED function
        // ========================================
        private List<(string word, int distance)> GetNearestWords(
            string inputWord,
            List<string> vocab,
            int k)
        {
            var results = new List<(string word, int distance)>();

            foreach (var dictWord in vocab)
            {
                if (string.IsNullOrWhiteSpace(dictWord))
                    continue;

                var (distance, _, _) = FindMinimumEditDistanceWithMatrix(inputWord, dictWord);
                if (distance == 0)
                    continue;
                results.Add((dictWord, distance));
            }

            return results
                .OrderBy(r => r.distance)
                .ThenBy(r => r.word)
                .Take(k)
                .ToList();
        }

        // ========================================
        //  MED ALGORITHM WITH MATRIX
        // ========================================
        static (int, List<(string, string, string)>, int[,]) FindMinimumEditDistanceWithMatrix(
            string sourceString, string targetString)
        {
            int[,] dp = new int[targetString.Length + 1, sourceString.Length + 1];

            for (int i = 1; i <= targetString.Length; i++)
            {
                dp[i, 0] = dp[i - 1, 0] + INS_COST;
            }
            for (int i = 1; i <= sourceString.Length; i++)
            {
                dp[0, i] = dp[0, i - 1] + DEL_COST;
            }

            List<(string operation, string char1, string char2)> operationsPerformed =
                new List<(string, string, string)>();

            for (int i = 1; i <= targetString.Length; i++)
            {
                for (int j = 1; j <= sourceString.Length; j++)
                {
                    if (sourceString[j - 1] == targetString[i - 1])
                    {
                        dp[i, j] = dp[i - 1, j - 1];
                    }
                    else
                    {
                        dp[i, j] = Math.Min(
                            dp[i - 1, j] + INS_COST,
                            Math.Min(
                                dp[i - 1, j - 1] + SUB_COST,
                                dp[i, j - 1] + DEL_COST));
                    }
                }
            }

            int row = targetString.Length;
            int col = sourceString.Length;

            while (row != 0 && col != 0)
            {
                if (targetString[row - 1] == sourceString[col - 1])
                {
                    row--;
                    col--;
                }
                else
                {
                    if (dp[row, col] == dp[row - 1, col - 1] + SUB_COST)
                    {
                        operationsPerformed.Add(("SUBSTITUTE",
                            sourceString[col - 1].ToString(),
                            targetString[row - 1].ToString()));
                        row--;
                        col--;
                    }
                    else if (dp[row, col] == dp[row - 1, col] + INS_COST)
                    {
                        operationsPerformed.Add(("INSERT",
                            targetString[row - 1].ToString(), ""));
                        row--;
                    }
                    else
                    {
                        operationsPerformed.Add(("DELETE",
                            sourceString[col - 1].ToString(), ""));
                        col--;
                    }
                }
            }

            while (col != 0)
            {
                operationsPerformed.Add(("DELETE", sourceString[col - 1].ToString(), ""));
                col--;
            }

            while (row != 0)
            {
                operationsPerformed.Add(("INSERT", targetString[row - 1].ToString(), ""));
                row--;
            }

            operationsPerformed.Reverse();
            return (dp[targetString.Length, sourceString.Length], operationsPerformed, dp);
        }

        // ========================================
        //  Display DP Matrix in DataGridView
        // ========================================
        private void DisplayMatrix(int[,] matrix, string source, string target)
        {
            dgvMatrix.Rows.Clear();
            dgvMatrix.Columns.Clear();

            dgvMatrix.Columns.Add("", "");
            dgvMatrix.Columns[0].Width = 60;

            for (int j = 0; j < source.Length; j++)
            {
                dgvMatrix.Columns.Add($"col{j}", source[j].ToString());
                dgvMatrix.Columns[j + 1].Width = 50;
            }

            // Get the shortest path
            var path = GetShortestPath(matrix, source, target);

            for (int i = 0; i <= target.Length; i++)
            {
                int rowIndex = dgvMatrix.Rows.Add();
                dgvMatrix.Rows[rowIndex].HeaderCell.Value = (i == 0) ? "" : target[i - 1].ToString();

                dgvMatrix.Rows[rowIndex].Cells[0].Value = matrix[i, 0];

                for (int j = 1; j <= source.Length; j++)
                {
                    dgvMatrix.Rows[rowIndex].Cells[j].Value = matrix[i, j];
                }
            }
            foreach (var (row, col) in path)
            {
                dgvMatrix.Rows[row].Cells[col].Style.BackColor = Color.Yellow;
                dgvMatrix.Rows[row].Cells[col].Style.Font = new Font("Consolas", 10, FontStyle.Bold);
            }

            // Highlight final cell
            dgvMatrix.Rows[target.Length].Cells[source.Length].Style.BackColor = Color.LightGreen;
            dgvMatrix.Rows[target.Length].Cells[source.Length].Style.Font = new Font("Consolas", 10, FontStyle.Bold);
        }

        // ========================================
        //  Get Shortest Path through matrix
        // ========================================
        private List<(int row, int col)> GetShortestPath(int[,] matrix, string source, string target)
{
    List<(int, int)> path = new List<(int, int)>();
    
    int row = target.Length;
    int col = source.Length;

    path.Add((row, col));

    while (row > 0 || col > 0)
    {
        if (row == 0)
        {
            col--;
            path.Add((row, col));
        }
        else if (col == 0)
        {
            row--;
            path.Add((row, col));
        }
        else if (source[col - 1] == target[row - 1])
        {
            row--;
            col--;
            path.Add((row, col));
        }
        else
        {
            int substitute = matrix[row - 1, col - 1];
            int insert = matrix[row - 1, col];
            int delete = matrix[row, col - 1];

            int minVal = Math.Min(substitute, Math.Min(insert, delete));

            if (minVal == substitute)
            {
                row--;
                col--;
            }
            else if (minVal == insert)
            {
                row--;
            }
            else
            {
                col--;
            }
            path.Add((row, col));
        }
    }

    path.Reverse();
    return path;  
}

     

        // ========================================
        //  Display Operations in ListBox
        // ========================================
        private void DisplayOperations(List<(string operation, string char1, string char2)> operations)
        {
            lstOperations.Items.Clear();

            if (operations.Count == 0)
            {
                lstOperations.Items.Add("No operations needed (words are identical).");
                return;
            }

            int stepNumber = 1;
            foreach (var (op, c1, c2) in operations)
            {
                string description = "";
                if (op == "SUBSTITUTE")
                {
                    description = $"{stepNumber}. SUBSTITUTE '{c1}' → '{c2}'";
                }
                else if (op == "INSERT")
                {
                    description = $"{stepNumber}. INSERT '{c1}'";
                }
                else if (op == "DELETE")
                {
                    description = $"{stepNumber}. DELETE '{c1}'";
                }

                lstOperations.Items.Add(description);
                stepNumber++;
            }

            lstOperations.Items.Add("");
            lstOperations.Items.Add($"Total operations: {operations.Count}");
        }

        // ========================================
        //  Matrix to String (for report)
        // ========================================
        private string MatrixToString(int[,] matrix, string source, string target)
        {
            StringBuilder sb = new StringBuilder();
            
            // Header row
            sb.Append("      ");
            for (int j = 0; j < source.Length; j++)
            {
                sb.Append($"{source[j],4}");
            }
            sb.AppendLine();

            // Matrix rows
            for (int i = 0; i <= target.Length; i++)
            {
                if (i == 0)
                    sb.Append("  ");
                else
                    sb.Append($"{target[i - 1]} ");

                sb.Append($"{matrix[i, 0],4}");

                for (int j = 1; j <= source.Length; j++)
                {
                    sb.Append($"{matrix[i, j],4}");
                }
                sb.AppendLine();
            }

            return sb.ToString();
        }
    }
}