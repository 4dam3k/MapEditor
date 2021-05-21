using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapEditor.Models.Schematic
{
    [Serializable]
    public class SaveDataObjectList
    {
        public string SchematicName;
        public string SchematicAuthor;
        public int topLevelInstanceID;
        public List<SchematicData> blocks;
        public SchematicData BlockWithinstanceID(int id)
        {
            SchematicData block = blocks.Find(c => c.DataID == id);
            return block;
        }

        public List<SchematicData> BlocksWithParentID(int id)
        {
            List<SchematicData> newList = blocks.FindAll(c => c.ParentID == id);
            return newList;
        }
    }
}
