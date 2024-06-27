using System.ComponentModel;
using System.Reflection;

namespace Gui.Util
{
    public static class EnumUtil
    {
        public static string Descripcion(this Enum value)
        {
            FieldInfo field = value.GetType().GetField(value.ToString());
            DescriptionAttribute attribute = (DescriptionAttribute)field.GetCustomAttribute(typeof(DescriptionAttribute));
            return attribute == null ? value.ToString() : attribute.Description;
        }
    }
}