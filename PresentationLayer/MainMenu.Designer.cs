namespace _420DA3AS_Demo_Trois_Tiers;

partial class MainMenu {
    /// <summary>
    ///  Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    ///  Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing) {
        if (disposing && (components != null)) {
            components.Dispose();
        }
        base.Dispose(disposing);
    }

    #region Windows Form Designer generated code

    /// <summary>
    ///  Required method for Designer support - do not modify
    ///  the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent() {
        buttonManageUsers = new Button();
        buttonExit = new Button();
        this.SuspendLayout();
        // 
        // buttonManageUsers
        // 
        buttonManageUsers.Location = new Point(291, 132);
        buttonManageUsers.Name = "buttonManageUsers";
        buttonManageUsers.Size = new Size(196, 50);
        buttonManageUsers.TabIndex = 0;
        buttonManageUsers.Text = "Manage Users";
        buttonManageUsers.UseVisualStyleBackColor = true;
        buttonManageUsers.Click += this.ButtonManageUsers_Click;
        // 
        // buttonExit
        // 
        buttonExit.Location = new Point(291, 263);
        buttonExit.Name = "buttonExit";
        buttonExit.Size = new Size(196, 50);
        buttonExit.TabIndex = 1;
        buttonExit.Text = "Exit";
        buttonExit.UseVisualStyleBackColor = true;
        buttonExit.Click += this.ButtonExit_Click;
        // 
        // MainMenu
        // 
        this.AutoScaleDimensions = new SizeF(8F, 20F);
        this.AutoScaleMode = AutoScaleMode.Font;
        this.ClientSize = new Size(800, 450);
        this.Controls.Add(buttonExit);
        this.Controls.Add(buttonManageUsers);
        this.Name = "MainMenu";
        this.Text = "Form1";
        this.ResumeLayout(false);
    }

    #endregion

    private Button buttonManageUsers;
    private Button buttonExit;
}
