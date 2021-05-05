using System;
using System.Collections.Generic;
using System.Text;
using MPI;

namespace AulaMPI2
{
    public class Splitter
    {
        private static int cont = 0;
        private static bool pass = true;
        public ulong[][] Splitting(List<object> lista)
        {
            int num_nodes = MPIEnv.Size;
            int len_partition = lista.Count / num_nodes;
            RequestList bag = new RequestList();
            if (MPIEnv.Rank == MPIEnv.Root)
            { // Quem entra aqui é o nó gerente!!!
                if (lista.Count == 0 || (lista.Count % num_nodes != 0))
                {
                    string msn = "Lista vazia, ou lista.Count nao é divisível perfeitamente por N processos!!!";
                    Console.WriteLine(msn);
                    throw new Exception(msn);
                }

                List<object> tmp = new List<object>(len_partition + 2);
                tmp.Add(lista[0]);

                int id_processo = 0;
                for (int i = 1; i <= lista.Count; i++)
                {

                    Console.WriteLine("i%len_partition: " + i % len_partition);
                    if (i % len_partition != 0)
                    {
                        tmp.Add(lista[i]);
                    }
                    else
                    {
                        int dest = id_processo++;
                        Console.WriteLine("Send to rank " + dest);
                        bag.Add(MPIEnv.Comm_world.ImmediateSend<object>(tmp, dest, TAGs.DIST_GERAL));
                        tmp = new List<object>(len_partition + 2);
                        if (i == lista.Count) break;
                        tmp.Add(lista[i]);
                    }
                }
            }

            object r = MPIEnv.Comm_world.Receive<object>(MPIEnv.Root, TAGs.DIST_GERAL);
            bag.WaitAll();

            List<ulong[][]> ms = new List<ulong[][]>();
            MMulti(r);
            while (pass) {
                if (MPIEnv.Root == MPIEnv.Rank)
                {
                    for (int i = 1; i < MPIEnv.Size; i++)
                    {
                        ms.Add(MPIEnv.Comm_world.Receive<ulong[][]>(i, 0));
                    }
                    List<ulong[][]> listM = new List<ulong[][]>();
                    foreach (ulong[][] m in ms)
                    {
                        listM.Add(m);
                        if (listM.Count == 2)
                        {
                            ulong[][] res = Util.multiply(listM[0], listM[1]);
                            listM = new List<ulong[][]>();
                            listM.Add(res);
                        }
                    }
                    ms = listM;
                }
            }
            

            return ms[0];
        }
        ulong[][] res = null;
        private List<ulong[][]> MMulti(object r)
        {
            List<ulong[][]> ms = new List<ulong[][]>();
            List<object> p = (List<object>)r;
            if(ms.Count != 1)
            {
                foreach (object matriz in p)
                {
                    ulong[][] m = (ulong[][])matriz;
                    ms.Add(m);
                    if (ms.Count == 2)
                    {
                        res = Util.multiply(ms[0], ms[1]);
                        ms = new List<ulong[][]>();
                        ms.Add(res);
                    }
                }
            }

            for (int i = 1; i < MPIEnv.Size; i++)
            {
                if (i == MPIEnv.Rank)
                {
                    MPIEnv.Comm_world.Send<ulong[][]>(ms[0], 0, 0);
                    cont++;
                }
                if (MPIEnv.Size == cont)
                {
                    pass = false;
                }
            }
            return ms;
        }
    }
}
