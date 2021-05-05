# MPI program in C# for matrix multiplication 

Steps:

* Download MS-MPI
* Download MPI.NET SDK
* Open solution in Visual Studio
* Adjust the file path for the matrices 
* Run Test 1 method first, with Test 2 commented(build only, so `CTRL + F5`)
* Run Test 2 with Test 1 commented(build only, so `CTRL + F5`)
* On the Command Prompt go inside the project folder then go inside bin -> netcoreapp3.1
* When on the final path, and Test2() being executed in build mode(`CTRL+F5`), type `mpiexec -n {number by which the matrices will be divided, a multiple of 4, or the number 2 or number 1} .\MPIAula2.exe`


This program was developed using the initial code provided by the professor, and finished by the students Mateus Palácio, Victor Santos and Felipe Holanda.
