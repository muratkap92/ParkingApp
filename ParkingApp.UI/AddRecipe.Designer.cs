namespace ParkingApp.UI
{
    partial class AddRecipe
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
            this.iTalk_ThemeContainer1 = new iTalk.iTalk_ThemeContainer();
            this.cheStatus = new Ambiance.Ambiance_CheckBox();
            this.cmbType = new Ambiance.Ambiance_ComboBox();
            this.txtCost = new Ambiance.Ambiance_TextBox();
            this.txtMaximumValue = new Ambiance.Ambiance_TextBox();
            this.txtRecipeName = new Ambiance.Ambiance_TextBox();
            this.txtMinimumValue = new Ambiance.Ambiance_TextBox();
            this.ambiance_Label4 = new Ambiance.Ambiance_Label();
            this.ambiance_Label5 = new Ambiance.Ambiance_Label();
            this.ambiance_Label3 = new Ambiance.Ambiance_Label();
            this.ambiance_Label2 = new Ambiance.Ambiance_Label();
            this.ambiance_Label1 = new Ambiance.Ambiance_Label();
            this.btnCancel = new Ambiance.Ambiance_Button_2();
            this.btnSave = new Ambiance.Ambiance_Button_1();
            this.iTalk_ThemeContainer1.SuspendLayout();
            this.SuspendLayout();
            // 
            // iTalk_ThemeContainer1
            // 
            this.iTalk_ThemeContainer1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(246)))), ((int)(((byte)(246)))), ((int)(((byte)(246)))));
            this.iTalk_ThemeContainer1.Controls.Add(this.cheStatus);
            this.iTalk_ThemeContainer1.Controls.Add(this.cmbType);
            this.iTalk_ThemeContainer1.Controls.Add(this.txtCost);
            this.iTalk_ThemeContainer1.Controls.Add(this.txtMaximumValue);
            this.iTalk_ThemeContainer1.Controls.Add(this.txtRecipeName);
            this.iTalk_ThemeContainer1.Controls.Add(this.txtMinimumValue);
            this.iTalk_ThemeContainer1.Controls.Add(this.ambiance_Label4);
            this.iTalk_ThemeContainer1.Controls.Add(this.ambiance_Label5);
            this.iTalk_ThemeContainer1.Controls.Add(this.ambiance_Label3);
            this.iTalk_ThemeContainer1.Controls.Add(this.ambiance_Label2);
            this.iTalk_ThemeContainer1.Controls.Add(this.ambiance_Label1);
            this.iTalk_ThemeContainer1.Controls.Add(this.btnCancel);
            this.iTalk_ThemeContainer1.Controls.Add(this.btnSave);
            this.iTalk_ThemeContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.iTalk_ThemeContainer1.Font = new System.Drawing.Font("Segoe UI", 8F);
            this.iTalk_ThemeContainer1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(142)))), ((int)(((byte)(142)))), ((int)(((byte)(142)))));
            this.iTalk_ThemeContainer1.Location = new System.Drawing.Point(0, 0);
            this.iTalk_ThemeContainer1.Name = "iTalk_ThemeContainer1";
            this.iTalk_ThemeContainer1.Padding = new System.Windows.Forms.Padding(3, 28, 3, 28);
            this.iTalk_ThemeContainer1.Sizable = true;
            this.iTalk_ThemeContainer1.Size = new System.Drawing.Size(405, 398);
            this.iTalk_ThemeContainer1.SmartBounds = false;
            this.iTalk_ThemeContainer1.StartPosition = System.Windows.Forms.FormStartPosition.WindowsDefaultLocation;
            this.iTalk_ThemeContainer1.TabIndex = 0;
            this.iTalk_ThemeContainer1.Text = "Tarife Ekle";
            // 
            // cheStatus
            // 
            this.cheStatus.BackColor = System.Drawing.Color.Transparent;
            this.cheStatus.Checked = false;
            this.cheStatus.Font = new System.Drawing.Font("Segoe UI", 12F);
            this.cheStatus.Location = new System.Drawing.Point(168, 266);
            this.cheStatus.Name = "cheStatus";
            this.cheStatus.Size = new System.Drawing.Size(63, 15);
            this.cheStatus.TabIndex = 6;
            this.cheStatus.Text = "Aktif";
            // 
            // cmbType
            // 
            this.cmbType.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(246)))), ((int)(((byte)(246)))), ((int)(((byte)(246)))));
            this.cmbType.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cmbType.DropDownHeight = 100;
            this.cmbType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbType.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.cmbType.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(142)))), ((int)(((byte)(142)))), ((int)(((byte)(142)))));
            this.cmbType.FormattingEnabled = true;
            this.cmbType.HoverSelectionColor = System.Drawing.Color.Empty;
            this.cmbType.IntegralHeight = false;
            this.cmbType.ItemHeight = 20;
            this.cmbType.Location = new System.Drawing.Point(168, 42);
            this.cmbType.Name = "cmbType";
            this.cmbType.Size = new System.Drawing.Size(135, 26);
            this.cmbType.StartIndex = 0;
            this.cmbType.TabIndex = 5;
            // 
            // txtCost
            // 
            this.txtCost.BackColor = System.Drawing.Color.Transparent;
            this.txtCost.Font = new System.Drawing.Font("Tahoma", 11F);
            this.txtCost.ForeColor = System.Drawing.Color.DimGray;
            this.txtCost.Location = new System.Drawing.Point(168, 214);
            this.txtCost.MaxLength = 32767;
            this.txtCost.Multiline = false;
            this.txtCost.Name = "txtCost";
            this.txtCost.ReadOnly = false;
            this.txtCost.Size = new System.Drawing.Size(135, 28);
            this.txtCost.TabIndex = 4;
            this.txtCost.TextAlignment = System.Windows.Forms.HorizontalAlignment.Left;
            this.txtCost.UseSystemPasswordChar = false;
            this.txtCost.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.CheckNumber);
            // 
            // txtMaximumValue
            // 
            this.txtMaximumValue.BackColor = System.Drawing.Color.Transparent;
            this.txtMaximumValue.Font = new System.Drawing.Font("Tahoma", 11F);
            this.txtMaximumValue.ForeColor = System.Drawing.Color.DimGray;
            this.txtMaximumValue.Location = new System.Drawing.Point(168, 169);
            this.txtMaximumValue.MaxLength = 32767;
            this.txtMaximumValue.Multiline = false;
            this.txtMaximumValue.Name = "txtMaximumValue";
            this.txtMaximumValue.ReadOnly = false;
            this.txtMaximumValue.Size = new System.Drawing.Size(135, 28);
            this.txtMaximumValue.TabIndex = 4;
            this.txtMaximumValue.TextAlignment = System.Windows.Forms.HorizontalAlignment.Left;
            this.txtMaximumValue.UseSystemPasswordChar = false;
            this.txtMaximumValue.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.CheckNumber);
            // 
            // txtRecipeName
            // 
            this.txtRecipeName.BackColor = System.Drawing.Color.Transparent;
            this.txtRecipeName.Font = new System.Drawing.Font("Tahoma", 11F);
            this.txtRecipeName.ForeColor = System.Drawing.Color.DimGray;
            this.txtRecipeName.Location = new System.Drawing.Point(168, 82);
            this.txtRecipeName.MaxLength = 50;
            this.txtRecipeName.Multiline = false;
            this.txtRecipeName.Name = "txtRecipeName";
            this.txtRecipeName.ReadOnly = false;
            this.txtRecipeName.Size = new System.Drawing.Size(135, 28);
            this.txtRecipeName.TabIndex = 4;
            this.txtRecipeName.TextAlignment = System.Windows.Forms.HorizontalAlignment.Left;
            this.txtRecipeName.UseSystemPasswordChar = false;
            // 
            // txtMinimumValue
            // 
            this.txtMinimumValue.BackColor = System.Drawing.Color.Transparent;
            this.txtMinimumValue.Font = new System.Drawing.Font("Tahoma", 11F);
            this.txtMinimumValue.ForeColor = System.Drawing.Color.DimGray;
            this.txtMinimumValue.Location = new System.Drawing.Point(168, 122);
            this.txtMinimumValue.MaxLength = 32767;
            this.txtMinimumValue.Multiline = false;
            this.txtMinimumValue.Name = "txtMinimumValue";
            this.txtMinimumValue.ReadOnly = false;
            this.txtMinimumValue.Size = new System.Drawing.Size(135, 28);
            this.txtMinimumValue.TabIndex = 4;
            this.txtMinimumValue.TextAlignment = System.Windows.Forms.HorizontalAlignment.Left;
            this.txtMinimumValue.UseSystemPasswordChar = false;
            this.txtMinimumValue.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.CheckNumber);
            // 
            // ambiance_Label4
            // 
            this.ambiance_Label4.AutoSize = true;
            this.ambiance_Label4.BackColor = System.Drawing.Color.Transparent;
            this.ambiance_Label4.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.ambiance_Label4.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(76)))), ((int)(((byte)(76)))), ((int)(((byte)(77)))));
            this.ambiance_Label4.Location = new System.Drawing.Point(107, 214);
            this.ambiance_Label4.Name = "ambiance_Label4";
            this.ambiance_Label4.Size = new System.Drawing.Size(55, 20);
            this.ambiance_Label4.TabIndex = 3;
            this.ambiance_Label4.Text = "Ücret  :";
            // 
            // ambiance_Label5
            // 
            this.ambiance_Label5.AutoSize = true;
            this.ambiance_Label5.BackColor = System.Drawing.Color.Transparent;
            this.ambiance_Label5.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.ambiance_Label5.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(76)))), ((int)(((byte)(76)))), ((int)(((byte)(77)))));
            this.ambiance_Label5.Location = new System.Drawing.Point(79, 87);
            this.ambiance_Label5.Name = "ambiance_Label5";
            this.ambiance_Label5.Size = new System.Drawing.Size(83, 20);
            this.ambiance_Label5.TabIndex = 3;
            this.ambiance_Label5.Text = "Tarife Adı  :";
            // 
            // ambiance_Label3
            // 
            this.ambiance_Label3.AutoSize = true;
            this.ambiance_Label3.BackColor = System.Drawing.Color.Transparent;
            this.ambiance_Label3.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.ambiance_Label3.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(76)))), ((int)(((byte)(76)))), ((int)(((byte)(77)))));
            this.ambiance_Label3.Location = new System.Drawing.Point(65, 172);
            this.ambiance_Label3.Name = "ambiance_Label3";
            this.ambiance_Label3.Size = new System.Drawing.Size(97, 20);
            this.ambiance_Label3.TabIndex = 3;
            this.ambiance_Label3.Text = "Bitiş Değeri  :";
            // 
            // ambiance_Label2
            // 
            this.ambiance_Label2.AutoSize = true;
            this.ambiance_Label2.BackColor = System.Drawing.Color.Transparent;
            this.ambiance_Label2.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.ambiance_Label2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(76)))), ((int)(((byte)(76)))), ((int)(((byte)(77)))));
            this.ambiance_Label2.Location = new System.Drawing.Point(30, 125);
            this.ambiance_Label2.Name = "ambiance_Label2";
            this.ambiance_Label2.Size = new System.Drawing.Size(132, 20);
            this.ambiance_Label2.TabIndex = 3;
            this.ambiance_Label2.Text = "Başlangıç Değeri  :";
            // 
            // ambiance_Label1
            // 
            this.ambiance_Label1.AutoSize = true;
            this.ambiance_Label1.BackColor = System.Drawing.Color.Transparent;
            this.ambiance_Label1.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.ambiance_Label1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(76)))), ((int)(((byte)(76)))), ((int)(((byte)(77)))));
            this.ambiance_Label1.Location = new System.Drawing.Point(77, 43);
            this.ambiance_Label1.Name = "ambiance_Label1";
            this.ambiance_Label1.Size = new System.Drawing.Size(85, 20);
            this.ambiance_Label1.TabIndex = 2;
            this.ambiance_Label1.Text = "Tarife Tipi  :";
            // 
            // btnCancel
            // 
            this.btnCancel.BackColor = System.Drawing.Color.Transparent;
            this.btnCancel.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.btnCancel.Image = null;
            this.btnCancel.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnCancel.Location = new System.Drawing.Point(12, 307);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(177, 30);
            this.btnCancel.TabIndex = 1;
            this.btnCancel.Text = "İptal";
            this.btnCancel.TextAlignment = System.Drawing.StringAlignment.Center;
            this.btnCancel.Click += new System.EventHandler(this.BtnCancel_Click);
            // 
            // btnSave
            // 
            this.btnSave.BackColor = System.Drawing.Color.Transparent;
            this.btnSave.Font = new System.Drawing.Font("Segoe UI", 12F);
            this.btnSave.Image = null;
            this.btnSave.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnSave.Location = new System.Drawing.Point(204, 307);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(177, 30);
            this.btnSave.TabIndex = 0;
            this.btnSave.Text = "Kaydet";
            this.btnSave.TextAlignment = System.Drawing.StringAlignment.Center;
            this.btnSave.Click += new System.EventHandler(this.BtnSave_Click);
            // 
            // AddRecipe
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(405, 398);
            this.Controls.Add(this.iTalk_ThemeContainer1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.MinimumSize = new System.Drawing.Size(126, 39);
            this.Name = "AddRecipe";
            this.Text = "Tarife Ekle";
            this.TransparencyKey = System.Drawing.Color.Fuchsia;
            this.iTalk_ThemeContainer1.ResumeLayout(false);
            this.iTalk_ThemeContainer1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private iTalk.iTalk_ThemeContainer iTalk_ThemeContainer1;
        private Ambiance.Ambiance_Button_1 btnSave;
        private Ambiance.Ambiance_Button_2 btnCancel;
        private Ambiance.Ambiance_Label ambiance_Label1;
        private Ambiance.Ambiance_Label ambiance_Label4;
        private Ambiance.Ambiance_Label ambiance_Label3;
        private Ambiance.Ambiance_Label ambiance_Label2;
        private Ambiance.Ambiance_CheckBox cheStatus;
        private Ambiance.Ambiance_ComboBox cmbType;
        private Ambiance.Ambiance_TextBox txtCost;
        private Ambiance.Ambiance_TextBox txtMaximumValue;
        private Ambiance.Ambiance_TextBox txtMinimumValue;
        private Ambiance.Ambiance_TextBox txtRecipeName;
        private Ambiance.Ambiance_Label ambiance_Label5;
    }
}