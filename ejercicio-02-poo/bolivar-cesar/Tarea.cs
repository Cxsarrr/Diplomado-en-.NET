using System;

namespace bolivar_cesar
{
    public class Tarea : IExportable
    {
        private static int _contadorId = 0;

        public int Id { get; set; }
        public string Titulo { get; set; }
        public string Descripcion { get; set; }
        public Prioridad Prioridad { get; set; }
        public string Categoria { get; set; }
        public bool Completada { get; set; }
        public DateTime FechaCreacion { get; set; }

        public Tarea(string titulo, string descripcion, Prioridad prioridad, string categoria)
        {
            _contadorId++;
            Id = _contadorId;
            Titulo = titulo;
            Descripcion = descripcion;
            Prioridad = prioridad;
            Categoria = categoria;
            Completada = false;
            FechaCreacion = DateTime.Now;
        }

        public Tarea()
        {
        }

        public static void SetContador(int valor)
        {
            _contadorId = valor;
        }

        public virtual void MostrarInfo()
        {
            string estado = Completada ? "Completada" : "Pendiente";
            Console.WriteLine($"ID: {Id} | {Titulo} | Prioridad: {Prioridad} | Categoria: {Categoria} | Estado: {estado} | Creada: {FechaCreacion:dd/MM/yyyy}");
            Console.WriteLine($"Descripcion: {Descripcion}");
        }

        public string Exportar()
        {
            return $"{Id}|{Titulo}|{Prioridad}|{Completada}";
        }
    }
}
