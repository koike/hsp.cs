# hsp.cs
HSPのコードをC#に変換し, 実行します  

## LICENSE
[The MIT License](https://github.com/kkrnt/hsp.cs/blob/master/LICENSE)

## Download
[v0.1.2](https://github.com/kkrnt/hsp.cs/releases/tag/v0.1.2)  
[v0.1.1](https://github.com/kkrnt/hsp.cs/releases/tag/v0.1.1)  
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

## Basic Grammar
- if
- else
- for
- next
- while
- wend
- repeat
- loop
- switch
- swend
- swbreak
- case
- default
- _break
- _continue

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
<img src="http://o8o.jp/hsp.cs.png" width="40%">  

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
if d == 0 {
        print "no"
}else{
        print "yes"
}
for i,0,3,1
        print i
next
e = 3
while e>0
        print e
        e--
wend
repeat 5
        print cnt
loop
f = 1
switch f
case 0
        print "f=0"
        swbreak
case 1
        print "f=1"
case 2
        print "f=2"
default
        print "f!=0"
        swbreak
swend

========================

<C# Code>
using System;
public class Program
{
public static void Main()
{
int cnt = 0;
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
if (d == 0)
{
Console.WriteLine("no");
}else{
Console.WriteLine("yes");
}
for (var i = 0; i != 3; i += 1 )
{
Console.WriteLine(i);
}
dynamic e = 3;
while (e>0)
{
Console.WriteLine(e);
e --;
}
for (cnt=0; cnt<5; cnt++)
{
Console.WriteLine(cnt);
}
dynamic f = 1;
string switchTmpString_5a156ec5380d4edc85b5ccdd7646d24d = f.ToString();
switch (switchTmpString_5a156ec5380d4edc85b5ccdd7646d24d)
{
case "0":
Console.WriteLine("f=0");
break;
case "1":
Console.WriteLine("f=1");
goto case "2";
case "2":
Console.WriteLine("f=2");Console.WriteLine("f!=0");
break;
default:
Console.WriteLine("f!=0");
break;
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
yes
0
1
2
3
2
1
0
1
2
3
4
f=1
f=2
f!=0

========================
```