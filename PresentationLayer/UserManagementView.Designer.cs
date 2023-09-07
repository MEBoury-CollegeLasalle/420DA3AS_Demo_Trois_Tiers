namespace _420DA3AS_Demo_Trois_Tiers.PresentationLayer;

partial class DtoView {
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
        dataGridView1 = new DataGridView();
        buttonSave = new Button();
        buttonCancel = new Button();
        ((System.ComponentModel.ISupportInitialize) dataGridView1).BeginInit();
        this.SuspendLayout();
        // 
        // dataGridView1
        // 
        dataGridView1.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
        dataGridView1.Location = new Point(12, 12);
        dataGridView1.Name = "dataGridView1";
        dataGridView1.RowHeadersWidth = 51;
        dataGridView1.RowTemplate.Height = 29;
        dataGridView1.Size = new Size(1109, 362);
        dataGridView1.TabIndex = 0;
        // 
        // buttonSave
        // 
        buttonSave.Location = new Point(865, 380);
        buttonSave.Name = "buttonSave";
        buttonSave.Size = new Size(125, 36);
        buttonSave.TabIndex = 1;
        buttonSave.Text = "Save";
        buttonSave.UseVisualStyleBackColor = true;
        buttonSave.Click += this.buttonSave_Click;
        // 
        // buttonCancel
        // 
        buttonCancel.Location = new Point(996, 380);
        buttonCancel.Name = "buttonCancel";
        buttonCancel.Size = new Size(125, 36);
        buttonCancel.TabIndex = 2;
        buttonCancel.Text = "Cancel";
        buttonCancel.UseVisualStyleBackColor = true;
        buttonCancel.Click += this.buttonCancel_Click;
        // 
        // UserManagementView
        // 
        this.AutoScaleDimensions = new SizeF(8F, 20F);
        this.AutoScaleMode = AutoScaleMode.Font;
        this.ClientSize = new Size(1133, 428);
        this.Controls.Add(buttonCancel);
        this.Controls.Add(buttonSave);
        this.Controls.Add(dataGridView1);
        this.Name = "UserManagementView";
        this.Text = "UserManagementView";
        ((System.ComponentModel.ISupportInitialize) dataGridView1).EndInit();
        this.ResumeLayout(false);
    }

    #endregion

    private DataGridView dataGridView1;
    private Button buttonSave;
    private Button buttonCancel;
}