<Query Kind="Program" />

void Main()
{
	// Consulta de información de la cuenta monetaria.
	IFabricaInformacionCuenta fabricaInformacionCuenta = new FabricaInformacionCuentaMonetaria();
	DatosCuenta datosCuenta = ObtenerDatosCuenta(fabricaInformacionCuenta, "01234");
	datosCuenta.Dump();
	
	// Consulta de información de la cuenta de ahorro.
	fabricaInformacionCuenta = new FabricaInformacionCuentaAhorro();
	datosCuenta = ObtenerDatosCuenta(fabricaInformacionCuenta, "56789");
	datosCuenta.Dump();
}

// El método que obtiene los datos de la cuenta sólo recibe una fábrica sin saber a qué tipo de cuenta corresponde.
public DatosCuenta ObtenerDatosCuenta(IFabricaInformacionCuenta fabrica, string numeroCuenta)
{
	IConsultaNombreCuenta consultaNombre = fabrica.CrearConsultaNombre();
	IConsultaSaldo consultaSaldo = fabrica.CrearConsultaSaldo();
	IConsultaMovimientosMes consultaMovimientosMes = fabrica.CrearConsultaMovimientosMes();
	
	string nombreCuenta = consultaNombre.ConsultarNombre(numeroCuenta);
	decimal saldo = consultaSaldo.ConsultarSaldo(numeroCuenta);
	IEnumerable<Movimiento> movimientosMes = consultaMovimientosMes.ConsultarMovimientosMes(numeroCuenta);
	return new DatosCuenta(numeroCuenta, nombreCuenta, saldo, movimientosMes);
}

// La fábrica que crea la familia de objetos necesaria para llevar a cabo una tarea.
public interface IFabricaInformacionCuenta
{
	IConsultaNombreCuenta CrearConsultaNombre();
	
	IConsultaSaldo CrearConsultaSaldo();
	
	IConsultaMovimientosMes CrearConsultaMovimientosMes();
}

// Interfaz para consulta del nombre de la cuenta.
public interface IConsultaNombreCuenta
{
	string ConsultarNombre(string numeroCuenta);
}

// Interfaz para consulta del saldo de la cuenta.
public interface IConsultaSaldo
{
	decimal ConsultarSaldo(string numeroCuenta);
}

// Interfaz para consulta de los movimientos del mes.
public interface IConsultaMovimientosMes
{
	IEnumerable<Movimiento> ConsultarMovimientosMes(string numeroCuenta);
}

// Implementación para consultar el nombre de la cuenta de ahorro.
public class ConsultaNombreCuentaAhorro : IConsultaNombreCuenta
{
	public string ConsultarNombre(string numeroCuenta)
	{
		Console.WriteLine($"Consultando el nombre de la cuenta de ahorro {numeroCuenta}...");
		return "Cuenta ahorro " + numeroCuenta;
	}
}

// Implementación para consultar el nombre de la cuenta monetaria.
public class ConsultaNombreCuentaMonetaria : IConsultaNombreCuenta
{
	public string ConsultarNombre(string numeroCuenta)
	{
		Console.WriteLine($"Consultando el nombre de la cuenta monetaria {numeroCuenta}...");
		return "Cuenta monetaria " + numeroCuenta;
	}
}

// Implementación para consultar el saldo de la cuenta de ahorro.
public class ConsultaSaldoCuentaAhorro : IConsultaSaldo
{
	public decimal ConsultarSaldo(string numeroCuenta)
	{
		Console.WriteLine($"Consultando el saldo de la cuenta de ahorro {numeroCuenta}...");
		return 2000M;
	}
}

// Implementación para consultar el saldo de la cuenta monetaria.
public class ConsultaSaldoCuentaMonetaria : IConsultaSaldo
{
	public decimal ConsultarSaldo(string numeroCuenta)
	{
		Console.WriteLine($"Consultando el saldo de la cuenta monetaria {numeroCuenta}...");
		return 1000M;
	}
}

// Implementación para consultar los movimientos del mes de la cuenta de ahorro.
public class ConsultaMovimientosMesCuentaAhorro : IConsultaMovimientosMes
{
	public IEnumerable<Movimiento> ConsultarMovimientosMes(string numeroCuenta)
	{
		Console.WriteLine($"Consultando los movimientos del mes de la cuenta de ahorro {numeroCuenta}...");
		return new List<Movimiento>()
			{
				new Movimiento(new DateTime(2024, 5, 1), 1000M, "Ahorro del mes anterior"),
				new Movimiento(new DateTime(2024, 5, 2), -200M, "Pago de servicios"),
			};
	}
}

// Implementación para consultar los movimientos del mes de la cuenta monetaria.
public class ConsultaMovimientosMesCuentaMonetaria : IConsultaMovimientosMes
{
	public IEnumerable<Movimiento> ConsultarMovimientosMes(string numeroCuenta)
	{
		Console.WriteLine($"Consultando los movimientos del mes de la cuenta monetaria {numeroCuenta}...");
		return new List<Movimiento>()
			{
				new Movimiento(new DateTime(2024, 5, 1), -100M, "Pago de teléfono"),
				new Movimiento(new DateTime(2024, 5, 1), -100M, "Pago de internet"),
				new Movimiento(new DateTime(2024, 5, 15), 1000M, "Anticipo de planilla"),
			};
	}
}

// Implementación de fábrica para consultar los datos de la cuenta de ahorro.
public class FabricaInformacionCuentaAhorro : IFabricaInformacionCuenta
{
	public IConsultaNombreCuenta CrearConsultaNombre() => new ConsultaNombreCuentaAhorro();
	
	public IConsultaSaldo CrearConsultaSaldo() => new ConsultaSaldoCuentaAhorro();
	
	public IConsultaMovimientosMes CrearConsultaMovimientosMes() => new ConsultaMovimientosMesCuentaAhorro();
}

// Implementación de fábrica para consultar los datos de la cuenta monetaria.
public class FabricaInformacionCuentaMonetaria : IFabricaInformacionCuenta
{
	public IConsultaNombreCuenta CrearConsultaNombre() => new ConsultaNombreCuentaMonetaria();
	
	public IConsultaSaldo CrearConsultaSaldo() => new ConsultaSaldoCuentaMonetaria();
	
	public IConsultaMovimientosMes CrearConsultaMovimientosMes() => new ConsultaMovimientosMesCuentaMonetaria();
}

// Registro para el movimiento de la cuenta.
public record Movimiento(DateTime Fecha, decimal Monto, string Descripcion)
{
}

// Registro para el conjunto de datos de una cuenta.
public record DatosCuenta(string Numero, string Nombre, decimal Saldo, IEnumerable<Movimiento> MovimientosMes)
{
}