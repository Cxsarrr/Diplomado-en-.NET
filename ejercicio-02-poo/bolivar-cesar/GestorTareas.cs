using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Linq;

namespace bolivar_cesar
{
    public class GestorTareas
    {
        private List<Tarea> tareas;

        public GestorTareas()
        {
            tareas = new List<Tarea>();
        }

        public void Agregar(Tarea tarea)
        {
            tareas.Add(tarea);
        }

        public void Completar(int id)
        {
            var tarea = tareas.FirstOrDefault(t => t.Id == id);
            if (tarea != null)
            {
                tarea.Completada = true;
            }
        }

        public List<Tarea> ObtenerTodas()
        {
            return tareas;
        }

        public List<Tarea> ListarPorCategoria(string categoria)
        {
            return tareas.Where(t => string.Equals(t.Categoria, categoria, StringComparison.OrdinalIgnoreCase)).ToList();
        }

        public List<Tarea> ListarPorPrioridad(Prioridad prioridad)
        {
            return tareas.Where(t => t.Prioridad == prioridad).ToList();
        }

        public List<Tarea> ObtenerVencidas()
        {
            var resultado = new List<Tarea>();
            foreach (var t in tareas)
            {
                if (t is TareaConVencimiento tv)
                {
                    if (DateTime.Compare(tv.FechaVencimiento.Date, DateTime.Now.Date) < 0)
                    {
                        resultado.Add(tv);
                    }
                }
            }
            return resultado;
        }

        public void Eliminar(int id)
        {
            tareas.RemoveAll(t => t.Id == id);
        }

        public void GuardarEnJSON(string archivo)
        {
            var dtos = tareas.Select(TareaDto.DesdeTarea).ToList();
            var options = new JsonSerializerOptions { WriteIndented = true };
            string json = JsonSerializer.Serialize(dtos, options);
            File.WriteAllText(archivo, json);
        }

        public List<Tarea> CargarDeJSON(string archivo)
        {
            try
            {
                if (!File.Exists(archivo))
                {
                    return new List<Tarea>();
                }

                string json = File.ReadAllText(archivo);
                var dtos = JsonSerializer.Deserialize<List<TareaDto>>(json);
                var loadedTareas = new List<Tarea>();
                int maxId = 0;

                if (dtos != null)
                {
                    foreach (var dto in dtos)
                    {
                        var t = dto.ToTarea();
                        loadedTareas.Add(t);
                        if (t.Id > maxId)
                        {
                            maxId = t.Id;
                        }
                    }
                }

                tareas = loadedTareas;
                Tarea.SetContador(maxId);
                return tareas;
            }
            catch
            {
                tareas = new List<Tarea>();
                return tareas;
            }
        }
    }
}
