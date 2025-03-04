// <copyright file="UploadSite.cs" company="Gorlov Kirill">
// Copyright (c) Gorlov Kirill. All rights reserved.
// Licensed under the MIT License. See LICENSE in the repository root for license information.
// https://github.com/GolrovKirill/SPbU-Prog3/blob/main/LICENSE
// </copyright>
namespace MyNUnitWeb.Services;

/// <summary>
/// Provides services for managing file uploads.
/// </summary>
public class UploadSite
{
    private const int ConstSize = 10485760;
    private readonly string baseUploadPath;
    private readonly long maxFileSize;

    /// <summary>
    /// Initializes a new instance of the <see cref="UploadSite"/> class.
    /// </summary>
    /// <param name="maxFileSize">The maximum allowable file size in bytes. Default is 10 MB.</param>
    public UploadSite(long maxFileSize = ConstSize)
    {
        this.baseUploadPath = Path.Combine(Path.GetTempPath(), "Upload");
        this.maxFileSize = maxFileSize;

        if (!Directory.Exists(this.baseUploadPath))
        {
            Directory.CreateDirectory(this.baseUploadPath);
        }
    }

    /// <summary>
    /// Uploads a list of files to the server under a directory identified by the client ID.
    /// </summary>
    public async Task<string> Upload(List<IFormFile>? files, string clientId)
    {
        if (files == null || files.Count == 0)
        {
            return "Пожалуйста, выберите файлы для загрузки.";
        }

        var clientUploadPath = Path.Combine(this.baseUploadPath, clientId);

        if (!Directory.Exists(clientUploadPath))
        {
            Directory.CreateDirectory(clientUploadPath);
        }

        foreach (var file in files)
        {
            if (file.Length > this.maxFileSize)
            {
                long maxFileSize1 = this.maxFileSize;
                return $"Файл {file.FileName} слишком велик. Максимальный размер файла: {maxFileSize1 / (1024 * 1024)} MB.";
            }

            var filePath = Path.Combine(clientUploadPath, file.FileName);
            await using var stream = new FileStream(filePath, FileMode.Create);
            await file.CopyToAsync(stream);
        }

        return "Файлы успешно загружены!";
    }

    /// <summary>
    /// Retrieves a list of uploaded file names for the specified client ID.
    /// </summary>
    public List<string> LoadUploadedFiles(string clientId)
    {
        var clientUploadPath = Path.Combine(this.baseUploadPath, clientId);

        var files = Directory.Exists(clientUploadPath) ?
            [.. Directory.GetFiles(clientUploadPath).Select(Path.GetFileName)] : new List<string>();

        return files;
    }
}
