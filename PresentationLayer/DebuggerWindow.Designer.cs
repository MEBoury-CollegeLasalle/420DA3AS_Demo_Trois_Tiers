namespace _420DA3AS_Demo_Trois_Tiers.PresentationLayer;

partial class DebuggerWindow {
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
        richTextBox1 = new RichTextBox();
        this.SuspendLayout();
        // 
        // richTextBox1
        // 
        richTextBox1.BackColor = Color.Black;
        richTextBox1.Dock = DockStyle.Fill;
        richTextBox1.Location = new Point(0, 0);
        richTextBox1.Name = "richTextBox1";
        richTextBox1.ReadOnly = true;
        richTextBox1.Size = new Size(582, 553);
        richTextBox1.TabIndex = 0;
        richTextBox1.Text = "";
        richTextBox1.WordWrap = false;
        // 
        // DebuggerWindow
        // 
        this.AutoScaleDimensions = new SizeF(8F, 20F);
        this.AutoScaleMode = AutoScaleMode.Font;
        this.ClientSize = new Size(582, 553);
        this.Controls.Add(richTextBox1);
        this.Name = "DebuggerWindow";
        this.Text = "Debugger Window";
        this.ResumeLayout(false);
    }

    #endregion

    private RichTextBox richTextBox1;
}