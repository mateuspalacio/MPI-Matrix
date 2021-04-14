using System;

namespace AulaMPI2 {
    public class AppMain {
        public static void Main(string[] args) {
            MPITest app = new MPITest();
            app.mpi_start();

            //app.reduce();
            //app.allReduce();
            app.iSendReceive();

            app.mpi_stop();
        }
    }
}
