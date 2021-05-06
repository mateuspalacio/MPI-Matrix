using System;
using System.Collections.Generic;
using System.Text;
using MPI;

namespace AulaMPI2
{
    public class Splitter
    {
        private static ulong[][] rootM;
        public List<ulong[][]> Splitting(List<object> lista)
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
            MMulti(r);

            bag.WaitAll();
            List<ulong[][]> ms = new List<ulong[][]>();

                if (MPIEnv.Root == MPIEnv.Rank)
                {
                    for (int i = 1; i < MPIEnv.Size; i++)
                    {
                        ms.Add(MPIEnv.Comm_world.Receive<ulong[][]>(i, 0));
                        Console.WriteLine("eu entrei no receive");

                    }
                    List<ulong[][]> listM = new List<ulong[][]>();
                    listM.Add(rootM);
                    foreach (ulong[][] m in ms)
                    {
                        listM.Add(m);
                        if (listM.Count == 2)
                        {
                            ulong[][] res = Util.multiply(listM[0], listM[1]);
                            listM = new List<ulong[][]>();
                            listM.Add(res);
                            Console.WriteLine("eu entrei no list add");

                        }
                    }
                    ms = listM;
                    verdade = false;
                    Console.WriteLine("eu entrei no if");

                }
            


            return ms;
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
                        if(MPIEnv.Rank == MPIEnv.Root)
                        {
                            Console.WriteLine("entrouuuu");
                        }
                    }
                }
            }
            if (MPIEnv.Rank == MPIEnv.Root)
            {
                rootM = ms[0];
                Console.WriteLine("entrouuuddsdsdu");
            }
            for (int i = 1; i < MPIEnv.Size; i++)
            {
                if (i == MPIEnv.Rank)
                {
                    MPIEnv.Comm_world.Send<ulong[][]>(ms[0], 0, 0);
                }

            }
            return ms;
        }
    }
}
