namespace FileMangerV2
{
    partial class MainForm
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.treeViewDir = new System.Windows.Forms.TreeView();
            this.listViewContent = new System.Windows.Forms.ListView();
            this.SuspendLayout();
            // 
            // treeViewDir
            // 
            this.treeViewDir.Dock = System.Windows.Forms.DockStyle.Left;
            this.treeViewDir.HideSelection = false;
            this.treeViewDir.Location = new System.Drawing.Point(0, 0);
            this.treeViewDir.Name = "treeViewDir";
            this.treeViewDir.Size = new System.Drawing.Size(536, 1054);
            this.treeViewDir.TabIndex = 0;
            this.treeViewDir.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeViewDir_AfterSelect);
            // 
            // listViewContent
            // 
            this.listViewContent.AllowDrop = true;
            this.listViewContent.Dock = System.Windows.Forms.DockStyle.Right;
            this.listViewContent.Location = new System.Drawing.Point(607, 0);
            this.listViewContent.Name = "listViewContent";
            this.listViewContent.Size = new System.Drawing.Size(809, 1054);
            this.listViewContent.TabIndex = 1;
            this.listViewContent.UseCompatibleStateImageBehavior = false;
            this.listViewContent.DragDrop += new System.Windows.Forms.DragEventHandler(this.listViewContent_DragDrop);
            this.listViewContent.DragEnter += new System.Windows.Forms.DragEventHandler(this.listViewContent_DragEnter);
            this.listViewContent.KeyUp += new System.Windows.Forms.KeyEventHandler(this.listViewContent_KeyUp);
            this.listViewContent.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.listViewContent_MouseDoubleClick);
            this.listViewContent.MouseUp += new System.Windows.Forms.MouseEventHandler(this.listViewContent_MouseUp);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 24F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1416, 1054);
            this.Controls.Add(this.listViewContent);
            this.Controls.Add(this.treeViewDir);
            this.Name = "MainForm";
            this.Text = "资源管理器";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TreeView treeViewDir;
        private System.Windows.Forms.ListView listViewContent;
    }
}

