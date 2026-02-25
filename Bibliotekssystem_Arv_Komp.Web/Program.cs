using Microsoft.EntityFrameworkCore;
using Bibliotekssystem_Arv_Komp.Data;
using Bibliotekssystem_Arv_Komp.Models;
using Bibliotekssystem_Arv_Komp.Web.Components;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddDbContextFactory<LibraryContext>(options =>
    options.UseSqlite("Data Source=library.db"));

builder.Services.AddScoped<IBookRepository, BookRepository>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<LibraryContext>();
    context.Database.EnsureCreated();

    if (!context.Books.Any())
    {
        context.Books.AddRange(
            new Book("978-0-13-468599-1", "The Pragmatic Programmer", "David Thomas", 2019, 352),
            new Book("978-0-596-51774-8", "JavaScript: The Good Parts", "Douglas Crockford", 2008, 176),
            new Book("978-0-13-235088-4", "Clean Code", "Robert C. Martin", 2008, 464),
            new Book("978-91-29-72348-2", "Mio, min Mio", "Astrid Lindgren", 1954, 198),
            new Book("978-91-29-66634-5", "Bröderna Lejonhjärta", "Astrid Lindgren", 1973, 256)
        );

        context.Members.AddRange(
            new Member("M001", "Anna Andersson", "anna@example.com"),
            new Member("M002", "Erik Eriksson", "erik@example.com"),
            new Member("M003", "Maria Johansson", "maria@example.com")
        );

        context.SaveChanges();
    }
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseAntiforgery();
app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
