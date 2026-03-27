namespace MvcNakamasCloud.ViewModels
{
    public class EmpleadoViewModel
    {
        public int IdEmpleado { get; set; }
        public string CodigoEmpleado { get; set; }
        public string FotoRuta { get; set; }
        public string PrimerNombre { get; set; }
        public string SegundoNombre { get; set; }
        public string PrimerApellido { get; set; }
        public string SegundoApellido { get; set; }
        public string Email { get; set; }
        public string Telefono { get; set; }
        public string Direccion { get; set; }
        public string Estado { get; set; }
        public DateOnly FechaNacimiento { get; set; }
        public DateOnly FechaContratacion { get; set; }

       // Datos de asignacion
        public List<AsignacionViewModel>? AsignacionPuestos { get; set; }

        // Propiedades calculadas para facilitar el acceso en la vista
        public string NombrePuesto => AsignacionPuestos?.FirstOrDefault(a => a.Estado == "Activo")?.IdPuestoNavigation?.NombrePuesto ?? "Sin asignación";
        public string NombreDepartamento => AsignacionPuestos?.FirstOrDefault(a => a.Estado == "Activo")?.IdPuestoNavigation?.IdDepartamentoNavigation?.NombreDepartamento ?? "Sin departamento";
        public decimal Salario => AsignacionPuestos?.FirstOrDefault(a => a.Estado == "Activo")?.Salario ?? 0;
    }

        public class AsignacionViewModel
    {
        public int IdAsignacion { get; set; }
        public int IdEmpleado { get; set; }
        public int IdPuesto { get; set; }
        public decimal Salario { get; set; }
        public string Estado { get; set; }
        public PuestoAsignacionViewModel? IdPuestoNavigation { get; set; }
    }

    public class PuestoAsignacionViewModel
    {
        public int IdPuesto { get; set; }
        public string NombrePuesto { get; set; }
        public int IdDepartamento { get; set; }
        public DepartamentoAsignacionViewModel? IdDepartamentoNavigation { get; set; }
    }

    public class DepartamentoAsignacionViewModel
    {
        public int IdDepartamento { get; set; }
        public string NombreDepartamento { get; set; }
    }
}
