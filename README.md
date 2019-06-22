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
у всех опциональных параметров есть значения по умолчанию
если cpus не указан используются все физические ядра

from main dir
```
utils/runner --from=1 --to=300 --cpus=6 'dotnet run -c Release --project src/WorkerWrapper.ConsoleApp'
rm -f tmp/sol.zip && zip -j tmp/sol.zip output/*.sol
```

### submit url
https://icfpcontest2019.github.io/submit/