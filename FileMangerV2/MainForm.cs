using FileMangerV2.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
/// <summary>
/// 描述：
/// 该程序用于在一个固定的目录层级结构下管理文件的快捷方式。
/// 用户可以在给定的任意目录中添加和删除文件，文件以快捷方式的形式保存在程序中。
/// 在程序中双击文件则可以直接将其打开。
/// 作者：dole
/// 日期：2018-5-13
/// </summary>
namespace FileMangerV2
{
    public partial class MainForm : Form
    {
        // 数据处理类
        private DataHelper _dataHelper;
        // 当前选中的文件夹节点（TreeViewDir.SelectedItem）
        private TreeNode _currentSelectedTreeNode;
        public MainForm()
        {
            InitializeComponent();
            // 初始化数据处理类
            _dataHelper = new DataHelper();
            // 初始化界面
            InitView();
        }
        /// <summary>
        /// 初始化界面
        /// </summary>
        private void InitView()
        {
            /*   treeViewDir为左侧文件夹列表展示控件
             *   listViewContent为右侧文件夹中详细内容的列表展示控件
             */

            // 设置图标源
            listViewContent.LargeImageList = _dataHelper.ImageList;
            treeViewDir.ImageList = _dataHelper.ImageList;
            // 加载数据
            DataEntity dataEntity = _dataHelper.GetDataList();   
            if(dataEntity != null)
            {
                treeViewDir.Nodes.Add(DataToTreeNode(dataEntity));
            }
        }
        /// <summary>
        /// 将数据转化为树状文件夹列表，用于填充treeViewDir。
        /// </summary>
        /// <param name="dataEntity">数据</param>
        /// <returns></returns>
        private TreeNode DataToTreeNode(DataEntity dataEntity)
        {
            TreeNode node = new TreeNode();
            node.Tag = dataEntity;
            node.Text = dataEntity.Name;
            node.ImageIndex = dataEntity.ImageIndex;

            if (dataEntity.Childs != null)
            {
                foreach (DataEntity data in dataEntity.Childs)
                {
                    if (data.Type != DataEntity.DataType.Dir)
                    {
                        continue;
                    }
                    node.Nodes.Add(DataToTreeNode(data)); 
                }
            }
            return node;
        }

        /// <summary>
        /// 将数据转化为文件夹内容列表，用于填充listViewContent。
        /// </summary>
        /// <param name="dataEntity">数据</param>
        /// <returns></returns>
        private List<ListViewItem> DataToListViewItems(DataEntity dataEntity)
        {
            List<ListViewItem> listViewItems = new List<ListViewItem>();
            if (dataEntity.Childs != null)
            {
                foreach (DataEntity data in dataEntity.Childs)
                {
                    ListViewItem item = new ListViewItem();
                    item.Text = Path.GetFileName(data.Name);
                    item.ImageIndex = data.ImageIndex;
                    item.Tag = data;
                    listViewItems.Add(item);
                }
            }
            return listViewItems;
        }

        /// <summary>
        /// 界面左侧文件夹列表被选中后，将文件夹的内容更新到右侧详细内容列表中
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void treeViewDir_AfterSelect(object sender, TreeViewEventArgs e)
        {
            _currentSelectedTreeNode = e.Node;
            DataEntity dataEntity = (DataEntity)_currentSelectedTreeNode.Tag;
            if (dataEntity != null)
            {
                listViewContent.Items.Clear();
                List<ListViewItem> listViewItems = DataToListViewItems(dataEntity);
                foreach (ListViewItem item in listViewItems)
                {
                    listViewContent.Items.Add(item);
                }
            }
        }

        /// <summary>
        /// 拖动文件到详细内容列表控件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void listViewContent_DragEnter(object sender, DragEventArgs e)
        {
            //判断拖来的是否是文件，是则将拖动源中的数据连接到控件
            if (e.Data.GetDataPresent(DataFormats.FileDrop))     
                e.Effect = DragDropEffects.Link;                
            else e.Effect = DragDropEffects.None;
        }

        /// <summary>
        /// 完成拖放动作后，将拖入的合法的文件路径保存并添加至界面显示
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void listViewContent_DragDrop(object sender, DragEventArgs e)
        {
            Array files = (System.Array)e.Data.GetData(DataFormats.FileDrop);
            foreach (object file in files)
            {
                string filePath = file.ToString();
                // 只有文件类型才能被拖入
                if(File.Exists(filePath))
                {
                    // 数据保存
                    DataEntity dataEntity = _dataHelper.AddFile((DataEntity)_currentSelectedTreeNode.Tag, filePath);
                    // 更新界面
                    ListViewItem item = new ListViewItem();
                    item.Text = Path.GetFileName(dataEntity.Name);
                    item.ImageIndex = dataEntity.ImageIndex;
                    item.Tag = dataEntity;
                    listViewContent.Items.Add(item);
                }
            }
        }
                
        /// <summary>
        /// 在详细内容界面中
        /// 双击文件夹，则进入该文件夹的详细内容页面
        /// 双击文件，则进入打开该文件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void listViewContent_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (listViewContent.SelectedItems.Count <= 0)
                return;
            
            ListViewItem selectItem = listViewContent.SelectedItems[0];
            DataEntity dataEntity = (DataEntity)selectItem.Tag;
            if (dataEntity.Type == DataEntity.DataType.File)
            {
                // 双击文件
                System.Diagnostics.Process.Start(dataEntity.Name);
            }
            else if (dataEntity.Type == DataEntity.DataType.Dir)
            {
                // 双击文件夹，通过更新左侧文件夹列表控件的选择项，来更新右侧内容显示
                // 复用逻辑，简化代码也保持业务逻辑的一致性
                foreach (TreeNode node in _currentSelectedTreeNode.Nodes)
                {
                    if (node.Tag == dataEntity)
                    {
                        treeViewDir.SelectedNode = node;
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// 主要用于监控鼠标右键点击
        /// 点击空白区域，则返回上一级文件夹
        /// 点击文件，则打开文件所在文件夹
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void listViewContent_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                // 是否点击了空白
                if (listViewContent.SelectedItems.Count <= 0)
                {
                    // 返回上一级
                    if(_currentSelectedTreeNode.Parent != null)
                    {
                        treeViewDir.SelectedNode = _currentSelectedTreeNode.Parent;
                    }
                    return;
                }
                // 打开文件所在文件夹
                ListViewItem selectedItem = listViewContent.SelectedItems[0];
                DataEntity dataEntity = (DataEntity)selectedItem.Tag;
                if (dataEntity.Type == DataEntity.DataType.File)
                {
                    string dirPath = Path.GetDirectoryName(dataEntity.Name);
                    System.Diagnostics.Process.Start(dirPath);
                }
            }
        }

        /// <summary>
        /// 选择文件，点击删除按钮，则删除保存的文件路径配置
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void listViewContent_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
            {
                if (listViewContent.SelectedItems.Count <= 0)
                {
                    return;
                }
                ListViewItem selectedItem = listViewContent.SelectedItems[0];
                DataEntity dataEntity = (DataEntity)selectedItem.Tag;
                if (dataEntity.Type == DataEntity.DataType.File && _dataHelper.RemoveFile(dataEntity))
                {
                    listViewContent.Items.Remove(selectedItem);
                }
            }
        }
    }
}
