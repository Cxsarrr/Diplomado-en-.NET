using System;

namespace bolivar_cesar
{
    public class TareaConVencimiento : Tarea
    {
        public DateTime FechaVencimiento { get; set; }

        public int DiasRestantes
        {
            get
            {
                return (FechaVencimiento.Date - DateTime.Now.Date).Days;
            }
        }

        public TareaConVencimiento(string titulo, string descripcion, Prioridad prioridad, string categoria, DateTime fechaVencimiento)
            : base(titulo, descripcion, prioridad, categoria)
        {
            FechaVencimiento = fechaVencimiento;
        }

        public TareaConVencimiento() : base()
        {
        }

        public override void MostrarInfo()
        {
            base.MostrarInfo();
            Console.WriteLine($"Vence: {FechaVencimiento:dd/MM/yyyy} | Dias restantes: {DiasRestantes}");
        }
    }
}
