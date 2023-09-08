using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace _420DA3AS_Demo_Trois_Tiers.PresentationLayer;
public partial class DebuggerWindow : Form {
    public DebuggerWindow() {
        this.InitializeComponent();
    }

    public void AddColoredLine(string line, Color? color) {
        if (color != null) {
            this.richTextBox1.SelectionStart = this.richTextBox1.TextLength;
            this.richTextBox1.SelectionLength = 0;
            this.richTextBox1.SelectionColor = (Color) color;
            this.richTextBox1.AppendText(line + Environment.NewLine);
            this.richTextBox1.SelectionColor = this.richTextBox1.ForeColor;
        } else {
            this.richTextBox1.AppendText(line + Environment.NewLine);
        }
    }
}
