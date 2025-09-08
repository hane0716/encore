var builder = WebApplication.CreateBuilder(args);

// Razor Pagesサービスを追加
builder.Services.AddRazorPages();
builder.Services.AddSession(); // ✅ セッション機能を追加（Build前）

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

app.UseSession(); // ✅ セッションミドルウェアを有効化

app.UseAuthorization();

app.MapRazorPages();

app.Run();
