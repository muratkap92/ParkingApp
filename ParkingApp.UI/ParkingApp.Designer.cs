namespace ParkingApp.UI
{
    partial class ParkingApp
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
            this.iTalk_ControlBox1 = new iTalk.iTalk_ControlBox();
            this.iTalk_TabControl1 = new iTalk.iTalk_TabControl();
            this.tabHome = new System.Windows.Forms.TabPage();
            this.btnEnter = new Ambiance.Ambiance_Button_1();
            this.iTalk_Label3 = new iTalk.iTalk_Label();
            this.iTalk_Label2 = new iTalk.iTalk_Label();
            this.btnExit = new Ambiance.Ambiance_Button_1();
            this.txtLicense = new Ambiance.Ambiance_TextBox();
            this.txtSearch = new Ambiance.Ambiance_TextBox();
            this.dataGridCars = new System.Windows.Forms.DataGridView();
            this.tabAbone = new System.Windows.Forms.TabPage();
            this.ambiance_Panel1 = new Ambiance.Ambiance_Panel();
            this.radioBtnSubscriptions = new Ambiance.Ambiance_RadioButton();
            this.radioBtnSubscribers = new Ambiance.Ambiance_RadioButton();
            this.btnAddSubscription = new Ambiance.Ambiance_Button_1();
            this.iTalk_Label1 = new iTalk.iTalk_Label();
            this.btnDelete = new Ambiance.Ambiance_Button_2();
            this.btnEdit = new Ambiance.Ambiance_Button_1();
            this.dataGridSubscribers = new System.Windows.Forms.DataGridView();
            this.txtSearchSub = new Ambiance.Ambiance_TextBox();
            this.btnAddSubs = new Ambiance.Ambiance_Button_1();
            this.tabRecipe = new System.Windows.Forms.TabPage();
            this.iTalk_Label4 = new iTalk.iTalk_Label();
            this.btnRecipeDelete = new Ambiance.Ambiance_Button_2();
            this.btnRecipeUpdate = new Ambiance.Ambiance_Button_1();
            this.txtSearchRecipe = new Ambiance.Ambiance_TextBox();
            this.dataGridRecipes = new System.Windows.Forms.DataGridView();
            this.btnAddRecipe = new Ambiance.Ambiance_Button_1();
            this.tabReport = new System.Windows.Forms.TabPage();
            this.dataGridReport = new System.Windows.Forms.DataGridView();
            this.cmbReportType = new Ambiance.Ambiance_ComboBox();
            this.ambiance_Label1 = new Ambiance.Ambiance_Label();
            this.txtReportLicense = new Ambiance.Ambiance_TextBox();
            this.ambiance_Label2 = new Ambiance.Ambiance_Label();
            this.btnReportList = new Ambiance.Ambiance_Button_1();
            this.iTalk_ThemeContainer1.SuspendLayout();
            this.iTalk_TabControl1.SuspendLayout();
            this.tabHome.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridCars)).BeginInit();
            this.tabAbone.SuspendLayout();
            this.ambiance_Panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridSubscribers)).BeginInit();
            this.tabRecipe.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridRecipes)).BeginInit();
            this.tabReport.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridReport)).BeginInit();
            this.SuspendLayout();
            // 
            // iTalk_ThemeContainer1
            // 
            this.iTalk_ThemeContainer1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(246)))), ((int)(((byte)(246)))), ((int)(((byte)(246)))));
            this.iTalk_ThemeContainer1.Controls.Add(this.iTalk_ControlBox1);
            this.iTalk_ThemeContainer1.Controls.Add(this.iTalk_TabControl1);
            this.iTalk_ThemeContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.iTalk_ThemeContainer1.Font = new System.Drawing.Font("Segoe UI", 8F);
            this.iTalk_ThemeContainer1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(142)))), ((int)(((byte)(142)))), ((int)(((byte)(142)))));
            this.iTalk_ThemeContainer1.Location = new System.Drawing.Point(0, 0);
            this.iTalk_ThemeContainer1.Name = "iTalk_ThemeContainer1";
            this.iTalk_ThemeContainer1.Padding = new System.Windows.Forms.Padding(3, 28, 3, 28);
            this.iTalk_ThemeContainer1.Sizable = true;
            this.iTalk_ThemeContainer1.Size = new System.Drawing.Size(835, 482);
            this.iTalk_ThemeContainer1.SmartBounds = false;
            this.iTalk_ThemeContainer1.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.iTalk_ThemeContainer1.TabIndex = 0;
            this.iTalk_ThemeContainer1.Text = "Parking App";
            // 
            // iTalk_ControlBox1
            // 
            this.iTalk_ControlBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.iTalk_ControlBox1.BackColor = System.Drawing.Color.Transparent;
            this.iTalk_ControlBox1.Location = new System.Drawing.Point(754, -1);
            this.iTalk_ControlBox1.Name = "iTalk_ControlBox1";
            this.iTalk_ControlBox1.Size = new System.Drawing.Size(77, 19);
            this.iTalk_ControlBox1.TabIndex = 1;
            this.iTalk_ControlBox1.Text = "iTalk_ControlBox1";
            // 
            // iTalk_TabControl1
            // 
            this.iTalk_TabControl1.Alignment = System.Windows.Forms.TabAlignment.Left;
            this.iTalk_TabControl1.Controls.Add(this.tabHome);
            this.iTalk_TabControl1.Controls.Add(this.tabAbone);
            this.iTalk_TabControl1.Controls.Add(this.tabRecipe);
            this.iTalk_TabControl1.Controls.Add(this.tabReport);
            this.iTalk_TabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.iTalk_TabControl1.DrawMode = System.Windows.Forms.TabDrawMode.OwnerDrawFixed;
            this.iTalk_TabControl1.ItemSize = new System.Drawing.Size(44, 135);
            this.iTalk_TabControl1.Location = new System.Drawing.Point(3, 28);
            this.iTalk_TabControl1.Multiline = true;
            this.iTalk_TabControl1.Name = "iTalk_TabControl1";
            this.iTalk_TabControl1.SelectedIndex = 0;
            this.iTalk_TabControl1.Size = new System.Drawing.Size(829, 426);
            this.iTalk_TabControl1.SizeMode = System.Windows.Forms.TabSizeMode.Fixed;
            this.iTalk_TabControl1.TabIndex = 0;
            // 
            // tabHome
            // 
            this.tabHome.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(246)))), ((int)(((byte)(246)))), ((int)(((byte)(246)))));
            this.tabHome.Controls.Add(this.btnEnter);
            this.tabHome.Controls.Add(this.iTalk_Label3);
            this.tabHome.Controls.Add(this.iTalk_Label2);
            this.tabHome.Controls.Add(this.btnExit);
            this.tabHome.Controls.Add(this.txtLicense);
            this.tabHome.Controls.Add(this.txtSearch);
            this.tabHome.Controls.Add(this.dataGridCars);
            this.tabHome.Location = new System.Drawing.Point(139, 4);
            this.tabHome.Name = "tabHome";
            this.tabHome.Padding = new System.Windows.Forms.Padding(3);
            this.tabHome.Size = new System.Drawing.Size(686, 418);
            this.tabHome.TabIndex = 0;
            this.tabHome.Text = "Ana Sayfa";
            // 
            // btnEnter
            // 
            this.btnEnter.BackColor = System.Drawing.Color.Transparent;
            this.btnEnter.Font = new System.Drawing.Font("Segoe UI", 12F);
            this.btnEnter.Image = null;
            this.btnEnter.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnEnter.Location = new System.Drawing.Point(290, 22);
            this.btnEnter.Name = "btnEnter";
            this.btnEnter.Size = new System.Drawing.Size(177, 30);
            this.btnEnter.TabIndex = 10;
            this.btnEnter.Text = "Araç Girişi";
            this.btnEnter.TextAlignment = System.Drawing.StringAlignment.Center;
            this.btnEnter.Click += new System.EventHandler(this.BtnEnter_Click);
            // 
            // iTalk_Label3
            // 
            this.iTalk_Label3.AutoSize = true;
            this.iTalk_Label3.BackColor = System.Drawing.Color.Transparent;
            this.iTalk_Label3.Font = new System.Drawing.Font("Segoe UI", 8F);
            this.iTalk_Label3.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(142)))), ((int)(((byte)(142)))), ((int)(((byte)(142)))));
            this.iTalk_Label3.Location = new System.Drawing.Point(6, 105);
            this.iTalk_Label3.Name = "iTalk_Label3";
            this.iTalk_Label3.Size = new System.Drawing.Size(143, 13);
            this.iTalk_Label3.TabIndex = 9;
            this.iTalk_Label3.Text = "Plaka No ile arama yapınız.";
            // 
            // iTalk_Label2
            // 
            this.iTalk_Label2.AutoSize = true;
            this.iTalk_Label2.BackColor = System.Drawing.Color.Transparent;
            this.iTalk_Label2.Font = new System.Drawing.Font("Segoe UI", 8F);
            this.iTalk_Label2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(142)))), ((int)(((byte)(142)))), ((int)(((byte)(142)))));
            this.iTalk_Label2.Location = new System.Drawing.Point(287, 108);
            this.iTalk_Label2.Name = "iTalk_Label2";
            this.iTalk_Label2.Size = new System.Drawing.Size(217, 13);
            this.iTalk_Label2.TabIndex = 9;
            this.iTalk_Label2.Text = "Listeden seçerek araç çıkış  işlemi yapınız.";
            // 
            // btnExit
            // 
            this.btnExit.BackColor = System.Drawing.Color.Transparent;
            this.btnExit.Font = new System.Drawing.Font("Segoe UI", 12F);
            this.btnExit.Image = null;
            this.btnExit.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnExit.Location = new System.Drawing.Point(290, 74);
            this.btnExit.Name = "btnExit";
            this.btnExit.Size = new System.Drawing.Size(177, 30);
            this.btnExit.TabIndex = 7;
            this.btnExit.Text = "Araç Çıkış";
            this.btnExit.TextAlignment = System.Drawing.StringAlignment.Center;
            this.btnExit.Click += new System.EventHandler(this.BtnExit_Click);
            // 
            // txtLicense
            // 
            this.txtLicense.BackColor = System.Drawing.Color.Transparent;
            this.txtLicense.Casing = System.Windows.Forms.CharacterCasing.Upper;
            this.txtLicense.Font = new System.Drawing.Font("Tahoma", 11F);
            this.txtLicense.ForeColor = System.Drawing.Color.DimGray;
            this.txtLicense.Location = new System.Drawing.Point(9, 22);
            this.txtLicense.MaxLength = 32767;
            this.txtLicense.Multiline = false;
            this.txtLicense.Name = "txtLicense";
            this.txtLicense.ReadOnly = false;
            this.txtLicense.Size = new System.Drawing.Size(275, 28);
            this.txtLicense.TabIndex = 6;
            this.txtLicense.TextAlignment = System.Windows.Forms.HorizontalAlignment.Left;
            this.txtLicense.UseSystemPasswordChar = false;
            // 
            // txtSearch
            // 
            this.txtSearch.BackColor = System.Drawing.Color.Transparent;
            this.txtSearch.Casing = System.Windows.Forms.CharacterCasing.Upper;
            this.txtSearch.Font = new System.Drawing.Font("Tahoma", 11F);
            this.txtSearch.ForeColor = System.Drawing.Color.DimGray;
            this.txtSearch.Location = new System.Drawing.Point(9, 74);
            this.txtSearch.MaxLength = 32767;
            this.txtSearch.Multiline = false;
            this.txtSearch.Name = "txtSearch";
            this.txtSearch.ReadOnly = false;
            this.txtSearch.Size = new System.Drawing.Size(275, 28);
            this.txtSearch.TabIndex = 6;
            this.txtSearch.TextAlignment = System.Windows.Forms.HorizontalAlignment.Left;
            this.txtSearch.UseSystemPasswordChar = false;
            this.txtSearch.TextChanged += new System.EventHandler(this.TxtSearch_TextChanged);
            // 
            // dataGridCars
            // 
            this.dataGridCars.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridCars.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.dataGridCars.Location = new System.Drawing.Point(3, 156);
            this.dataGridCars.Name = "dataGridCars";
            this.dataGridCars.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridCars.Size = new System.Drawing.Size(680, 259);
            this.dataGridCars.TabIndex = 0;
            // 
            // tabAbone
            // 
            this.tabAbone.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(246)))), ((int)(((byte)(246)))), ((int)(((byte)(246)))));
            this.tabAbone.Controls.Add(this.ambiance_Panel1);
            this.tabAbone.Controls.Add(this.btnAddSubscription);
            this.tabAbone.Controls.Add(this.iTalk_Label1);
            this.tabAbone.Controls.Add(this.btnDelete);
            this.tabAbone.Controls.Add(this.btnEdit);
            this.tabAbone.Controls.Add(this.dataGridSubscribers);
            this.tabAbone.Controls.Add(this.txtSearchSub);
            this.tabAbone.Controls.Add(this.btnAddSubs);
            this.tabAbone.Location = new System.Drawing.Point(139, 4);
            this.tabAbone.Name = "tabAbone";
            this.tabAbone.Padding = new System.Windows.Forms.Padding(3);
            this.tabAbone.Size = new System.Drawing.Size(686, 418);
            this.tabAbone.TabIndex = 1;
            this.tabAbone.Text = "Aboneler";
            // 
            // ambiance_Panel1
            // 
            this.ambiance_Panel1.BackColor = System.Drawing.Color.White;
            this.ambiance_Panel1.Controls.Add(this.radioBtnSubscriptions);
            this.ambiance_Panel1.Controls.Add(this.radioBtnSubscribers);
            this.ambiance_Panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.ambiance_Panel1.Font = new System.Drawing.Font("Tahoma", 9F);
            this.ambiance_Panel1.Location = new System.Drawing.Point(3, 119);
            this.ambiance_Panel1.Name = "ambiance_Panel1";
            this.ambiance_Panel1.Size = new System.Drawing.Size(680, 23);
            this.ambiance_Panel1.TabIndex = 11;
            this.ambiance_Panel1.Text = "ambiance_Panel1";
            // 
            // radioBtnSubscriptions
            // 
            this.radioBtnSubscriptions.BackColor = System.Drawing.Color.Transparent;
            this.radioBtnSubscriptions.Checked = false;
            this.radioBtnSubscriptions.Font = new System.Drawing.Font("Segoe UI", 12F);
            this.radioBtnSubscriptions.Location = new System.Drawing.Point(105, 3);
            this.radioBtnSubscriptions.Name = "radioBtnSubscriptions";
            this.radioBtnSubscriptions.Size = new System.Drawing.Size(115, 15);
            this.radioBtnSubscriptions.TabIndex = 9;
            this.radioBtnSubscriptions.Text = "Abonelikler";
            this.radioBtnSubscriptions.CheckedChanged += new Ambiance.Ambiance_RadioButton.CheckedChangedEventHandler(this.RadioBtnSubscriptions_CheckedChanged);
            // 
            // radioBtnSubscribers
            // 
            this.radioBtnSubscribers.BackColor = System.Drawing.Color.Transparent;
            this.radioBtnSubscribers.Checked = false;
            this.radioBtnSubscribers.Font = new System.Drawing.Font("Segoe UI", 12F);
            this.radioBtnSubscribers.Location = new System.Drawing.Point(3, 3);
            this.radioBtnSubscribers.Name = "radioBtnSubscribers";
            this.radioBtnSubscribers.Size = new System.Drawing.Size(85, 15);
            this.radioBtnSubscribers.TabIndex = 9;
            this.radioBtnSubscribers.Text = "Aboneler";
            this.radioBtnSubscribers.CheckedChanged += new Ambiance.Ambiance_RadioButton.CheckedChangedEventHandler(this.RadioBtnSubscribers_CheckedChanged);
            // 
            // btnAddSubscription
            // 
            this.btnAddSubscription.BackColor = System.Drawing.Color.Transparent;
            this.btnAddSubscription.Font = new System.Drawing.Font("Segoe UI", 12F);
            this.btnAddSubscription.Image = null;
            this.btnAddSubscription.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnAddSubscription.Location = new System.Drawing.Point(199, 16);
            this.btnAddSubscription.Name = "btnAddSubscription";
            this.btnAddSubscription.Size = new System.Drawing.Size(177, 30);
            this.btnAddSubscription.TabIndex = 6;
            this.btnAddSubscription.Text = "Abonelik Tanımla";
            this.btnAddSubscription.TextAlignment = System.Drawing.StringAlignment.Center;
            this.btnAddSubscription.Click += new System.EventHandler(this.BtnAddSubscription_Click);
            // 
            // iTalk_Label1
            // 
            this.iTalk_Label1.AutoSize = true;
            this.iTalk_Label1.BackColor = System.Drawing.Color.Transparent;
            this.iTalk_Label1.Font = new System.Drawing.Font("Segoe UI", 8F);
            this.iTalk_Label1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(142)))), ((int)(((byte)(142)))), ((int)(((byte)(142)))));
            this.iTalk_Label1.Location = new System.Drawing.Point(292, 93);
            this.iTalk_Label1.Name = "iTalk_Label1";
            this.iTalk_Label1.Size = new System.Drawing.Size(270, 13);
            this.iTalk_Label1.TabIndex = 5;
            this.iTalk_Label1.Text = "Listeden seçtiğiniz kayıt ile ilgili işlem yapabilirsiniz.";
            // 
            // btnDelete
            // 
            this.btnDelete.BackColor = System.Drawing.Color.Transparent;
            this.btnDelete.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.btnDelete.Image = null;
            this.btnDelete.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnDelete.Location = new System.Drawing.Point(479, 60);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(164, 30);
            this.btnDelete.TabIndex = 4;
            this.btnDelete.Text = "Sil";
            this.btnDelete.TextAlignment = System.Drawing.StringAlignment.Center;
            this.btnDelete.Click += new System.EventHandler(this.BtnDelete_Click);
            // 
            // btnEdit
            // 
            this.btnEdit.BackColor = System.Drawing.Color.Transparent;
            this.btnEdit.Font = new System.Drawing.Font("Segoe UI", 12F);
            this.btnEdit.Image = null;
            this.btnEdit.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnEdit.Location = new System.Drawing.Point(295, 60);
            this.btnEdit.Name = "btnEdit";
            this.btnEdit.Size = new System.Drawing.Size(177, 30);
            this.btnEdit.TabIndex = 3;
            this.btnEdit.Text = "Güncelle";
            this.btnEdit.TextAlignment = System.Drawing.StringAlignment.Center;
            this.btnEdit.Click += new System.EventHandler(this.BtnEdit_Click);
            // 
            // dataGridSubscribers
            // 
            this.dataGridSubscribers.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridSubscribers.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.dataGridSubscribers.Location = new System.Drawing.Point(3, 142);
            this.dataGridSubscribers.Name = "dataGridSubscribers";
            this.dataGridSubscribers.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridSubscribers.Size = new System.Drawing.Size(680, 273);
            this.dataGridSubscribers.TabIndex = 2;
            // 
            // txtSearchSub
            // 
            this.txtSearchSub.BackColor = System.Drawing.Color.Transparent;
            this.txtSearchSub.Font = new System.Drawing.Font("Tahoma", 11F);
            this.txtSearchSub.ForeColor = System.Drawing.Color.DimGray;
            this.txtSearchSub.Location = new System.Drawing.Point(14, 62);
            this.txtSearchSub.MaxLength = 32767;
            this.txtSearchSub.Multiline = false;
            this.txtSearchSub.Name = "txtSearchSub";
            this.txtSearchSub.ReadOnly = false;
            this.txtSearchSub.Size = new System.Drawing.Size(275, 28);
            this.txtSearchSub.TabIndex = 1;
            this.txtSearchSub.TextAlignment = System.Windows.Forms.HorizontalAlignment.Left;
            this.txtSearchSub.UseSystemPasswordChar = false;
            this.txtSearchSub.TextChanged += new System.EventHandler(this.TxtSarchSub_TextChanged);
            // 
            // btnAddSubs
            // 
            this.btnAddSubs.BackColor = System.Drawing.Color.Transparent;
            this.btnAddSubs.Font = new System.Drawing.Font("Segoe UI", 12F);
            this.btnAddSubs.Image = null;
            this.btnAddSubs.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnAddSubs.Location = new System.Drawing.Point(16, 16);
            this.btnAddSubs.Name = "btnAddSubs";
            this.btnAddSubs.Size = new System.Drawing.Size(177, 30);
            this.btnAddSubs.TabIndex = 0;
            this.btnAddSubs.Text = "Abone Ekle";
            this.btnAddSubs.TextAlignment = System.Drawing.StringAlignment.Center;
            this.btnAddSubs.Click += new System.EventHandler(this.BtnAddSubs_Click);
            // 
            // tabRecipe
            // 
            this.tabRecipe.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(246)))), ((int)(((byte)(246)))), ((int)(((byte)(246)))));
            this.tabRecipe.Controls.Add(this.iTalk_Label4);
            this.tabRecipe.Controls.Add(this.btnRecipeDelete);
            this.tabRecipe.Controls.Add(this.btnRecipeUpdate);
            this.tabRecipe.Controls.Add(this.txtSearchRecipe);
            this.tabRecipe.Controls.Add(this.dataGridRecipes);
            this.tabRecipe.Controls.Add(this.btnAddRecipe);
            this.tabRecipe.Location = new System.Drawing.Point(139, 4);
            this.tabRecipe.Name = "tabRecipe";
            this.tabRecipe.Padding = new System.Windows.Forms.Padding(3);
            this.tabRecipe.Size = new System.Drawing.Size(686, 418);
            this.tabRecipe.TabIndex = 2;
            this.tabRecipe.Text = "Tarifeler";
            // 
            // iTalk_Label4
            // 
            this.iTalk_Label4.AutoSize = true;
            this.iTalk_Label4.BackColor = System.Drawing.Color.Transparent;
            this.iTalk_Label4.Font = new System.Drawing.Font("Segoe UI", 8F);
            this.iTalk_Label4.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(142)))), ((int)(((byte)(142)))), ((int)(((byte)(142)))));
            this.iTalk_Label4.Location = new System.Drawing.Point(295, 108);
            this.iTalk_Label4.Name = "iTalk_Label4";
            this.iTalk_Label4.Size = new System.Drawing.Size(270, 13);
            this.iTalk_Label4.TabIndex = 9;
            this.iTalk_Label4.Text = "Listeden seçtiğiniz kayıt ile ilgili işlem yapabilirsiniz.";
            // 
            // btnRecipeDelete
            // 
            this.btnRecipeDelete.BackColor = System.Drawing.Color.Transparent;
            this.btnRecipeDelete.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.btnRecipeDelete.Image = null;
            this.btnRecipeDelete.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnRecipeDelete.Location = new System.Drawing.Point(482, 75);
            this.btnRecipeDelete.Name = "btnRecipeDelete";
            this.btnRecipeDelete.Size = new System.Drawing.Size(164, 30);
            this.btnRecipeDelete.TabIndex = 8;
            this.btnRecipeDelete.Text = "Sil";
            this.btnRecipeDelete.TextAlignment = System.Drawing.StringAlignment.Center;
            this.btnRecipeDelete.Click += new System.EventHandler(this.BtnRecipeDelete_Click);
            // 
            // btnRecipeUpdate
            // 
            this.btnRecipeUpdate.BackColor = System.Drawing.Color.Transparent;
            this.btnRecipeUpdate.Font = new System.Drawing.Font("Segoe UI", 12F);
            this.btnRecipeUpdate.Image = null;
            this.btnRecipeUpdate.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnRecipeUpdate.Location = new System.Drawing.Point(298, 75);
            this.btnRecipeUpdate.Name = "btnRecipeUpdate";
            this.btnRecipeUpdate.Size = new System.Drawing.Size(177, 30);
            this.btnRecipeUpdate.TabIndex = 7;
            this.btnRecipeUpdate.Text = "Güncelle";
            this.btnRecipeUpdate.TextAlignment = System.Drawing.StringAlignment.Center;
            this.btnRecipeUpdate.Click += new System.EventHandler(this.BtnRecipeUpdate_Click);
            // 
            // txtSearchRecipe
            // 
            this.txtSearchRecipe.BackColor = System.Drawing.Color.Transparent;
            this.txtSearchRecipe.Font = new System.Drawing.Font("Tahoma", 11F);
            this.txtSearchRecipe.ForeColor = System.Drawing.Color.DimGray;
            this.txtSearchRecipe.Location = new System.Drawing.Point(17, 77);
            this.txtSearchRecipe.MaxLength = 32767;
            this.txtSearchRecipe.Multiline = false;
            this.txtSearchRecipe.Name = "txtSearchRecipe";
            this.txtSearchRecipe.ReadOnly = false;
            this.txtSearchRecipe.Size = new System.Drawing.Size(275, 28);
            this.txtSearchRecipe.TabIndex = 6;
            this.txtSearchRecipe.TextAlignment = System.Windows.Forms.HorizontalAlignment.Left;
            this.txtSearchRecipe.UseSystemPasswordChar = false;
            this.txtSearchRecipe.TextChanged += new System.EventHandler(this.TxtSearchRecipe_TextChanged);
            // 
            // dataGridRecipes
            // 
            this.dataGridRecipes.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridRecipes.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.dataGridRecipes.Location = new System.Drawing.Point(3, 142);
            this.dataGridRecipes.Name = "dataGridRecipes";
            this.dataGridRecipes.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridRecipes.Size = new System.Drawing.Size(680, 273);
            this.dataGridRecipes.TabIndex = 1;
            // 
            // btnAddRecipe
            // 
            this.btnAddRecipe.BackColor = System.Drawing.Color.Transparent;
            this.btnAddRecipe.Font = new System.Drawing.Font("Segoe UI", 12F);
            this.btnAddRecipe.Image = null;
            this.btnAddRecipe.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnAddRecipe.Location = new System.Drawing.Point(21, 18);
            this.btnAddRecipe.Name = "btnAddRecipe";
            this.btnAddRecipe.Size = new System.Drawing.Size(177, 30);
            this.btnAddRecipe.TabIndex = 0;
            this.btnAddRecipe.Text = "Tarife Ekle";
            this.btnAddRecipe.TextAlignment = System.Drawing.StringAlignment.Center;
            this.btnAddRecipe.Click += new System.EventHandler(this.BtnAddRecipe_Click);
            // 
            // tabReport
            // 
            this.tabReport.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(246)))), ((int)(((byte)(246)))), ((int)(((byte)(246)))));
            this.tabReport.Controls.Add(this.btnReportList);
            this.tabReport.Controls.Add(this.ambiance_Label2);
            this.tabReport.Controls.Add(this.txtReportLicense);
            this.tabReport.Controls.Add(this.ambiance_Label1);
            this.tabReport.Controls.Add(this.cmbReportType);
            this.tabReport.Controls.Add(this.dataGridReport);
            this.tabReport.Location = new System.Drawing.Point(139, 4);
            this.tabReport.Name = "tabReport";
            this.tabReport.Padding = new System.Windows.Forms.Padding(3);
            this.tabReport.Size = new System.Drawing.Size(686, 418);
            this.tabReport.TabIndex = 3;
            this.tabReport.Text = "Rapor";
            // 
            // dataGridReport
            // 
            this.dataGridReport.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridReport.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.dataGridReport.Location = new System.Drawing.Point(3, 102);
            this.dataGridReport.Name = "dataGridReport";
            this.dataGridReport.Size = new System.Drawing.Size(680, 313);
            this.dataGridReport.TabIndex = 0;
            // 
            // cmbReportType
            // 
            this.cmbReportType.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(246)))), ((int)(((byte)(246)))), ((int)(((byte)(246)))));
            this.cmbReportType.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cmbReportType.DropDownHeight = 100;
            this.cmbReportType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbReportType.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.cmbReportType.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(142)))), ((int)(((byte)(142)))), ((int)(((byte)(142)))));
            this.cmbReportType.FormattingEnabled = true;
            this.cmbReportType.HoverSelectionColor = System.Drawing.Color.Empty;
            this.cmbReportType.IntegralHeight = false;
            this.cmbReportType.ItemHeight = 20;
            this.cmbReportType.Location = new System.Drawing.Point(101, 12);
            this.cmbReportType.Name = "cmbReportType";
            this.cmbReportType.Size = new System.Drawing.Size(135, 26);
            this.cmbReportType.StartIndex = 0;
            this.cmbReportType.TabIndex = 1;
            this.cmbReportType.SelectedIndexChanged += new System.EventHandler(this.CmbReportType_SelectedIndexChanged);
            // 
            // ambiance_Label1
            // 
            this.ambiance_Label1.AutoSize = true;
            this.ambiance_Label1.BackColor = System.Drawing.Color.Transparent;
            this.ambiance_Label1.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.ambiance_Label1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(76)))), ((int)(((byte)(76)))), ((int)(((byte)(77)))));
            this.ambiance_Label1.Location = new System.Drawing.Point(6, 13);
            this.ambiance_Label1.Name = "ambiance_Label1";
            this.ambiance_Label1.Size = new System.Drawing.Size(89, 20);
            this.ambiance_Label1.TabIndex = 2;
            this.ambiance_Label1.Text = "Rapor Tipi  :";
            // 
            // txtReportLicense
            // 
            this.txtReportLicense.BackColor = System.Drawing.Color.Transparent;
            this.txtReportLicense.Font = new System.Drawing.Font("Tahoma", 11F);
            this.txtReportLicense.ForeColor = System.Drawing.Color.DimGray;
            this.txtReportLicense.Location = new System.Drawing.Point(369, 10);
            this.txtReportLicense.MaxLength = 32767;
            this.txtReportLicense.Multiline = false;
            this.txtReportLicense.Name = "txtReportLicense";
            this.txtReportLicense.ReadOnly = false;
            this.txtReportLicense.Size = new System.Drawing.Size(135, 28);
            this.txtReportLicense.TabIndex = 3;
            this.txtReportLicense.TextAlignment = System.Windows.Forms.HorizontalAlignment.Left;
            this.txtReportLicense.UseSystemPasswordChar = false;
            // 
            // ambiance_Label2
            // 
            this.ambiance_Label2.AutoSize = true;
            this.ambiance_Label2.BackColor = System.Drawing.Color.Transparent;
            this.ambiance_Label2.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.ambiance_Label2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(76)))), ((int)(((byte)(76)))), ((int)(((byte)(77)))));
            this.ambiance_Label2.Location = new System.Drawing.Point(292, 13);
            this.ambiance_Label2.Name = "ambiance_Label2";
            this.ambiance_Label2.Size = new System.Drawing.Size(55, 20);
            this.ambiance_Label2.TabIndex = 4;
            this.ambiance_Label2.Text = "Plaka  :";
            // 
            // btnReportList
            // 
            this.btnReportList.BackColor = System.Drawing.Color.Transparent;
            this.btnReportList.Font = new System.Drawing.Font("Segoe UI", 12F);
            this.btnReportList.Image = null;
            this.btnReportList.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnReportList.Location = new System.Drawing.Point(59, 55);
            this.btnReportList.Name = "btnReportList";
            this.btnReportList.Size = new System.Drawing.Size(177, 30);
            this.btnReportList.TabIndex = 5;
            this.btnReportList.Text = "Listele";
            this.btnReportList.TextAlignment = System.Drawing.StringAlignment.Center;
            this.btnReportList.Click += new System.EventHandler(this.BtnReportList_Click);
            // 
            // ParkingApp
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(835, 482);
            this.Controls.Add(this.iTalk_ThemeContainer1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.MinimumSize = new System.Drawing.Size(126, 39);
            this.Name = "ParkingApp";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Parking App";
            this.TransparencyKey = System.Drawing.Color.Fuchsia;
            this.iTalk_ThemeContainer1.ResumeLayout(false);
            this.iTalk_TabControl1.ResumeLayout(false);
            this.tabHome.ResumeLayout(false);
            this.tabHome.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridCars)).EndInit();
            this.tabAbone.ResumeLayout(false);
            this.tabAbone.PerformLayout();
            this.ambiance_Panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridSubscribers)).EndInit();
            this.tabRecipe.ResumeLayout(false);
            this.tabRecipe.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridRecipes)).EndInit();
            this.tabReport.ResumeLayout(false);
            this.tabReport.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridReport)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private iTalk.iTalk_ThemeContainer iTalk_ThemeContainer1;
        private iTalk.iTalk_TabControl iTalk_TabControl1;
        private System.Windows.Forms.TabPage tabHome;
        private System.Windows.Forms.TabPage tabAbone;
        private iTalk.iTalk_ControlBox iTalk_ControlBox1;
        private iTalk.iTalk_Label iTalk_Label1;
        private Ambiance.Ambiance_Button_2 btnDelete;
        private Ambiance.Ambiance_Button_1 btnEdit;
        private System.Windows.Forms.DataGridView dataGridSubscribers;
        private Ambiance.Ambiance_TextBox txtSearchSub;
        private Ambiance.Ambiance_Button_1 btnAddSubs;
        private System.Windows.Forms.TabPage tabRecipe;
        private Ambiance.Ambiance_Button_1 btnEnter;
        private iTalk.iTalk_Label iTalk_Label2;
        private Ambiance.Ambiance_Button_1 btnExit;
        private Ambiance.Ambiance_TextBox txtSearch;
        private System.Windows.Forms.DataGridView dataGridCars;
        private iTalk.iTalk_Label iTalk_Label3;
        private Ambiance.Ambiance_TextBox txtLicense;
        private Ambiance.Ambiance_Button_1 btnAddRecipe;
        private Ambiance.Ambiance_Button_1 btnAddSubscription;
        private System.Windows.Forms.DataGridView dataGridRecipes;
        private Ambiance.Ambiance_RadioButton radioBtnSubscribers;
        private Ambiance.Ambiance_Panel ambiance_Panel1;
        private Ambiance.Ambiance_RadioButton radioBtnSubscriptions;
        private iTalk.iTalk_Label iTalk_Label4;
        private Ambiance.Ambiance_Button_2 btnRecipeDelete;
        private Ambiance.Ambiance_Button_1 btnRecipeUpdate;
        private Ambiance.Ambiance_TextBox txtSearchRecipe;
        private System.Windows.Forms.TabPage tabReport;
        private Ambiance.Ambiance_Label ambiance_Label2;
        private Ambiance.Ambiance_TextBox txtReportLicense;
        private Ambiance.Ambiance_Label ambiance_Label1;
        private Ambiance.Ambiance_ComboBox cmbReportType;
        private System.Windows.Forms.DataGridView dataGridReport;
        private Ambiance.Ambiance_Button_1 btnReportList;
    }
}