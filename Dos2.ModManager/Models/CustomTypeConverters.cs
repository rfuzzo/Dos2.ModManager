using System;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Collections;
using System.Globalization;
using System.Collections.Generic;

namespace Dos2.ModManager
{
    #region List Converter
    public class ExpandableCollectionPropertyDescriptor : PropertyDescriptor
    {
        private IList collection;
        private readonly int _index;

        public ExpandableCollectionPropertyDescriptor(IList coll, int idx)
            : base(GetDisplayName(coll, idx), null)
        {
            collection = coll;
            _index = idx;
        }

        private static string GetDisplayName(IList list, int index)
        {
            int width = (list.Count + 1).ToString("d").Length;
            return "[" + index.ToString($"D{width}") + "]  " + CSharpName(list[index].GetType());
        }

        private static string CSharpName(Type type)
        {
            var sb = new StringBuilder();
            var name = type.Name;
            if (!type.IsGenericType)
                return name;
            sb.Append(name.Substring(0, name.IndexOf('`')));
            sb.Append("<");
            sb.Append(string.Join(", ", type.GetGenericArguments()
                                            .Select(CSharpName)));
            sb.Append(">");
            return sb.ToString();
        }

        public override bool CanResetValue(object component)
        {
            return true;
        }

        public override Type ComponentType
        {
            get { return this.collection.GetType(); }
        }

        public override object GetValue(object component)
        {
            return collection[_index];
        }

        public override bool IsReadOnly
        {
            get { return false; }
        }

        public override string Name
        {
            get { return _index.ToString(CultureInfo.InvariantCulture); }
        }

        public override Type PropertyType
        {
            get { return collection[_index].GetType(); }
        }

        public override void ResetValue(object component)
        {
        }

        public override bool ShouldSerializeValue(object component)
        {
            return true;
        }

        public override void SetValue(object component, object value)
        {
            collection[_index] = value;
        }
    }

    public class ListConverter : CollectionConverter
    {
        public override bool GetPropertiesSupported(ITypeDescriptorContext context)
        {
            return true;
        }

        public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes)
        {
            IList list = value as IList;
            if (list == null || list.Count == 0)
                return base.GetProperties(context, value, attributes);

            var items = new PropertyDescriptorCollection(null);
            for (int i = 0; i < list.Count; i++)
            {
                object item = list[i];
                items.Add(new ExpandableCollectionPropertyDescriptor(list, i));
            }
            //items.Sort();
            return items;
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (value is IList)
            {
                string displayText = "";
                string myListElementType = ((IList)value).GetType().GetGenericArguments().Single().ToString().Split('.').Last();
                displayText = $"{myListElementType}: {((IList)value).Count.ToString()}";
                return displayText;
            }
            return base.ConvertTo(context, culture, value, destinationType);
        }

    }
    #endregion

   

}
