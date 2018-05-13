using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using System.Xml;

namespace FileMangerV2.Data
{
    /// <summary>
    /// 数据处理类
    /// </summary>
    class DataHelper
    {
        // 数据源地址（xml文件地址）
        private const string DATA_FILE_PATH = @"Data.xml";

        // 文件夹类型节点名称
        private const string DATA_NODE_DIR = @"Dir";
        // 文件类型节点名称
        private const string DATA_NODE_FILE = @"File";

        // key属性字段名称
        private const string DATA_ATTR_KEY = @"key";
        // name属性字段名称
        private const string DATA_ATTR_NAME = @"name";

        // xml文档处理类
        private XmlDocument _doc;
        // 图标数据源，提供界面展示
        private ImageList _imageList;

        public DataHelper()
        {
            // 初始化图标数据源，并加入文件夹图标
            // 图标均来自系统图标
            _imageList = new ImageList();
            _imageList.ImageSize = new System.Drawing.Size(24, 24);   //分别是宽和高
            _imageList.Images.Add(DATA_NODE_DIR, SystemInfoIcon.GetDirectoryIcon(@"", true));

            // 初始化xml文档处理类，加载xml文件
            _doc = new XmlDocument();
            _doc.Load(DATA_FILE_PATH);

        }

        public ImageList ImageList { get => _imageList; }

        /// <summary>
        /// 获取文件夹图标序号，该需要对应_imageList中图标的序号
        /// </summary>
        /// <returns></returns>
        public int GetDirectoryImageIndex()
        {
            return 0;
        }

        /// <summary>
        /// 获取文件图标序号
        /// 根据文件扩展名查看_imageList中是否缓存了改图标，如果有则直接返回图标序号
        /// 反之，则获取文件的系统图标，并缓存入_imageList
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public int GetFileImageIndex(string filePath)
        {
            string fileExtension = Path.GetExtension(filePath);
            int index = _imageList.Images.IndexOfKey(fileExtension);
            if (index < 0)
            {
                _imageList.Images.Add(fileExtension, SystemInfoIcon.GetFileIcon(filePath, true));
                index = _imageList.Images.IndexOfKey(fileExtension);
            }
            return index;
        }

        /// <summary>
        /// 获取整个数据列表
        /// </summary>
        /// <returns></returns>
        public DataEntity GetDataList()
        {
            List<DataEntity> list = GetDataList(_doc.DocumentElement, null);
            if (list == null || list.Count <= 0)
            {
                return null;
            }
            return list[0];
        }

        /// <summary>
        /// 获取某个XmlNode下的数据列表
        /// 注意：其中有递归调用
        /// </summary>
        /// <param name="parentNode"></param>
        /// <param name="parentDataEntity"></param>
        /// <returns></returns>
        public List<DataEntity> GetDataList(XmlNode parentNode, DataEntity parentDataEntity)
        {
            XmlNodeList nodes = parentNode.ChildNodes;
            if (nodes == null || nodes.Count <= 0)
            {
                return null;
            }
            List<DataEntity> list = new List<DataEntity>();
            foreach (XmlNode node in nodes)
            {
                DataEntity dataEntity = new DataEntity();
                dataEntity.Name = node.Attributes[DATA_ATTR_NAME].Value;
                switch (node.Name)
                {
                    case DATA_NODE_DIR:
                        dataEntity.Type = DataEntity.DataType.Dir;
                        dataEntity.ImageIndex = GetDirectoryImageIndex();
                        break;
                    case DATA_NODE_FILE:
                        dataEntity.Type = DataEntity.DataType.File;
                        dataEntity.ImageIndex = GetFileImageIndex(dataEntity.Name);
                        break;
                    default:
                        dataEntity.Type = DataEntity.DataType.Unknown;
                        break;

                }
                
                dataEntity.Key = node.Attributes[DATA_ATTR_KEY].Value;
                dataEntity.Parent = parentDataEntity;
                // 递归调用获取下一级数据列表
                dataEntity.Childs = GetDataList(node, dataEntity);
                list.Add(dataEntity);
            }

            return list;
        }

        /// <summary>
        /// 添加文件路径配置
        /// </summary>
        /// <param name="parentDataEntity">父级节点</param>
        /// <param name="filePath">文件路径</param>
        /// <returns></returns>
        public DataEntity AddFile(DataEntity parentDataEntity, string filePath)
        {
            if (parentDataEntity == null || parentDataEntity.Type != DataEntity.DataType.Dir)
            {
                return null;
            }

            if (filePath == null)
            {
                return null;
            }

            XmlNode parentNode = _doc.SelectSingleNode($"//{DATA_NODE_DIR}[@{DATA_ATTR_KEY}={parentDataEntity.Key}]");
            if (parentNode == null)
            {
                return null;
            }

            DataEntity fileDataEntity = new DataEntity();
            fileDataEntity.Key = DateTime.Now.Ticks.ToString();
            fileDataEntity.Type = DataEntity.DataType.File;
            fileDataEntity.Name = filePath;
            fileDataEntity.Parent = parentDataEntity;
            fileDataEntity.ImageIndex = GetFileImageIndex(Path.GetExtension(filePath));

            if (parentDataEntity.Childs == null)
            {
                parentDataEntity.Childs = new List<DataEntity>();
            }
            parentDataEntity.Childs.Add(fileDataEntity);

            XmlElement fileNode = _doc.CreateElement(DATA_NODE_FILE);
            XmlAttribute keyAttr = _doc.CreateAttribute(DATA_ATTR_KEY);
            keyAttr.Value = fileDataEntity.Key;

            XmlAttribute nameAttr = _doc.CreateAttribute(DATA_ATTR_NAME);
            nameAttr.Value = fileDataEntity.Name;

            fileNode.Attributes.Append(keyAttr);
            fileNode.Attributes.Append(nameAttr);

            parentNode.AppendChild(fileNode);
            _doc.Save(DATA_FILE_PATH);

            
            return fileDataEntity;
        }

        /// <summary>
        /// 删除文件路径配置
        /// </summary>
        /// <param name="fileDataEntity">文件数据实体</param>
        /// <returns></returns>
        public bool RemoveFile(DataEntity fileDataEntity)
        {
            fileDataEntity.Parent.Childs.Remove(fileDataEntity);
            XmlNode itemNode = _doc.SelectSingleNode($"//{DATA_NODE_FILE}[@{DATA_ATTR_KEY}={fileDataEntity.Key}]");
            if (itemNode != null)
            {
                itemNode.ParentNode.RemoveChild(itemNode);
                _doc.Save(DATA_FILE_PATH);
                return true;
            }
            return false;
        }

    }
}
