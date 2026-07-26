using System;

namespace bolivar_cesar
{
    public class TareaDto
    {
        public string Tipo { get; set; }
        public int Id { get; set; }
        public string Titulo { get; set; }
        public string Descripcion { get; set; }
        public Prioridad Prioridad { get; set; }
        public string Categoria { get; set; }
        public bool Completada { get; set; }
        public DateTime FechaCreacion { get; set; }
        public DateTime? FechaVencimiento { get; set; }

        public static TareaDto DesdeTarea(Tarea tarea)
        {
            var dto = new TareaDto
            {
                Id = tarea.Id,
                Titulo = tarea.Titulo,
                Descripcion = tarea.Descripcion,
                Prioridad = tarea.Prioridad,
                Categoria = tarea.Categoria,
                Completada = tarea.Completada,
                FechaCreacion = tarea.FechaCreacion
            };

            if (tarea is TareaConVencimiento tareaConVencimiento)
            {
                dto.Tipo = "TareaConVencimiento";
                dto.FechaVencimiento = tareaConVencimiento.FechaVencimiento;
            }
            else
            {
                dto.Tipo = "Tarea";
                dto.FechaVencimiento = null;
            }

            return dto;
        }

        public Tarea ToTarea()
        {
            if (Tipo == "TareaConVencimiento" && FechaVencimiento.HasValue)
            {
                return new TareaConVencimiento
                {
                    Id = this.Id,
                    Titulo = this.Titulo,
                    Descripcion = this.Descripcion,
                    Prioridad = this.Prioridad,
                    Categoria = this.Categoria,
                    Completada = this.Completada,
                    FechaCreacion = this.FechaCreacion,
                    FechaVencimiento = this.FechaVencimiento.Value
                };
            }
            else
            {
                return new Tarea
                {
                    Id = this.Id,
                    Titulo = this.Titulo,
                    Descripcion = this.Descripcion,
                    Prioridad = this.Prioridad,
                    Categoria = this.Categoria,
                    Completada = this.Completada,
                    FechaCreacion = this.FechaCreacion
                };
            }
        }
    }
}
