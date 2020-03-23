using System;
using System.Threading;

namespace ConmutadorSO
{
    public class Pcb
    {
        public int quantum;
        public int quantumProgress;
        public int noFuncion;
        public int[] context; 
        public Action funcion;
        private volatile bool activo;

        // Trabajandolo como un constructor de instancias
        public Pcb(){}

        public Pcb(int quantum, int noFuncion){
            this.quantum = quantum;
            this.noFuncion = noFuncion;
        }

        public bool estaActivo(){
            return activo;
        }

        public void activar(){
            activo = true;
        }

        public void apagar(){
            if(quantumProgress == (quantum)){
                quantumProgress = 0;
                activo = false;
            };
        }


        
    }
}