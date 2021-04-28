using System;
using System.Collections;
using System.Collections.Generic;

namespace AulaMPI2 {
    public class AppMain {
        // Gerador de matriz:
        public static readonly int N = 16; // Nro de matrizes para multiplicar
        public static int MIN = 5;
        public static int MAX = 10;
        public static string file = "D:\\tmp\\matrizes\\matrix" + N+".txt";

        public static void Main(string[] args) {
            MPIEnv.mpi_start();
            test1();
            //test2();
            MPIEnv.mpi_stop();
        }
        public static void test2() {
            List<object> lista = new List<object>();
            Splitter splitter = new Splitter();
            List<ulong[][]> ms = Generator.MatrixList(file);


            if (MPIEnv.Rank == MPIEnv.Root) {
                for (int i = 0; i < ms.Count; i++)
                    lista.Add(ms[i]);
            }
            List<object> bloco = splitter.Splitting(lista);
            if (bloco != null) { 
                Console.WriteLine("Rank" + MPIEnv.Rank + ":\n" + printMatrix(bloco));
            }
        }
        public static string printMatrix(List<object> lista) {
            string s = "";
            foreach (object o in lista) {
                ulong[][] ms = (ulong[][])o;
                s += ms.Length + " x " + ms[0].Length + "\n";
                foreach (var a in ms) {
                    foreach (var b in a) {
                        s = s + b + " ";
                    }
                    s = s + "\n";
                }
            }
            return s;
        }
        /* // Aula anterior
        */
        public static void test1() {
            //var lc = Generator.Gen(file, N, MIN, MAX);
            //MPIEnv.reduce();
            //MPIEnv.allReduce();
            //MPIEnv.immediateSendReceive();
            //MPIEnv.broadcast(10);
            //MPIEnv.scatter();
            //MPIEnv.allGather();
            MPIEnv.allToAll();
        }
    }
}
