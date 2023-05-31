# Horizon Discovery - Technical Test (C#)

## Objective

> *Write an application (console is fine) that outputs a ranked list of URLs along with how many "hits" each has got. Do use artistic license but document any assumptions you make.*

## Run the application on example logs

### Test environment

```shell
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
```

### Steps

#### 1 Clone this repository

```shell
git clone https://github.com/rustam-isangulov/HorizonDiscoveryCSharp.git
```

#### 2 Change directoty to `HorizonDiscoveryCSharp\LogProcessor`

```shell
cd ./HorizonDiscoveryCSharp/LogProcessor
```

#### 3 *(optional)* Run unit tests

```shell
dotnet test -l:"console;verbosity=detailed"
```

<details><summary>expected output</summary>
<p>

```shell
[xUnit.net 00:00:00.00] xUnit.net VSTest Adapter v2.4.5+1caef2f33e (64-bit .NET 7.0.5)
[xUnit.net 00:00:00.26]   Discovering: LogProcessor.Tests
[xUnit.net 00:00:00.28]   Discovered:  LogProcessor.Tests
[xUnit.net 00:00:00.29]   Starting:    LogProcessor.Tests
  Passed LogProcessor.Tests.FilterUnitTests.ApplyingFilterWithFieldsMismatch [4 ms]

  Passed LogProcessor.Tests.FormatInfoUnitTests.W3CParsing [6 ms]

  Passed LogProcessor.Tests.ProcessorUnitTests.CreaingProcessorWithNoFormatInfo [3 ms]

  Passed LogProcessor.Tests.AggregateUnitTests.BasicAggregateWithNonUniqueDestinationFields [3 ms]

  Passed LogProcessor.Tests.SorterUnitTests.SortingByNonExistingField [5 ms]

  Passed LogProcessor.Tests.LogEntriesUnitTests.CreatingLogEntries [8 ms]

  Passed LogProcessor.Tests.LogFieldsUnitTests.CreatingFieldsObject [8 ms]

  Passed LogProcessor.Tests.SorterUnitTests.SortingLogEntriesAscending [1 ms]

  Passed LogProcessor.Tests.LogFieldsUnitTests.CreatingFieldsWithNonDistinctNames [< 1 ms]

  Passed LogProcessor.Tests.SorterUnitTests.SortingLogEntriesDescending [< 1 ms]

  Passed LogProcessor.Tests.FormatInfoUnitTests.W3CColumnNamesParsingForBadFieldEmpty [4 ms]

  Passed LogProcessor.Tests.AggregateUnitTests.BasicAggregate [4 ms]

  Passed LogProcessor.Tests.FilterUnitTests.FilteringLogEntries [7 ms]

  Passed LogProcessor.Tests.FormatInfoUnitTests.W3CSelectingEntries [3 ms]

  Passed LogProcessor.Tests.FormatInfoUnitTests.NCSAParsing [< 1 ms]
  Passed LogProcessor.Tests.ProcessorUnitTests.ProcessingNCSALogs [9 ms]

  Passed LogProcessor.Tests.ProcessorUnitTests.EmptyProcessorEmptyFileList [< 1 ms]

  Passed LogProcessor.Tests.FormatInfoUnitTests.W3CColumnNamesParsingForBadFieldSpecifier [2 ms]

  Passed LogProcessor.Tests.ProcessorUnitTests.ProcessingW3CLogsWithFilter [2 ms]

  Passed LogProcessor.Tests.FormatInfoUnitTests.W3CColumnNamesParsing [2 ms]
  Passed LogProcessor.Tests.ProcessorUnitTests.ProcessingW3CLogs [2 ms]

  Passed LogProcessor.Tests.ProcessorUnitTests.ProcessingW3CLogsWithSorter [3 ms]

[xUnit.net 00:00:00.39]   Finished:    LogProcessor.Tests
  Passed LogProcessor.Tests.ProcessorUnitTests.ProcessingNCSALogsWithFilter [2 ms]

  Passed LogProcessor.Tests.ProcessorUnitTests.ProcessingW3CLogsWithAggregator [2 ms]

  Passed LogProcessor.Tests.ProgramUnitTests.GoodArgumentsString [38 ms]

  Passed LogProcessor.Tests.ProgramUnitTests.EmptyArgumentsString [23 ms]
  Standard Output Messages:
 Description:
   Log processor parses log files, filters, aggregates and outputs ranked list of entries.

 Usage:
   testhost [options]

 Options:
   --files <files> (REQUIRED)    Files to process
   --type <NCSA|W3C> (REQUIRED)  Processor type
   --version                     Show version information
   -?, -h, --help                Show help and usage information






Test Run Successful.
Total tests: 26
     Passed: 26
 Total time: 0.7814 Seconds
```
</p>
</details>

#### 4 Publish locally

```shell
dotnet publish -o publish ./src/LogProcessor/
```
#### 5 Run application

```shell
./publish/LogProcessor --files "../TestLogs/W3CLog.txt" "../TestLogs/W3CLog1.txt" --type "W3C"
```

<details><summary>expected output</summary>
<p>

```shell
files to process:
         ..\TestLogs\W3CLog.txt
         ..\TestLogs\W3CLog1.txt
log type: W3C
Processed logs output:
18 /images/picture.jpg 2002-05-04 17:42:22
12 /images/cartoon.gif 2002-05-04 17:42:25
6 /images/text.txt 2002-05-03 17:42:25
```
</p>
</details>

```shell
./publish/LogProcessor.exe --files "../TestLogs/NCSALog.txt" "../TestLogs/NCSALog1.txt" --type "NCSA"
```

<details><summary>expected output</summary>
<p>

```shell
files to process:
         ..\TestLogs\NCSALog.txt
         ..\TestLogs\NCSALog1.txt
log type: NCSA
Processed logs output:
18 /images/picture.jpg 04/May/2002:17:42:22 +0100 3256
12 /images/cartoon.gif 04/May/2002:17:42:25 +0100 3256
6 /images/text.txt 03/May/2002:17:42:25 +0100 3256
```
</p>
</details>
