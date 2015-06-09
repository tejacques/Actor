namespace Actor
{
    partial class Actor
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.webBrowser1 = new System.Windows.Forms.WebBrowser();
            this.StartStop = new System.Windows.Forms.Button();
            this.PlayMove = new System.Windows.Forms.Button();
            this.DeckChoice = new System.Windows.Forms.ComboBox();
            this.actorBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.RematchButton = new System.Windows.Forms.Button();
            this.StartGameButton = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.actorBindingSource)).BeginInit();
            this.SuspendLayout();
            // 
            // webBrowser1
            // 
            this.webBrowser1.Location = new System.Drawing.Point(2, 128);
            this.webBrowser1.MinimumSize = new System.Drawing.Size(20, 20);
            this.webBrowser1.Name = "webBrowser1";
            this.webBrowser1.Size = new System.Drawing.Size(1147, 644);
            this.webBrowser1.TabIndex = 0;
            this.webBrowser1.DocumentCompleted += new System.Windows.Forms.WebBrowserDocumentCompletedEventHandler(this.webBrowser1_DocumentCompleted);
            // 
            // StartStop
            // 
            this.StartStop.Location = new System.Drawing.Point(12, 12);
            this.StartStop.Name = "StartStop";
            this.StartStop.Size = new System.Drawing.Size(75, 23);
            this.StartStop.TabIndex = 1;
            this.StartStop.Text = "Start";
            this.StartStop.UseVisualStyleBackColor = true;
            this.StartStop.Click += new System.EventHandler(this.StartStop_Click);
            // 
            // PlayMove
            // 
            this.PlayMove.Location = new System.Drawing.Point(12, 41);
            this.PlayMove.Name = "PlayMove";
            this.PlayMove.Size = new System.Drawing.Size(75, 23);
            this.PlayMove.TabIndex = 2;
            this.PlayMove.Text = "Play Move";
            this.PlayMove.UseVisualStyleBackColor = true;
            this.PlayMove.Click += new System.EventHandler(this.PlayMove_Click);
            // 
            // DeckChoice
            // 
            this.DeckChoice.Cursor = System.Windows.Forms.Cursors.Default;
            this.DeckChoice.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.DeckChoice.FormattingEnabled = true;
            this.DeckChoice.Items.AddRange(new object[] {
            "Deck 1",
            "Deck 2",
            "Deck 3",
            "Deck 4",
            "Deck 5",
            "Optimized"});
            this.DeckChoice.Location = new System.Drawing.Point(117, 14);
            this.DeckChoice.Name = "DeckChoice";
            this.DeckChoice.Size = new System.Drawing.Size(121, 21);
            this.DeckChoice.TabIndex = 3;
            this.DeckChoice.SelectedIndexChanged += new System.EventHandler(this.DeckChoice_SelectedIndexChanged);
            // 
            // RematchButton
            // 
            this.RematchButton.Location = new System.Drawing.Point(93, 70);
            this.RematchButton.Name = "RematchButton";
            this.RematchButton.Size = new System.Drawing.Size(75, 23);
            this.RematchButton.TabIndex = 4;
            this.RematchButton.Text = "Rematch";
            this.RematchButton.UseVisualStyleBackColor = true;
            this.RematchButton.Click += new System.EventHandler(this.RematchButton_Click);
            // 
            // StartGameButton
            // 
            this.StartGameButton.Location = new System.Drawing.Point(12, 70);
            this.StartGameButton.Name = "StartGameButton";
            this.StartGameButton.Size = new System.Drawing.Size(75, 23);
            this.StartGameButton.TabIndex = 5;
            this.StartGameButton.Text = "Start Game";
            this.StartGameButton.UseVisualStyleBackColor = true;
            this.StartGameButton.Click += new System.EventHandler(this.StartGameButton_Click);
            // 
            // Actor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1150, 770);
            this.Controls.Add(this.StartGameButton);
            this.Controls.Add(this.RematchButton);
            this.Controls.Add(this.DeckChoice);
            this.Controls.Add(this.PlayMove);
            this.Controls.Add(this.StartStop);
            this.Controls.Add(this.webBrowser1);
            this.Name = "Actor";
            this.Text = "Actor";
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.actorBindingSource)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.WebBrowser webBrowser1;
        private System.Windows.Forms.Button StartStop;
        private System.Windows.Forms.Button PlayMove;
        private System.Windows.Forms.ComboBox DeckChoice;
        private System.Windows.Forms.BindingSource actorBindingSource;
        private System.Windows.Forms.Button RematchButton;
        private System.Windows.Forms.Button StartGameButton;
    }
}

