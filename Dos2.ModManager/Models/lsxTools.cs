using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Dos2.ModManager.Models
{
    class LsxTools
    {
        private XDocument doc;


        public LsxTools(XDocument xml)
        {
            doc = xml;
        }



        /// <summary>
        /// Gets xml attributes by name from a list of XElements
        /// </summary>
        /// <param name="list"></param>
        /// <param name="v"></param>
        /// <returns></returns>
        public string GetAttributeByName(List<XElement> list, string v)
        {
            return list.FirstOrDefault(x => x.Attribute("id").Value == v).Attribute("value").Value;
        }
    }
}
