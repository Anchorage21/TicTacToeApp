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
        private char currentPlayer;
        private char humanSymbol;
        private char computerSymbol;
        private bool gameOver;
        private int humanWins;
        private int computerWins;
        private int draws;
        private int pvpWinsPlayer1;
        private int pvpWinsPlayer2;
        private List<string> moveHistory;
        private Stack<Move> moveStack; // 用于悔棋功能
        private int timeLimit;
        private int remainingTime;
        private System.Windows.Forms.Timer gameTimer;
        private GameMode gameMode;

        private Button[] boardButtons;
        private const string STATS_FILE = "game_stats.txt";
        private const string HISTORY_DIR = "GameHistory";

        private enum GameMode
        {
            HumanVsComputer,
            PlayerVsPlayer
        }

        private struct Move
        {
            public int Position;
            public char Symbol;
            public Move(int pos, char sym)
            {
                Position = pos;
                Symbol = sym;
            }
        }

        public Form1()
        {
            InitializeComponent();
            InitializeGame();
            InitializeTimer();
            LoadStatistics();
            EnsureHistoryDirectory();
        }

        private void InitializeGame()
        {
            board = new char[9];
            humanSymbol = 'X';
            computerSymbol = 'O';
            currentPlayer = humanSymbol;
            gameOver = true;
            moveHistory = new List<string>();
            moveStack = new Stack<Move>();
            timeLimit = 30;
            remainingTime = timeLimit;
            gameMode = GameMode.HumanVsComputer;

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
            UpdateTimerLabel();
            UpdateGameModeDisplay();
        }

        private void InitializeTimer()
        {
            gameTimer = new System.Windows.Forms.Timer();
            gameTimer.Interval = 1000;
            gameTimer.Tick += GameTimer_Tick;
        }

        private void GameTimer_Tick(object sender, EventArgs e)
        {
            if (gameOver) return;

            remainingTime--;
            UpdateTimerLabel();

            if (remainingTime <= 0)
            {
                gameTimer.Stop();
                TimeOut();
            }
        }

        private void TimeOut()
        {
            gameOver = true;

            if (gameMode == GameMode.HumanVsComputer)
            {
                computerWins++;
                moveHistory.Add($"玩家超时判负 - {DateTime.Now}");
                SaveGameResult("玩家超时判负");
                MessageBox.Show("时间到！玩家超时判负！", "游戏结束", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                // PVP模式下，超时的一方判负
                char winner = currentPlayer == 'X' ? 'O' : 'X';
                if (winner == 'X')
                {
                    pvpWinsPlayer1++;
                }
                else
                {
                    pvpWinsPlayer2++;
                }

                moveHistory.Add($"玩家 {(currentPlayer == 'X' ? "2" : "1")} 超时判负 - {DateTime.Now}");
                SaveGameResult($"玩家 {(currentPlayer == 'X' ? "2" : "1")} 超时判负");
                MessageBox.Show($"时间到！玩家 {(currentPlayer == 'X' ? "2" : "1")} 超时判负！", "游戏结束", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

            UpdateStatusLabel();
            SaveStatistics();
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

            // 保存移动以便悔棋
            moveStack.Push(new Move(position, board[position]));
            btnUndo.Enabled = true;

            // 重置计时器
            ResetTimer();

            // 当前玩家移动
            MakeMove(position, currentPlayer);
            boardButtons[position].Text = currentPlayer.ToString();
            boardButtons[position].Enabled = false;

            if (CheckWin(currentPlayer))
            {
                gameOver = true;
                gameTimer.Stop();

                if (gameMode == GameMode.HumanVsComputer)
                {
                    if (currentPlayer == humanSymbol)
                    {
                        humanWins++;
                        moveHistory.Add($"玩家获胜 - {DateTime.Now}");
                        SaveGameResult("玩家获胜");
                        MessageBox.Show("恭喜！你赢了！", "游戏结束", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        computerWins++;
                        moveHistory.Add($"电脑获胜 - {DateTime.Now}");
                        SaveGameResult("电脑获胜");
                        MessageBox.Show("电脑赢了！", "游戏结束", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
                else
                {
                    // PVP模式
                    if (currentPlayer == 'X')
                    {
                        pvpWinsPlayer1++;
                        moveHistory.Add($"玩家1获胜 - {DateTime.Now}");
                        SaveGameResult("玩家1获胜");
                        MessageBox.Show("玩家1获胜！", "游戏结束", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        pvpWinsPlayer2++;
                        moveHistory.Add($"玩家2获胜 - {DateTime.Now}");
                        SaveGameResult("玩家2获胜");
                        MessageBox.Show("玩家2获胜！", "游戏结束", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }

                HighlightWinningCells();
                UpdateStatusLabel();
                SaveStatistics();
                return;
            }

            if (CheckDraw())
            {
                gameOver = true;
                gameTimer.Stop();
                draws++;
                moveHistory.Add($"平局 - {DateTime.Now}");
                SaveGameResult("平局");
                MessageBox.Show("平局！", "游戏结束", MessageBoxButtons.OK, MessageBoxIcon.Information);
                UpdateStatusLabel();
                SaveStatistics();
                return;
            }

            // 切换玩家
            SwitchPlayer();

            // 如果是人机模式且当前是电脑的回合，则让电脑移动
            if (gameMode == GameMode.HumanVsComputer && currentPlayer == computerSymbol)
            {
                ComputerMove();
            }
        }

        private void ComputerMove()
        {
            if (gameOver) return;

            // 暂停计时器（电脑思考不计时）
            gameTimer.Stop();

            int move = FindBestMove();

            if (move != -1)
            {
                // 保存移动以便悔棋
                moveStack.Push(new Move(move, board[move]));
                btnUndo.Enabled = true;

                MakeMove(move, computerSymbol);
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
                else
                {
                    // 电脑移动后，切换回玩家并重新启动计时器
                    SwitchPlayer();
                    ResetTimer();
                }
            }
        }

        private int FindBestMove()
        {
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

            return move;
        }

        private void MakeMove(int position, char symbol)
        {
            board[position] = symbol;
            string player = symbol == humanSymbol ? "玩家" : "电脑";
            if (gameMode == GameMode.PlayerVsPlayer)
            {
                player = symbol == 'X' ? "玩家1" : "玩家2";
            }
            moveHistory.Add($"{player}在位置 {position} 放置 {symbol}");
        }

        private void SwitchPlayer()
        {
            currentPlayer = currentPlayer == 'X' ? 'O' : 'X';
            UpdateCurrentPlayerDisplay();
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

            currentPlayer = humanFirst ? 'X' : 'O';
            gameOver = false;
            moveHistory.Clear();
            moveStack.Clear();
            btnUndo.Enabled = false;

            moveHistory.Add($"新游戏开始 - {DateTime.Now}");
            moveHistory.Add($"模式: {((gameMode == GameMode.HumanVsComputer) ? "人机对战" : "玩家对战")}");
            moveHistory.Add($"先手: {(currentPlayer == 'X' ? (gameMode == GameMode.HumanVsComputer ? "玩家" : "玩家1") : (gameMode == GameMode.HumanVsComputer ? "电脑" : "玩家2"))}");
            moveHistory.Add($"时间限制: {timeLimit}秒");

            // 重置计时器
            ResetTimer();

            UpdateStatusLabel();
            UpdateCurrentPlayerDisplay();

            // 如果是人机模式且电脑先手，则让电脑移动
            if (gameMode == GameMode.HumanVsComputer && !humanFirst)
            {
                ComputerMove();
            }
        }

        private void ResetTimer()
        {
            remainingTime = timeLimit;
            UpdateTimerLabel();
            gameTimer.Stop();
            if (!gameOver) gameTimer.Start();
        }

        private void UpdateStatusLabel()
        {
            if (gameMode == GameMode.HumanVsComputer)
            {
                lblStatus.Text = $"玩家: {humanWins} 胜 | 电脑: {computerWins} 胜 | 平局: {draws}";
            }
            else
            {
                lblStatus.Text = $"玩家1: {pvpWinsPlayer1} 胜 | 玩家2: {pvpWinsPlayer2} 胜 | 平局: {draws}";
            }
        }

        private void UpdateTimerLabel()
        {
            lblTimer.Text = $"剩余时间: {remainingTime}秒";

            // 根据剩余时间改变颜色
            if (remainingTime <= 10)
            {
                lblTimer.ForeColor = Color.Red;
            }
            else if (remainingTime <= 20)
            {
                lblTimer.ForeColor = Color.Orange;
            }
            else
            {
                lblTimer.ForeColor = Color.Black;
            }
        }

        private void UpdateCurrentPlayerDisplay()
        {
            if (gameMode == GameMode.HumanVsComputer)
            {
                lblCurrentPlayer.Text = $"当前: {(currentPlayer == 'X' ? "玩家" : "电脑")}";
                lblCurrentPlayer.ForeColor = currentPlayer == 'X' ? Color.Blue : Color.Red;
            }
            else
            {
                lblCurrentPlayer.Text = $"当前: {(currentPlayer == 'X' ? "玩家1" : "玩家2")}";
                lblCurrentPlayer.ForeColor = currentPlayer == 'X' ? Color.Blue : Color.Red;
            }
        }

        private void UpdateGameModeDisplay()
        {
            lblGameMode.Text = $"模式: {(gameMode == GameMode.HumanVsComputer ? "人机对战" : "玩家对战")}";
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
                    writer.WriteLine($"模式: {((gameMode == GameMode.HumanVsComputer) ? "人机对战" : "玩家对战")}");
                    writer.WriteLine($"结果: {result}");
                    writer.WriteLine($"先手: {(currentPlayer == 'X' ? (gameMode == GameMode.HumanVsComputer ? "玩家" : "玩家1") : (gameMode == GameMode.HumanVsComputer ? "电脑" : "玩家2"))}");
                    writer.WriteLine($"时间限制: {timeLimit}秒");
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
                    writer.WriteLine($" {GetBoardSymbol(board[0])} | {GetBoardSymbol(board[1])} | {GetBoardSymbol(board[2])} ");
                    writer.WriteLine("-----------");
                    writer.WriteLine($" {GetBoardSymbol(board[3])} | {GetBoardSymbol(board[4])} | {GetBoardSymbol(board[5])} ");
                    writer.WriteLine("-----------");
                    writer.WriteLine($" {GetBoardSymbol(board[6])} | {GetBoardSymbol(board[7])} | {GetBoardSymbol(board[8])} ");
                }

                MessageBox.Show($"游戏记录已保存到: {filename}", "记录保存", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"保存游戏记录时出错: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private string GetBoardSymbol(char cell)
        {
            return cell == '\0' ? " " : cell.ToString();
        }

        private void SaveStatistics()
        {
            try
            {
                using (StreamWriter writer = new StreamWriter(STATS_FILE))
                {
                    writer.WriteLine($"HumanWins:{humanWins}");
                    writer.WriteLine($"ComputerWins:{computerWins}");
                    writer.WriteLine($"PVPPlayer1Wins:{pvpWinsPlayer1}");
                    writer.WriteLine($"PVPPlayer2Wins:{pvpWinsPlayer2}");
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
                                case "PVPPlayer1Wins":
                                    int.TryParse(parts[1], out pvpWinsPlayer1);
                                    break;
                                case "PVPPlayer2Wins":
                                    int.TryParse(parts[1], out pvpWinsPlayer2);
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
            if (gameMode == GameMode.HumanVsComputer)
            {
                NewGame(false);
            }
            else
            {
                // PVP模式下，第二个玩家先手
                NewGame(false);
            }
        }

        private void btnShowHistory_Click(object sender, EventArgs e)
        {
            HistoryForm historyForm = new HistoryForm(HISTORY_DIR);
            historyForm.ShowDialog();
        }

        private void btnShowStats_Click(object sender, EventArgs e)
        {
            if (gameMode == GameMode.HumanVsComputer)
            {
                MessageBox.Show(
                    $"玩家获胜: {humanWins} 次\n电脑获胜: {computerWins} 次\n平局: {draws} 次",
                    "游戏统计",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );
            }
            else
            {
                MessageBox.Show(
                    $"玩家1获胜: {pvpWinsPlayer1} 次\n玩家2获胜: {pvpWinsPlayer2} 次\n平局: {draws} 次",
                    "游戏统计",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );
            }
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
                pvpWinsPlayer1 = 0;
                pvpWinsPlayer2 = 0;
                draws = 0;

                try
                {
                    if (File.Exists(STATS_FILE))
                        File.Delete(STATS_FILE);
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

        private void btnSettings_Click(object sender, EventArgs e)
        {
            SettingsForm settingsForm = new SettingsForm(timeLimit);
            if (settingsForm.ShowDialog() == DialogResult.OK)
            {
                timeLimit = settingsForm.TimeLimit;
                if (!gameOver)
                {
                    ResetTimer();
                }
            }
        }

        private void btnToggleMode_Click(object sender, EventArgs e)
        {
            // 切换游戏模式
            gameMode = gameMode == GameMode.HumanVsComputer ? GameMode.PlayerVsPlayer : GameMode.HumanVsComputer;
            UpdateGameModeDisplay();
            UpdateStatusLabel();

            // 更新按钮文本
            if (gameMode == GameMode.HumanVsComputer)
            {
                btnNewGameComputerFirst.Text = "新游戏 (电脑先手)";
            }
            else
            {
                btnNewGameComputerFirst.Text = "新游戏 (玩家2先手)";
            }
        }

        private void btnUndo_Click(object sender, EventArgs e)
        {
            if (gameOver || moveStack.Count == 0) return;

            // 悔棋操作
            Move lastMove = moveStack.Pop();
            board[lastMove.Position] = lastMove.Symbol;
            boardButtons[lastMove.Position].Text = lastMove.Symbol == '\0' ? "" : lastMove.Symbol.ToString();
            boardButtons[lastMove.Position].Enabled = true;

            // 切换回上一个玩家
            SwitchPlayer();

            // 更新悔棋按钮状态
            btnUndo.Enabled = moveStack.Count > 0;

            // 重置计时器
            ResetTimer();

            // 记录悔棋操作
            moveHistory.Add($"悔棋: 位置 {lastMove.Position} 恢复为 {(lastMove.Symbol == '\0' ? "空" : lastMove.Symbol.ToString())}");
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

    // 设置窗体
    public class SettingsForm : Form
    {
        private NumericUpDown numTimeLimit;
        private Button btnOK;
        private Button btnCancel;

        public int TimeLimit { get; private set; }

        public SettingsForm(int currentTimeLimit)
        {
            TimeLimit = currentTimeLimit;
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.Text = "游戏设置";
            this.Size = new Size(300, 150);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            // 时间限制设置
            Label lblTimeLimit = new Label();
            lblTimeLimit.Text = "时间限制 (秒):";
            lblTimeLimit.Location = new Point(20, 20);
            lblTimeLimit.Size = new Size(100, 20);
            this.Controls.Add(lblTimeLimit);

            numTimeLimit = new NumericUpDown();
            numTimeLimit.Location = new Point(130, 20);
            numTimeLimit.Size = new Size(100, 20);
            numTimeLimit.Minimum = 5;
            numTimeLimit.Maximum = 120;
            numTimeLimit.Value = TimeLimit;
            this.Controls.Add(numTimeLimit);

            // 确定按钮
            btnOK = new Button();
            btnOK.Text = "确定";
            btnOK.Location = new Point(50, 70);
            btnOK.Size = new Size(75, 25);
            btnOK.DialogResult = DialogResult.OK;
            btnOK.Click += (s, e) =>
            {
                TimeLimit = (int)numTimeLimit.Value;
                this.Close();
            };
            this.Controls.Add(btnOK);

            // 取消按钮
            btnCancel = new Button();
            btnCancel.Text = "取消";
            btnCancel.Location = new Point(150, 70);
            btnCancel.Size = new Size(75, 25);
            btnCancel.DialogResult = DialogResult.Cancel;
            this.Controls.Add(btnCancel);
        }
    }
}