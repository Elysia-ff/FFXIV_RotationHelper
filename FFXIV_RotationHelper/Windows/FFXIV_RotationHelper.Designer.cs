namespace FFXIV_RotationHelper
{
    partial class FFXIV_RotationHelper
    {
        /// <summary> 
        /// 필수 디자이너 변수입니다.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// 사용 중인 모든 리소스를 정리합니다.
        /// </summary>
        /// <param name="disposing">관리되는 리소스를 삭제해야 하면 true이고, 그렇지 않으면 false입니다.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region 구성 요소 디자이너에서 생성한 코드

        /// <summary> 
        /// 디자이너 지원에 필요한 메서드입니다. 
        /// 이 메서드의 내용을 코드 편집기로 수정하지 마세요.
        /// </summary>
        private void InitializeComponent()
        {
			this.urlTextBox = new System.Windows.Forms.TextBox();
			this.loadBtn = new System.Windows.Forms.Button();
			this.startBtn = new System.Windows.Forms.Button();
            this.loadBtn = new System.Windows.Forms.Button();
            this.startBtn = new System.Windows.Forms.Button();
#if DEBUG
			this.debugTextBox = new System.Windows.Forms.TextBox();
			this.debugLabel = new System.Windows.Forms.Label();
			this.logLineBox = new System.Windows.Forms.TextBox();
			this.logInsertBtn = new System.Windows.Forms.Button();
#endif
            this.nameTitle = new System.Windows.Forms.Label();
            this.nameText = new System.Windows.Forms.Label();
            this.petTitle = new System.Windows.Forms.Label();
            this.petText = new System.Windows.Forms.Label();
            this.statusLabel = new System.Windows.Forms.Label();
            this.thanksToLabel = new System.Windows.Forms.LinkLabel();
            this.isClickthroughCheckBox = new System.Windows.Forms.CheckBox();
            this.sizeComboBox = new System.Windows.Forms.ComboBox();
            this.sizeTitle = new System.Windows.Forms.Label();
            this.saveBtn = new System.Windows.Forms.Button();
            this.gvSelectedUrls = new System.Windows.Forms.DataGridView();
            this.URL = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Memo = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Loop = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.textBox1 = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.gvSelectedUrls)).BeginInit();
            this.SuspendLayout();
            // 
            // loadBtn
            // 
            this.loadBtn.Location = new System.Drawing.Point(511, 48);
            this.loadBtn.Name = "loadBtn";
            this.loadBtn.Size = new System.Drawing.Size(64, 25);
            this.loadBtn.TabIndex = 1;
            this.loadBtn.Text = "Load";
            this.loadBtn.UseVisualStyleBackColor = true;
            this.loadBtn.Click += new System.EventHandler(this.LoadBtn_Click);
            // 
            // startBtn
            // 
            this.startBtn.Enabled = false;
            this.startBtn.Location = new System.Drawing.Point(10, 194);
            this.startBtn.Name = "startBtn";
            this.startBtn.Size = new System.Drawing.Size(64, 25);
            this.startBtn.TabIndex = 2;
            this.startBtn.Text = "Start";
            this.startBtn.UseVisualStyleBackColor = true;
            this.startBtn.Click += new System.EventHandler(this.StartBtn_Click);
#if DEBUG
            // 
            // debugTextBox
            // 
            this.debugTextBox.Location = new System.Drawing.Point(10, 262);
            this.debugTextBox.Name = "debugTextBox";
            this.debugTextBox.Size = new System.Drawing.Size(245, 20);
            this.debugTextBox.TabIndex = 3;
            this.debugTextBox.TextChanged += new System.EventHandler(this.DebugTextBox_TextChanged);
            // 
            // debugLabel
            // 
            this.debugLabel.AutoSize = true;
            this.debugLabel.Location = new System.Drawing.Point(8, 299);
            this.debugLabel.Name = "debugLabel";
            this.debugLabel.Size = new System.Drawing.Size(13, 13);
            this.debugLabel.TabIndex = 4;
            this.debugLabel.Text = "=";
            // 
            // logLineBox
            // 
            this.logLineBox.Location = new System.Drawing.Point(10, 393);
            this.logLineBox.Name = "logLineBox";
            this.logLineBox.Size = new System.Drawing.Size(245, 20);
            this.logLineBox.TabIndex = 17;
            // 
            // logInsertBtn
            // 
            this.logInsertBtn.Location = new System.Drawing.Point(259, 391);
            this.logInsertBtn.Name = "logInsertBtn";
            this.logInsertBtn.Size = new System.Drawing.Size(64, 25);
            this.logInsertBtn.TabIndex = 18;
            this.logInsertBtn.Text = "Insert";
            this.logInsertBtn.UseVisualStyleBackColor = true;
            this.logInsertBtn.Click += new System.EventHandler(this.LogInsertBtn_Click);
#endif
            // 
            // nameTitle
            // 
            this.nameTitle.AutoSize = true;
            this.nameTitle.Location = new System.Drawing.Point(13, 141);
            this.nameTitle.Name = "nameTitle";
            this.nameTitle.Size = new System.Drawing.Size(41, 13);
            this.nameTitle.TabIndex = 5;
            this.nameTitle.Text = "Name :";
            // 
            // nameText
            // 
            this.nameText.AutoSize = true;
            this.nameText.Location = new System.Drawing.Point(59, 141);
            this.nameText.Name = "nameText";
            this.nameText.Size = new System.Drawing.Size(57, 13);
            this.nameText.TabIndex = 6;
            this.nameText.Text = "Not Found";
            // 
            // petTitle
            // 
            this.petTitle.AutoSize = true;
            this.petTitle.Location = new System.Drawing.Point(13, 168);
            this.petTitle.Name = "petTitle";
            this.petTitle.Size = new System.Drawing.Size(41, 13);
            this.petTitle.TabIndex = 7;
            this.petTitle.Text = "Pet     :";
            // 
            // petText
            // 
            this.petText.AutoSize = true;
            this.petText.Location = new System.Drawing.Point(59, 168);
            this.petText.Name = "petText";
            this.petText.Size = new System.Drawing.Size(57, 13);
            this.petText.TabIndex = 9;
            this.petText.Text = "Not Found";
            // 
            // statusLabel
            // 
            this.statusLabel.AutoSize = true;
            this.statusLabel.Location = new System.Drawing.Point(10, 114);
            this.statusLabel.Name = "statusLabel";
            this.statusLabel.Size = new System.Drawing.Size(76, 13);
            this.statusLabel.TabIndex = 10;
            this.statusLabel.Text = "Not Initialized..";
            // 
            // thanksToLabel
            // 
            this.thanksToLabel.AutoSize = true;
            this.thanksToLabel.Location = new System.Drawing.Point(292, 114);
            this.thanksToLabel.Name = "thanksToLabel";
            this.thanksToLabel.Size = new System.Drawing.Size(140, 13);
            this.thanksToLabel.TabIndex = 11;
            this.thanksToLabel.TabStop = true;
            this.thanksToLabel.Text = "Thanks to ffxivrotations.com";
            this.thanksToLabel.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.ThanksToLabel_LinkClicked);
            // 
            // isClickthroughCheckBox
            // 
            this.isClickthroughCheckBox.AutoSize = true;
            this.isClickthroughCheckBox.Location = new System.Drawing.Point(341, 141);
            this.isClickthroughCheckBox.Name = "isClickthroughCheckBox";
            this.isClickthroughCheckBox.Size = new System.Drawing.Size(88, 17);
            this.isClickthroughCheckBox.TabIndex = 12;
            this.isClickthroughCheckBox.Text = "Click-through";
            this.isClickthroughCheckBox.UseVisualStyleBackColor = true;
            this.isClickthroughCheckBox.CheckedChanged += new System.EventHandler(this.IsClickthroughCheckBox_CheckedChanged);
            // 
            // sizeComboBox
            // 
            this.sizeComboBox.FormattingEnabled = true;
            this.sizeComboBox.Items.AddRange(new object[] {
            "60",
            "80",
            "90",
            "100",
            "110",
            "120",
            "140",
            "160",
            "180",
            "200"});
            this.sizeComboBox.Location = new System.Drawing.Point(341, 163);
            this.sizeComboBox.Name = "sizeComboBox";
            this.sizeComboBox.Size = new System.Drawing.Size(74, 21);
            this.sizeComboBox.TabIndex = 14;
            this.sizeComboBox.SelectedIndexChanged += new System.EventHandler(this.SizeComboBox_SelectedIndexChanged);
            // 
            // sizeTitle
            // 
            this.sizeTitle.AutoSize = true;
            this.sizeTitle.Location = new System.Drawing.Point(417, 166);
            this.sizeTitle.Name = "sizeTitle";
            this.sizeTitle.Size = new System.Drawing.Size(15, 13);
            this.sizeTitle.TabIndex = 15;
            this.sizeTitle.Text = "%";
            // 
            // saveBtn
            // 
            this.saveBtn.Location = new System.Drawing.Point(511, 17);
            this.saveBtn.Name = "saveBtn";
            this.saveBtn.Size = new System.Drawing.Size(80, 25);
            this.saveBtn.TabIndex = 16;
            this.saveBtn.Text = "Saved URLs";
            this.saveBtn.UseVisualStyleBackColor = true;
            this.saveBtn.Click += new System.EventHandler(this.SaveBtn_Click);
            // 
            // gvSelectedUrls
            // 
            this.gvSelectedUrls.AllowDrop = true;
            this.gvSelectedUrls.BackgroundColor = System.Drawing.Color.White;
            this.gvSelectedUrls.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.gvSelectedUrls.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.URL,
            this.Memo,
            this.Loop});
            this.gvSelectedUrls.Location = new System.Drawing.Point(13, 3);
            this.gvSelectedUrls.Name = "gvSelectedUrls";
            this.gvSelectedUrls.RowTemplate.Height = 23;
            this.gvSelectedUrls.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.gvSelectedUrls.Size = new System.Drawing.Size(471, 108);
            this.gvSelectedUrls.TabIndex = 19;
            this.gvSelectedUrls.CellEndEdit += new System.Windows.Forms.DataGridViewCellEventHandler(this.GvSelectedUrls_CellEndEdit);
            this.gvSelectedUrls.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.GvSelectedUrls_CellValueChanged);
            this.gvSelectedUrls.CurrentCellDirtyStateChanged += new System.EventHandler(this.GvSelectedUrls_CurrentCellDirtyStateChanged);
            this.gvSelectedUrls.RowsRemoved += new System.Windows.Forms.DataGridViewRowsRemovedEventHandler(this.GvSelectedUrls_RowsRemoved);
            this.gvSelectedUrls.VisibleChanged += new System.EventHandler(this.GvSelectedUrls_VisibleChanged);
            this.gvSelectedUrls.DragDrop += new System.Windows.Forms.DragEventHandler(this.GvSelectedUrls_DragDrop);
            this.gvSelectedUrls.DragOver += new System.Windows.Forms.DragEventHandler(this.GvSelectedUrls_DragOver);
            this.gvSelectedUrls.MouseDown += new System.Windows.Forms.MouseEventHandler(this.GvSelectedUrls_MouseDown);
            this.gvSelectedUrls.MouseMove += new System.Windows.Forms.MouseEventHandler(this.GvSelectedUrls_MouseMove);
            // 
            // URL
            // 
            this.URL.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.URL.FillWeight = 50F;
            this.URL.HeaderText = "URL";
            this.URL.Name = "URL";
            this.URL.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // Memo
            // 
            this.Memo.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.Memo.FillWeight = 50F;
            this.Memo.HeaderText = "Memo";
            this.Memo.Name = "Memo";
            this.Memo.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // Loop
            // 
            this.Loop.HeaderText = "Loop Rotation?";
            this.Loop.Name = "Loop";
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(10, 226);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.ReadOnly = true;
            this.textBox1.Size = new System.Drawing.Size(474, 30);
            this.textBox1.TabIndex = 20;
            this.textBox1.Text = "Drag and drop URL entries to re-order.  Check Loop to enable looping individual r" +
    "otations.";
            // 
            // FFXIV_RotationHelper
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.gvSelectedUrls);
#if DEBUG
			this.Controls.Add(this.debugLabel);
			this.Controls.Add(this.debugTextBox);
			this.Controls.Add(this.logInsertBtn);
			this.Controls.Add(this.logLineBox);
#endif
            this.Controls.Add(this.saveBtn);
            this.Controls.Add(this.sizeTitle);
            this.Controls.Add(this.sizeComboBox);
            this.Controls.Add(this.isClickthroughCheckBox);
            this.Controls.Add(this.thanksToLabel);
            this.Controls.Add(this.statusLabel);
            this.Controls.Add(this.petText);
            this.Controls.Add(this.petTitle);
            this.Controls.Add(this.nameText);
            this.Controls.Add(this.nameTitle);
            this.Controls.Add(this.startBtn);
            this.Controls.Add(this.loadBtn);
            this.Name = "FFXIV_RotationHelper";
            this.Size = new System.Drawing.Size(604, 443);
            ((System.ComponentModel.ISupportInitialize)(this.gvSelectedUrls)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

#endregion
        private System.Windows.Forms.Button loadBtn;
        private System.Windows.Forms.Button startBtn;
#if DEBUG
        private System.Windows.Forms.TextBox debugTextBox;
        private System.Windows.Forms.Label debugLabel;
        private System.Windows.Forms.TextBox logLineBox;
        private System.Windows.Forms.Button logInsertBtn;
#endif
        private System.Windows.Forms.Label nameTitle;
        private System.Windows.Forms.Label nameText;
        private System.Windows.Forms.Label petTitle;
        private System.Windows.Forms.Label petText;
        private System.Windows.Forms.Label statusLabel;
        private System.Windows.Forms.LinkLabel thanksToLabel;
        private System.Windows.Forms.CheckBox isClickthroughCheckBox;
        private System.Windows.Forms.ComboBox sizeComboBox;
        private System.Windows.Forms.Label sizeTitle;
        private System.Windows.Forms.Button saveBtn;
        private System.Windows.Forms.DataGridView gvSelectedUrls;
        private System.Windows.Forms.DataGridViewTextBoxColumn URL;
        private System.Windows.Forms.DataGridViewTextBoxColumn Memo;
        private System.Windows.Forms.DataGridViewCheckBoxColumn Loop;
        private System.Windows.Forms.TextBox textBox1;
    }
}
