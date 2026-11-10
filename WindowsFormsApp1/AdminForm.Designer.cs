namespace WindowsFormsApp1
{
    partial class AdminForm
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
            this.dgvWords = new System.Windows.Forms.DataGridView();
            this.txtWord = new System.Windows.Forms.TextBox();
            this.txtTranslation = new System.Windows.Forms.TextBox();
            this.txtCategory = new System.Windows.Forms.TextBox();
            this.btnAdd = new System.Windows.Forms.Button();
            this.btnDelete = new System.Windows.Forms.Button();
            this.btnRefresh = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.txtExample = new System.Windows.Forms.TextBox();
            this.labelExample = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.dgvWords)).BeginInit();
            this.SuspendLayout();
            // 
            // dgvWords
            // 
            this.dgvWords.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvWords.Location = new System.Drawing.Point(285, 12);
            this.dgvWords.Name = "dgvWords";
            this.dgvWords.Size = new System.Drawing.Size(503, 312);
            this.dgvWords.TabIndex = 0;
            this.dgvWords.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvWords_CellClick);
            // 
            // txtWord
            // 
            this.txtWord.Location = new System.Drawing.Point(25, 39);
            this.txtWord.Name = "txtWord";
            this.txtWord.Size = new System.Drawing.Size(100, 20);
            this.txtWord.TabIndex = 1;
            // 
            // txtTranslation
            // 
            this.txtTranslation.Location = new System.Drawing.Point(25, 98);
            this.txtTranslation.Name = "txtTranslation";
            this.txtTranslation.Size = new System.Drawing.Size(100, 20);
            this.txtTranslation.TabIndex = 2;
            // 
            // txtCategory
            // 
            this.txtCategory.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.txtCategory.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.CustomSource;
            this.txtCategory.Location = new System.Drawing.Point(25, 160);
            this.txtCategory.Name = "txtCategory";
            this.txtCategory.Size = new System.Drawing.Size(100, 20);
            this.txtCategory.TabIndex = 3;
            // 
            // btnAdd
            // 
            this.btnAdd.Location = new System.Drawing.Point(388, 347);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(75, 23);
            this.btnAdd.TabIndex = 4;
            this.btnAdd.Text = "Добавить";
            this.btnAdd.UseVisualStyleBackColor = true;
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
            // 
            // btnDelete
            // 
            this.btnDelete.Location = new System.Drawing.Point(501, 347);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(75, 23);
            this.btnDelete.TabIndex = 5;
            this.btnDelete.Text = "Удалить";
            this.btnDelete.UseVisualStyleBackColor = true;
            this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
            // 
            // btnRefresh
            // 
            this.btnRefresh.Location = new System.Drawing.Point(616, 347);
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size(75, 23);
            this.btnRefresh.TabIndex = 6;
            this.btnRefresh.Text = "Обновить";
            this.btnRefresh.UseVisualStyleBackColor = true;
            this.btnRefresh.Click += new System.EventHandler(this.btnRefresh_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(19, 20);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(107, 13);
            this.label1.TabIndex = 7;
            this.label1.Text = "Иностранное слово";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(20, 84);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(110, 13);
            this.label2.TabIndex = 8;
            this.label2.Text = "Перевод на русский";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(22, 144);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(60, 13);
            this.label3.TabIndex = 9;
            this.label3.Text = "Категория";
            // 
            // txtExample
            // 
            this.txtExample.Location = new System.Drawing.Point(25, 219);
            this.txtExample.Name = "txtExample";
            this.txtExample.Size = new System.Drawing.Size(217, 20);
            this.txtExample.TabIndex = 10;
            // 
            // labelExample
            // 
            this.labelExample.AutoSize = true;
            this.labelExample.Location = new System.Drawing.Point(22, 203);
            this.labelExample.Name = "labelExample";
            this.labelExample.Size = new System.Drawing.Size(127, 13);
            this.labelExample.TabIndex = 11;
            this.labelExample.Text = "Пример в предложении";
            // 
            // AdminForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.labelExample);
            this.Controls.Add(this.txtExample);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnRefresh);
            this.Controls.Add(this.btnDelete);
            this.Controls.Add(this.btnAdd);
            this.Controls.Add(this.txtCategory);
            this.Controls.Add(this.txtTranslation);
            this.Controls.Add(this.txtWord);
            this.Controls.Add(this.dgvWords);
            this.Name = "AdminForm";
            this.Text = "Администратор";
            ((System.ComponentModel.ISupportInitialize)(this.dgvWords)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView dgvWords;
        private System.Windows.Forms.TextBox txtWord;
        private System.Windows.Forms.TextBox txtTranslation;
        private System.Windows.Forms.TextBox txtCategory;
        private System.Windows.Forms.Button btnAdd;
        private System.Windows.Forms.Button btnDelete;
        private System.Windows.Forms.Button btnRefresh;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtExample;
        private System.Windows.Forms.Label labelExample;
    }
}