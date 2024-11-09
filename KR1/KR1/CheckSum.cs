namespace KR1;

using System;
using System.IO;
using System.Security.Cryptography;

public class CheckSum
{
   private async Task<byte[]> CalculateMd5Async(string path)
    {
        if (!File.Exists(path))
        {
            throw new FileNotFoundException();
        }

        byte[] fileBytes;
        await using (var stream = new FileStream(path, FileMode.Open, FileAccess.Read))
        {
            fileBytes = new byte[stream.Length];
            var readAsync = await stream.ReadAsync(fileBytes, 0, (int)stream.Length);
        }

        using (var md5 = MD5.Create())
        {
            return md5.ComputeHash(fileBytes);
        }
    }

   /// <summary>
   /// Read in one thread.
   /// </summary>
   /// <param name="path">path directory or file.</param>
   /// <returns>array byte.</returns>
   /// <exception cref="AggregateException">Incorrect path.</exception>
   public async Task<byte[]> SingleChekSum(string path)
    {
        if (!Directory.Exists(path) && !File.Exists(path))
        {
            throw new AggregateException();
        }

        if (Directory.Exists(path) && Directory.GetFileSystemEntries(path).Length > 0 && !File.Exists(path))
        {
            var entries = Directory.GetFileSystemEntries(path);

            foreach (var element in entries)
            {
                await SingleChekSum(element);
            }
        }

        return await CalculateMd5Async(path);
    }

    /// <summary>
    /// Read in multythread.
    /// </summary>
    /// <param name="path">path directory or file.</param>
    /// <returns>array byte.</returns>
    /// <exception cref="AggregateException">Incorrect path.</exception>
   public async Task<byte[]> MultiChekSum(string path)
    {
        if (!Directory.Exists(path) && !File.Exists(path))
        {
            throw new AggregateException();
        }

        if (Directory.Exists(path) && Directory.GetFileSystemEntries(path).Length > 0 && !File.Exists(path))
        {
            var entries = Directory.GetFileSystemEntries(path);

            await Task.Run(() =>
            {
                foreach (var element in entries)
                {
                    SingleChekSum(element);
                }
            });
        }

        return await CalculateMd5Async(path);
    }
}