namespace WindowsFormsApp1
{
    partial class UserForm
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
            this.lblForeignWord = new System.Windows.Forms.Label();
            this.txtUserAnswer = new System.Windows.Forms.TextBox();
            this.btnCheck = new System.Windows.Forms.Button();
            this.lblResult = new System.Windows.Forms.Label();
            this.btnNext = new System.Windows.Forms.Button();
            this.lblCategory = new System.Windows.Forms.Label();
            this.lblProgress = new System.Windows.Forms.Label();
            this.btnHelp = new System.Windows.Forms.Button();
            this.lblHelp = new System.Windows.Forms.Label();
            this.chkShuffle = new System.Windows.Forms.CheckBox();
            this.label1 = new System.Windows.Forms.Label();
            this.cmbCategories = new System.Windows.Forms.ComboBox();
            this.SuspendLayout();
            // 
            // lblForeignWord
            // 
            this.lblForeignWord.AutoSize = true;
            this.lblForeignWord.Location = new System.Drawing.Point(54, 106);
            this.lblForeignWord.Name = "lblForeignWord";
            this.lblForeignWord.Size = new System.Drawing.Size(107, 13);
            this.lblForeignWord.TabIndex = 0;
            this.lblForeignWord.Text = "Иностранное слово";
            // 
            // txtUserAnswer
            // 
            this.txtUserAnswer.Location = new System.Drawing.Point(128, 203);
            this.txtUserAnswer.Name = "txtUserAnswer";
            this.txtUserAnswer.Size = new System.Drawing.Size(100, 20);
            this.txtUserAnswer.TabIndex = 1;
            // 
            // btnCheck
            // 
            this.btnCheck.Location = new System.Drawing.Point(128, 259);
            this.btnCheck.Name = "btnCheck";
            this.btnCheck.Size = new System.Drawing.Size(75, 23);
            this.btnCheck.TabIndex = 2;
            this.btnCheck.Text = "Проверить";
            this.btnCheck.UseVisualStyleBackColor = true;
            this.btnCheck.Click += new System.EventHandler(this.btnCheck_Click);
            // 
            // lblResult
            // 
            this.lblResult.AutoSize = true;
            this.lblResult.Location = new System.Drawing.Point(356, 206);
            this.lblResult.Name = "lblResult";
            this.lblResult.Size = new System.Drawing.Size(35, 13);
            this.lblResult.TabIndex = 3;
            this.lblResult.Text = "label1";
            // 
            // btnNext
            // 
            this.btnNext.Enabled = false;
            this.btnNext.Location = new System.Drawing.Point(348, 259);
            this.btnNext.Name = "btnNext";
            this.btnNext.Size = new System.Drawing.Size(146, 23);
            this.btnNext.TabIndex = 4;
            this.btnNext.Text = "Следующее слово";
            this.btnNext.UseVisualStyleBackColor = true;
            this.btnNext.Click += new System.EventHandler(this.btnNext_Click);
            // 
            // lblCategory
            // 
            this.lblCategory.AutoSize = true;
            this.lblCategory.ForeColor = System.Drawing.Color.Gray;
            this.lblCategory.Location = new System.Drawing.Point(257, 49);
            this.lblCategory.Name = "lblCategory";
            this.lblCategory.Size = new System.Drawing.Size(35, 13);
            this.lblCategory.TabIndex = 5;
            this.lblCategory.Text = "label1";
            // 
            // lblProgress
            // 
            this.lblProgress.AutoSize = true;
            this.lblProgress.Location = new System.Drawing.Point(231, 157);
            this.lblProgress.Name = "lblProgress";
            this.lblProgress.Size = new System.Drawing.Size(74, 13);
            this.lblProgress.TabIndex = 6;
            this.lblProgress.Text = "Слово: 0 из 0";
            // 
            // btnHelp
            // 
            this.btnHelp.Location = new System.Drawing.Point(234, 315);
            this.btnHelp.Name = "btnHelp";
            this.btnHelp.Size = new System.Drawing.Size(75, 23);
            this.btnHelp.TabIndex = 7;
            this.btnHelp.Text = "Подсказка";
            this.btnHelp.UseVisualStyleBackColor = true;
            this.btnHelp.Click += new System.EventHandler(this.btnHelp_Click);
            // 
            // lblHelp
            // 
            this.lblHelp.AutoSize = true;
            this.lblHelp.Location = new System.Drawing.Point(274, 106);
            this.lblHelp.Name = "lblHelp";
            this.lblHelp.Size = new System.Drawing.Size(0, 13);
            this.lblHelp.TabIndex = 8;
            // 
            // chkShuffle
            // 
            this.chkShuffle.AutoSize = true;
            this.chkShuffle.Location = new System.Drawing.Point(33, 321);
            this.chkShuffle.Name = "chkShuffle";
            this.chkShuffle.Size = new System.Drawing.Size(135, 17);
            this.chkShuffle.TabIndex = 9;
            this.chkShuffle.Text = "В случайном порядке";
            this.chkShuffle.UseVisualStyleBackColor = true;
            this.chkShuffle.CheckedChanged += new System.EventHandler(this.chkShuffle_CheckedChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(437, 79);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(73, 13);
            this.label1.TabIndex = 10;
            this.label1.Text = "Выбор темы:";
            // 
            // cmbCategories
            // 
            this.cmbCategories.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbCategories.FormattingEnabled = true;
            this.cmbCategories.Location = new System.Drawing.Point(526, 76);
            this.cmbCategories.Name = "cmbCategories";
            this.cmbCategories.Size = new System.Drawing.Size(121, 21);
            this.cmbCategories.TabIndex = 11;
            this.cmbCategories.SelectedIndexChanged += new System.EventHandler(this.cmbCategories_SelectedIndexChanged);
            // 
            // UserForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(659, 383);
            this.Controls.Add(this.cmbCategories);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.chkShuffle);
            this.Controls.Add(this.lblHelp);
            this.Controls.Add(this.btnHelp);
            this.Controls.Add(this.lblProgress);
            this.Controls.Add(this.lblCategory);
            this.Controls.Add(this.btnNext);
            this.Controls.Add(this.lblResult);
            this.Controls.Add(this.btnCheck);
            this.Controls.Add(this.txtUserAnswer);
            this.Controls.Add(this.lblForeignWord);
            this.Name = "UserForm";
            this.Text = "Угадай слово";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblForeignWord;
        private System.Windows.Forms.TextBox txtUserAnswer;
        private System.Windows.Forms.Button btnCheck;
        private System.Windows.Forms.Label lblResult;
        private System.Windows.Forms.Button btnNext;
        private System.Windows.Forms.Label lblCategory;
        private System.Windows.Forms.Label lblProgress;
        private System.Windows.Forms.Button btnHelp;
        private System.Windows.Forms.Label lblHelp;
        private System.Windows.Forms.CheckBox chkShuffle;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cmbCategories;
    }
}