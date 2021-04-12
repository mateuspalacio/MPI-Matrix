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
    }
}
