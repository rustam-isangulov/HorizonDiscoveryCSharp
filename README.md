# HorizonDiscoveryCSharp
Horizon Discovery Test (C#)

### Test environment

.NET SDK:
 Version:   7.0.203
 Commit:    5b005c19f5

Runtime Environment:
 OS Name:     Windows
 OS Version:  10.0.19045
 OS Platform: Windows
 RID:         win10-x64

Host:
  Version:      7.0.5
  Architecture: x64
  Commit:       8042d61b17

### Steps

1 Clone this repository

```shell
git clone https://github.com/rustam-isangulov/HorizonDiscoveryCSharp.git
```

2 Change directoty to `HorizonDiscoveryCSharp\LogProcessor`

```shell
cd .\HorizonDiscoveryCSharp\LogProcessor
```

3 Run unit tests

```shell
dotnet test -l:"console;verbosity=detailed"
```

<details><summary>expected output</summary>
<p>

```shell
[xUnit.net 00:00:00.00] xUnit.net VSTest Adapter v2.4.5+1caef2f33e (64-bit .NET 7.0.5)
[xUnit.net 00:00:00.36]   Discovering: LogProcessor.Tests
[xUnit.net 00:00:00.38]   Discovered:  LogProcessor.Tests
[xUnit.net 00:00:00.39]   Starting:    LogProcessor.Tests
[xUnit.net 00:00:00.48]   Finished:    LogProcessor.Tests
  Passed LogProcessor.Tests.ProgramUnitTests.Test_GoodArgumentsString [24 ms]
  Passed LogProcessor.Tests.ProgramUnitTests.Test_EmptyArgumentsString [22 ms]

Test Run Successful.
Total tests: 2
     Passed: 2
 Total time: 1.0208 Seconds
```
</p>
</details>

4 Publish locally

```shell
dotnet publish -o publish .\src\LogProcessor\
```
5 Run application

```shell
.\publish\LogProcessor.exe --files "one.txt" "two.txt" --type "NCSA"
```

<details><summary>expected output</summary>
<p>

```shell
files to process:
        : one.txt
        : two.txt
log type: NCSA
```
</p>
</details>
