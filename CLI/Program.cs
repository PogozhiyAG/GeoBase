using System.Text;
using System.Diagnostics;
using System.Net;
using GeoIp.Data;
using GeoIp.Utils;
using GeoIp;

namespace CLI
{
    unsafe class Program
    {
        static GeoIpDatabase? service;


        static void Main(string[] args)
        {
            Init();
            Menu();
        }


        private static void Init()
        {
            Stopwatch sw = Stopwatch.StartNew();
            service = new GeoIpDatabase(File.ReadAllBytes("geobase.dat"));
            sw.Stop();
            Console.WriteLine("{0} за {1}", service.GetRecordCount(), sw.Elapsed);
            Console.WriteLine();
        }

        private static void Menu()
        {

            while (true)
            {
                Console.WriteLine();
                Console.WriteLine("1 - Поиск по IP");
                Console.WriteLine("2 - Поиск по городу equals");
                Console.WriteLine("3 - Поиск по городу starts");
                Console.WriteLine("4 - Тест производительности");
                Console.WriteLine("q - Выход");

                ConsoleKeyInfo key = Console.ReadKey(true);

                switch (key.Key)
                {
                    case ConsoleKey.D1:
                    case ConsoleKey.NumPad1:    FindByIP(); break;
                    case ConsoleKey.D2:
                    case ConsoleKey.NumPad2:    FindByCity(); break;
                    case ConsoleKey.D3:
                    case ConsoleKey.NumPad3:    FindByCityStarts(); break;
                    case ConsoleKey.D4:
                    case ConsoleKey.NumPad4:    PerformanceTest(); break;
                    case ConsoleKey.Q: return;
                }

            }
        }


        private static void PerformanceTest()
        {
            Console.Clear();
            Console.WriteLine("Тест производительности");

            Stopwatch sw = Stopwatch.StartNew();

            Random random = new Random(DateTime.Now.Millisecond);

            sw.Restart();
            Location? p_location = service!.FindLocationByIpAddress((uint)random.Next(500000000));
            sw.Stop();
            Console.WriteLine("№1 Поиск по IP {0}", sw.Elapsed);


            sw.Restart();
            p_location = service!.FindLocationByIpAddress((uint)random.Next(500000000));
            sw.Stop();
            Console.WriteLine("№2 Поиск по IP {0}", sw.Elapsed);


            int repeatIP = 1000000;
            sw.Restart();
            for (uint i = 0; i < repeatIP; i++)
            {
                p_location = service.FindLocationByIpAddress((uint)random.Next(500000000));
            }
            sw.Stop();
            Console.WriteLine($"Поиск по IP {repeatIP} раз {sw.Elapsed}");

            Location.Managed managedLocation = p_location!.Value.GetManaged();
            Console.WriteLine(managedLocation);
            Console.WriteLine();



            sw.Restart();
            for (uint i = 0; i < repeatIP; i++)
            {
                p_location = service.FindLocationByIpAddress((uint)random.Next(50000000));
                if (p_location.HasValue)
                {
                    managedLocation = p_location.Value.GetManaged();
                }
            }
            sw.Stop();
            Console.WriteLine($"Поиск по IP (managedLocation) {repeatIP} раз {sw.Elapsed}");
            Console.WriteLine(managedLocation);
            Console.WriteLine();



            string s = "";
            sw.Restart();
            for (uint i = 0; i < repeatIP; i++)
            {
                p_location = service.FindLocationByIpAddress((uint)random.Next(50000000));
                if (p_location.HasValue)
                {
                    managedLocation = p_location.Value.GetManaged();
                    s = managedLocation.ToString();
                }
            }
            sw.Stop();
            Console.WriteLine($"Поиск по IP (managedLocation, ToString) {repeatIP} раз {sw.Elapsed}");
            Console.WriteLine(managedLocation);
            Console.WriteLine();








            string s1 = "cit_Epimyj";
            byte[] s_city = new ASCIIEncoding().GetBytes(s1);

            sw.Restart();
            Location foundLocation = default;
            service.FindLocationsByCity(s1, StringSearchMode.Exact, location =>
            {
                foundLocation = location;
                return false;
            });
            sw.Stop();
            Console.WriteLine($"№1 Поиск по городу equals {sw.Elapsed}");



            sw.Restart();
            service.FindLocationsByCity(s1, StringSearchMode.Exact, location =>
            {
                foundLocation = location;
                return false;
            });
            sw.Stop();
            Console.WriteLine($"№2 Поиск по городу equals {sw.Elapsed}");


            int repeatCityLower = 1000000;
            sw.Restart();
            for (int i = 0; i < repeatCityLower; i++)
            {
                service.FindLocationsByCity(s1, StringSearchMode.Exact, location =>
                {
                    foundLocation = location;
                    return false;
                });
            }
            sw.Stop();
            Console.WriteLine($"Поиск по городу equals {repeatCityLower} раз {sw.Elapsed}");


            Console.WriteLine(foundLocation.GetManaged());


        }





        public static void FindByIP()
        {
            string input;

            while (true)
            {
                Console.WriteLine();
                Console.Write("Поиск по IP адресу (q для выхода): ");

                input = Console.ReadLine()!;
                if (input.ToLower() == "q")
                {
                    return;
                }

                if (IPAddress.TryParse(input, out IPAddress? ip_address))
                {
                    Location? p_location = service!.FindLocationByIpAddress(ip_address.ToInteger());
                    if (p_location.HasValue)
                    {
                        Location.Managed managedLocation = p_location.Value.GetManaged();
                        Console.WriteLine(managedLocation);
                    }
                    else
                    {
                        Console.WriteLine("Не найдено");
                    }

                }
                else
                {
                    Console.WriteLine("Неверный формат IP адреса");
                }

                Console.WriteLine();

            }
        }




        public static void FindByCity()
        {
            string input = "";

            while (true)
            {
                Console.WriteLine();
                Console.Write("Поиск по городу Exact (q для выхода): ");

                input = Console.ReadLine()!;
                if (input.ToLower() == "q")
                {
                    return;
                }

                int i = 0;
                int count = 200;

                service!.FindLocationsByCity(input, StringSearchMode.Exact, location =>
                {
                    Console.WriteLine($"№{i++}:");
                    Console.WriteLine(location.GetManaged());
                    Console.WriteLine();
                    return i < count;
                });

                if (i == 0)
                {
                    Console.WriteLine("Не найдено");
                }

                Console.WriteLine();

            }
        }







        public static void FindByCityStarts()
        {
            string input = "";

            while (true)
            {
                Console.WriteLine();
                Console.Write("Поиск по городу Starts (q для выхода): ");

                input = Console.ReadLine()!;
                if (input.ToLower() == "q")
                {
                    return;
                }

                int i = 0;
                int count = 200;

                service!.FindLocationsByCity(input, StringSearchMode.Starts, location =>
                {
                    Console.WriteLine($"№{i++}:");
                    Console.WriteLine(location.GetManaged());
                    Console.WriteLine();
                    return i < count;
                });

                if (i == 0)
                {
                    Console.WriteLine("Не найдено");
                }

                Console.WriteLine();

            }
        }





    }
}
