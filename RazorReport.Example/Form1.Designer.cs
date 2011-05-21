namespace RazorReport.Example {
    partial class Form1 {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose (bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose ();
            }
            base.Dispose (disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent () {
            this.precompiled = new System.Windows.Forms.Button ();
            this.webBrowser1 = new System.Windows.Forms.WebBrowser ();
            this.run = new System.Windows.Forms.Button ();
            this.SuspendLayout ();
            // 
            // precompiled
            // 
            this.precompiled.Location = new System.Drawing.Point (21, 12);
            this.precompiled.Name = "precompiled";
            this.precompiled.Size = new System.Drawing.Size (99, 23);
            this.precompiled.TabIndex = 0;
            this.precompiled.Text = "Run Compiled";
            this.precompiled.UseVisualStyleBackColor = true;
            this.precompiled.Click += new System.EventHandler (this.runCompiled_Click);
            // 
            // webBrowser1
            // 
            this.webBrowser1.Location = new System.Drawing.Point (12, 41);
            this.webBrowser1.MinimumSize = new System.Drawing.Size (20, 20);
            this.webBrowser1.Name = "webBrowser1";
            this.webBrowser1.Size = new System.Drawing.Size (635, 447);
            this.webBrowser1.TabIndex = 1;
            // 
            // run
            // 
            this.run.AutoEllipsis = true;
            this.run.Location = new System.Drawing.Point (142, 12);
            this.run.Name = "run";
            this.run.Size = new System.Drawing.Size (99, 23);
            this.run.TabIndex = 2;
            this.run.Text = "Run";
            this.run.UseVisualStyleBackColor = true;
            this.run.Click += new System.EventHandler (this.run_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF (6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size (649, 487);
            this.Controls.Add (this.run);
            this.Controls.Add (this.webBrowser1);
            this.Controls.Add (this.precompiled);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout (false);

        }

        #endregion

        private System.Windows.Forms.Button precompiled;
        private System.Windows.Forms.WebBrowser webBrowser1;
        private System.Windows.Forms.Button run;
    }
}

