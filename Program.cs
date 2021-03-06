﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace ConmutadorSO
{
    class Program
    {
        // Parámetros de funciones y el contador general (Simulador de clock)
        static int a, contador, c, d;
        static int b = 1;
        static int pid; //Proceso actual
        static Pcb pcbActual = null; //Instancia del PCB actual 
        static List<ConsoleColor> colores = new List<ConsoleColor>(); //Colores para hacer más chilero el programa
        static List<Pcb> listaPcb = new List<Pcb>(); //Lista de PCB's listos
        static bool encendido = true; //Indica si el programa se detiene
        

        // Primera funcion del proyecto
        public static int[] funcion1(int[] valores)
        {
            // obtener el contexto anterior
            a = valores[0];
            a++;
            if(a > 2000) {
                a = 0;
            }
            consoleInfo.outputBuffer[pid] = "El valor de a es " + a; //  Es el print console de nuestra consola emulada

            // guardar el contexto antes de irme 
            valores[0] = a;
            return valores;
        }
        // Segunda función del proyecto
        public static int[] funcion2(int[] valores)
        {
            b = valores[0];

            b = b * (b + 1);
            //b++;
            if (b > 12000){
                b = 1;
            }
            consoleInfo.outputBuffer[pid] = "El valor de b es " + b; //Es el print console de la consola emulada

            valores[0] = b;
            return valores;
        }

        public static int[] funcion3(int[] valores)
        {
            c = pcbActual.quantum;
            d = pcbActual.quantum;
            consoleInfo.outputBuffer[pid] = "El valor de c es " + pcbActual.quantum + " y el valor de d es " + d; //Es el print console de la consola emulada
            return valores;
        }

        // Instancia de ConsoleInfo [Impresión a consola]
        private static ConsoleInfo consoleInfo = new ConsoleInfo();

        // Método principal
        static void Main(string[] args)
        {
            //Inicializando lista de colores [4]
            colores.Add(ConsoleColor.Blue); 
            colores.Add(ConsoleColor.Red); 
            colores.Add(ConsoleColor.Yellow);
            colores.Add(ConsoleColor.Green);



            // Thread de escritura a pantalla
            Thread consoleWriter = new Thread(new ThreadStart(ConsoleWriter)); //Método que actualiza la consola
            consoleWriter.IsBackground = true; // Lo ponemos a trabajar tras bambalinas
            consoleWriter.Start(); //Iniciar el hilo
            

            
            // Impresión de consola por defecto
            consoleInfo.outputBuffer.Add("Proceso sin iniciar");
            consoleInfo.outputBuffer.Add("Proceso sin iniciar");
            consoleInfo.outputBuffer.Add("Proceso sin iniciar");
            consoleInfo.outputBuffer.Add("Proceso sin iniciar");
            consoleInfo.outputBuffer.Add("Contador global." + contador);
            // Try y Catch por si hay un PCB nulo [inicio del programa o no hay pcb en la lista]
            try{
                consoleInfo.outputBuffer.Add("PID." + pid);
            } catch {
                consoleInfo.outputBuffer.Add("PID." + pid + " P" + " null");
            }

            //consoleInfo.outputBuffer.Add("Nada por el momento...");
    

            while (encendido)
            {   //Consiste en el hilo principal que mantiene siempre la guardia a la espera de nuevas teclas

                var key = Console.ReadKey(true);

                lock (consoleInfo)
                {
                    if (key.Key == ConsoleKey.Enter) //Si oprime enter se guarda el comando
                    {
                        consoleInfo.commandReaty = true;
                    }
                    else if(key.Key == ConsoleKey.Backspace)
                    {
                        if (consoleInfo.sbRead.Length > 0) {
                            consoleInfo.sbRead.Remove(consoleInfo.sbRead.Length - 1, 1);
                        }
                    }
                    else
                    {
                        consoleInfo.sbRead.Append(key.KeyChar.ToString());
                    }
                }
            }

        }

        static void AgregarProceso(String[] parametros){
            consoleInfo.lastResult.Clear(); // Limpia el sector de advertencias
            // Agrega un proceso a la lista. Verificar que Count no sea mayor que 4
            if(listaPcb.Count < 4) {
                try{
                    Pcb nuevo  = new Pcb();
                    nuevo.quantum = int.Parse(parametros[2]);
                    nuevo.noFuncion = int.Parse(parametros[1]);
                    nuevo.context = new int[] { 1 };
                    switch (int.Parse(parametros[1]))
                    {
                        case 1:
                            nuevo.funcion_utilizar = new funcion_generica(funcion1);
                            //nuevo.funcion = (() => funcion1());
                            break;
                        case 2:
                            nuevo.funcion_utilizar = new funcion_generica(funcion2);
                            //nuevo.funcion = (() => funcion2());
                            break;
                        case 3:
                            nuevo.funcion_utilizar = new funcion_generica(funcion3);
                            //nuevo.funcion = (() => funcion3());
                            break;
                        default:
                            consoleInfo.lastResult.AppendLine("Función inválida");
                            break;
                    }
                    if (nuevo.quantum > 10 || nuevo.quantum < 1) {
                        consoleInfo.lastResult.AppendLine("Quantum muy grande o muy pequeño.");
                    } 
                    if (consoleInfo.lastResult.Length < 1) {
                        listaPcb.Add(nuevo);
                        consoleInfo.lastResult.Append("Programa añadido");
                    } 
                } catch {
                    consoleInfo.lastResult.Clear();
                    consoleInfo.lastResult.AppendLine("Hubo un error procesando su comando");
                }
            } else {
                consoleInfo.lastResult.Clear();
                consoleInfo.lastResult.AppendLine("StAcK oVeRfLoW");  
            }
            
            
        }

        static void ListarProcesos(){
            // Lista Procesos en consoleInfo.lastResult
            consoleInfo.lastResult.Clear();
            foreach (Pcb item in listaPcb)
            {
                consoleInfo.lastResult.AppendLine("Proceso con id " + listaPcb.IndexOf(item) + " con Q " + item.quantum + " ejecutando función " + item.noFuncion);
            }
        }

        static void EliminarProcesos(String[] parametros){
            // Elimina procesos y actualiza consoleInfo.outputBuffer
            try
            {
                listaPcb.RemoveAt(int.Parse(parametros[1]));
                consoleInfo.lastResult.Clear();
                consoleInfo.lastResult.AppendLine("No me quiero ir Señor Stark"); 
            }
            catch (System.Exception)
            {
                consoleInfo.lastResult.Clear();
                consoleInfo.lastResult.AppendLine("No se pudo eliminar"); 
            }
        }

        static void ModificarProcesos(String[] parametros){
            // Modifica procesos y actualiza consoleInfo.outputBuffer
            try
            {
                int intento = int.Parse(parametros[2]);
                int temp_id = int.Parse(parametros[1]);
                if (intento > 0 && intento <= 10)
                {
                    listaPcb[temp_id].quantum = intento;
                    consoleInfo.lastResult.Clear();
                    consoleInfo.lastResult.AppendLine("Quantum de " + temp_id + " modificado con éxito a " + intento);
                }
                else {
                    consoleInfo.lastResult.Clear();
                    consoleInfo.lastResult.AppendLine("Quantum muy grande o muy pequeño.");
                }
            }
            catch (System.Exception)
            {
                consoleInfo.lastResult.Clear();
                consoleInfo.lastResult.AppendLine("No se pudo modificar por alguna razón muy específica."); 
            }
        }

        static void ConsoleWriter()
        {
            while (encendido)
            {
                contador++;
                // Este es el context Switcher por así decirlo
                if(pcbActual == null & listaPcb.Count > 0){
                    pid = pid%listaPcb.Count;
                    pcbActual = listaPcb[pid];
                    pcbActual.activar();
                } else if(listaPcb.Count == 0) {
                    pcbActual = null;
                } else if(pcbActual.estaActivo()) {
                    pcbActual.context = pcbActual.funcion_utilizar(pcbActual.context);
                    //pcbActual.funcion();
                    pcbActual.quantumProgress++;
                    pcbActual.apagar();                        
                } else {
                    pid++;
                    pid = pid%listaPcb.Count;
                    pcbActual = listaPcb[pid];
                    pcbActual.activar();
                }
                
                // Impresión de la consola
                lock(consoleInfo)
                {                         
                    Console.Clear();
                    
                    for (int i = listaPcb.Count; i<4; i++) {
                        consoleInfo.outputBuffer[i] = "Proceso sin iniciar";
                    }
                    consoleInfo.outputBuffer[4] = "Contador global." + contador;
                    consoleInfo.outputBuffer[5] = "PID." + pid;


                    foreach (var item in consoleInfo.outputBuffer)
                    {
                        if(consoleInfo.outputBuffer.IndexOf(item) < 4) {
                            if(consoleInfo.outputBuffer.IndexOf(item) == pid) {
                                Console.ForegroundColor = colores[consoleInfo.outputBuffer.IndexOf(item)];
                            }
                            
                        }
                        Console.WriteLine(item);
                        Console.ForegroundColor = ConsoleColor.Gray;
                    }

                    Console.WriteLine("--------------------------------------------------------------");

                    if (consoleInfo.commandReaty)
                    {
                        consoleInfo.commandReaty = false;
                        consoleInfo.lastCommand = consoleInfo.sbRead.ToString();
                        consoleInfo.sbRead.Clear();
                        consoleInfo.lastResult.Clear();
                        String[] strlist = consoleInfo.lastCommand.Split(" ", 3, StringSplitOptions.RemoveEmptyEntries);
                        switch (strlist[0].ToLower())
                        {
                            case "quit":
                                encendido = false;
                                consoleInfo.lastResult.Append("¡Programa terminado exitosamente!");
                                break;
                            case "add":
                                AgregarProceso(strlist);
                                break;
                            case "kill":
                                EliminarProcesos(strlist);
                                break;
                            case "ch":
                                ModificarProcesos(strlist);
                                break;
                            case "list":
                                ListarProcesos();
                                break;
                            case "?":
                                consoleInfo.lastResult.AppendLine("Comandos disponibles:");
                                consoleInfo.lastResult.AppendLine("add #function #quantum     Agrega un nuevo proceso. Máximo 4.");
                                consoleInfo.lastResult.AppendLine("ch #pid #quantum     Cambia el quantum a un proceso.");
                                consoleInfo.lastResult.AppendLine("list     Lista los procesos actuales.");
                                consoleInfo.lastResult.AppendLine("kill #pid     Mata un proceso según su ID.");
                                consoleInfo.lastResult.AppendLine("quit #pid     Salir del programa.");
                                break;
                            default:
                                consoleInfo.lastResult.Append("Comando inválido, use ? para ver la lista de comandos");
                                break;
                        }
                    }
                    Console.WriteLine(consoleInfo.lastCommand);
                    Console.WriteLine(consoleInfo.lastResult);
                    Console.WriteLine();
                    Console.Write(">");
                    Console.WriteLine(consoleInfo.sbRead.ToString());
                    Console.WriteLine();
                }
                Thread.Sleep(150);
            }
        }

        // Objeto de consola para imprimir
        private class ConsoleInfo
        {
            public bool commandReaty { get; set; }
            public StringBuilder sbRead { get; set; }
            public List<string> outputBuffer { get; set; }
            public String lastCommand { get; set; }
            public StringBuilder lastResult { get; set; }

            public ConsoleInfo()
            {
                sbRead = new StringBuilder("?");
                outputBuffer = new List<string>();
                commandReaty = true;
                lastResult = new StringBuilder();
            }
        }
    }
}