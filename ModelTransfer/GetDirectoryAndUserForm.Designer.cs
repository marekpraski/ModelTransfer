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
            this.button1 = new System.Windows.Forms.Button();
            this.userListView = new System.Windows.Forms.ListView();
            this.directoryTreeControl1 = new ModelTransfer.DirectoryTreeControl();
            this.SuspendLayout();
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(231, 12);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(98, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "wybierz właściciela";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(179, 320);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 4;
            this.button1.Text = "zatwierdź";
            this.button1.UseVisualStyleBackColor = true;
            // 
            // userListView
            // 
            this.userListView.HideSelection = false;
            this.userListView.Location = new System.Drawing.Point(231, 36);
            this.userListView.MultiSelect = false;
            this.userListView.Name = "userListView";
            this.userListView.Size = new System.Drawing.Size(190, 278);
            this.userListView.TabIndex = 5;
            this.userListView.UseCompatibleStateImageBehavior = false;
            this.userListView.View = System.Windows.Forms.View.List;
            // 
            // directoryTreeControl1
            // 
            this.directoryTreeControl1.Location = new System.Drawing.Point(12, 12);
            this.directoryTreeControl1.Name = "directoryTreeControl1";
            this.directoryTreeControl1.Size = new System.Drawing.Size(213, 302);
            this.directoryTreeControl1.TabIndex = 6;
            // 
            // GetDirectoryAndUserForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(432, 353);
            this.Controls.Add(this.directoryTreeControl1);
            this.Controls.Add(this.userListView);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.label2);
            this.Name = "GetDirectoryAndUserForm";
            this.Text = "Wybór katalogu i właściciela modeli";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.ListView userListView;
        private DirectoryTreeControl directoryTreeControl1;
    }
}