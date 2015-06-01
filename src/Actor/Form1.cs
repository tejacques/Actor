using Microsoft.Win32;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Security.Permissions;
using System.Text;
using System.Windows.Forms;
using Utility;

namespace Actor
{
    [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
    public partial class Actor : Form
    {
        private bool started = false;
        private bool pageLoaded = false;

        private GameInput gameInput;
        private Timer updateTimer;

        public void SetRegistryKey(bool Is64BitOperatingSystem)
        {
            RegistryKey Regkey = null;
            var exeName = System.AppDomain.CurrentDomain.FriendlyName;
            try
            {

                //For 64 bit Machine 
                if (Is64BitOperatingSystem)
                {
                    Regkey = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(@"SOFTWARE\\Wow6432Node\\Microsoft\\Internet Explorer\\MAIN\\FeatureControl\\FEATURE_BROWSER_EMULATION", true);
                }
                else  //For 32 bit Machine 
                {
                    Regkey = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(@"SOFTWARE\\Microsoft\\Internet Explorer\\MAIN\\FeatureControl\\FEATURE_BROWSER_EMULATION", true);
                }


                //If the path is not correct or 
                //If user't have priviledges to access registry 
                if (Regkey == null)
                {
                    Console.WriteLine("Application Settings Failed - Address Not found");
                    return;
                }


                string FindAppkey = Convert.ToString(Regkey.GetValue(exeName));

                //Check if key is already present 
                if (FindAppkey == "11000")
                {
                    Console.WriteLine("Required Application Settings Present");
                    Regkey.Close();
                    return;
                }

                //If key is not present add the key , Kev value 8000-Decimal 
                if (string.IsNullOrEmpty(FindAppkey))
                    Regkey.SetValue(exeName, unchecked((int)11000),
                        RegistryValueKind.DWord);


                //check for the key after adding 
                FindAppkey = Convert.ToString(Regkey.GetValue(exeName));

                if (FindAppkey == "11000")
                {
                    Console.WriteLine("Application Settings Applied Successfully");
                }
                else
                {
                    Console.WriteLine("Application Settings Failed");
                }


            }
            catch (Exception ex)
            {
                Console.WriteLine("Application Settings Failed");
                Console.WriteLine(ex.Message);
            }
            finally
            {
                //Close the Registry 
                if (Regkey != null)
                    Regkey.Close();
            }
        }
        public Actor()
        {
            SetRegistryKey(true);
            SetRegistryKey(false);
            InitializeComponent();

            //webBrowser1.Navigate("https://whatbrowser.org/");
#if DEBUG
            webBrowser1.Navigate("http://localhost:3000");
#elif RELEASE
            webBrowser1.Navigate("https://tejacques.github.io/Thinker");
#endif

            player0Hand = new int[5];
            player1Hand = new int[5];
            board = new int[9];
            rules = 0;
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
        private void webBrowser1_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            pageLoaded = true;
        }

        private void StartStop_Click(object sender, EventArgs e)
        {
            started = !started;
            var button = sender as Button;
            if (started)
            {
                button.Text = "Stop";
                Start();
            }
            else
            {
                button.Text = "Start";
                Stop();
            }
        }

        private void Start()
        {
            gameInput = new GameInput();

            updateTimer = new Timer();
            updateTimer.Tick += updateTimer_Tick;
            updateTimer.Interval = 200;
            updateTimer.Start();
        }

        int[] player0Hand;
        int[] player1Hand;
        int[] board;
        int rules;

        void updateTimer_Tick(object sender, EventArgs e)
        {
            var gs = gameInput.GameScanner;
            gs.Refresh();

            if (!gs.BoardOpen())
            {
                return;
            }

            var p0Hand = gs.PlayerHand(0);
            var p1Hand = gs.PlayerHand(1);
            var board = gs.Board();
            var rules = gs.GetBoardRules();

            if (!boardChanged(p0Hand, p1Hand, board, rules))
            {
                return;
            }

            this.player0Hand = p0Hand;
            this.player1Hand = p1Hand;
            this.board = board;
            this.rules = rules;
            var turnPlayerId = gs.GetTurnPlayerId();

            updateJavaScript(p0Hand, p1Hand, board, rules, turnPlayerId);


        }

        public bool boardChanged(int[] p0, int[] p1, int[] b, int rules)
        {
            if (p0.Length != player0Hand.Length) return true;
            if (p1.Length != player1Hand.Length) return true;
            if (b.Length != board.Length) return true;
            if (rules != this.rules) return true;

            for(var handIndex = 0; handIndex < p0.Length; handIndex++)
            {
                if((p0[handIndex] != player0Hand[handIndex])
                    || (p1[handIndex] != player1Hand[handIndex]))
                {
                    return true;
                }
            }

            for(var boardIndex = 0; boardIndex < b.Length; boardIndex++)
            {
                if (b[boardIndex] != board[boardIndex])
                {
                    return true;
                }
            }

            return false;
        }

        public void updateJavaScript(
            int[] p0,
            int[] p1,
            int[] board,
            int rules,
            int currentPlayerTurn)
        {
            string fmt = @"
var game = board.state.game.clone();
game.parent = board.state.game;
var h0={0};
var h1={1};
var ph0 = game.players[0].hand;
var ph1 = game.players[1].hand;
ph0.forEach(function(card, index, hand) {{
    hand[index] = Math.max(hand[index], h0[index]);
}});
ph1.forEach(function(card, index, hand) {{
    hand[index] = Math.max(hand[index], h1[index]);
}});
game.board={2};
game.rules={3};
game.turn={4};
game.firstMove={5};
board.setState({{ game: game }});
";
            var p0s = JsonConvert.SerializeObject(p0);
            var p1s = JsonConvert.SerializeObject(p1);
            var bs = JsonConvert.SerializeObject(board);
            var rs = JsonConvert.SerializeObject(rules);
            var turnNum = board.Count(x => x != 255);
            var turn = JsonConvert.SerializeObject(turnNum);

            Console.WriteLine("Turn: " + turn + " CurrentPlayerTurn: " + currentPlayerTurn);
            var firstMove = (turnNum % 2) == currentPlayerTurn ? 1 : 0;
            var firstMoveStr = JsonConvert.SerializeObject(firstMove);

            var toRun = string.Format(fmt,
                p0s,
                p1s,
                bs,
                rs,
                turn,
                firstMoveStr);

            webBrowser1.Document.InvokeScript("eval", new[] { toRun });
        }

        private void Stop()
        {
            updateTimer.Stop();
        }
    }
}
