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
        private List<string> gameHistory;

        private Button[] boardButtons;
        private const string STATS_FILE = "game_stats.txt";
        private const string HISTORY_FILE = "game_history.txt";

        public Form1()
        {
            InitializeComponent();
            InitializeGame();
            LoadStatistics();
            LoadGameHistory();
        }

        private void InitializeGame()
        {
            board = new char[9];
            humanSymbol = 'X';
            computerSymbol = 'O';
            gameOver = true;
            gameHistory = new List<string>();

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
                int index = i; // 重要：创建局部变量捕获
                boardButtons[i].Click += (s, e) => BoardButtonClick(index);
            }

            UpdateStatusLabel();
        }

        private void BoardButtonClick(int position)
        {
            if (gameOver || board[position] != '\0')
                return;

            // 玩家移动
            MakeMove(position, humanSymbol);
            boardButtons[position].Text = humanSymbol.ToString();
            boardButtons[position].Enabled = false;

            if (CheckWin(humanSymbol))
            {
                gameOver = true;
                humanWins++;
                gameHistory.Add($"{DateTime.Now}: 玩家获胜");
                MessageBox.Show("恭喜！你赢了！", "游戏结束", MessageBoxButtons.OK, MessageBoxIcon.Information);
                UpdateStatusLabel();
                SaveGameHistory();
                SaveStatistics();
                return;
            }

            if (CheckDraw())
            {
                gameOver = true;
                draws++;
                gameHistory.Add($"{DateTime.Now}: 平局");
                MessageBox.Show("平局！", "游戏结束", MessageBoxButtons.OK, MessageBoxIcon.Information);
                UpdateStatusLabel();
                SaveGameHistory();
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
                        board[i] = '\0'; // 撤销测试移动
                        break;
                    }
                    board[i] = '\0'; // 撤销测试移动
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
                            board[i] = '\0'; // 撤销测试移动
                            break;
                        }
                        board[i] = '\0'; // 撤销测试移动
                    }
                }
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
                MakeMove(move, computerSymbol);
                boardButtons[move].Text = computerSymbol.ToString();
                boardButtons[move].Enabled = false;

                if (CheckWin(computerSymbol))
                {
                    gameOver = true;
                    computerWins++;
                    gameHistory.Add($"{DateTime.Now}: 电脑获胜");
                    MessageBox.Show("电脑赢了！", "游戏结束", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    UpdateStatusLabel();
                    SaveGameHistory();
                    SaveStatistics();
                }
                else if (CheckDraw())
                {
                    gameOver = true;
                    draws++;
                    gameHistory.Add($"{DateTime.Now}: 平局");
                    MessageBox.Show("平局！", "游戏结束", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    UpdateStatusLabel();
                    SaveGameHistory();
                    SaveStatistics();
                }
            }
        }

        private void MakeMove(int position, char symbol)
        {
            board[position] = symbol;
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
            }

            this.humanFirst = humanFirst;
            gameOver = false;

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

        private void SaveGameHistory()
        {
            try
            {
                File.WriteAllLines(HISTORY_FILE, gameHistory);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"保存游戏历史时出错: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadGameHistory()
        {
            if (File.Exists(HISTORY_FILE))
            {
                try
                {
                    gameHistory = new List<string>(File.ReadAllLines(HISTORY_FILE));
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"加载游戏历史时出错: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
            string history = gameHistory.Count > 0 ?
                string.Join(Environment.NewLine, gameHistory) :
                "暂无游戏记录";

            MessageBox.Show(history, "游戏历史", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
                gameHistory.Clear();

                try
                {
                    if (File.Exists(STATS_FILE))
                        File.Delete(STATS_FILE);
                    if (File.Exists(HISTORY_FILE))
                        File.Delete(HISTORY_FILE);
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
            SaveGameHistory();
            Application.Exit();
        }
    }
}