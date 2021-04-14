using System;
using System.Collections.Generic;
using System.Text;
using MPI;

namespace AulaMPI2 {
    public class MPITest {
        protected MPI.Environment mpi = null;
        protected Intracommunicator comm_world;
        protected readonly int ROOT = 0;
        protected int rank;
        protected int size;

        public void mpi_start() {
            string[] args = System.Environment.GetCommandLineArgs();
            mpi = new MPI.Environment(ref args);
            comm_world = Communicator.world;

            rank = comm_world.Rank;
            size = comm_world.Size;
        }
        public void mpi_stop() { mpi.Dispose(); }

        public void reduce() {
            Console.WriteLine("Meu id é: " + rank + " size: " + size);

            int valor = rank+1;
            valor = comm_world.Reduce<int>(valor, Operation<int>.Add, ROOT);
            if (rank == ROOT) {
                Console.WriteLine("Eu sou o node "+rank+"!!! O valor é: " + valor);
            }
            comm_world.Barrier(); // Tarefas vao aguardar aqui
        }

        public void allReduce() {
            int n = 10;
            var data = new ulong[n];
            Random r = new Random();
            for (int i = 0; i < n; i++) {
                data[i] = (ulong) r.Next(1, 1000);
            }

            var resultado = new ulong[n];
            comm_world.Allreduce<ulong>(data, Operation<ulong>.Min, ref resultado);

            if (rank == ROOT) {
                foreach (var d in resultado)
                    Console.Write(d + " ");
            }
            comm_world.Barrier();
        }
        public void iSendReceive() {
            int tag = 0;
            Request[] requests = new Request[2];
            RequestList bag_requests = new RequestList();

            int dest = (rank + 1) == size ? 0 : rank + 1;
            int source = (rank - 1) < 0 ? size - 1 : rank - 1;

            ulong[] matriz = { (ulong)rank, (ulong)rank, (ulong)rank, (ulong)rank };

            var tmp = new ulong[matriz.Length];
            requests[0] = comm_world.ImmediateSend<ulong>(matriz, dest, tag);
            requests[1] = comm_world.ImmediateReceive<ulong>(source, tag, tmp);

            foreach (Request r in requests)
                bag_requests.Add(r);

            bag_requests.WaitAll();

            string s = "\nnode" + rank + "recebeu: " + " ";
            foreach (var a in tmp)
                s = s + a + " ";

            s = s + "\nnode" + rank + " enviou: ";
            foreach (var a in matriz)
                s = s + a + " ";

            s = s + "\n";
            Console.WriteLine(s);

        }
    }
}
