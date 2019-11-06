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
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.infoLabel = new System.Windows.Forms.Label();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.numberLabel = new System.Windows.Forms.Label();
            this.abortButton = new System.Windows.Forms.Button();
            this.progressAreaPanel = new System.Windows.Forms.Panel();
            this.modelNameLabel = new System.Windows.Forms.Label();
            this.directoryTreeControl1 = new ModelTransfer.DirectoryTreeControl();
            this.toolStrip1.SuspendLayout();
            this.progressAreaPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // modelsListView
            // 
            this.modelsListView.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.modelsListView.CheckBoxes = true;
            this.modelsListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.modelIdColHeader,
            this.modelNameColHeader});
            this.modelsListView.HideSelection = false;
            this.modelsListView.Location = new System.Drawing.Point(327, 51);
            this.modelsListView.Name = "modelsListView";
            this.modelsListView.Size = new System.Drawing.Size(241, 395);
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
            this.toolStrip1.Size = new System.Drawing.Size(578, 25);
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
            this.chooseModelsLabel.Location = new System.Drawing.Point(327, 35);
            this.chooseModelsLabel.Name = "chooseModelsLabel";
            this.chooseModelsLabel.Size = new System.Drawing.Size(79, 13);
            this.chooseModelsLabel.TabIndex = 3;
            this.chooseModelsLabel.Text = "wybierz modele";
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "modele";
            this.openFileDialog1.FileOk += new System.ComponentModel.CancelEventHandler(this.OpenFileDialog1_FileOk);
            // 
            // infoLabel
            // 
            this.infoLabel.AutoSize = true;
            this.infoLabel.Location = new System.Drawing.Point(3, 35);
            this.infoLabel.Name = "infoLabel";
            this.infoLabel.Size = new System.Drawing.Size(24, 13);
            this.infoLabel.TabIndex = 6;
            this.infoLabel.Text = "info";
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(167, 28);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(305, 23);
            this.progressBar1.TabIndex = 4;
            // 
            // numberLabel
            // 
            this.numberLabel.AutoSize = true;
            this.numberLabel.Location = new System.Drawing.Point(164, 12);
            this.numberLabel.Name = "numberLabel";
            this.numberLabel.Size = new System.Drawing.Size(42, 13);
            this.numberLabel.TabIndex = 5;
            this.numberLabel.Text = "number";
            // 
            // abortButton
            // 
            this.abortButton.Location = new System.Drawing.Point(243, 57);
            this.abortButton.Name = "abortButton";
            this.abortButton.Size = new System.Drawing.Size(123, 23);
            this.abortButton.TabIndex = 7;
            this.abortButton.Text = "przerwij";
            this.abortButton.UseVisualStyleBackColor = true;
            this.abortButton.Click += new System.EventHandler(this.AbortButton_Click);
            // 
            // progressAreaPanel
            // 
            this.progressAreaPanel.BackColor = System.Drawing.SystemColors.Control;
            this.progressAreaPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.progressAreaPanel.Controls.Add(this.modelNameLabel);
            this.progressAreaPanel.Controls.Add(this.infoLabel);
            this.progressAreaPanel.Controls.Add(this.abortButton);
            this.progressAreaPanel.Controls.Add(this.progressBar1);
            this.progressAreaPanel.Controls.Add(this.numberLabel);
            this.progressAreaPanel.Location = new System.Drawing.Point(51, 310);
            this.progressAreaPanel.Name = "progressAreaPanel";
            this.progressAreaPanel.Size = new System.Drawing.Size(477, 86);
            this.progressAreaPanel.TabIndex = 8;
            // 
            // modelNameLabel
            // 
            this.modelNameLabel.AutoSize = true;
            this.modelNameLabel.Location = new System.Drawing.Point(240, 11);
            this.modelNameLabel.Name = "modelNameLabel";
            this.modelNameLabel.Size = new System.Drawing.Size(75, 13);
            this.modelNameLabel.TabIndex = 8;
            this.modelNameLabel.Text = "nazwa modelu";
            // 
            // directoryTreeControl1
            // 
            this.directoryTreeControl1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.directoryTreeControl1.Location = new System.Drawing.Point(12, 28);
            this.directoryTreeControl1.Name = "directoryTreeControl1";
            this.directoryTreeControl1.Size = new System.Drawing.Size(309, 422);
            this.directoryTreeControl1.TabIndex = 2;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(578, 450);
            this.Controls.Add(this.progressAreaPanel);
            this.Controls.Add(this.chooseModelsLabel);
            this.Controls.Add(this.directoryTreeControl1);
            this.Controls.Add(this.toolStrip1);
            this.Controls.Add(this.modelsListView);
            this.Name = "MainForm";
            this.Text = "Modeler2D model transfer";
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.progressAreaPanel.ResumeLayout(false);
            this.progressAreaPanel.PerformLayout();
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
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.Label infoLabel;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.Label numberLabel;
        private System.Windows.Forms.Button abortButton;
        private System.Windows.Forms.Panel progressAreaPanel;
        private System.Windows.Forms.Label modelNameLabel;
    }
}

