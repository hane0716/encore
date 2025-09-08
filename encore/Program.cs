var builder = WebApplication.CreateBuilder(args);

// Razor Pages�T�[�r�X��ǉ�
builder.Services.AddRazorPages();

var app = builder.Build();

// �G���[�n���h�����O�Ȃ�
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

// Razor Pages���[�e�B���O�̂ݎg�p
app.MapRazorPages();

app.Run();
