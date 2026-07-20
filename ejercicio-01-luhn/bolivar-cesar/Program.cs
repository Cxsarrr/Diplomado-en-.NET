using System;
using System.IO;

namespace LuhnAlgorithm
{
    internal class Program
    {
        static int totalProcesadas = 0;
        static int totalValidas = 0;
        static int totalInvalidas = 0;

        static int visaCount = 0;
        static int mastercardCount = 0;
        static int amexCount = 0;
        static int discoverCount = 0;
        static int desconocidaCount = 0;

        static void Main(string[] args)
        {
            int opcion = 0;

            do
            {
                Console.WriteLine("\n=== VALIDADOR DE TARJETAS ===");
                Console.WriteLine("1. Validar una tarjeta");
                Console.WriteLine("2. Validar desde archivo");
                Console.WriteLine("3. Generar número válido");
                Console.WriteLine("4. Estadísticas");
                Console.WriteLine("5. Salir");
                Console.Write("Seleccione una opción (1-5): ");

                try
                {
                    string entrada = Console.ReadLine() ?? "";
                    
                    if (int.TryParse(entrada, out opcion))
                    {
                        Console.WriteLine();
                        switch (opcion)
                        {
                            case 1:
                                MenuValidarUnaTarjeta();
                                break;
                            case 2:
                                MenuValidarDesdeArchivo();
                                break;
                            case 3:
                                MenuGenerarNumeroValido();
                                break;
                            case 4:
                                MostrarEstadisticas();
                                break;
                            case 5:
                                Console.WriteLine("¡Hasta luego!");
                                break;
                            default:
                                Console.WriteLine("Opción no válida. Ingrese un número del 1 al 5.");
                                break;
                        }
                    }
                    else
                    {
                        Console.WriteLine("Entrada no válida. Ingrese un número.");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Ocurrió un error: " + ex.Message);
                }

            } while (opcion != 5);
        }

        static void MenuValidarUnaTarjeta()
        {
            Console.Write("Ingrese el número de tarjeta: ");
            string numero = Console.ReadLine() ?? "";
            ProcesarTarjeta(numero);
        }

        static void MenuValidarDesdeArchivo()
        {
            Console.Write("Ingrese la ruta del archivo (Enter para 'tarjetas_ejemplo.txt'): ");
            string ruta = Console.ReadLine() ?? "";
            if (string.IsNullOrWhiteSpace(ruta))
            {
                ruta = "tarjetas_ejemplo.txt";
            }
            ValidarDesdeArchivo(ruta.Trim());
        }

        static void MenuGenerarNumeroValido()
        {
            string numeroValido = GenerarNumeroValido();
            Console.WriteLine("Número generado exitosamente:");
            ProcesarTarjeta(numeroValido);
        }

        static void ProcesarTarjeta(string numeroRaw)
        {
            string limpio = numeroRaw.Replace(" ", "").Replace("-", "").Trim();

            bool esValida = ValidarTarjeta(limpio);
            string marca = IdentificarMarca(limpio);

            Console.WriteLine("Número: " + limpio);
            Console.WriteLine("Marca: " + marca);
            if (esValida)
            {
                Console.WriteLine("Estado: VÁLIDA");
            }
            else
            {
                Console.WriteLine("Estado: INVÁLIDA");
            }

            totalProcesadas++;
            if (esValida)
            {
                totalValidas++;
            }
            else
            {
                totalInvalidas++;
            }

            switch (marca)
            {
                case "Visa": visaCount++; break;
                case "Mastercard": mastercardCount++; break;
                case "American Express": amexCount++; break;
                case "Discover": discoverCount++; break;
                default: desconocidaCount++; break;
            }
        }

        public static bool ValidarTarjeta(string numero)
        {
            if (string.IsNullOrEmpty(numero)) return false;

            int suma = 0;
            bool duplicar = false;

            for (int i = numero.Length - 1; i >= 0; i--)
            {
                if (!char.IsDigit(numero[i])) return false;

                int digito = (int)char.GetNumericValue(numero[i]);

                if (duplicar)
                {
                    digito = digito * 2;
                    if (digito >= 10)
                    {
                        digito = digito - 9;
                    }
                }

                suma += digito;
                duplicar = !duplicar;
            }

            return (suma % 10 == 0);
        }

        public static string IdentificarMarca(string numero)
        {
            if (string.IsNullOrEmpty(numero)) return "Desconocida";

            int len = numero.Length;

            if ((len == 13 || len == 16) && numero.StartsWith("4"))
            {
                return "Visa";
            }

            if (len == 16 && (numero.StartsWith("51") || numero.StartsWith("52") ||
                              numero.StartsWith("53") || numero.StartsWith("54") || numero.StartsWith("55")))
            {
                return "Mastercard";
            }

            if (len == 15 && (numero.StartsWith("34") || numero.StartsWith("37")))
            {
                return "American Express";
            }

            if (len >= 16 && len <= 19 && (numero.StartsWith("6011") || numero.StartsWith("65") || numero.StartsWith("64")))
            {
                return "Discover";
            }

            return "Desconocida";
        }

        public static void ValidarDesdeArchivo(string ruta)
        {
            try
            {
                if (!File.Exists(ruta))
                {
                    string rutaAlt = Path.Combine(AppContext.BaseDirectory, ruta);
                    if (File.Exists(rutaAlt))
                    {
                        ruta = rutaAlt;
                    }
                    else
                    {
                        Console.WriteLine("Error: El archivo '" + ruta + "' no fue encontrado.");
                        return;
                    }
                }

                string[] lineas = File.ReadAllLines(ruta);
                Console.WriteLine("Procesando " + lineas.Length + " tarjetas desde archivo...\n");

                foreach (string linea in lineas)
                {
                    if (!string.IsNullOrWhiteSpace(linea))
                    {
                        ProcesarTarjeta(linea);
                        Console.WriteLine();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al leer el archivo: " + ex.Message);
            }
        }

        public static string GenerarNumeroValido()
        {
            string prefijo = "453201511283036";
            int suma = 0;
            bool duplicar = true;

            for (int i = prefijo.Length - 1; i >= 0; i--)
            {
                int digito = (int)char.GetNumericValue(prefijo[i]);
                if (duplicar)
                {
                    digito = digito * 2;
                    if (digito >= 10) digito = digito - 9;
                }
                suma += digito;
                duplicar = !duplicar;
            }

            int residuo = suma % 10;
            int ultimoDigito = (residuo == 0) ? 0 : (10 - residuo);

            return prefijo + ultimoDigito;
        }

        public static void MostrarEstadisticas()
        {
            Console.WriteLine("=== ESTADÍSTICAS DE LA SESIÓN ===");
            Console.WriteLine("Total tarjetas procesadas: " + totalProcesadas);
            Console.WriteLine("Válidas: " + totalValidas);
            Console.WriteLine("Inválidas: " + totalInvalidas);
            Console.WriteLine("\nDesglose por marca:");
            Console.WriteLine("- Visa: " + visaCount);
            Console.WriteLine("- Mastercard: " + mastercardCount);
            Console.WriteLine("- American Express: " + amexCount);
            Console.WriteLine("- Discover: " + discoverCount);
            Console.WriteLine("- Desconocida: " + desconocidaCount);
        }
    }
}
