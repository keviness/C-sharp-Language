# [C#压缩解压zip 文件](https://www.cnblogs.com/greatverve/archive/2011/12/27/csharp-zip.html)

我在做项目的时候需要将文件进行压缩和解压缩，于是就从[http://www.icsharpcode.net](http://www.icsharpcode.net/)下载了关于压缩和解压缩的源码，但是下载下来后，面对这么多的代码，一时不知如何下手。只好耐下心来，慢慢的研究，总算找到了门路。针对自己的需要改写了文件压缩和解压缩的两个类，分别为ZipClass和UnZipClass。其中碰到了不少困难，就决定写出来压缩和解压的程序后，一定把源码贴出来共享，让首次接触压缩和解压缩的朋友可以少走些弯路。下面就来解释如何在C#里用[http://www.icsharpcode.net](http://www.icsharpcode.net/)下载的SharpZipLib进行文件的压缩和解压缩。

```
首先需要在项目里引用SharpZipLib.dll。然后修改其中的关于压缩和解压缩的类。实现源码如下： 
```


/// `<summary>`
/// 压缩文件
/// `</summary>`
using System;
using System.IO;
using ICSharpCode.SharpZipLib.Checksums;
using ICSharpCode.SharpZipLib.Zip;
using ICSharpCode.SharpZipLib.GZip;
namespace Compression
{
public class ZipClass
{

public void ZipFile(string FileToZip, string ZipedFile ,int CompressionLevel, int BlockSize)
{
//如果文件没有找到，则报错
if (! System.IO.File.Exists(FileToZip))
{
throw new System.IO.FileNotFoundException("The specified file " + FileToZip + " could not be found. Zipping aborderd");
}

System.IO.FileStream StreamToZip = new System.IO.FileStream(FileToZip,System.IO.FileMode.Open , System.IO.FileAccess.Read);
System.IO.FileStream ZipFile = System.IO.File.Create(ZipedFile);
ZipOutputStream ZipStream = new ZipOutputStream(ZipFile);
ZipEntry ZipEntry = new ZipEntry("ZippedFile");
ZipStream.PutNextEntry(ZipEntry);
ZipStream.SetLevel(CompressionLevel);
byte[] buffer = new byte[BlockSize];
System.Int32 size =StreamToZip.Read(buffer,0,buffer.Length);
ZipStream.Write(buffer,0,size);
try
{
while (size < StreamToZip.Length)
{
int sizeRead =StreamToZip.Read(buffer,0,buffer.Length);
ZipStream.Write(buffer,0,sizeRead);
size += sizeRead;
}
}
catch(System.Exception ex)
{
throw ex;
}
ZipStream.Finish();
ZipStream.Close();
StreamToZip.Close();
}

public void ZipFileMain(string[] args)
{
string[] filenames = Directory.GetFiles(args[0]);

Crc32 crc = new Crc32();
ZipOutputStream s = new ZipOutputStream(File.Create(args[1]));

s.SetLevel(6); // 0 - store only to 9 - means best compression

foreach (string file in filenames)
{
//打开压缩文件
FileStream fs = File.OpenRead(file);

```
byte[] buffer = new byte[fs.Length];
fs.Read(buffer, 0, buffer.Length);
ZipEntry entry = new ZipEntry(file);

entry.DateTime = DateTime.Now;

// set Size and the crc, because the information
// about the size and crc should be stored in the header
// if it is not set it is automatically written in the footer.
// (in this case size == crc == -1 in the header)
// Some ZIP programs have problems with zip files that don't store
// the size and crc in the header.
entry.Size = fs.Length;
fs.Close();

crc.Reset();
crc.Update(buffer);

entry.Crc  = crc.Value;

s.PutNextEntry(entry);

s.Write(buffer, 0, buffer.Length);
```

}

s.Finish();
s.Close();
}
}
}
现在再来看看解压文件类的源码
/// `<summary>`
/// 解压文件
/// `</summary>`
using System;
using System.Text;
using System.Collections;
using System.IO;
using System.Diagnostics;
using System.Runtime.Serialization.Formatters.Binary;
using System.Data;
using ICSharpCode.SharpZipLib.BZip2;
using ICSharpCode.SharpZipLib.Zip;
using ICSharpCode.SharpZipLib.Zip.Compression;
using ICSharpCode.SharpZipLib.Zip.Compression.Streams;
using ICSharpCode.SharpZipLib.GZip;
namespace DeCompression
{
public class UnZipClass
{
public void UnZip(string[] args)
{
ZipInputStream s = new ZipInputStream(File.OpenRead(args[0]));

ZipEntry theEntry;
while ((theEntry = s.GetNextEntry()) != null)
{

```
      string directoryName = Path.GetDirectoryName(args[1]);
string fileName      = Path.GetFileName(theEntry.Name);

//生成解压目录
Directory.CreateDirectory(directoryName);

if (fileName != String.Empty) 
{   
 //解压文件到指定的目录
 FileStream streamWriter = File.Create(args[1]+theEntry.Name);

 int size = 2048;
 byte[] data = new byte[2048];
 while (true) 
 {
  size = s.Read(data, 0, data.Length);
  if (size > 0) 
  {
   streamWriter.Write(data, 0, size);
  } 
  else 
  {
   break;
  }
 }

 streamWriter.Close();
}
```

}
s.Close();
}
}
}
有了压缩和解压缩的类以后，就要在窗体里调用了。怎么？是新手，不会调用？Ok,接着往下看如何在窗体里调用。
首先在窗体里放置两个命令按钮（不要告诉我你不会放啊~），然后编写以下源码
/// `<summary>`
/// 调用源码
/// `</summary>`
private void button2_Click_1(object sender, System.EventArgs e)
{
string []FileProperties=new string[2];
FileProperties[0]="C:\\unzipped\\";//待压缩文件目录
FileProperties[1]="C:\\zip\\a.zip";  //压缩后的目标文件
ZipClass Zc=new ZipClass();
Zc.ZipFileMain(FileProperties);
}
private void button2_Click(object sender, System.EventArgs e)
{
string []FileProperties=new string[2];
FileProperties[0]="C:\\zip\\test.zip";//待解压的文件
FileProperties[1]="C:\\unzipped\\";//解压后放置的目标目录
UnZipClass UnZc=new UnZipClass();
UnZc.UnZip(FileProperties);
}


---


/// `<summary>`
/// 压缩文件
/// `</summary>`
using System;
using System.IO;
using ICSharpCode.SharpZipLib.Checksums;
using ICSharpCode.SharpZipLib.Zip;
using ICSharpCode.SharpZipLib.GZip;
namespace Compression
{
public class ZipClass
{

public void ZipFile(string FileToZip, string ZipedFile ,int CompressionLevel, int BlockSize)
{
//如果文件没有找到，则报错
if (! System.IO.File.Exists(FileToZip))
{
throw new System.IO.FileNotFoundException("The specified file " + FileToZip + " could not be found. Zipping aborderd");
}

System.IO.FileStream StreamToZip = new System.IO.FileStream(FileToZip,System.IO.FileMode.Open , System.IO.FileAccess.Read);
System.IO.FileStream ZipFile = System.IO.File.Create(ZipedFile);
ZipOutputStream ZipStream = new ZipOutputStream(ZipFile);
ZipEntry ZipEntry = new ZipEntry("ZippedFile");
ZipStream.PutNextEntry(ZipEntry);
ZipStream.SetLevel(CompressionLevel);
byte[] buffer = new byte[BlockSize];
System.Int32 size =StreamToZip.Read(buffer,0,buffer.Length);
ZipStream.Write(buffer,0,size);
try
{
while (size < StreamToZip.Length)
{
int sizeRead =StreamToZip.Read(buffer,0,buffer.Length);
ZipStream.Write(buffer,0,sizeRead);
size += sizeRead;
}
}
catch(System.Exception ex)
{
throw ex;
}
ZipStream.Finish();
ZipStream.Close();
StreamToZip.Close();
}

public void ZipFileMain(string[] args)
{
string[] filenames = Directory.GetFiles(args[0]);

Crc32 crc = new Crc32();
ZipOutputStream s = new ZipOutputStream(File.Create(args[1]));

s.SetLevel(6); // 0 - store only to 9 - means best compression

foreach (string file in filenames)
{
//打开压缩文件
FileStream fs = File.OpenRead(file);

```
byte[] buffer = new byte[fs.Length];
fs.Read(buffer, 0, buffer.Length);
ZipEntry entry = new ZipEntry(file);

entry.DateTime = DateTime.Now;

// set Size and the crc, because the information
// about the size and crc should be stored in the header
// if it is not set it is automatically written in the footer.
// (in this case size == crc == -1 in the header)
// Some ZIP programs have problems with zip files that don't store
// the size and crc in the header.
entry.Size = fs.Length;
fs.Close();

crc.Reset();
crc.Update(buffer);

entry.Crc  = crc.Value;

s.PutNextEntry(entry);

s.Write(buffer, 0, buffer.Length);
```

}

s.Finish();
s.Close();
}
}
}
现在再来看看解压文件类的源码
/// `<summary>`
/// 解压文件
/// `</summary>`
using System;
using System.Text;
using System.Collections;
using System.IO;
using System.Diagnostics;
using System.Runtime.Serialization.Formatters.Binary;
using System.Data;
using ICSharpCode.SharpZipLib.BZip2;
using ICSharpCode.SharpZipLib.Zip;
using ICSharpCode.SharpZipLib.Zip.Compression;
using ICSharpCode.SharpZipLib.Zip.Compression.Streams;
using ICSharpCode.SharpZipLib.GZip;
namespace DeCompression
{
public class UnZipClass
{
public void UnZip(string[] args)
{
ZipInputStream s = new ZipInputStream(File.OpenRead(args[0]));

ZipEntry theEntry;
while ((theEntry = s.GetNextEntry()) != null)
{

```
      string directoryName = Path.GetDirectoryName(args[1]);
string fileName      = Path.GetFileName(theEntry.Name);

//生成解压目录
Directory.CreateDirectory(directoryName);

if (fileName != String.Empty) 
{   
 //解压文件到指定的目录
 FileStream streamWriter = File.Create(args[1]+theEntry.Name);

 int size = 2048;
 byte[] data = new byte[2048];
 while (true) 
 {
  size = s.Read(data, 0, data.Length);
  if (size > 0) 
  {
   streamWriter.Write(data, 0, size);
  } 
  else 
  {
   break;
  }
 }

 streamWriter.Close();
}
```

}
s.Close();
}
}
}
有了压缩和解压缩的类以后，就要在窗体里调用了。怎么？是新手，不会调用？Ok,接着往下看如何在窗体里调用。
首先在窗体里放置两个命令按钮（不要告诉我你不会放啊~），然后编写以下源码
/// `<summary>`
/// 调用源码
/// `</summary>`
private void button2_Click_1(object sender, System.EventArgs e)
{
string []FileProperties=new string[2];
FileProperties[0]="C:\\unzipped\\";//待压缩文件目录
FileProperties[1]="C:\\zip\\a.zip";  //压缩后的目标文件
ZipClass Zc=new ZipClass();
Zc.ZipFileMain(FileProperties);
}
private void button2_Click(object sender, System.EventArgs e)
{
string []FileProperties=new string[2];
FileProperties[0]="C:\\zip\\test.zip";//待解压的文件
FileProperties[1]="C:\\unzipped\\";//解压后放置的目标目录
UnZipClass UnZc=new UnZipClass();
UnZc.UnZip(FileProperties);
}


示例二：


/// `<summary>`
/// Zip 压缩文件
/// `</summary>`
public class Zip
{
public Zip()
{

```
}
#region 加压方法
/// <summary>
/// 功能：压缩文件（暂时只压缩文件夹下一级目录中的文件，文件夹及其子级被忽略）
/// </summary>
/// <param name="dirPath">被压缩的文件夹夹路径</param>
/// <param name="zipFilePath">生成压缩文件的路径，为空则默认与被压缩文件夹同一级目录，名称为：文件夹名+.zip</param>
/// <param name="err">出错信息</param>
/// <returns>是否压缩成功</returns>
public static bool ZipFile(string dirPath, string zipFilePath, out string err)
{
    err = "";
    if (dirPath == string.Empty)
    {
        err = "要压缩的文件夹不能为空！";
        return false;
    }
    if (!Directory.Exists(dirPath))
    {
        err = "要压缩的文件夹不存在！";
        return false;
    }
    //压缩文件名为空时使用文件夹名＋.zip
    if (zipFilePath == string.Empty)
    {
        if (dirPath.EndsWith("//"))
        {
            dirPath = dirPath.Substring(0, dirPath.Length - 1);
        }
        zipFilePath = dirPath + ".zip";
    }

    try
    {
        string[] filenames = Directory.GetFiles(dirPath);
        using (ZipOutputStream s = new ZipOutputStream(File.Create(zipFilePath)))
        {
            s.SetLevel(9);
            byte[] buffer = new byte[4096];
            foreach (string file in filenames)
            {
                ZipEntry entry = new ZipEntry(Path.GetFileName(file));
                entry.DateTime = DateTime.Now;
                s.PutNextEntry(entry);
                using (FileStream fs = File.OpenRead(file))
                {
                    int sourceBytes;
                    do
                    {
                        sourceBytes = fs.Read(buffer, 0, buffer.Length);
                        s.Write(buffer, 0, sourceBytes);
                    } while (sourceBytes > 0);
                }
            }
            s.Finish();
            s.Close();
        }
    }
    catch (Exception ex)
    {
        err = ex.Message;
        return false;
    }
    return true;
}
#endregion 

#region 解压
/// <summary>
/// 功能：解压zip格式的文件。
/// </summary>
/// <param name="zipFilePath">压缩文件路径</param>
/// <param name="unZipDir">解压文件存放路径,为空时默认与压缩文件同一级目录下，跟压缩文件同名的文件夹</param>
/// <param name="err">出错信息</param>
/// <returns>解压是否成功</returns>
public static bool UnZipFile(string zipFilePath, string unZipDir, out string err)
{
    err = "";
    if (zipFilePath == string.Empty)
    {
        err = "压缩文件不能为空！";
        return false;
    }
    if (!File.Exists(zipFilePath))
    {
        err = "压缩文件不存在！";
        return false;
    }
    //解压文件夹为空时默认与压缩文件同一级目录下，跟压缩文件同名的文件夹
    if (unZipDir == string.Empty)
        unZipDir = zipFilePath.Replace(Path.GetFileName(zipFilePath), Path.GetFileNameWithoutExtension(zipFilePath));
    if (!unZipDir.EndsWith("//"))
        unZipDir += "//";
    if (!Directory.Exists(unZipDir))
        Directory.CreateDirectory(unZipDir);

    try
    {
        using (ZipInputStream s = new ZipInputStream(File.OpenRead(zipFilePath)))
        {

            ZipEntry theEntry;
            while ((theEntry = s.GetNextEntry()) != null)
            {
                string directoryName = Path.GetDirectoryName(theEntry.Name);
                string fileName = Path.GetFileName(theEntry.Name);
                if (directoryName.Length > 0)
                {
                    Directory.CreateDirectory(unZipDir + directoryName);
                }
                if (!directoryName.EndsWith("//"))
                    directoryName += "//";
                if (fileName != String.Empty)
                {
                    using (FileStream streamWriter = File.Create(unZipDir + theEntry.Name))
                    {

                        int size = 2048;
                        byte[] data = new byte[2048];
                        while (true)
                        {
                            size = s.Read(data, 0, data.Length);
                            if (size > 0)
                            {
                                streamWriter.Write(data, 0, size);
                            }
                            else
                            {
                                break;
                            }
                        }
                    }
                }
            }//while
        }
    }
    catch (Exception ex)
    {
        err = ex.Message;
        return false;
    }
    return true;
}//解压结束
#endregion
```


事例源码：[https://files.cnblogs.com/greatverve/SharpZipLib_0860_Bin.zip](https://files.cnblogs.com/greatverve/SharpZipLib_0860_Bin.zip)

---

C#自带源码：[https://files.cnblogs.com/greatverve/Compress.rar](https://files.cnblogs.com/greatverve/Compress.rar)
-------------------------------------------------------------

System.IO.Compression 命名空间
注意：此命名空间在 .NET Framework 2.0 版中是新增的。
System.IO.Compression 命名空间包含提供基本的流压缩和解压缩服务的类。
(downmoon原作)
类                               说明
DeflateStream         提供用于使用 Deflate 算法压缩和解压缩流的方法和属性。
GZipStream             提供用于压缩和解压缩流的方法和属性。
枚举                         说明
CompressionMode 指定是否压缩或解压缩基础流。

下面以 GZipStream  为例说明

注意：此类在 .NET Framework 2.0 版中是新增的。

提供用于压缩和解压缩流的方法和属性。
命名空间:System.IO.Compression
程序集:System（在 system.dll 中）
语法
Visual Basic（声明）
Public Class GZipStream
Inherits Stream
Visual Basic（用法）
Dim instance As GZipStream

C#
public class GZipStream : Stream

C++
public ref class GZipStream : public Stream

J#
public class GZipStream extends Stream

JScript
public class GZipStream extends Stream

备注
此 类表示 GZip 数据格式，它使用无损压缩和解压缩文件的行业标准算法。这种格式包括一个检测数据损坏的循环冗余校验值。GZip 数据格式使用的算法与 DeflateStream 类的算法相同，但它可以扩展以使用其他压缩格式。这种格式可以通过不涉及专利使用权的方式轻松实现。gzip 的格式可以从 RFC 1952“GZIP file format specification 4.3（GZIP 文件格式规范 4.3）GZIP file format specification 4.3（GZIP 文件格式规范 4.3）”中获得。此类不能用于压缩大于 4 GB 的文件。

给继承者的说明 当从 GZipStream 继承时，必须重写下列成员：CanSeek、CanWrite 和 CanRead。

下面提供 一个完整的压缩与解压类(downmoon原作 )：

![](http://images.csdn.net/syntaxhighlighting/OutliningIndicators/None.gif)class clsZip
![](http://images.csdn.net/syntaxhighlighting/OutliningIndicators/ExpandedBlockStart.gif)    {
![](http://images.csdn.net/syntaxhighlighting/OutliningIndicators/InBlock.gif)        public void CompressFile ( string sourceFile, string destinationFile )
![](http://images.csdn.net/syntaxhighlighting/OutliningIndicators/ExpandedSubBlockStart.gif)        {
![](http://images.csdn.net/syntaxhighlighting/OutliningIndicators/InBlock.gif)            // make sure the source file is there
![](http://images.csdn.net/syntaxhighlighting/OutliningIndicators/InBlock.gif)            if ( File.Exists ( sourceFile ) == false )
![](http://images.csdn.net/syntaxhighlighting/OutliningIndicators/InBlock.gif)                throw new FileNotFoundException ( );
![](http://images.csdn.net/syntaxhighlighting/OutliningIndicators/InBlock.gif)
![](http://images.csdn.net/syntaxhighlighting/OutliningIndicators/InBlock.gif)            // Create the streams and byte arrays needed
![](http://images.csdn.net/syntaxhighlighting/OutliningIndicators/InBlock.gif)            byte[] buffer = null;
![](http://images.csdn.net/syntaxhighlighting/OutliningIndicators/InBlock.gif)            FileStream sourceStream = null;
![](http://images.csdn.net/syntaxhighlighting/OutliningIndicators/InBlock.gif)            FileStream destinationStream = null;
![](http://images.csdn.net/syntaxhighlighting/OutliningIndicators/InBlock.gif)            GZipStream compressedStream = null;
![](http://images.csdn.net/syntaxhighlighting/OutliningIndicators/InBlock.gif)
![](http://images.csdn.net/syntaxhighlighting/OutliningIndicators/InBlock.gif)            try
![](http://images.csdn.net/syntaxhighlighting/OutliningIndicators/ExpandedSubBlockStart.gif)            {
![](http://images.csdn.net/syntaxhighlighting/OutliningIndicators/InBlock.gif)                // Read the bytes from the source file into a byte array
![](http://images.csdn.net/syntaxhighlighting/OutliningIndicators/InBlock.gif)                sourceStream = new FileStream ( sourceFile, FileMode.Open, FileAccess.Read, FileShare.Read );
![](http://images.csdn.net/syntaxhighlighting/OutliningIndicators/InBlock.gif)
![](http://images.csdn.net/syntaxhighlighting/OutliningIndicators/InBlock.gif)                // Read the source stream values into the buffer
![](http://images.csdn.net/syntaxhighlighting/OutliningIndicators/InBlock.gif)                buffer = new byte[sourceStream.Length];
![](http://images.csdn.net/syntaxhighlighting/OutliningIndicators/InBlock.gif)                int checkCounter = sourceStream.Read ( buffer, 0, buffer.Length );
![](http://images.csdn.net/syntaxhighlighting/OutliningIndicators/InBlock.gif)
![](http://images.csdn.net/syntaxhighlighting/OutliningIndicators/InBlock.gif)                if ( checkCounter != buffer.Length )
![](http://images.csdn.net/syntaxhighlighting/OutliningIndicators/ExpandedSubBlockStart.gif)                {
![](http://images.csdn.net/syntaxhighlighting/OutliningIndicators/InBlock.gif)                    throw new ApplicationException ( );
![](http://images.csdn.net/syntaxhighlighting/OutliningIndicators/ExpandedSubBlockEnd.gif)                }
![](http://images.csdn.net/syntaxhighlighting/OutliningIndicators/InBlock.gif)
![](http://images.csdn.net/syntaxhighlighting/OutliningIndicators/InBlock.gif)                // Open the FileStream to write to
![](http://images.csdn.net/syntaxhighlighting/OutliningIndicators/InBlock.gif)                destinationStream = new FileStream ( destinationFile, FileMode.OpenOrCreate, FileAccess.Write );
![](http://images.csdn.net/syntaxhighlighting/OutliningIndicators/InBlock.gif)
![](http://images.csdn.net/syntaxhighlighting/OutliningIndicators/InBlock.gif)                // Create a compression stream pointing to the destiantion stream
![](http://images.csdn.net/syntaxhighlighting/OutliningIndicators/InBlock.gif)                compressedStream = new GZipStream ( destinationStream, CompressionMode.Compress, true );
![](http://images.csdn.net/syntaxhighlighting/OutliningIndicators/InBlock.gif)
![](http://images.csdn.net/syntaxhighlighting/OutliningIndicators/InBlock.gif)                // Now write the compressed data to the destination file
![](http://images.csdn.net/syntaxhighlighting/OutliningIndicators/InBlock.gif)                compressedStream.Write ( buffer, 0, buffer.Length );
![](http://images.csdn.net/syntaxhighlighting/OutliningIndicators/ExpandedSubBlockEnd.gif)            }
![](http://images.csdn.net/syntaxhighlighting/OutliningIndicators/InBlock.gif)            catch ( ApplicationException ex )
![](http://images.csdn.net/syntaxhighlighting/OutliningIndicators/ExpandedSubBlockStart.gif)            {
![](http://images.csdn.net/syntaxhighlighting/OutliningIndicators/InBlock.gif)                MessageBox.Show ( ex.Message, "压缩文件时发生错误：", MessageBoxButtons.OK, MessageBoxIcon.Error );
![](http://images.csdn.net/syntaxhighlighting/OutliningIndicators/ExpandedSubBlockEnd.gif)            }
![](http://images.csdn.net/syntaxhighlighting/OutliningIndicators/InBlock.gif)            finally
![](http://images.csdn.net/syntaxhighlighting/OutliningIndicators/ExpandedSubBlockStart.gif)            {
![](http://images.csdn.net/syntaxhighlighting/OutliningIndicators/InBlock.gif)                // Make sure we allways close all streams
![](http://images.csdn.net/syntaxhighlighting/OutliningIndicators/InBlock.gif)                if ( sourceStream != null )
![](http://images.csdn.net/syntaxhighlighting/OutliningIndicators/InBlock.gif)                    sourceStream.Close ( );
![](http://images.csdn.net/syntaxhighlighting/OutliningIndicators/InBlock.gif)
![](http://images.csdn.net/syntaxhighlighting/OutliningIndicators/InBlock.gif)                if ( compressedStream != null )
![](http://images.csdn.net/syntaxhighlighting/OutliningIndicators/InBlock.gif)                    compressedStream.Close ( );
![](http://images.csdn.net/syntaxhighlighting/OutliningIndicators/InBlock.gif)
![](http://images.csdn.net/syntaxhighlighting/OutliningIndicators/InBlock.gif)                if ( destinationStream != null )
![](http://images.csdn.net/syntaxhighlighting/OutliningIndicators/InBlock.gif)                    destinationStream.Close ( );
![](http://images.csdn.net/syntaxhighlighting/OutliningIndicators/ExpandedSubBlockEnd.gif)            }
![](http://images.csdn.net/syntaxhighlighting/OutliningIndicators/ExpandedSubBlockEnd.gif)        }
![](http://images.csdn.net/syntaxhighlighting/OutliningIndicators/InBlock.gif)
![](http://images.csdn.net/syntaxhighlighting/OutliningIndicators/InBlock.gif)        public void DecompressFile ( string sourceFile, string destinationFile )
![](http://images.csdn.net/syntaxhighlighting/OutliningIndicators/ExpandedSubBlockStart.gif)        {
![](http://images.csdn.net/syntaxhighlighting/OutliningIndicators/InBlock.gif)            // make sure the source file is there
![](http://images.csdn.net/syntaxhighlighting/OutliningIndicators/InBlock.gif)            if ( File.Exists ( sourceFile ) == false )
![](http://images.csdn.net/syntaxhighlighting/OutliningIndicators/InBlock.gif)                throw new FileNotFoundException ( );
![](http://images.csdn.net/syntaxhighlighting/OutliningIndicators/InBlock.gif)
![](http://images.csdn.net/syntaxhighlighting/OutliningIndicators/InBlock.gif)            // Create the streams and byte arrays needed
![](http://images.csdn.net/syntaxhighlighting/OutliningIndicators/InBlock.gif)            FileStream sourceStream = null;
![](http://images.csdn.net/syntaxhighlighting/OutliningIndicators/InBlock.gif)            FileStream destinationStream = null;
![](http://images.csdn.net/syntaxhighlighting/OutliningIndicators/InBlock.gif)            GZipStream decompressedStream = null;
![](http://images.csdn.net/syntaxhighlighting/OutliningIndicators/InBlock.gif)            byte[] quartetBuffer = null;
![](http://images.csdn.net/syntaxhighlighting/OutliningIndicators/InBlock.gif)
![](http://images.csdn.net/syntaxhighlighting/OutliningIndicators/InBlock.gif)            try
![](http://images.csdn.net/syntaxhighlighting/OutliningIndicators/ExpandedSubBlockStart.gif)            {
![](http://images.csdn.net/syntaxhighlighting/OutliningIndicators/InBlock.gif)                // Read in the compressed source stream
![](http://images.csdn.net/syntaxhighlighting/OutliningIndicators/InBlock.gif)                sourceStream = new FileStream ( sourceFile, FileMode.Open );
![](http://images.csdn.net/syntaxhighlighting/OutliningIndicators/InBlock.gif)
![](http://images.csdn.net/syntaxhighlighting/OutliningIndicators/InBlock.gif)                // Create a compression stream pointing to the destiantion stream
![](http://images.csdn.net/syntaxhighlighting/OutliningIndicators/InBlock.gif)                decompressedStream = new GZipStream ( sourceStream, CompressionMode.Decompress, true );
![](http://images.csdn.net/syntaxhighlighting/OutliningIndicators/InBlock.gif)
![](http://images.csdn.net/syntaxhighlighting/OutliningIndicators/InBlock.gif)                // Read the footer to determine the length of the destiantion file
![](http://images.csdn.net/syntaxhighlighting/OutliningIndicators/InBlock.gif)                quartetBuffer = new byte[4];
![](http://images.csdn.net/syntaxhighlighting/OutliningIndicators/InBlock.gif)                int position = (int)sourceStream.Length - 4;
![](http://images.csdn.net/syntaxhighlighting/OutliningIndicators/InBlock.gif)                sourceStream.Position = position;
![](http://images.csdn.net/syntaxhighlighting/OutliningIndicators/InBlock.gif)                sourceStream.Read ( quartetBuffer, 0, 4 );
![](http://images.csdn.net/syntaxhighlighting/OutliningIndicators/InBlock.gif)                sourceStream.Position = 0;
![](http://images.csdn.net/syntaxhighlighting/OutliningIndicators/InBlock.gif)                int checkLength = BitConverter.ToInt32 ( quartetBuffer, 0 );
![](http://images.csdn.net/syntaxhighlighting/OutliningIndicators/InBlock.gif)
![](http://images.csdn.net/syntaxhighlighting/OutliningIndicators/InBlock.gif)                byte[] buffer = new byte[checkLength + 100];
![](http://images.csdn.net/syntaxhighlighting/OutliningIndicators/InBlock.gif)
![](http://images.csdn.net/syntaxhighlighting/OutliningIndicators/InBlock.gif)                int offset = 0;
![](http://images.csdn.net/syntaxhighlighting/OutliningIndicators/InBlock.gif)                int total = 0;
![](http://images.csdn.net/syntaxhighlighting/OutliningIndicators/InBlock.gif)
![](http://images.csdn.net/syntaxhighlighting/OutliningIndicators/InBlock.gif)                // Read the compressed data into the buffer
![](http://images.csdn.net/syntaxhighlighting/OutliningIndicators/InBlock.gif)                while ( true )
![](http://images.csdn.net/syntaxhighlighting/OutliningIndicators/ExpandedSubBlockStart.gif)                {
![](http://images.csdn.net/syntaxhighlighting/OutliningIndicators/InBlock.gif)                    int bytesRead = decompressedStream.Read ( buffer, offset, 100 );
![](http://images.csdn.net/syntaxhighlighting/OutliningIndicators/InBlock.gif)
![](http://images.csdn.net/syntaxhighlighting/OutliningIndicators/InBlock.gif)                    if ( bytesRead == 0 )
![](http://images.csdn.net/syntaxhighlighting/OutliningIndicators/InBlock.gif)                        break;
![](http://images.csdn.net/syntaxhighlighting/OutliningIndicators/InBlock.gif)
![](http://images.csdn.net/syntaxhighlighting/OutliningIndicators/InBlock.gif)                    offset += bytesRead;
![](http://images.csdn.net/syntaxhighlighting/OutliningIndicators/InBlock.gif)                    total += bytesRead;
![](http://images.csdn.net/syntaxhighlighting/OutliningIndicators/ExpandedSubBlockEnd.gif)                }
![](http://images.csdn.net/syntaxhighlighting/OutliningIndicators/InBlock.gif)
![](http://images.csdn.net/syntaxhighlighting/OutliningIndicators/InBlock.gif)                // Now write everything to the destination file
![](http://images.csdn.net/syntaxhighlighting/OutliningIndicators/InBlock.gif)                destinationStream = new FileStream ( destinationFile, FileMode.Create );
![](http://images.csdn.net/syntaxhighlighting/OutliningIndicators/InBlock.gif)                destinationStream.Write ( buffer, 0, total );
![](http://images.csdn.net/syntaxhighlighting/OutliningIndicators/InBlock.gif)
![](http://images.csdn.net/syntaxhighlighting/OutliningIndicators/InBlock.gif)                // and flush everyhting to clean out the buffer
![](http://images.csdn.net/syntaxhighlighting/OutliningIndicators/InBlock.gif)                destinationStream.Flush ( );
![](http://images.csdn.net/syntaxhighlighting/OutliningIndicators/ExpandedSubBlockEnd.gif)            }
![](http://images.csdn.net/syntaxhighlighting/OutliningIndicators/InBlock.gif)            catch ( ApplicationException ex )
![](http://images.csdn.net/syntaxhighlighting/OutliningIndicators/ExpandedSubBlockStart.gif)            {
![](http://images.csdn.net/syntaxhighlighting/OutliningIndicators/InBlock.gif)                MessageBox.Show(ex.Message, "解压文件时发生错误：", MessageBoxButtons.OK, MessageBoxIcon.Error);
![](http://images.csdn.net/syntaxhighlighting/OutliningIndicators/ExpandedSubBlockEnd.gif)            }
![](http://images.csdn.net/syntaxhighlighting/OutliningIndicators/InBlock.gif)            finally
![](http://images.csdn.net/syntaxhighlighting/OutliningIndicators/ExpandedSubBlockStart.gif)            {
![](http://images.csdn.net/syntaxhighlighting/OutliningIndicators/InBlock.gif)                // Make sure we allways close all streams
![](http://images.csdn.net/syntaxhighlighting/OutliningIndicators/InBlock.gif)                if ( sourceStream != null )
![](http://images.csdn.net/syntaxhighlighting/OutliningIndicators/InBlock.gif)                    sourceStream.Close ( );
![](http://images.csdn.net/syntaxhighlighting/OutliningIndicators/InBlock.gif)
![](http://images.csdn.net/syntaxhighlighting/OutliningIndicators/InBlock.gif)                if ( decompressedStream != null )
![](http://images.csdn.net/syntaxhighlighting/OutliningIndicators/InBlock.gif)                    decompressedStream.Close ( );
![](http://images.csdn.net/syntaxhighlighting/OutliningIndicators/InBlock.gif)
![](http://images.csdn.net/syntaxhighlighting/OutliningIndicators/InBlock.gif)                if ( destinationStream != null )
![](http://images.csdn.net/syntaxhighlighting/OutliningIndicators/InBlock.gif)                    destinationStream.Close ( );
![](http://images.csdn.net/syntaxhighlighting/OutliningIndicators/ExpandedSubBlockEnd.gif)            }
![](http://images.csdn.net/syntaxhighlighting/OutliningIndicators/InBlock.gif)
![](http://images.csdn.net/syntaxhighlighting/OutliningIndicators/ExpandedSubBlockEnd.gif)        }
![](http://images.csdn.net/syntaxhighlighting/OutliningIndicators/ExpandedBlockEnd.gif)    }
