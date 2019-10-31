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
            this.saveToFileTimer = new System.Timers.Timer();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.label2 = new System.Windows.Forms.Label();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.label1 = new System.Windows.Forms.Label();
            this.abortButton = new System.Windows.Forms.Button();
            this.progressAreaPanel = new System.Windows.Forms.Panel();
            this.directoryTreeControl1 = new ModelTransfer.DirectoryTreeControl();
            this.readFromFileTimer = new System.Timers.Timer();
            this.toolStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.saveToFileTimer)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.readFromFileTimer)).BeginInit();
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
            // saveToFileTimer
            // 
            this.saveToFileTimer.Enabled = true;
            this.saveToFileTimer.SynchronizingObject = this;
            // 
            // readFromFileTimer
            // 
            this.readFromFileTimer.Enabled = true;
            this.readFromFileTimer.SynchronizingObject = this;
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "modele";
            this.openFileDialog1.FileOk += new System.ComponentModel.CancelEventHandler(this.OpenFileDialog1_FileOk);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(115, 12);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(35, 13);
            this.label2.TabIndex = 6;
            this.label2.Text = "label2";
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(69, 28);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(228, 23);
            this.progressBar1.TabIndex = 4;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 32);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(35, 13);
            this.label1.TabIndex = 5;
            this.label1.Text = "label1";
            // 
            // abortButton
            // 
            this.abortButton.Location = new System.Drawing.Point(118, 57);
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
            this.progressAreaPanel.Controls.Add(this.label2);
            this.progressAreaPanel.Controls.Add(this.abortButton);
            this.progressAreaPanel.Controls.Add(this.progressBar1);
            this.progressAreaPanel.Controls.Add(this.label1);
            this.progressAreaPanel.Location = new System.Drawing.Point(117, 310);
            this.progressAreaPanel.Name = "progressAreaPanel";
            this.progressAreaPanel.Size = new System.Drawing.Size(321, 86);
            this.progressAreaPanel.TabIndex = 8;
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
            ((System.ComponentModel.ISupportInitialize)(this.saveToFileTimer)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.readFromFileTimer)).EndInit();
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
        private System.Timers.Timer saveToFileTimer;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button abortButton;
        private System.Windows.Forms.Panel progressAreaPanel;
        private System.Timers.Timer readFromFileTimer;
    }
}

