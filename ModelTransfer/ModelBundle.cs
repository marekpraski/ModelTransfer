using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelTransfer
{
    [Serializable]
    public class ModelBundle
    {
        public List<Model2D> models { get; set; }
        public Dictionary<string, ModelDirectory> checkedDirectories { get; set; }



        public ModelBundle()
        {
            models = new List<Model2D>();
            checkedDirectories = new Dictionary<string, ModelDirectory>();
        }

        public void addModel(Model2D model)
        {
            models.Add(model);
        }
        
        public void clear()
        {
            models.Clear();
            checkedDirectories.Clear();
        }

    }
}
