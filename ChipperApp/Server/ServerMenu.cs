﻿using Communication;
using Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Server
{
    public class ServerMenu
    {
        public static void MenuServer()
        {
            Console.WriteLine("1. Lista de Usuarios del Sistema");
            Console.WriteLine("2. Bloquear Usuario");
            Console.WriteLine("3. Desblquear Usuario");
            Console.WriteLine("4. Buscar chip por palabras");
            Console.WriteLine("5. Top 5, usuarios con más seguidores");
            Console.WriteLine("6. Top 5, usuarios más activos para un periodo de tiempo");
            Console.WriteLine($"{Protocol.SERVER_SHOW_MENU_COMMAND} - para mostrar el menú.");
            Console.WriteLine($"{Protocol.SERVER_EXIT_COMMAND} - para cerrar la conexión.");
            Console.WriteLine("");
        }

        public static void ExecuteMenuOption(int opcion)
        {
            ServerFunctionalities server = new();
            switch (opcion)
            {
                case 1:
                    server.GetUsers();
                    break;
                case 2:
                    server.LockUser();
                    break;
                case 3:
                    server.UnlockUser();
                    break;
                case 4:
                    server.SearchChipByKey();
                    break;
                case 5:
                    server.TopUsersByFollowers();
                    break;
                case 6:
                    server.TopUsersByActivity();
                    break;
                case 7:
                    server.CARGARDATOS();
                    break;
                case 8:
                    server.MOSTRARCHIPS();
                    break;
                //LAS OPCIONES 7 y 8 ESTAN FUERA DEL MENU PARA CARGAR DATOS Y PROBAR
                default:
                    Console.WriteLine("La opción ingresada no es válida");
                    break;
            }
        }
       
    }
}