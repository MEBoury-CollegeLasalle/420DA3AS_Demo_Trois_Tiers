namespace _420DA3AS_Demo_Trois_Tiers.PresentationLayer;

partial class UserManagementView {
    /// <summary>
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    /// Clean up any resources being used.
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
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent() {
        userCombobox = new ComboBox();
        buttonCreateUser = new Button();
        buttonEditUser = new Button();
        buttonUserDetails = new Button();
        buttonUserDelete = new Button();
        this.SuspendLayout();
        // 
        // userCombobox
        // 
        userCombobox.FormattingEnabled = true;
        userCombobox.Location = new Point(12, 48);
        userCombobox.Name = "userCombobox";
        userCombobox.Size = new Size(350, 28);
        userCombobox.TabIndex = 0;
        userCombobox.SelectedIndexChanged += this.UserComboBox_SelectedIndexChanged;
        // 
        // buttonCreateUser
        // 
        buttonCreateUser.Location = new Point(368, 46);
        buttonCreateUser.Name = "buttonCreateUser";
        buttonCreateUser.Size = new Size(50, 30);
        buttonCreateUser.TabIndex = 1;
        buttonCreateUser.Text = "NEW";
        buttonCreateUser.UseVisualStyleBackColor = true;
        buttonCreateUser.Click += this.buttonCreateUser_Click;
        // 
        // buttonEditUser
        // 
        buttonEditUser.Location = new Point(503, 46);
        buttonEditUser.Name = "buttonEditUser";
        buttonEditUser.Size = new Size(50, 30);
        buttonEditUser.TabIndex = 2;
        buttonEditUser.Text = "EDIT";
        buttonEditUser.UseVisualStyleBackColor = true;
        buttonEditUser.Click += this.buttonEditUser_Click;
        // 
        // buttonUserDetails
        // 
        buttonUserDetails.Location = new Point(424, 46);
        buttonUserDetails.Name = "buttonUserDetails";
        buttonUserDetails.Size = new Size(73, 30);
        buttonUserDetails.TabIndex = 3;
        buttonUserDetails.Text = "DETAILS";
        buttonUserDetails.UseVisualStyleBackColor = true;
        buttonUserDetails.Click += this.buttonUserDetails_Click;
        // 
        // buttonUserDelete
        // 
        buttonUserDelete.Location = new Point(559, 46);
        buttonUserDelete.Name = "buttonUserDelete";
        buttonUserDelete.Size = new Size(50, 30);
        buttonUserDelete.TabIndex = 4;
        buttonUserDelete.Text = "NEW";
        buttonUserDelete.UseVisualStyleBackColor = true;
        buttonUserDelete.Click += this.buttonUserDelete_Click;
        // 
        // UserManagementView
        // 
        this.AutoScaleDimensions = new SizeF(8F, 20F);
        this.AutoScaleMode = AutoScaleMode.Font;
        this.ClientSize = new Size(621, 116);
        this.Controls.Add(buttonUserDelete);
        this.Controls.Add(buttonUserDetails);
        this.Controls.Add(buttonEditUser);
        this.Controls.Add(buttonCreateUser);
        this.Controls.Add(userCombobox);
        this.Name = "UserManagementView";
        this.Text = "UserManagementView";
        this.ResumeLayout(false);
    }

    #endregion

    private ComboBox userCombobox;
    private Button buttonCreateUser;
    private Button buttonEditUser;
    private Button buttonUserDetails;
    private Button buttonUserDelete;
}