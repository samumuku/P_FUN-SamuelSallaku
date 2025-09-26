namespace PlotThoseLines_P_FUN_SamuelSallaku
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            formsPlot1 = new ScottPlot.WinForms.FormsPlot();
            button1 = new Button();
            Years = new CheckedListBox();
            SuspendLayout();
            // 
            // formsPlot1
            // 
            formsPlot1.DisplayScale = 1F;
            formsPlot1.Location = new Point(244, 25);
            formsPlot1.Name = "formsPlot1";
            formsPlot1.Size = new Size(728, 399);
            formsPlot1.TabIndex = 0;
            // 
            // button1
            // 
            button1.BackColor = SystemColors.InactiveCaption;
            button1.Font = new Font("Verdana", 14.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            button1.ForeColor = SystemColors.ControlText;
            button1.Location = new Point(24, 43);
            button1.Name = "button1";
            button1.Size = new Size(160, 55);
            button1.TabIndex = 1;
            button1.Text = "Import Data";
            button1.UseVisualStyleBackColor = false;
            button1.Click += button1_Click;
            // 
            // Years
            // 
            Years.BackColor = SystemColors.InactiveCaption;
            Years.FormattingEnabled = true;
            Years.Location = new Point(24, 118);
            Years.Name = "Years";
            Years.Size = new Size(160, 94);
            Years.TabIndex = 2;
            Years.SelectedIndexChanged += Years_SelectedIndexChanged;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = SystemColors.ActiveCaption;
            ClientSize = new Size(1008, 574);
            Controls.Add(Years);
            Controls.Add(button1);
            Controls.Add(formsPlot1);
            ForeColor = SystemColors.ControlText;
            Name = "Form1";
            Text = "Plot Data";
            Load += Form1_Load_1;
            ResumeLayout(false);
        }

        #endregion

        public ScottPlot.WinForms.FormsPlot formsPlot1;
        private Button button1;
        private CheckedListBox Years;
    }
}
