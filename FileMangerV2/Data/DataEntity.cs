using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FileMangerV2.Data
{
    /// <summary>
    /// 通用数据实体
    /// </summary>
    class DataEntity
    {
        public enum DataType {
            Dir,      // 文件夹类型
            File,     // 文件类型
            Unknown   // 未知类型
        };

        // 数据类型
        private DataType type;
        // 唯一标识（对应xml中的key属性）
        private string key;
        // 名称（对应xml中的name属性）
        private string name;
        // 父节点
        private DataEntity parent;
        // 子节点
        private List<DataEntity> childs;
        // 用于界面图标展示
        private int imageIndex;
       

        public DataType Type { get => type; set => type = value; }
        public string Key { get => key; set => key = value; }
        public string Name { get => name; set => name = value; }
        internal DataEntity Parent { get => parent; set => parent = value; }
        public List<DataEntity> Childs { get => childs; set => childs = value; }
        public int ImageIndex { get => imageIndex; set => imageIndex = value; }
    }
}
