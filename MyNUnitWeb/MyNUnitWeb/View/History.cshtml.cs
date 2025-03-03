// <copyright file="History.cs" company="Gorlov Kirill">
// Copyright (c) Gorlov Kirill. All rights reserved.
// Licensed under the MIT License. See LICENSE in the repository root for license information.
// https://github.com/GolrovKirill/SPbU-Prog3/blob/main/LICENSE
// </copyright>
namespace MyNUnitWeb.Pages;

using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MyNUnitWeb.Data;
using MyNUnitWeb.Model;

/// <summary>
///  Represents the page model for displaying the test execution history.
/// </summary>
public class TestHistory(WebData dbContext) : PageModel
{
    /// <summary>
    /// A list of test run records retrieved from the database.
    /// </summary>
    public List<TestRun> TestRuns { get; set; } = new();

    /// <summary>
    /// Handles GET requests to retrieve and display the test run history.
    /// </summary>
    public async void OnGet()
    {
        TestRuns = await dbContext.TestRuns
            .Include(tr => tr.TestClasses)
            .ThenInclude(tc => tc.TestDetails)
            .ToListAsync();
    }
}