using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Linq;

namespace bolivar_cesar
{
    class Program
    {
        private static GestorTareas gestor = new GestorTareas();
        private static List<Categoria> categorias = new List<Categoria>();

        static void Main(string[] args)
        {
            CargarCategorias();
            gestor.CargarDeJSON("tareas.json");

            bool salir = false;
            while (!salir)
            {
                Console.WriteLine("=== GESTOR DE TAREAS ===");
                Console.WriteLine("1. Agregar tarea");
                Console.WriteLine("2. Listar todas");
                Console.WriteLine("3. Listar por categoria");
                Console.WriteLine("4. Listar por prioridad");
                Console.WriteLine("5. Marcar como completada");
                Console.WriteLine("6. Mostrar tareas vencidas");
                Console.WriteLine("7. Eliminar tarea");
                Console.WriteLine("8. Exportar a JSON");
                Console.WriteLine("9. Salir");
                Console.Write("Seleccione una opcion: ");

                string opcion = Console.ReadLine();
                Console.WriteLine();

                switch (opcion)
                {
                    case "1":
                        AgregarTarea();
                        break;
                    case "2":
                        ListarTodas();
                        break;
                    case "3":
                        ListarPorCategoria();
                        break;
                    case "4":
                        ListarPorPrioridad();
                        break;
                    case "5":
                        MarcarComoCompletada();
                        break;
                    case "6":
                        MostrarTareasVencidas();
                        break;
                    case "7":
                        EliminarTarea();
                        break;
                    case "8":
                        ExportarAJson();
                        break;
                    case "9":
                        SalirYGuardar();
                        salir = true;
                        break;
                    default:
                        Console.WriteLine("Opcion invalida.");
                        break;
                }
                Console.WriteLine();
            }
        }

        private static void CargarCategorias()
        {
            try
            {
                if (File.Exists("categorias.json"))
                {
                    string json = File.ReadAllText("categorias.json");
                    categorias = JsonSerializer.Deserialize<List<Categoria>>(json) ?? new List<Categoria>();
                }
            }
            catch
            {
            }

            if (categorias.Count == 0)
            {
                categorias.Add(new Categoria("Trabajo", "Rojo", "Tareas del trabajo"));
                categorias.Add(new Categoria("Personal", "Azul", "Tareas de la vida diaria"));
                categorias.Add(new Categoria("Estudio", "Verde", "Tareas de estudio"));
            }
        }

        private static void GuardarCategorias()
        {
            try
            {
                var options = new JsonSerializerOptions { WriteIndented = true };
                string json = JsonSerializer.Serialize(categorias, options);
                File.WriteAllText("categorias.json", json);
            }
            catch
            {
            }
        }

        private static void AgregarTarea()
        {
            Console.Write("Titulo: ");
            string titulo = Console.ReadLine();

            Console.Write("Descripcion: ");
            string descripcion = Console.ReadLine();

            Prioridad prioridad = Prioridad.Baja;
            bool prioridadValida = false;
            while (!prioridadValida)
            {
                Console.WriteLine("Prioridad:");
                Console.WriteLine("0. Baja");
                Console.WriteLine("1. Media");
                Console.WriteLine("2. Alta");
                Console.WriteLine("3. Critica");
                Console.Write("Seleccione prioridad: ");
                string pInput = Console.ReadLine();
                if (int.TryParse(pInput, out int pVal) && pVal >= 0 && pVal <= 3)
                {
                    prioridad = (Prioridad)pVal;
                    prioridadValida = true;
                }
                else
                {
                    Console.WriteLine("Prioridad invalida.");
                }
            }

            string categoriaSeleccionada = "";
            bool categoriaValida = false;
            while (!categoriaValida)
            {
                Console.WriteLine("Categorias:");
                for (int i = 0; i < categorias.Count; i++)
                {
                    Console.WriteLine($"{i + 1}. {categorias[i].Nombre} ({categorias[i].Color})");
                }
                Console.WriteLine($"{categorias.Count + 1}. [Crear nueva categoria]");
                Console.Write("Seleccione categoria: ");
                string catInput = Console.ReadLine();

                if (int.TryParse(catInput, out int catVal) && catVal >= 1 && catVal <= categorias.Count + 1)
                {
                    if (catVal == categorias.Count + 1)
                    {
                        Console.Write("Nombre de la nueva categoria: ");
                        string nombreCat = Console.ReadLine();
                        Console.Write("Color (ej: Rojo, Azul, Verde): ");
                        string colorCat = Console.ReadLine();
                        Console.Write("Descripcion de la categoria: ");
                        string descCat = Console.ReadLine();

                        var nuevaCat = new Categoria(nombreCat, colorCat, descCat);
                        categorias.Add(nuevaCat);
                        categoriaSeleccionada = nombreCat;
                    }
                    else
                    {
                        categoriaSeleccionada = categorias[catVal - 1].Nombre;
                    }
                    categoriaValida = true;
                }
                else
                {
                    Console.WriteLine("Seleccion invalida.");
                }
            }

            Console.Write("Tiene fecha de vencimiento? (S/N): ");
            string tieneVencimiento = Console.ReadLine()?.Trim().ToUpper();

            if (tieneVencimiento == "S")
            {
                DateTime fechaVencimiento = DateTime.Now;
                bool fechaValida = false;
                while (!fechaValida)
                {
                    Console.Write("Fecha de vencimiento (dd/MM/yyyy): ");
                    string fechaInput = Console.ReadLine();
                    if (DateTime.TryParseExact(fechaInput, "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out fechaVencimiento))
                    {
                        fechaValida = true;
                    }
                    else
                    {
                        Console.WriteLine("Formato de fecha invalido. Use dd/MM/yyyy.");
                    }
                }

                var tareaConVencimiento = new TareaConVencimiento(titulo, descripcion, prioridad, categoriaSeleccionada, fechaVencimiento);
                gestor.Agregar(tareaConVencimiento);
                Console.WriteLine("Tarea con vencimiento agregada.");
            }
            else
            {
                var tareaSimple = new Tarea(titulo, descripcion, prioridad, categoriaSeleccionada);
                gestor.Agregar(tareaSimple);
                Console.WriteLine("Tarea simple agregada.");
            }
        }

        private static void ListarTodas()
        {
            var tareas = gestor.ObtenerTodas();
            if (tareas.Count == 0)
            {
                Console.WriteLine("No hay tareas registradas.");
                return;
            }

            Console.WriteLine("=== LISTA DE TODAS LAS TAREAS ===");
            foreach (var t in tareas)
            {
                t.MostrarInfo();
                Console.WriteLine("----------------------------------------");
            }
        }

        private static void ListarPorCategoria()
        {
            Console.Write("Ingrese la categoria a buscar: ");
            string cat = Console.ReadLine();

            var tareas = gestor.ListarPorCategoria(cat);
            if (tareas.Count == 0)
            {
                Console.WriteLine("No hay tareas en esa categoria.");
                return;
            }

            Console.WriteLine($"=== TAREAS EN CATEGORIA: {cat} ===");
            foreach (var t in tareas)
            {
                t.MostrarInfo();
                Console.WriteLine("----------------------------------------");
            }
        }

        private static void ListarPorPrioridad()
        {
            Prioridad prioridad = Prioridad.Baja;
            bool prioridadValida = false;
            while (!prioridadValida)
            {
                Console.WriteLine("Prioridad a buscar:");
                Console.WriteLine("0. Baja");
                Console.WriteLine("1. Media");
                Console.WriteLine("2. Alta");
                Console.WriteLine("3. Critica");
                Console.Write("Seleccione prioridad: ");
                string pInput = Console.ReadLine();
                if (int.TryParse(pInput, out int pVal) && pVal >= 0 && pVal <= 3)
                {
                    prioridad = (Prioridad)pVal;
                    prioridadValida = true;
                }
                else
                {
                    Console.WriteLine("Prioridad invalida.");
                }
            }

            var tareas = gestor.ListarPorPrioridad(prioridad);
            if (tareas.Count == 0)
            {
                Console.WriteLine("No hay tareas con esa prioridad.");
                return;
            }

            Console.WriteLine($"=== TAREAS CON PRIORIDAD: {prioridad} ===");
            foreach (var t in tareas)
            {
                t.MostrarInfo();
                Console.WriteLine("----------------------------------------");
            }
        }

        private static void MarcarComoCompletada()
        {
            Console.Write("Ingrese el ID de la tarea a completar: ");
            if (int.TryParse(Console.ReadLine(), out int id))
            {
                gestor.Completar(id);
                Console.WriteLine("Operacion realizada.");
            }
            else
            {
                Console.WriteLine("ID invalido.");
            }
        }

        private static void MostrarTareasVencidas()
        {
            var tareas = gestor.ObtenerVencidas();
            if (tareas.Count == 0)
            {
                Console.WriteLine("No hay tareas vencidas.");
                return;
            }

            Console.WriteLine("=== TAREAS VENCIDAS ===");
            foreach (var t in tareas)
            {
                t.MostrarInfo();
                Console.WriteLine("----------------------------------------");
            }
        }

        private static void EliminarTarea()
        {
            Console.Write("Ingrese el ID de la tarea a eliminar: ");
            if (int.TryParse(Console.ReadLine(), out int id))
            {
                gestor.Eliminar(id);
                Console.WriteLine("Operacion realizada.");
            }
            else
            {
                Console.WriteLine("ID invalido.");
            }
        }

        private static void ExportarAJson()
        {
            Console.Write("Nombre del archivo para exportar (ej: tareas_exportadas.json): ");
            string archivo = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(archivo))
            {
                Console.WriteLine("Nombre de archivo invalido.");
                return;
            }

            try
            {
                gestor.GuardarEnJSON(archivo);
                Console.WriteLine($"Tareas exportadas a {archivo} correctamente.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al exportar: {ex.Message}");
            }
        }

        private static void SalirYGuardar()
        {
            try
            {
                gestor.GuardarEnJSON("tareas.json");
                GuardarCategorias();
                Console.WriteLine("Datos guardados automaticamente.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al guardar datos: {ex.Message}");
            }
            Console.WriteLine("Saliendo del programa...");
        }
    }
}
