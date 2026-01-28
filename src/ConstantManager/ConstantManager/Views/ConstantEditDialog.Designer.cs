namespace ConstantManager.Views
{
    partial class ConstantEditDialog
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
            this.components = new System.ComponentModel.Container();
            this.lblPhysicalName = new System.Windows.Forms.Label();
            this.txtPhysicalName = new System.Windows.Forms.TextBox();
            this.lblLogicalName = new System.Windows.Forms.Label();
            this.txtLogicalName = new System.Windows.Forms.TextBox();
            this.lblValue = new System.Windows.Forms.Label();
            this.txtValue = new System.Windows.Forms.TextBox();
            this.lblUnit = new System.Windows.Forms.Label();
            this.txtUnit = new System.Windows.Forms.TextBox();
            this.lblDescription = new System.Windows.Forms.Label();
            this.txtDescription = new System.Windows.Forms.TextBox();
            this.btnOk = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.SuspendLayout();

            // lblPhysicalName
            this.lblPhysicalName.AutoSize = false;
            this.lblPhysicalName.Location = new System.Drawing.Point(20, 20);
            this.lblPhysicalName.Name = "lblPhysicalName";
            this.lblPhysicalName.Size = new System.Drawing.Size(150, 20);
            this.lblPhysicalName.TabIndex = 0;
            this.lblPhysicalName.Text = "定数名(物理名) *";
            this.lblPhysicalName.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;

            // txtPhysicalName
            this.txtPhysicalName.Location = new System.Drawing.Point(180, 20);
            this.txtPhysicalName.MaxLength = 32;
            this.txtPhysicalName.Name = "txtPhysicalName";
            this.txtPhysicalName.Size = new System.Drawing.Size(340, 20);
            this.txtPhysicalName.TabIndex = 1;

            // lblLogicalName
            this.lblLogicalName.AutoSize = false;
            this.lblLogicalName.Location = new System.Drawing.Point(20, 55);
            this.lblLogicalName.Name = "lblLogicalName";
            this.lblLogicalName.Size = new System.Drawing.Size(150, 20);
            this.lblLogicalName.TabIndex = 2;
            this.lblLogicalName.Text = "日本語名(論理名) *";
            this.lblLogicalName.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;

            // txtLogicalName
            this.txtLogicalName.Location = new System.Drawing.Point(180, 55);
            this.txtLogicalName.MaxLength = 64;
            this.txtLogicalName.Name = "txtLogicalName";
            this.txtLogicalName.Size = new System.Drawing.Size(340, 20);
            this.txtLogicalName.TabIndex = 3;

            // lblValue
            this.lblValue.AutoSize = false;
            this.lblValue.Location = new System.Drawing.Point(20, 90);
            this.lblValue.Name = "lblValue";
            this.lblValue.Size = new System.Drawing.Size(150, 20);
            this.lblValue.TabIndex = 4;
            this.lblValue.Text = "値 *";
            this.lblValue.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;

            // txtValue
            this.txtValue.Location = new System.Drawing.Point(180, 90);
            this.txtValue.MaxLength = 256;
            this.txtValue.Name = "txtValue";
            this.txtValue.Size = new System.Drawing.Size(340, 20);
            this.txtValue.TabIndex = 5;

            // lblUnit
            this.lblUnit.AutoSize = false;
            this.lblUnit.Location = new System.Drawing.Point(20, 125);
            this.lblUnit.Name = "lblUnit";
            this.lblUnit.Size = new System.Drawing.Size(150, 20);
            this.lblUnit.TabIndex = 6;
            this.lblUnit.Text = "単位";
            this.lblUnit.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;

            // txtUnit
            this.txtUnit.Location = new System.Drawing.Point(180, 125);
            this.txtUnit.MaxLength = 16;
            this.txtUnit.Name = "txtUnit";
            this.txtUnit.Size = new System.Drawing.Size(340, 20);
            this.txtUnit.TabIndex = 7;

            // lblDescription
            this.lblDescription.AutoSize = false;
            this.lblDescription.Location = new System.Drawing.Point(20, 160);
            this.lblDescription.Name = "lblDescription";
            this.lblDescription.Size = new System.Drawing.Size(150, 20);
            this.lblDescription.TabIndex = 8;
            this.lblDescription.Text = "説明";
            this.lblDescription.TextAlign = System.Drawing.ContentAlignment.TopLeft;

            // txtDescription
            this.txtDescription.Location = new System.Drawing.Point(180, 160);
            this.txtDescription.MaxLength = 256;
            this.txtDescription.Multiline = true;
            this.txtDescription.Name = "txtDescription";
            this.txtDescription.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtDescription.Size = new System.Drawing.Size(340, 120);
            this.txtDescription.TabIndex = 9;
            this.txtDescription.WordWrap = true;

            // btnOk
            this.btnOk.DialogResult = System.Windows.Forms.DialogResult.None;
            this.btnOk.Location = new System.Drawing.Point(360, 300);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(80, 30);
            this.btnOk.TabIndex = 10;
            this.btnOk.Text = "OK";
            this.btnOk.UseVisualStyleBackColor = true;
            this.btnOk.Click += new System.EventHandler(this.BtnOk_Click);

            // btnCancel
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(450, 300);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(80, 30);
            this.btnCancel.TabIndex = 11;
            this.btnCancel.Text = "キャンセル";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.BtnCancel_Click);

            // ConstantEditDialog
            this.AcceptButton = null;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(550, 380);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOk);
            this.Controls.Add(this.txtDescription);
            this.Controls.Add(this.lblDescription);
            this.Controls.Add(this.txtUnit);
            this.Controls.Add(this.lblUnit);
            this.Controls.Add(this.txtValue);
            this.Controls.Add(this.lblValue);
            this.Controls.Add(this.txtLogicalName);
            this.Controls.Add(this.lblLogicalName);
            this.Controls.Add(this.txtPhysicalName);
            this.Controls.Add(this.lblPhysicalName);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ConstantEditDialog";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "定数編集";
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion

        private System.Windows.Forms.Label lblPhysicalName;
        private System.Windows.Forms.TextBox txtPhysicalName;
        private System.Windows.Forms.Label lblLogicalName;
        private System.Windows.Forms.TextBox txtLogicalName;
        private System.Windows.Forms.Label lblValue;
        private System.Windows.Forms.TextBox txtValue;
        private System.Windows.Forms.Label lblUnit;
        private System.Windows.Forms.TextBox txtUnit;
        private System.Windows.Forms.Label lblDescription;
        private System.Windows.Forms.TextBox txtDescription;
        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.Button btnCancel;
    }
}
