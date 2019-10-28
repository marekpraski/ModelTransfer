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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.modelsListView = new System.Windows.Forms.ListView();
            this.modelIdColHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.modelNameColHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.toolStripSaveToFileButton = new System.Windows.Forms.ToolStripButton();
            this.saveModelOptionsCombo = new System.Windows.Forms.ToolStripComboBox();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.modelsFromFileButton = new System.Windows.Forms.ToolStripButton();
            this.helpButton = new System.Windows.Forms.ToolStripButton();
            this.chooseModelsLabel = new System.Windows.Forms.Label();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.directoryTreeControl1 = new ModelTransfer.DirectoryTreeControl();
            this.toolStrip1.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // modelsListView
            // 
            this.modelsListView.CheckBoxes = true;
            this.modelsListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.modelIdColHeader,
            this.modelNameColHeader});
            this.modelsListView.HideSelection = false;
            this.modelsListView.Location = new System.Drawing.Point(232, 49);
            this.modelsListView.Name = "modelsListView";
            this.modelsListView.Size = new System.Drawing.Size(241, 281);
            this.modelsListView.TabIndex = 0;
            this.modelsListView.UseCompatibleStateImageBehavior = false;
            this.modelsListView.View = System.Windows.Forms.View.Details;
            this.modelsListView.ItemChecked += new System.Windows.Forms.ItemCheckedEventHandler(this.modelsListView_ItemChecked);
            this.modelsListView.MouseClick += new System.Windows.Forms.MouseEventHandler(this.modelsListView_MouseClick);
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
            this.saveModelOptionsCombo,
            this.toolStripSeparator1,
            this.modelsFromFileButton,
            this.helpButton});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(481, 25);
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
            // saveModelOptionsCombo
            // 
            this.saveModelOptionsCombo.Name = "saveModelOptionsCombo";
            this.saveModelOptionsCombo.Size = new System.Drawing.Size(121, 25);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // modelsFromFileButton
            // 
            this.modelsFromFileButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.modelsFromFileButton.Image = ((System.Drawing.Image)(resources.GetObject("modelsFromFileButton.Image")));
            this.modelsFromFileButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.modelsFromFileButton.Name = "modelsFromFileButton";
            this.modelsFromFileButton.Size = new System.Drawing.Size(23, 22);
            this.modelsFromFileButton.ToolTipText = "zapisz modele z pliku do bazy";
            this.modelsFromFileButton.Click += new System.EventHandler(this.SaveToDBButton_Click);
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
            // chooseModelsLabel
            // 
            this.chooseModelsLabel.AutoSize = true;
            this.chooseModelsLabel.Location = new System.Drawing.Point(232, 29);
            this.chooseModelsLabel.Name = "chooseModelsLabel";
            this.chooseModelsLabel.Size = new System.Drawing.Size(79, 13);
            this.chooseModelsLabel.TabIndex = 3;
            this.chooseModelsLabel.Text = "wybierz modele";
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(215, 16);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(228, 23);
            this.progressBar1.TabIndex = 4;
            // 
            // timer1
            // 
            this.timer1.Tick += new System.EventHandler(this.Timer1_Tick);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 26);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(35, 13);
            this.label1.TabIndex = 5;
            this.label1.Text = "label1";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(84, 26);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(35, 13);
            this.label2.TabIndex = 6;
            this.label2.Text = "label2";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.progressBar1);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Location = new System.Drawing.Point(20, 227);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(449, 55);
            this.groupBox1.TabIndex = 7;
            this.groupBox1.TabStop = false;
            // 
            // directoryTreeControl1
            // 
            this.directoryTreeControl1.Location = new System.Drawing.Point(12, 28);
            this.directoryTreeControl1.Name = "directoryTreeControl1";
            this.directoryTreeControl1.Size = new System.Drawing.Size(213, 302);
            this.directoryTreeControl1.TabIndex = 2;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(481, 336);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.chooseModelsLabel);
            this.Controls.Add(this.directoryTreeControl1);
            this.Controls.Add(this.toolStrip1);
            this.Controls.Add(this.modelsListView);
            this.Name = "MainForm";
            this.Text = "Modeler2D model transfer";
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListView modelsListView;
        private System.Windows.Forms.ColumnHeader modelIdColHeader;
        private System.Windows.Forms.ColumnHeader modelNameColHeader;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton toolStripSaveToFileButton;
        private System.Windows.Forms.ToolStripButton modelsFromFileButton;
        private System.Windows.Forms.ToolStripButton helpButton;
        private DirectoryTreeControl directoryTreeControl1;
        private System.Windows.Forms.Label chooseModelsLabel;
        private System.Windows.Forms.ToolStripComboBox saveModelOptionsCombo;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.GroupBox groupBox1;
    }
}

