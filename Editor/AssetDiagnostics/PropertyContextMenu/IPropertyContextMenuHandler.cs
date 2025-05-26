namespace ilodev.stationeersmods.tools.diagnostics
{
    public interface IPropertyContextMenuHandler
    {
        /// <summary>
        /// Gets a ref to a PropertyContextMenuRegistry to add custom
        /// menu contextual menu entries
        /// </summary>
        /// <param name="registry"></param>
        void Register(PropertyContextMenuRegistry registry);
    }
}
