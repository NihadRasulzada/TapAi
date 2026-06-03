using Microsoft.EntityFrameworkCore;

namespace TapAi.Shared.Application.Context;

public abstract class AppDbContext(DbContextOptions options) : DbContext(options) { }