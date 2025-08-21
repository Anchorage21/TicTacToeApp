using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace TicTacToeWinForms
{
    public partial class Form1 : Form
    {
        private char[] board;
        private char humanSymbol;
        private char computerSymbol;
        private bool humanFirst;
        private bool gameOver;
        private int humanWins;
        private int computerWins;
        private int draws;
        private List<string> moveHistory;

        private Button[] boardButtons;
        private const string STATS_FILE = "game_stats.txt";
        private const string HISTORY_DIR = "GameHistory";

        public Form1()
        {
            InitializeComponent();
            InitializeGame();
            LoadStatistics();
            EnsureHistoryDirectory();
        }

        private void InitializeGame()
        {
            board = new char[9];
            humanSymbol = 'X';
            computerSymbol = 'O';
            gameOver = true;
            moveHistory = new List<string>();

            // 关联棋盘按钮
            boardButtons = new Button[]
            {
                btnPos0, btnPos1, btnPos2,
                btnPos3, btnPos4, btnPos5,
                btnPos6, btnPos7, btnPos8
            };

            // 设置按钮事件
            for (int i = 0; i < boardButtons.Length; i++)
            {
                int index = i;
                boardButtons[i].Click += (s, e) => BoardButtonClick(index);
                boardButtons[i].Font = new Font("Arial", 20, FontStyle.Bold);
            }

            UpdateStatusLabel();
        }

        private void EnsureHistoryDirectory()
        {
            if (!Directory.Exists(HISTORY_DIR))
            {
                Directory.CreateDirectory(HISTORY_DIR);
            }
        }

        private void BoardButtonClick(int position)
        {
            if (gameOver || board[position] != '\0')
                return;

            // 玩家移动
            MakeMove(position, humanSymbol, true);
            boardButtons[position].Text = humanSymbol.ToString();
            boardButtons[position].Enabled = false;

            if (CheckWin(humanSymbol))
            {
                gameOver = true;
                humanWins++;
                moveHistory.Add($"玩家获胜 - {DateTime.Now}");
                SaveGameResult("玩家获胜");
                HighlightWinningCells();
                MessageBox.Show("恭喜！你赢了！", "游戏结束", MessageBoxButtons.OK, MessageBoxIcon.Information);
                UpdateStatusLabel();
                SaveStatistics();
                return;
            }

            if (CheckDraw())
            {
                gameOver = true;
                draws++;
                moveHistory.Add($"平局 - {DateTime.Now}");
                SaveGameResult("平局");
                MessageBox.Show("平局！", "游戏结束", MessageBoxButtons.OK, MessageBoxIcon.Information);
                UpdateStatusLabel();
                SaveStatistics();
                return;
            }

            // 电脑移动
            ComputerMove();
        }

        private void ComputerMove()
        {
            if (gameOver) return;

            // 简单的AI：先尝试赢，再阻止玩家赢，否则随机
            int move = -1;

            // 尝试赢
            for (int i = 0; i < 9; i++)
            {
                if (board[i] == '\0')
                {
                    board[i] = computerSymbol;
                    if (CheckWin(computerSymbol))
                    {
                        move = i;
                        board[i] = '\0';
                        break;
                    }
                    board[i] = '\0';
                }
            }

            // 阻止玩家赢
            if (move == -1)
            {
                for (int i = 0; i < 9; i++)
                {
                    if (board[i] == '\0')
                    {
                        board[i] = humanSymbol;
                        if (CheckWin(humanSymbol))
                        {
                            move = i;
                            board[i] = '\0';
                            break;
                        }
                        board[i] = '\0';
                    }
                }
            }

            // 占中
            if (move == -1 && board[4] == '\0')
            {
                move = 4;
            }

            // 随机移动
            if (move == -1)
            {
                Random rand = new Random();
                List<int> emptyPositions = new List<int>();
                for (int i = 0; i < 9; i++)
                {
                    if (board[i] == '\0')
                        emptyPositions.Add(i);
                }

                if (emptyPositions.Count > 0)
                    move = emptyPositions[rand.Next(emptyPositions.Count)];
            }

            if (move != -1)
            {
                MakeMove(move, computerSymbol, false);
                boardButtons[move].Text = computerSymbol.ToString();
                boardButtons[move].Enabled = false;

                if (CheckWin(computerSymbol))
                {
                    gameOver = true;
                    computerWins++;
                    moveHistory.Add($"电脑获胜 - {DateTime.Now}");
                    SaveGameResult("电脑获胜");
                    HighlightWinningCells();
                    MessageBox.Show("电脑赢了！", "游戏结束", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    UpdateStatusLabel();
                    SaveStatistics();
                }
                else if (CheckDraw())
                {
                    gameOver = true;
                    draws++;
                    moveHistory.Add($"平局 - {DateTime.Now}");
                    SaveGameResult("平局");
                    MessageBox.Show("平局！", "游戏结束", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    UpdateStatusLabel();
                    SaveStatistics();
                }
            }
        }

        private void MakeMove(int position, char symbol, bool isHuman)
        {
            board[position] = symbol;
            string player = isHuman ? "玩家" : "电脑";
            moveHistory.Add($"{player}在位置 {position} 放置 {symbol}");
        }

        private bool CheckWin(char symbol)
        {
            // 检查行
            for (int i = 0; i < 3; i++)
            {
                if (board[i * 3] == symbol && board[i * 3 + 1] == symbol && board[i * 3 + 2] == symbol)
                    return true;
            }

            // 检查列
            for (int i = 0; i < 3; i++)
            {
                if (board[i] == symbol && board[i + 3] == symbol && board[i + 6] == symbol)
                    return true;
            }

            // 检查对角线
            if (board[0] == symbol && board[4] == symbol && board[8] == symbol)
                return true;

            if (board[2] == symbol && board[4] == symbol && board[6] == symbol)
                return true;

            return false;
        }

        private void HighlightWinningCells()
        {
            // 检查行
            for (int i = 0; i < 3; i++)
            {
                if (board[i * 3] != '\0' && board[i * 3] == board[i * 3 + 1] && board[i * 3] == board[i * 3 + 2])
                {
                    boardButtons[i * 3].BackColor = Color.LightGreen;
                    boardButtons[i * 3 + 1].BackColor = Color.LightGreen;
                    boardButtons[i * 3 + 2].BackColor = Color.LightGreen;
                    return;
                }
            }

            // 检查列
            for (int i = 0; i < 3; i++)
            {
                if (board[i] != '\0' && board[i] == board[i + 3] && board[i] == board[i + 6])
                {
                    boardButtons[i].BackColor = Color.LightGreen;
                    boardButtons[i + 3].BackColor = Color.LightGreen;
                    boardButtons[i + 6].BackColor = Color.LightGreen;
                    return;
                }
            }

            // 检查对角线
            if (board[0] != '\0' && board[0] == board[4] && board[0] == board[8])
            {
                boardButtons[0].BackColor = Color.LightGreen;
                boardButtons[4].BackColor = Color.LightGreen;
                boardButtons[8].BackColor = Color.LightGreen;
                return;
            }

            if (board[2] != '\0' && board[2] == board[4] && board[2] == board[6])
            {
                boardButtons[2].BackColor = Color.LightGreen;
                boardButtons[4].BackColor = Color.LightGreen;
                boardButtons[6].BackColor = Color.LightGreen;
            }
        }

        private bool CheckDraw()
        {
            return board.All(cell => cell != '\0');
        }

        private void NewGame(bool humanFirst)
        {
            // 重置棋盘
            for (int i = 0; i < 9; i++)
            {
                board[i] = '\0';
                boardButtons[i].Text = "";
                boardButtons[i].Enabled = true;
                boardButtons[i].BackColor = SystemColors.Control;
            }

            this.humanFirst = humanFirst;
            gameOver = false;
            moveHistory.Clear();

            moveHistory.Add($"新游戏开始 - {DateTime.Now}");
            moveHistory.Add($"先手: {(humanFirst ? "玩家" : "电脑")}");

            if (!humanFirst)
            {
                ComputerMove();
            }

            UpdateStatusLabel();
        }

        private void UpdateStatusLabel()
        {
            lblStatus.Text = $"玩家: {humanWins} 胜 | 电脑: {computerWins} 胜 | 平局: {draws}";
        }

        private void SaveGameResult(string result)
        {
            try
            {
                string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
                string filename = Path.Combine(HISTORY_DIR, $"game_{timestamp}.txt");

                using (StreamWriter writer = new StreamWriter(filename))
                {
                    writer.WriteLine($"井字棋游戏记录 - {DateTime.Now}");
                    writer.WriteLine("====================");
                    writer.WriteLine($"结果: {result}");
                    writer.WriteLine($"先手: {(humanFirst ? "玩家" : "电脑")}");
                    writer.WriteLine();
                    writer.WriteLine("走子记录:");
                    writer.WriteLine("---------");

                    foreach (string move in moveHistory)
                    {
                        writer.WriteLine(move);
                    }

                    writer.WriteLine();
                    writer.WriteLine("最终棋盘:");
                    writer.WriteLine("---------");
                    writer.WriteLine($" {board[0]} | {board[1]} | {board[2]} ");
                    writer.WriteLine("-----------");
                    writer.WriteLine($" {board[3]} | {board[4]} | {board[5]} ");
                    writer.WriteLine("-----------");
                    writer.WriteLine($" {board[6]} | {board[7]} | {board[8]} ");
                }

                MessageBox.Show($"游戏记录已保存到: {filename}", "记录保存", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"保存游戏记录时出错: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void SaveStatistics()
        {
            try
            {
                using (StreamWriter writer = new StreamWriter(STATS_FILE))
                {
                    writer.WriteLine($"HumanWins:{humanWins}");
                    writer.WriteLine($"ComputerWins:{computerWins}");
                    writer.WriteLine($"Draws:{draws}");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"保存统计数据时出错: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadStatistics()
        {
            if (File.Exists(STATS_FILE))
            {
                try
                {
                    string[] lines = File.ReadAllLines(STATS_FILE);
                    foreach (string line in lines)
                    {
                        string[] parts = line.Split(':');
                        if (parts.Length == 2)
                        {
                            switch (parts[0])
                            {
                                case "HumanWins":
                                    int.TryParse(parts[1], out humanWins);
                                    break;
                                case "ComputerWins":
                                    int.TryParse(parts[1], out computerWins);
                                    break;
                                case "Draws":
                                    int.TryParse(parts[1], out draws);
                                    break;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"加载统计数据时出错: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void btnNewGamePlayerFirst_Click(object sender, EventArgs e)
        {
            NewGame(true);
        }

        private void btnNewGameComputerFirst_Click(object sender, EventArgs e)
        {
            NewGame(false);
        }

        private void btnShowHistory_Click(object sender, EventArgs e)
        {
            HistoryForm historyForm = new HistoryForm(HISTORY_DIR);
            historyForm.ShowDialog();
        }

        private void btnShowStats_Click(object sender, EventArgs e)
        {
            MessageBox.Show(
                $"玩家获胜: {humanWins} 次\n电脑获胜: {computerWins} 次\n平局: {draws} 次",
                "游戏统计",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information
            );
        }

        private void btnResetStats_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show(
                "确定要重置所有统计数据吗？",
                "确认重置",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question
            );

            if (result == DialogResult.Yes)
            {
                humanWins = 0;
                computerWins = 0;
                draws = 0;

                try
                {
                    if (File.Exists(STATS_FILE))
                        File.Delete(STATS_FILE);

                    // 不清除历史记录文件，只重置统计
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"删除统计文件时出错: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

                UpdateStatusLabel();
                MessageBox.Show("统计数据已重置", "信息", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            SaveStatistics();
            Application.Exit();
        }
    }

    // 历史记录查看窗体
    public class HistoryForm : Form
    {
        private ListBox listBoxGames;
        private TextBox textBoxContent;
        private Button btnClose;
        private string historyDir;

        public HistoryForm(string historyDir)
        {
            this.historyDir = historyDir;
            InitializeComponent();
            LoadGameFiles();
        }

        private void InitializeComponent()
        {
            this.Text = "游戏历史记录";
            this.Size = new Size(600, 500);
            this.StartPosition = FormStartPosition.CenterParent;

            // 列表框显示游戏文件
            listBoxGames = new ListBox();
            listBoxGames.Location = new Point(10, 10);
            listBoxGames.Size = new Size(200, 400);
            listBoxGames.SelectedIndexChanged += ListBoxGames_SelectedIndexChanged;
            this.Controls.Add(listBoxGames);

            // 文本框显示文件内容
            textBoxContent = new TextBox();
            textBoxContent.Location = new Point(220, 10);
            textBoxContent.Size = new Size(360, 400);
            textBoxContent.Multiline = true;
            textBoxContent.ScrollBars = ScrollBars.Vertical;
            textBoxContent.ReadOnly = true;
            this.Controls.Add(textBoxContent);

            // 关闭按钮
            btnClose = new Button();
            btnClose.Text = "关闭";
            btnClose.Location = new Point(250, 420);
            btnClose.Size = new Size(100, 30);
            btnClose.Click += (s, e) => this.Close();
            this.Controls.Add(btnClose);
        }

        private void LoadGameFiles()
        {
            if (Directory.Exists(historyDir))
            {
                var files = Directory.GetFiles(historyDir, "game_*.txt")
                                    .OrderByDescending(f => f)
                                    .ToArray();

                foreach (string file in files)
                {
                    listBoxGames.Items.Add(Path.GetFileName(file));
                }
            }
        }

        private void ListBoxGames_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBoxGames.SelectedIndex >= 0)
            {
                string filename = Path.Combine(historyDir, listBoxGames.SelectedItem.ToString());
                try
                {
                    textBoxContent.Text = File.ReadAllText(filename);
                }
                catch (Exception ex)
                {
                    textBoxContent.Text = $"读取文件时出错: {ex.Message}";
                }
            }
        }
    }
}