// <copyright file="WebData.cs" company="Gorlov Kirill">
// Copyright (c) Gorlov Kirill. All rights reserved.
// Licensed under the MIT License. See LICENSE in the repository root for license information.
// https://github.com/GolrovKirill/SPbU-Prog3/blob/main/LICENSE
// </copyright>
namespace MyNUnitWeb.Data;

using Microsoft.EntityFrameworkCore;
using MyNUnitWeb.Model;

/// <summary>
/// Manages the database context for the application.
/// </summary>
public class WebData : DbContext
{
    public WebData(DbContextOptions<WebData> options)
        : base(options)
    {
    }

    public DbSet<TestRun> TestRuns { get; set; }

    public DbSet<TestClassModel> TestClasses { get; set; }

    public DbSet<TestDetailModel> TestDetails { get; set; }

    /// <summary>
    /// Configures the data model, including relationships between entities.
    /// </summary>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<TestRun>()
            .HasMany(tr => tr.TestClasses)
            .WithOne(tc => tc.TestRun)
            .HasForeignKey(tc => tc.TestRunModelId);

        modelBuilder.Entity<TestClassModel>()
            .HasMany(tc => tc.TestDetails)
            .WithOne(tc => tc.TestClass)
            .HasForeignKey(tc => tc.TestClassModelId);
    }
}