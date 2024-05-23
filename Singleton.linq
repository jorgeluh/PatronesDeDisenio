<Query Kind="Program" />

void Main()
{
	string direccionServicio = ConfiguracionesAplicacion.ObtenerInstancia().ObtenerValor<string>("DireccionServicio");
	int cantidadHilos = ConfiguracionesAplicacion.ObtenerInstancia().ObtenerValor<int>("CantidadHilos");
	DateTime fechaInicial = ConfiguracionesAplicacion.ObtenerInstancia().ObtenerValor<DateTime>("FechaInicial");
	
	ConfiguracionesAplicacion configuraciones = ConfiguracionesAplicacion.ObtenerInstancia();
	direccionServicio = configuraciones.ObtenerValor<string>("DireccionServicio");
	cantidadHilos = configuraciones.ObtenerValor<int>("CantidadHilos");
	fechaInicial = configuraciones.ObtenerValor<DateTime>("FechaInicial");
}

public class ConfiguracionesAplicacion
{
	private static readonly object Candado = new object();
	
	private static ConfiguracionesAplicacion instancia;
	
	private readonly Dictionary<string, object> repositorioConfiguraciones;
	
	private ConfiguracionesAplicacion()
	{
		Console.WriteLine("Construyendo la instancia...");
		this.repositorioConfiguraciones = new Dictionary<string, object>();
		this.ConsultarValores();
	}
	
	public static ConfiguracionesAplicacion ObtenerInstancia()
	{
		if (instancia == null)
		{
			lock (Candado)
			{
				if (instancia == null)
				{
					Console.WriteLine("La instancia no existe, creando una...");
					instancia = new ConfiguracionesAplicacion();
				}
			}
		}
		
		Console.WriteLine("La instancia ya existe.");
		return instancia;
	}
	
	public T ObtenerValor<T>(string nombreConfiguracion)
	{
		Console.WriteLine($"Consultando la configuración {nombreConfiguracion}...");
		return (T)this.repositorioConfiguraciones[nombreConfiguracion];
	}
	
	private void ConsultarValores()
	{
		Console.WriteLine("Cargando las configuraciones...");
		this.repositorioConfiguraciones.Add("CantidadHilos", 10 );
		this.repositorioConfiguraciones.Add("DireccionServicio", "http://servicio.com/api/consultas");
		this.repositorioConfiguraciones.Add("FechaInicial", new DateTime(2024, 1, 1));
	}
}