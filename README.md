# icfpc-2019

### How to build

```
$ dotnet build -c Release
```

### How to run

Go to _./src/WorkerWrapper.ConsoleApp_

```
$ cd ./src/WorkerWrapper.ConsoleApp
```

Run console app passing problem description file as STDIN and write it to appropriate file

```
$ dotnet run -c Release < prob-NNN.desc > prob-NNN.sol
```



### runner
from main dir
```
utils/runner 'dotnet run -c Release --project src/WorkerWrapper.ConsoleApp'
rm -f tmp/sol.zip && zip -j tmp/sol.zip output/*.sol
```