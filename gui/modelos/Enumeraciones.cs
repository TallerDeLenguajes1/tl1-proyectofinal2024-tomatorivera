using System.ComponentModel;

namespace Gui.Modelo
{
    public enum TipoMenu
    {
        [Description("Salir del programa")]
        PRINCIPAL,

        [Description("Volver al menu principal")]
        SECUNDARIO
    }
}