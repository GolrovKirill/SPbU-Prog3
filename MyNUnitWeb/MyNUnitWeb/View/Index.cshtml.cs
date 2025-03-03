// <copyright file="IndexModel.cs" company="Gorlov Kirill">
// Copyright (c) Gorlov Kirill. All rights reserved.
// Licensed under the MIT License. See LICENSE in the repository root for license information.
// https://github.com/GolrovKirill/SPbU-Prog3/blob/main/LICENSE
// </copyright>
namespace MyNUnitWeb.Pages;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MyNUnit;
using MyNUnitWeb.Data;
using MyNUnitWeb.Model;
using MyNUnitWeb.Services;

/// <summary>
/// Represents the main page model for the MyNUnitWeb application. 
/// Handles file uploads, test execution.
/// </summary>
public class IndexModel : PageModel
{
    private readonly UploadSite uploadService;
    private readonly TestSite testRunnerService;

    public Upload Upload { get; set; }

    public TestRun? TestRun { get; set; }

    private const string ClientIdSessionKey = "ClientId";

    /// <summary>
    /// Initializes a new instance of the <see cref="IndexModel"/> class.
    /// </summary>
    /// <param name="context">Database context.</param>
    /// <param name="uploadService">Service for handling file uploads.</param>
    /// <param name="testRunnerService">Service for running tests.</param>
    public IndexModel(WebData context, UploadSite uploadService, TestSite testRunnerService)
    {
        this.uploadService = uploadService;
        this.testRunnerService = testRunnerService;

        Upload = new Upload
        {
            UploadFiles = new List<string>(),
        };
    }

    /// <summary>
    /// Handles GET requests to load the page and initialize uploaded file data.
    /// </summary>
    public void OnGet()
    {
        var clientId = GetClientId(HttpContext.Session, ClientIdSessionKey);

        Upload.UploadFiles = uploadService.LoadUploadedFiles(clientId);
    }

    /// <summary>
    /// Handles POST requests to upload files.
    /// </summary>
    public async Task<IActionResult> OnPostUploadAsync(List<IFormFile>? files)
    {
        var clientId = GetClientId(HttpContext.Session, ClientIdSessionKey);

        Upload.UploadMessage = await (uploadService.Upload(files, clientId));
        Upload.UploadFiles = uploadService.LoadUploadedFiles(clientId);
        return Page();
    }

    /// <summary>
    /// Handles POST requests to execute tests on uploaded files.
    /// </summary>
    public async Task<IActionResult> OnPostRunTestsAsync()
    {
        var clientId = GetClientId(HttpContext.Session, ClientIdSessionKey);

        TestRun = await testRunnerService.RunTests(clientId);
        clientId = RefreshClientId(HttpContext.Session, ClientIdSessionKey);
        HttpContext.Session.SetString(ClientIdSessionKey, clientId);

        Upload.UploadFiles = uploadService.LoadUploadedFiles(clientId);
        return Page();
    }

    /// <summary>
    /// Retrieves the client ID from the session.
    /// </summary>
    private static string GetClientId(ISession session, string clientIdKey)
    {
        var clientId = session.GetString(clientIdKey);

        if (string.IsNullOrEmpty(clientId))
        {
            clientId = Guid.NewGuid().ToString();
            session.SetString(clientIdKey, clientId);
        }

        return clientId;
    }

    /// <summary>
    /// Generates a new client ID and updates the session with it.
    /// </summary>
    private static string RefreshClientId(ISession session, string clientIdKey)
    {
        var clientId = Guid.NewGuid().ToString();
        session.SetString(clientIdKey, clientId);
        return clientId;
    }
}