namespace MvcNakamasCloud.ViewModels
{
    public class PuestoViewModel
    {
        public int IdPuesto { get; set; }
        public string CodigoPuesto { get; set; }
        public string NombrePuesto { get; set; }
        public string? DescripcionPuesto { get; set; }
        public int IdDepartamento { get; set; }
        public bool Activo { get; set; }
        public IdDepartamentoNavigationViewModel? IdDepartamentoNavigation { get; set; }

        // Propiedad calculada para facilitar el uso en vistas
        public string NombreDepartamento => IdDepartamentoNavigation?.NombreDepartamento ?? "";
    }

    public class IdDepartamentoNavigationViewModel
    {
        public int IdDepartamento { get; set; }
        public string NombreDepartamento { get; set; }
    }
}
