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
            this.debugTextBox = new System.Windows.Forms.TextBox();
            this.debugLabel = new System.Windows.Forms.Label();
            this.nameTitle = new System.Windows.Forms.Label();
            this.nameText = new System.Windows.Forms.Label();
            this.petTitle = new System.Windows.Forms.Label();
            this.petText = new System.Windows.Forms.Label();
            this.statusLabel = new System.Windows.Forms.Label();
            this.thanksToLabel = new System.Windows.Forms.LinkLabel();
            this.SuspendLayout();
            // 
            // urlTextBox
            // 
            this.urlTextBox.Location = new System.Drawing.Point(16, 17);
            this.urlTextBox.Name = "urlTextBox";
            this.urlTextBox.Size = new System.Drawing.Size(493, 21);
            this.urlTextBox.TabIndex = 0;
            this.urlTextBox.TextChanged += new System.EventHandler(this.URLTextBox_TextChanged);
            // 
            // loadBtn
            // 
            this.loadBtn.Location = new System.Drawing.Point(515, 16);
            this.loadBtn.Name = "loadBtn";
            this.loadBtn.Size = new System.Drawing.Size(75, 23);
            this.loadBtn.TabIndex = 1;
            this.loadBtn.Text = "Load";
            this.loadBtn.UseVisualStyleBackColor = true;
            this.loadBtn.Click += new System.EventHandler(this.LoadBtn_Click);
            // 
            // startBtn
            // 
            this.startBtn.Enabled = false;
            this.startBtn.Location = new System.Drawing.Point(16, 119);
            this.startBtn.Name = "startBtn";
            this.startBtn.Size = new System.Drawing.Size(75, 23);
            this.startBtn.TabIndex = 2;
            this.startBtn.Text = "Start";
            this.startBtn.UseVisualStyleBackColor = true;
            this.startBtn.Click += new System.EventHandler(this.StartBtn_Click);
            // 
            // debugTextBox
            // 
            this.debugTextBox.Location = new System.Drawing.Point(224, 96);
            this.debugTextBox.Name = "debugTextBox";
            this.debugTextBox.Size = new System.Drawing.Size(285, 21);
            this.debugTextBox.TabIndex = 3;
            this.debugTextBox.TextChanged += new System.EventHandler(this.DebugTextBox_TextChanged);
            // 
            // debugLabel
            // 
            this.debugLabel.AutoSize = true;
            this.debugLabel.Location = new System.Drawing.Point(222, 130);
            this.debugLabel.Name = "debugLabel";
            this.debugLabel.Size = new System.Drawing.Size(11, 12);
            this.debugLabel.TabIndex = 4;
            this.debugLabel.Text = "=";
            // 
            // nameTitle
            // 
            this.nameTitle.AutoSize = true;
            this.nameTitle.Location = new System.Drawing.Point(20, 70);
            this.nameTitle.Name = "nameTitle";
            this.nameTitle.Size = new System.Drawing.Size(47, 12);
            this.nameTitle.TabIndex = 5;
            this.nameTitle.Text = "Name :";
            // 
            // nameText
            // 
            this.nameText.AutoSize = true;
            this.nameText.Location = new System.Drawing.Point(73, 70);
            this.nameText.Name = "nameText";
            this.nameText.Size = new System.Drawing.Size(63, 12);
            this.nameText.TabIndex = 6;
            this.nameText.Text = "Not Found";
            // 
            // petTitle
            // 
            this.petTitle.AutoSize = true;
            this.petTitle.Location = new System.Drawing.Point(20, 95);
            this.petTitle.Name = "petTitle";
            this.petTitle.Size = new System.Drawing.Size(47, 12);
            this.petTitle.TabIndex = 7;
            this.petTitle.Text = "Pet     :";
            // 
            // petText
            // 
            this.petText.AutoSize = true;
            this.petText.Location = new System.Drawing.Point(73, 95);
            this.petText.Name = "petText";
            this.petText.Size = new System.Drawing.Size(63, 12);
            this.petText.TabIndex = 9;
            this.petText.Text = "Not Found";
            // 
            // statusLabel
            // 
            this.statusLabel.AutoSize = true;
            this.statusLabel.Location = new System.Drawing.Point(16, 45);
            this.statusLabel.Name = "statusLabel";
            this.statusLabel.Size = new System.Drawing.Size(89, 12);
            this.statusLabel.TabIndex = 10;
            this.statusLabel.Text = "Not Initialized..";
            // 
            // thanksToLabel
            // 
            this.thanksToLabel.AutoSize = true;
            this.thanksToLabel.Location = new System.Drawing.Point(345, 45);
            this.thanksToLabel.Name = "thanksToLabel";
            this.thanksToLabel.Size = new System.Drawing.Size(164, 12);
            this.thanksToLabel.TabIndex = 11;
            this.thanksToLabel.TabStop = true;
            this.thanksToLabel.Text = "Thanks to ffxivrotations.com";
            this.thanksToLabel.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.ThanksToLabel_LinkClicked);
            // 
            // FFXIV_RotationHelper
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.thanksToLabel);
            this.Controls.Add(this.statusLabel);
            this.Controls.Add(this.petText);
            this.Controls.Add(this.petTitle);
            this.Controls.Add(this.nameText);
            this.Controls.Add(this.nameTitle);
            this.Controls.Add(this.debugLabel);
            this.Controls.Add(this.debugTextBox);
            this.Controls.Add(this.startBtn);
            this.Controls.Add(this.loadBtn);
            this.Controls.Add(this.urlTextBox);
            this.Name = "FFXIV_RotationHelper";
            this.Size = new System.Drawing.Size(705, 409);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox urlTextBox;
        private System.Windows.Forms.Button loadBtn;
        private System.Windows.Forms.Button startBtn;
        private System.Windows.Forms.TextBox debugTextBox;
        private System.Windows.Forms.Label debugLabel;
        private System.Windows.Forms.Label nameTitle;
        private System.Windows.Forms.Label nameText;
        private System.Windows.Forms.Label petTitle;
        private System.Windows.Forms.Label petText;
        private System.Windows.Forms.Label statusLabel;
        private System.Windows.Forms.LinkLabel thanksToLabel;
    }
}
