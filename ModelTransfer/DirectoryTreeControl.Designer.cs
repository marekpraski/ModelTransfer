namespace ModelTransfer
{
    partial class DirectoryTreeControl
    {
        /// <summary> 
        /// Wymagana zmienna projektanta.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Wyczyść wszystkie używane zasoby.
        /// </summary>
        /// <param name="disposing">prawda, jeżeli zarządzane zasoby powinny zostać zlikwidowane; Fałsz w przeciwnym wypadku.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Kod wygenerowany przez Projektanta składników

        /// <summary> 
        /// Metoda wymagana do obsługi projektanta — nie należy modyfikować 
        /// jej zawartości w edytorze kodu.
        /// </summary>
        private void InitializeComponent()
        {
            this.treeView1 = new System.Windows.Forms.TreeView();
            this.label1 = new System.Windows.Forms.Label();
            this.uncheckAllLabel = new System.Windows.Forms.Label();
            this.infoLabel = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // treeView1
            // 
            this.treeView1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.treeView1.HideSelection = false;
            this.treeView1.Location = new System.Drawing.Point(4, 22);
            this.treeView1.Name = "treeView1";
            this.treeView1.Size = new System.Drawing.Size(298, 397);
            this.treeView1.TabIndex = 0;
            this.treeView1.AfterCheck += new System.Windows.Forms.TreeViewEventHandler(this.TreeView1_AfterCheck);
            this.treeView1.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.TreeView1_AfterSelect);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(4, 4);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(80, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "wybierz katalog";
            // 
            // uncheckAllLabel
            // 
            this.uncheckAllLabel.AutoSize = true;
            this.uncheckAllLabel.Cursor = System.Windows.Forms.Cursors.Hand;
            this.uncheckAllLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.uncheckAllLabel.ForeColor = System.Drawing.Color.SteelBlue;
            this.uncheckAllLabel.Location = new System.Drawing.Point(200, 4);
            this.uncheckAllLabel.Name = "uncheckAllLabel";
            this.uncheckAllLabel.Size = new System.Drawing.Size(95, 13);
            this.uncheckAllLabel.TabIndex = 4;
            this.uncheckAllLabel.Text = "odznacz wszystkie";
            this.uncheckAllLabel.Visible = false;
            this.uncheckAllLabel.Click += new System.EventHandler(this.UncheckAllLabel_Click);
            // 
            // infoLabel
            // 
            this.infoLabel.AutoSize = true;
            this.infoLabel.Cursor = System.Windows.Forms.Cursors.Help;
            this.infoLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Underline))), System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.infoLabel.ForeColor = System.Drawing.Color.Blue;
            this.infoLabel.Location = new System.Drawing.Point(85, 2);
            this.infoLabel.Name = "infoLabel";
            this.infoLabel.Size = new System.Drawing.Size(15, 15);
            this.infoLabel.TabIndex = 5;
            this.infoLabel.Text = "?";
            this.infoLabel.MouseEnter += new System.EventHandler(this.InfoLabel_MouseEnter);
            // 
            // DirectoryTreeControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.infoLabel);
            this.Controls.Add(this.uncheckAllLabel);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.treeView1);
            this.Name = "DirectoryTreeControl";
            this.Size = new System.Drawing.Size(305, 423);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TreeView treeView1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label uncheckAllLabel;
        private System.Windows.Forms.Label infoLabel;
    }
}
