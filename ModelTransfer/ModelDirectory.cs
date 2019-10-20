using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelTransfer
{
    public class ModelDirectory
    {
        public string name { get; set; }
        public string id { get; set; }
        public string parentId { get; set; }
        public List<ModelDirectory> children { get; }

        public ModelDirectory()
        {
            children = new List<ModelDirectory>();
        }

        public void addChild(ModelDirectory dir)
        {
            children.Add(dir);
        }

        public bool isParent()
        {
            return children.Count > 0;
        }
        public int numberOfChildren()
        {
            return children.Count();
        }
    }

}
