namespace ModelTransfer
{
    partial class MainForm
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

        #region Kod generowany przez Projektanta formularzy systemu Windows

        /// <summary>
        /// Metoda wymagana do obsługi projektanta — nie należy modyfikować
        /// jej zawartości w edytorze kodu.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.listView1 = new System.Windows.Forms.ListView();
            this.modelIdColHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.modelNameColHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.toolStripSaveToFileButton = new System.Windows.Forms.ToolStripButton();
            this.modelsFromFileButton = new System.Windows.Forms.ToolStripButton();
            this.helpButton = new System.Windows.Forms.ToolStripButton();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // listView1
            // 
            this.listView1.CheckBoxes = true;
            this.listView1.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.modelIdColHeader,
            this.modelNameColHeader});
            this.listView1.HideSelection = false;
            this.listView1.Location = new System.Drawing.Point(2, 28);
            this.listView1.Name = "listView1";
            this.listView1.Size = new System.Drawing.Size(241, 477);
            this.listView1.TabIndex = 0;
            this.listView1.UseCompatibleStateImageBehavior = false;
            this.listView1.View = System.Windows.Forms.View.Details;
            this.listView1.ItemChecked += new System.Windows.Forms.ItemCheckedEventHandler(this.ListView1_ItemChecked);
            this.listView1.MouseClick += new System.Windows.Forms.MouseEventHandler(this.ListView1_MouseClick);
            // 
            // modelIdColHeader
            // 
            this.modelIdColHeader.Text = "ID";
            this.modelIdColHeader.Width = 73;
            // 
            // modelNameColHeader
            // 
            this.modelNameColHeader.Text = "Nazwa Modelu";
            this.modelNameColHeader.Width = 157;
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripSaveToFileButton,
            this.modelsFromFileButton,
            this.helpButton});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(275, 25);
            this.toolStrip1.TabIndex = 1;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // toolStripSaveToFileButton
            // 
            this.toolStripSaveToFileButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripSaveToFileButton.Enabled = false;
            this.toolStripSaveToFileButton.Image = ((System.Drawing.Image)(resources.GetObject("toolStripSaveToFileButton.Image")));
            this.toolStripSaveToFileButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripSaveToFileButton.Name = "toolStripSaveToFileButton";
            this.toolStripSaveToFileButton.Size = new System.Drawing.Size(23, 22);
            this.toolStripSaveToFileButton.ToolTipText = "zapisz modele do pliku";
            this.toolStripSaveToFileButton.Click += new System.EventHandler(this.SaveToFileButton_Click);
            // 
            // modelsFromFileButton
            // 
            this.modelsFromFileButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.modelsFromFileButton.Image = ((System.Drawing.Image)(resources.GetObject("modelsFromFileButton.Image")));
            this.modelsFromFileButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.modelsFromFileButton.Name = "modelsFromFileButton";
            this.modelsFromFileButton.Size = new System.Drawing.Size(23, 22);
            this.modelsFromFileButton.ToolTipText = "zapisz modele z pliku do bazy";
            this.modelsFromFileButton.Click += new System.EventHandler(this.ModelsFromFileButton_Click);
            // 
            // helpButton
            // 
            this.helpButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.helpButton.Image = ((System.Drawing.Image)(resources.GetObject("helpButton.Image")));
            this.helpButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.helpButton.Name = "helpButton";
            this.helpButton.Size = new System.Drawing.Size(23, 22);
            this.helpButton.ToolTipText = "pomoc";
            this.helpButton.Click += new System.EventHandler(this.HelpButton_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(275, 508);
            this.Controls.Add(this.toolStrip1);
            this.Controls.Add(this.listView1);
            this.Name = "Form1";
            this.Text = "Modeler2D model transfer";
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListView listView1;
        private System.Windows.Forms.ColumnHeader modelIdColHeader;
        private System.Windows.Forms.ColumnHeader modelNameColHeader;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton toolStripSaveToFileButton;
        private System.Windows.Forms.ToolStripButton modelsFromFileButton;
        private System.Windows.Forms.ToolStripButton helpButton;
    }
}

