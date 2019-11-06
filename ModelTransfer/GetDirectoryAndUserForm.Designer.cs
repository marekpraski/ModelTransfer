namespace ModelTransfer
{
    partial class GetDirectoryAndUserForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.label2 = new System.Windows.Forms.Label();
            this.acceptButton = new System.Windows.Forms.Button();
            this.userListView = new System.Windows.Forms.ListView();
            this.directoryTreeControl1 = new ModelTransfer.DirectoryTreeControl();
            this.restoreTreeRadioButton = new System.Windows.Forms.RadioButton();
            this.saveToOneFolderRadioButton = new System.Windows.Forms.RadioButton();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(230, 55);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(98, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "wybierz właściciela";
            // 
            // acceptButton
            // 
            this.acceptButton.Location = new System.Drawing.Point(178, 363);
            this.acceptButton.Name = "acceptButton";
            this.acceptButton.Size = new System.Drawing.Size(75, 23);
            this.acceptButton.TabIndex = 4;
            this.acceptButton.Text = "zatwierdź";
            this.acceptButton.UseVisualStyleBackColor = true;
            this.acceptButton.Click += new System.EventHandler(this.acceptButton_Click);
            // 
            // userListView
            // 
            this.userListView.HideSelection = false;
            this.userListView.Location = new System.Drawing.Point(230, 79);
            this.userListView.MultiSelect = false;
            this.userListView.Name = "userListView";
            this.userListView.Size = new System.Drawing.Size(190, 278);
            this.userListView.TabIndex = 5;
            this.userListView.UseCompatibleStateImageBehavior = false;
            this.userListView.View = System.Windows.Forms.View.List;
            // 
            // directoryTreeControl1
            // 
            this.directoryTreeControl1.Location = new System.Drawing.Point(11, 55);
            this.directoryTreeControl1.Name = "directoryTreeControl1";
            this.directoryTreeControl1.Size = new System.Drawing.Size(213, 302);
            this.directoryTreeControl1.TabIndex = 6;
            // 
            // restoreTreeRadioButton
            // 
            this.restoreTreeRadioButton.AutoSize = true;
            this.restoreTreeRadioButton.Checked = true;
            this.restoreTreeRadioButton.Location = new System.Drawing.Point(6, 19);
            this.restoreTreeRadioButton.Name = "restoreTreeRadioButton";
            this.restoreTreeRadioButton.Size = new System.Drawing.Size(151, 17);
            this.restoreTreeRadioButton.TabIndex = 7;
            this.restoreTreeRadioButton.TabStop = true;
            this.restoreTreeRadioButton.Text = "odtwórz drzewo katalogów";
            this.restoreTreeRadioButton.UseVisualStyleBackColor = true;
            // 
            // saveToOneFolderRadioButton
            // 
            this.saveToOneFolderRadioButton.AutoSize = true;
            this.saveToOneFolderRadioButton.Location = new System.Drawing.Point(167, 19);
            this.saveToOneFolderRadioButton.Name = "saveToOneFolderRadioButton";
            this.saveToOneFolderRadioButton.Size = new System.Drawing.Size(154, 17);
            this.saveToOneFolderRadioButton.TabIndex = 8;
            this.saveToOneFolderRadioButton.Text = "zapisz do jednego katalogu";
            this.saveToOneFolderRadioButton.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.restoreTreeRadioButton);
            this.groupBox1.Controls.Add(this.saveToOneFolderRadioButton);
            this.groupBox1.Location = new System.Drawing.Point(11, 3);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(409, 46);
            this.groupBox1.TabIndex = 9;
            this.groupBox1.TabStop = false;
            // 
            // GetDirectoryAndUserForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(432, 392);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.directoryTreeControl1);
            this.Controls.Add(this.userListView);
            this.Controls.Add(this.acceptButton);
            this.Controls.Add(this.label2);
            this.Name = "GetDirectoryAndUserForm";
            this.Text = "Wybór katalogu i właściciela modeli";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button acceptButton;
        private System.Windows.Forms.ListView userListView;
        private DirectoryTreeControl directoryTreeControl1;
        private System.Windows.Forms.RadioButton restoreTreeRadioButton;
        private System.Windows.Forms.RadioButton saveToOneFolderRadioButton;
        private System.Windows.Forms.GroupBox groupBox1;
    }
}