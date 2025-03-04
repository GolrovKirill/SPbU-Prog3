// <copyright file="Upload.cs" company="Gorlov Kirill">
// Copyright (c) Gorlov Kirill. All rights reserved.
// Licensed under the MIT License. See LICENSE in the repository root for license information.
// https://github.com/GolrovKirill/SPbU-Prog3/blob/main/LICENSE
// </copyright>
namespace MyNUnitWeb.Model;

/// <summary>
/// Represents the data model for handling file uploads in the application.
/// </summary>
public class Upload
{
    public required List<IFormFile> Files { get; set; }

    public required string UploadMessage { get; set; }

    public required List<string> UploadFiles { get; set; }
}
