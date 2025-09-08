var builder = WebApplication.CreateBuilder(args);

// Razor Pagesサービスを追加
builder.Services.AddRazorPages();

var app = builder.Build();

// エラーハンドリングなど
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

// Razor Pagesルーティングのみ使用
app.MapRazorPages();

app.Run();
