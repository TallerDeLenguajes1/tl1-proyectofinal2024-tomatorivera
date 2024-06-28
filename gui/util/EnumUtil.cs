using System.ComponentModel;
using System.Reflection;

namespace Gui.Util
{
    /// <summary>
    /// Clase de funciones de utilidad para los Enum
    /// </summary>
    public static class EnumUtil
    {
        /// <summary>
        /// Obtiene la que contiene la flag Description de un campo de algún Enum
        /// </summary>
        /// <param name="value">Valor de enumeración a evaluar</param>
        /// <returns>Lo que contenga la flag "Description", o el valor textual del campo si no tiene la flag Description</returns>
        public static string Descripcion(this Enum value)
        {
            FieldInfo field = value.GetType().GetField(value.ToString());
            DescriptionAttribute attribute = (DescriptionAttribute) field.GetCustomAttribute(typeof(DescriptionAttribute));
            return attribute == null ? value.ToString() : attribute.Description;
        }
    }
}