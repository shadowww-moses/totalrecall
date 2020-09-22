# CORS

*Cross-Origin Resource Sharing* - механизм, который позволяет странице получать доступ к ресурсам другого сервера, а не только того, с которого она сама загружена. Работает через использование специальных HTTP-заголовков.

Я узнал о CORS, когда столкнулся с тем, что не смог отправить AJAX-запрос из React-приложения, запущенного на порте, отличном от порта, на котором запущен сервер.

Глубоко в настройку пока не углублялся, но чтобы к API можно было обращаться в таких случаях, нужно создать и подключить к Web API CORS-политику.

Это делается в классе `Startup.cs`. В метод `ConfigureServices` добавляем политику:

```c#
public void ConfigureServices(IServiceCollection services)
{
    services.AddCors(options =>
        options.AddPolicy("AllowEverything", builder => builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader())
    );

    services.AddScoped<NorthwindContext>();

    services.AddControllers();
}
```

и в методе `Configure` задействуем ее:

```c#
public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
{
    if (env.IsDevelopment())
    {
        app.UseDeveloperExceptionPage();
    }

    app.UseCors("AllowEverything");

    app.UseRouting();

    app.UseAuthorization();

    app.UseEndpoints(endpoints =>
    {
        endpoints.MapControllers();
    });
}
```

Причем важно подключить ее до всего остального, поэтому она размещена в начале не просто так.

Эта политика разрешает все и отовсюду, поэтому использоваться может только в тестовых целях.