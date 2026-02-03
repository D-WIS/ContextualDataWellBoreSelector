using DWIS.Client.ReferenceImplementation;
using DWIS.Client.ReferenceImplementation.OPCFoundation;
using DWIS.ContextualData.WellBoreSelector.WebApp;
using MudBlazor;
using MudBlazor.Services;
using Plotly.Blazor.Traces.SankeyLib;
using WebApp;

DWISConnector.Instance.ConnectToBlackboard(null);

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddMudServices(config =>
{
    config.SnackbarConfiguration.PositionClass = Defaults.Classes.Position.BottomLeft;
    config.SnackbarConfiguration.PreventDuplicates = false;
    config.SnackbarConfiguration.NewestOnTop = false;
    config.SnackbarConfiguration.ShowCloseIcon = true;
    config.SnackbarConfiguration.VisibleStateDuration = 5000;
    config.SnackbarConfiguration.HideTransitionDuration = 500;
    config.SnackbarConfiguration.ShowTransitionDuration = 500;
    config.SnackbarConfiguration.SnackbarVariant = Variant.Filled;
});

var app = builder.Build();

app.UseForwardedHeaders();
// This needs to match with what is defined in "charts/<helm-chart-name>/templates/values.yaml ingress.Path
app.UsePathBase("/WellBoreSelector/webapp");

DWIS.ContextualData.WellBoreSelector.WebApp.Configuration.LoadFromHome(builder.Environment.ContentRootPath);
var appConfig = DWIS.ContextualData.WellBoreSelector.WebApp.Configuration.Instance;
if (!String.IsNullOrEmpty(builder.Configuration["WellBoreSelectorHostURL"]))
    appConfig.WellBoreSelectorHostURL = builder.Configuration["WellBoreSelectorHostURL"];
if (!String.IsNullOrEmpty(builder.Configuration["UnitConversionHostURL"]))
    appConfig.UnitConversionHostURL = builder.Configuration["UnitConversionHostURL"];
if (!String.IsNullOrEmpty(builder.Configuration["FieldHostURL"]))
    appConfig.FieldHostURL = builder.Configuration["FieldHostURL"];
if (!String.IsNullOrEmpty(builder.Configuration["ClusterHostURL"]))
    appConfig.ClusterHostURL = builder.Configuration["ClusterHostURL"];
if (!String.IsNullOrEmpty(builder.Configuration["WellHostURL"]))
    appConfig.WellHostURL = builder.Configuration["WellHostURL"];
if (!String.IsNullOrEmpty(builder.Configuration["WellBoreHostURL"]))
    appConfig.WellBoreHostURL = builder.Configuration["WellBoreHostURL"];

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();
