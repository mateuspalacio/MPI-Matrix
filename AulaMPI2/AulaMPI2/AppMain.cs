using System;
using System.Collections;
using System.Collections.Generic;
using MPI;

namespace AulaMPI2 {
    public class AppMain {
        // Gerador de matriz:
        public static readonly int N = 16; // Nro de matrizes para multiplicar
        public static int MIN = 5;
        public static int MAX = 10;
        public static string file = "C:\\Users\\USUARIO\\Documents\\Programs\\Comp Paralela\\MPI-Matrix\\AulaMPI2\\AulaMPI2\\tmp\\matrizes\\matrix" + N+".txt";

        public static void Main(string[] args) {
            try
            {
                MPIEnv.mpi_start();
                //test1();
                test2();
                MPIEnv.mpi_stop();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            Guid.NewGuid();
        }
        public static void test2() {
            List<object> lista = new List<object>();
            Splitter splitter = new Splitter();
            List<ulong[][]> ms = Generator.MatrixList(file);


            if (MPIEnv.Rank == MPIEnv.Root)
            {
                for (int i = 0; i < ms.Count; i++)
                    lista.Add(ms[i]);
            }
            List<ulong[][]> bloco = splitter.Splitting(lista);
            if (MPIEnv.Root == MPIEnv.Rank)
            {
                Console.WriteLine("Rank" + MPIEnv.Rank + ":\n" + printMatrix(bloco));
            }

            ulong[][] b1 = new ulong[1][];
            int len = 0;
            foreach (ulong[][] o in bloco)
            {
                b1 = o;
                len = b1.Length;
            }
            int b2 = MPIEnv.Size;
            int tam =  len/ b2;
            if(len%b2 != 0)
            {
                tam++;
            }
            int cont = 0;
            List<ulong[]> vetores = new List<ulong[]>();
            RequestList bag = new RequestList();
            int process = 0;
            foreach (var a in b1)
            {
                vetores.Add(a);
                cont++;
                if(cont >= tam || a == b1[b1.Length - 1])
                {
                    int rank = process++;
                    bag.Add(MPIEnv.Comm_world.ImmediateSend<List<ulong[]>>(vetores, rank, TAGs.DIST_GERAL));
                    vetores = new List<ulong[]>();
                    cont = 0;
                }
            }

            List<ulong[]> r = MPIEnv.Comm_world.Receive<List<ulong[]>>(MPIEnv.Root, TAGs.DIST_GERAL);
            foreach (var col in r)
            {
                Quick_Sort(col, 0, col.Length -1);
            }
            List<List<ulong[]>> v = new List<List<ulong[]>>();
            if (MPIEnv.Root == MPIEnv.Rank)
            {
                v.Add(r);
            }
            else
            {
                MPIEnv.Comm_world.Send<List<ulong[]>>(r, MPIEnv.Root, 0);
            }
            bag.WaitAll();
            int tam2 = b1.Length * b1[0].Length;
            ulong[] vetorR = new ulong[tam2];
            
            if (MPIEnv.Root == MPIEnv.Rank)
            {
                for (int i = 1; i < MPIEnv.Size; i++)
                {
                    v.Add(MPIEnv.Comm_world.Receive<List<ulong[]>>(i, 0));

                }
                foreach(var array1 in v)
                {
                    foreach (var array2 in array1)
                    {
                        try
                        {
                            for (int i = 0; i < array2.Length; i++)
                            {
                                vetorR[cont] = array2[i];
                                cont++;
                            }
                        } catch(Exception e)
                        {
                            Console.WriteLine("Olha o erro "+ e);
                        }
                    }
                }
                Quick_Sort(vetorR, 0, vetorR.Length - 1);
                cont = 0;
                for(int i = 0; i < bloco[0].Length; i++)
                {
                    for(int j = 0; j < bloco[0][0].Length; j++)
                    {
                        bloco[0][i][j] = vetorR[cont];
                        cont++;
                    }
                }

                Console.WriteLine("Rank" + MPIEnv.Rank + ":\n" + printMatrix(bloco));
            }

        }
        public static string printMatrix(List<ulong[][]> lista)
        {
            string s = "";
            foreach (object o in lista) {
                ulong[][] ms = (ulong[][])o;
                s += ms.Length + " x " + ms[0].Length + "\n";
                foreach (var a in ms)
                {
                    foreach (var b in a)
                    {
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
            var lc = Generator.Gen(file, N, MIN, MAX);
            //MPIEnv.reduce();
            //MPIEnv.allReduce();
            //MPIEnv.immediateSendReceive();
            //MPIEnv.broadcast(10);
            //MPIEnv.scatter();
            //MPIEnv.allGather();
            MPIEnv.allToAll();
        }
        private static void Quick_Sort(ulong[] arr, int left, int right)
        {
            if (left < right)
            {
                int pivot = Partition(arr, left, right);

                if (pivot > 1)
                {
                    Quick_Sort(arr, left, pivot - 1);
                }
                if (pivot + 1 < right)
                {
                    Quick_Sort(arr, pivot + 1, right);
                }
            }

        }

        private static int Partition(ulong[] arr, int left, int right)
        {
            ulong pivot = arr[left];
            while (true)
            {
                while (arr[left] < pivot)
                {
                    left++;
                }
                while (arr[right] > pivot)
                {
                    right--;
                }
                if (left < right)
                {
                    if (arr[left] == arr[right]) return right;
                    ulong temp = arr[left];
                    arr[left] = arr[right];
                    arr[right] = temp;

                } else {
                    return right;
                }
            }
        }
    }
}
