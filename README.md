# hsp.cs
HSPのコードをC#に変換し, 実行します  

## LICENSE
[The MIT License](https://github.com/kkrnt/hsp.cs/blob/master/LICENSE)

## Download
[v0.1.0](https://github.com/kkrnt/hsp.cs/releases/tag/v0.1.0)  

## Usage
```
hsp.cs [.hsp] [option]
Option:
     -o  --output <file>       output executive file(.exe)
     -c  --cs     <file>       output csharp source file(.cs)
Example:
     hsp.cs.exe sample.hsp
     hsp.cs.exe sample.hsp -o sample.exe -c sample.cs
```

## BasicGrammar
- if

## Function
- int
- double
- str
- abs
- absf
- sin
- cos
- tan
- atan
- expf
- logf
- powf
- sqrt
- instr
- strlen
- limit
- limitf
- length
- length2
- length3
- length4
- gettime

## Command
- print
- mes
- exist
- delete
- mkdir
- split

## Example
```
hsp.cs.exe sample.hsp

<HSP Code>
a="123"
b="456"
hoge=str (int (a+double (b)))
print hoge
c = 0
if c==0 : print "c=0" : print "ok"
d = 1
if d==1 {
        print "d=0"
        print "ok"
}

========================

<C# Code>
using System;
public class Program
{
public static void Main()
{
dynamic a = "123";
dynamic b = "456";
dynamic hoge =  ( int.Parse ( a + double.Parse ( b ) ) ).ToString();
Console.WriteLine(hoge);
dynamic c = 0;
if (c == 0)
{
Console.WriteLine("c=0");
Console.WriteLine("ok");
};
dynamic d = 1;
if (d == 1)
{
Console.WriteLine("d=0");
Console.WriteLine("ok");
}
}
}
========================

構文エラーなし

========================

意味エラーなし

========================

<実行結果>
123456
c=0
ok
d=0
ok

========================
```

<img src="http://o8o.jp/hsp.cs.png" width="40%">
